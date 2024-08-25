using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using Castle.MicroKernel.Registration;
using FeeManagementSystem.Classes;
using FeeManagementSystem.Fees;
using FeeManagementSystem.PaymentPlans;
using FeeManagementSystem.Payments;
using FeeManagementSystem.StudentFees;
using FeeManagementSystem.Students;
using FeeSystem.Students.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;

namespace FeeSystem.Students
{
    [AbpAuthorize]
    public class StudentAppService : ApplicationService, IStudentAppService
    {
        private readonly IRepository<Student, int> _studentRepository;
        private readonly IRepository<Class, int> _classRepository;
        private readonly IRepository<Fee, int> _feeRepository;
        private readonly IRepository<StudentFee, int> _studentFeeRepository;
        private readonly IRepository<Payment, long> _paymentRepository;

        public StudentAppService(
            IRepository<Student, int> studentRepository,
            IRepository<Class, int> classRepository,
            IRepository<Fee, int> feeRepository,
            IRepository<StudentFee, int> studentFeeRepository,
            IRepository<Payment, long> paymentRepository)
        {
            _studentRepository = studentRepository;
            _classRepository = classRepository;
            _feeRepository = feeRepository;
            _studentFeeRepository = studentFeeRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<List<StudentDto>> GetAllStudents()
        {
            int currTenantId;
            if (AbpSession.TenantId.HasValue)
            {
                currTenantId = (int)AbpSession.TenantId;
            }
            else
            {
                currTenantId = 0;
            }

            var students = getstudentFunc(currTenantId);
            

            if (students == null || !students.Any())
            {
                Console.WriteLine("No students found in the database.");
                return new List<StudentDto>(); // Returning an empty list
            }

            // Map the list of students to DTOs
            var studentDtos = ObjectMapper.Map<List<StudentDto>>(students);

            return studentDtos;
        }

        

        public async Task UpdateStudentFeeAsync(UpdateStudentFeeDto input)
        {
            // Retrieve the existing StudentFee
            var studentFee = _studentFeeRepository
                .GetAllIncluding(s=>s.Fee,s=>s.PaymentPlan)
                .Where(s=>s.Fee.FeeHeading ==input.FeeHeading)
                .FirstOrDefault(s => s.StudentId == input.StudentId);
    
            if (studentFee == null)
            {
                throw new EntityNotFoundException(typeof(StudentFee), input.StudentId);
            }

            double totalPeriod;
            switch (studentFee.PaymentPlan.IntervalInMonths)
            {
                case 12: // Yearly
                    totalPeriod = 1;
                    break;
                case 3: // Quarterly
                    totalPeriod =  4;
                    break;
                case 1: // Monthly
                    totalPeriod = 12;
                    break;
                default:
                    throw new ArgumentException($"Unsupported interval: {studentFee.PaymentPlan.IntervalInMonths}");
            }
            // Update the fields
            studentFee.AdditionalFee = input.AdditionalFee;
            studentFee.Discount = input.Discount;

            
            // Recalculate the total amount considering the additional fee and discount
            studentFee.UpdatedTotalAmount = ((studentFee.TotalAmount) + studentFee.AdditionalFee) - studentFee.Discount;

            studentFee.AmountPerPeriod = (studentFee.UpdatedTotalAmount) / totalPeriod;

            // Save the changes
            await _studentFeeRepository.UpdateAsync(studentFee);
        }

        public async Task<StudentDto> GetStudentById(string id)
        {
            var student = await _studentRepository
        .GetAllIncluding(s => s.Class, s => s.StudentFees)
        .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                // Consider using a specific exception or a custom exception class
                throw new UserFriendlyException("Student not found!");
            }

            // Ensure your mapping configuration is set correctly
            var studentDto = ObjectMapper.Map<StudentDto>(student);

            return studentDto;
        }

        public async Task<StudentDto> CreateStudent(CreateStudentDto input)
        {
            int localTenantId = (int)AbpSession.TenantId;
            var classExist =  _classRepository
                .GetAll()
                .Where(cl => cl.ClassId == input.ClassId)
                .Where(cl => cl.TenantId == localTenantId)
                .ToList();

            if (classExist.IsNullOrEmpty())
            {
                throw new Exception("Class not found");
            }
            
            // Generate the custom Student ID
            string customStudentId = await GenerateCustomStudentId(localTenantId, input.ClassId);
            
            var student = ObjectMapper.Map<Student>(input);
            student.StudentId = customStudentId;
            await _studentRepository.InsertAsync(student);
            await CurrentUnitOfWork.SaveChangesAsync();

            var classFees = await _feeRepository
                .GetAllIncluding(f => f.PaymentPlan)
                .Where(f => f.ClassId == student.ClassId)
                .ToListAsync();

            var studentId =  _studentRepository
                .GetAll().Where(s=>s.TenantId == localTenantId)
                .FirstOrDefault(s => s.ContactNumber == student.ContactNumber);


            double totalAmount = 0;
            double amountPerPeriod = 0;

            if (classFees.Any())
            {
                foreach (var fee in classFees)
                {
                    var paymentPlan = fee.PaymentPlan;
                    if (paymentPlan == null)
                    {
                        throw new Exception("Payment plan is null for Fee ID: " + fee.Id);
                    }


                    // Calculate the amount per period based on the payment plan's interval.
                    double feeAmountPerPeriod = fee.Amount;
                    switch (paymentPlan.IntervalInMonths)
                    {
                        case 12: // Yearly
                            feeAmountPerPeriod = fee.Amount / 1;
                            break;
                        case 3: // Quarterly
                            feeAmountPerPeriod = fee.Amount / 4;
                            break;
                        case 1: // Monthly
                            feeAmountPerPeriod = fee.Amount / 12;
                            break;
                        default:
                            throw new ArgumentException($"Unsupported interval: {paymentPlan.IntervalInMonths}");
                    }

                    // Add to the total amounts.
                    totalAmount += fee.Amount;
                    amountPerPeriod += feeAmountPerPeriod;

                    var studentFee = new StudentFee
                    {
                        StudentId = studentId.StudentId,
                        FeeId = fee.FeeId,
                        PaymentPlanId = fee.PaymentPlanId,
                        AmountPerPeriod = feeAmountPerPeriod,
                        TotalAmount = fee.Amount,
                        UpdatedTotalAmount = fee.Amount,
                        TenantId = localTenantId,// This represents the full fee.
                        EffectiveFrom = DateTime.Now // Setting the effective date.
                    };


                    if (studentFee.StudentId == null || studentFee.FeeId == 0 || studentFee.PaymentPlanId == 0)
                    {
                        Logger.Error("Invalid data for StudentFee. One or more IDs are missing.");
                        throw new Exception("Invalid data for StudentFee. One or more IDs are missing.");
                    }
                    await _studentFeeRepository.InsertAsync(studentFee);
                }
            }

            return ObjectMapper.Map<StudentDto>(student);
        }

        public async Task<StudentDto> UpdateStudent(UpdateStudentDto input)
        {
            var student = await _studentRepository.FirstOrDefaultAsync(s=>s.StudentId == input.StudentId);
            if (student == null)
            {
                throw new Exception("Student not found!");
            }

            var classId = await _classRepository.FirstOrDefaultAsync(c => c.ClassName == input.ClassName);
            ObjectMapper.Map(input, student);
            student.ClassId = classId.ClassId;
            await _studentRepository.UpdateAsync(student);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<StudentDto>(student);
        }

        public async Task DeleteStudent(string id)
        {
            await _studentRepository.DeleteAsync(s=>s.StudentId==id);
            // if (student == null)
            // {
            //     throw new Exception("Student not found!");
            // }
            //
            // await _studentRepository.DeleteAsync(student);
        }

        public async Task<List<StudentFeeDto>> GetFeesForStudent(string studentId)
        {
            var student = await _studentRepository
              .GetAllIncluding(s => s.Class)
              .FirstOrDefaultAsync(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new Exception("Student not found!");
            }

            var classId = student.ClassId;

            var studentFees = await _studentFeeRepository
                .GetAll()
                .Where(sf => sf.StudentId == studentId)
                // .Include(sf => sf.Student)
                .Include( sf => sf.Fee)
                .Include(sf => sf.PaymentPlan)
                .ToListAsync();
            
            
            var StudentFee = ObjectMapper.Map<List<StudentFeeDto>>(studentFees);

            return StudentFee;
        }

        public async Task PayFee(string studentId, int feeId, double amountPaid, string paymentMethod, int discount = 0)
        {
            var studentFee = await _studentFeeRepository
                .FirstOrDefaultAsync(sf => sf.StudentId == studentId && sf.FeeId == feeId);

            if (studentFee == null)
            {
                throw new Exception("Fee not found for this student!");
            }

            if (studentFee.TotalAmount <= amountPaid)
            {
                await _studentFeeRepository.DeleteAsync(studentFee);
            }
            else
            {
                studentFee.TotalAmount -= amountPaid;
                await _studentFeeRepository.UpdateAsync(studentFee);
            }

            var payment = new Payment
            {
                StudentId = studentId,
                FeeId = feeId,
                AmountPaid = amountPaid,
                PaymentDate = DateTime.Now,
                PaymentMethod = paymentMethod,
                Discount = discount
            };

            await _paymentRepository.InsertAsync(payment);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        // Class Management

        public async Task<ClassDto> GetClassAsync(int id)
        {
            var classEntity = await _classRepository.GetAll()
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.ClassId == id); // Ensure this matches the property name

            if (classEntity == null)
            {
                throw new UserFriendlyException("Class not found!");
            }

            var classDto = ObjectMapper.Map<ClassDto>(classEntity);
            return classDto;
        }

        public async Task<List<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _classRepository.GetAllListAsync();
            return ObjectMapper.Map<List<ClassDto>>(classes);
        }

        public async Task CreateOrUpdateClassAsync(CreateClassDto input)
        {
            
            var classEntity = ObjectMapper.Map<Class>(input);
            if (classEntity.Id == 0)
            {
                await _classRepository.InsertAsync(classEntity);
            }
            else
            {
                await _classRepository.UpdateAsync(classEntity);
            }
        }
        public async Task DeleteClassAsync(int id)
        {
            var _class = _classRepository
                .GetAllIncluding(cl => cl.Students)
                .Where(c=>c.Students.Count==0)
                .FirstOrDefault(cl => cl.ClassId == id);
            await _classRepository.DeleteAsync(_class);
        }

        
        
        private List<Student> getstudentFunc(int currTenantId)
        {
            if (currTenantId == 0){
                return  _studentRepository
                    .GetAll()
                    .Include(s => s.Class)
                    .Include(s => s.StudentFees)
                    .ToList();  
            }else
            {
                return _studentRepository
                    .GetAll()
                    .Include(s => s.Class)
                    .Where(S => S.TenantId == currTenantId)
                    .Include(s => s.StudentFees)
                    .ToList();
            }
        }
        
        private async Task<string> GenerateCustomStudentId(int tenantId, int classId)
        {
            string fixedRandomNumber = "0967";
            string tenantIdPart = tenantId.ToString("D3"); //  it's always 3 digits
            string classIdPart = classId.ToString("D2"); //  it's always 2 digits

            // Retrieve the highest existing student number for this class and tenant
            var lastStudentInClass = await _studentRepository
                .GetAll()
                .Where(s => s.TenantId == tenantId && s.ClassId == classId)
                .OrderByDescending(s => s.StudentId)
                .FirstOrDefaultAsync();

            // Determine the next incremental number
            int nextIncrementalNumber = 1;
            if (lastStudentInClass != null)
            {
                string lastStudentId = lastStudentInClass.StudentId;
                string lastIncrementalPart = lastStudentId.Substring(9); // Extracting the last 3 digits
                if (int.TryParse(lastIncrementalPart, out int lastIncrement))
                {
                    nextIncrementalNumber = lastIncrement + 1;
                }
            }

            string incrementalPart = nextIncrementalNumber.ToString("D3"); // it's always 3 digits

            // Constructing the final Student ID
            string studentId = $"{fixedRandomNumber}{tenantIdPart}{classIdPart}{incrementalPart}";
            return studentId;
        }


        
        private double CalculateAmountPerPeriod(double totalFeeAmount, int intervalInMonths)
        {
            // Handling different payment intervals
            if (intervalInMonths == 1) // Monthly
            {
                return totalFeeAmount / 12;
            }
            else if (intervalInMonths == 4) // Quarterly
            {
                return totalFeeAmount / 4;
            }
            else if (intervalInMonths == 12) // Yearly
            {
                return totalFeeAmount; // No division needed, as it’s a one-time payment.
            }
            else
            {
                throw new Exception("Unsupported payment plan interval."); // Handle unsupported intervals.
            }
        }

    }
}


