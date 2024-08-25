using System;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using FeeManagementSystem.Fees;
using FeeManagementSystem.FeeTypes;
using FeeManagementSystem.PaymentPlans;
using FeeSystem.Fees.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using FeeManagementSystem.Classes;
using FeeManagementSystem.StudentFees;
using FeeManagementSystem.Students;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeeSystem.Fees
{
 
    [AbpAuthorize]
    public class FeeAppService : ApplicationService, IFeeAppService
    {
        private readonly IRepository<Fee, int> _feeRepository;
        private readonly IRepository<FeeType, int> _feeTypeRepository;
        private readonly IRepository<PaymentPlan, int> _paymentPlanRepository;
        private readonly IRepository<Student, int> _studentRepository;
        private readonly IRepository<StudentFee, int> _studentFeeRepository;
        private readonly IRepository<Class, int> _classRepository;

        
     
        public FeeAppService(IRepository<Fee, int> feeRepository, IRepository<FeeType, int> feeTypeRepository,
            IRepository<Student, int> studentRepository,IRepository<StudentFee, int> studentFeeRepository,IRepository<Class, int> classRepository,
        IRepository<PaymentPlan, int> paymentPlanRepository)
        {
            _studentRepository = studentRepository;
            _feeRepository = feeRepository;
            _feeTypeRepository = feeTypeRepository;
            _studentFeeRepository = studentFeeRepository;
            _classRepository = classRepository;
            _paymentPlanRepository = paymentPlanRepository;
        }

            //Fee Management
        public async Task<ListResultDto<FeeDto>> GetAllAsync()
        {
            int currTenantId = (int)AbpSession.TenantId;
            var fees = await _feeRepository.GetAll()
                .Where(fe=>fe.TenantId==currTenantId)
                .Include(fe=>fe.PaymentPlan)
                .Include(fe=>fe.Class)
                .ToListAsync();
            
            
            return new ListResultDto<FeeDto>(
                ObjectMapper.Map<List<FeeDto>>(fees)
            );
        }
                
        public async Task<FeeDto> GetAsync(int id)
        {
            var fee = await _feeRepository.FirstOrDefaultAsync(x => x.FeeId == id);
            if (fee == null)
            {
                throw new EntityNotFoundException(typeof(Fee), id);
            }

            return ObjectMapper.Map<FeeDto>(fee);
        }
        
        public async Task CreateOrUpdateAsync(CreateOrUpdateFeeInput input)
        {
            if (input.FeeId.HasValue)
            {
                await UpdateAsync(input);
            }
            else
            {
                await CreateAsync(input);
            }
        }
 
        public async Task<FeeDto> CreateAsync(CreateOrUpdateFeeInput input)
{
    int localTenantId = (int)AbpSession.TenantId;

    // 1. Validate that the class exists for the tenant
    var classExists = _classRepository
        .GetAll()
        .Where(cl => cl.ClassId == input.ClassId)
        .Where(cl => cl.TenantId == localTenantId)
        .Any();

    if (!classExists)
    {
        throw new Exception("Class not found for this tenant.");
    }

    // 2. Map and create the Fee entity
    var fee = ObjectMapper.Map<Fee>(input);
    fee.TenantId = localTenantId;
    await _feeRepository.InsertAsync(fee);
    await CurrentUnitOfWork.SaveChangesAsync(); // Save to generate the FeeId

    // 3. Retrieve all students in the class
    var students = await _studentRepository
        .GetAll()
        .Where(s => s.ClassId == input.ClassId && s.TenantId == localTenantId)
        .ToListAsync();

    // 4. Add StudentFee records for each student
    foreach (var student in students)
    {
        var paymentPlan = await _paymentPlanRepository.FirstOrDefaultAsync(pp => pp.PaymentPlanId == input.PaymentPlanId);
        if (paymentPlan == null)
        {
            throw new Exception($"Payment plan not found for PaymentPlanId: {input.PaymentPlanId}");
        }

        // Calculate the amount per period based on the payment plan's interval
        double feeAmountPerPeriod = input.Amount;
        switch (paymentPlan.IntervalInMonths)
        {
            case 12: // Yearly
                feeAmountPerPeriod = input.Amount / 1;
                break;
            case 3: // Quarterly
                feeAmountPerPeriod = input.Amount / 4;
                break;
            case 1: // Monthly
                feeAmountPerPeriod = input.Amount / 12;
                break;
            default:
                throw new ArgumentException($"Unsupported interval: {paymentPlan.IntervalInMonths}");
        }

        // Create the StudentFee record
        var studentFee = new StudentFee
        {
            StudentId = student.StudentId,
            FeeId = fee.FeeId, // Use the generated FeeId
            PaymentPlanId = input.PaymentPlanId,
            AmountPerPeriod = feeAmountPerPeriod,
            TotalAmount = input.Amount, // Assuming this is the total fee amount
            TenantId = localTenantId, // Ensure TenantId is set correctly
            EffectiveFrom = DateTime.Now // Set the effective date
        };

        // Insert the StudentFee record
        await _studentFeeRepository.InsertAsync(studentFee);
    }

    try
    {
        await CurrentUnitOfWork.SaveChangesAsync(); // Save all changes

        return ObjectMapper.Map<FeeDto>(fee); // Return the created fee as a DTO
    }
    catch (Exception ex)
    {
        Logger.Error("Error occurred while creating fee", ex);
        throw;
    }
}

        protected virtual async Task UpdateAsync(CreateOrUpdateFeeInput input)
        {
            var fee = await _feeRepository.FirstOrDefaultAsync(input.FeeId.Value);
            if (fee == null)
            {
                throw new EntityNotFoundException(typeof(Fee), input.FeeId.Value);
            }

            ObjectMapper.Map(input, fee);
            await _feeRepository.UpdateAsync(fee);
        }
        
        public async Task DeleteAsync(int id)
        {
            var fee = await _feeRepository.FirstOrDefaultAsync(s => s.FeeId == id);
            if (fee == null)
            {
                throw new EntityNotFoundException(typeof(Fee), id);
            }

            await _feeRepository.DeleteAsync(fee);
        }



        //Fee Type Management
        public async Task<List<FeeTypeDto>> GetAllFeeTypes()
        {
            var feeTypes = await _feeTypeRepository.GetAllListAsync();
            return ObjectMapper.Map<List<FeeTypeDto>>(feeTypes);
        }
        
        public async Task<FeeTypeDto> CreateFeeType(CreateFeeTypeDto input)
        {
            var feeType = ObjectMapper.Map<FeeType>(input);
            feeType.TenantId = (int)AbpSession.TenantId;
            feeType.NormalizedFeeTypeName = input.FeeTypeName.ToUpper();
            await _feeTypeRepository.InsertAsync(feeType);
            return ObjectMapper.Map<FeeTypeDto>(feeType);
        }
        
        public async Task UpdateFeeType(UpdateFeeTypeDto input)
        {
            var feeType = await _feeTypeRepository.FirstOrDefaultAsync(fe=>fe.FeeTypeId== input.FeeTypeId);
            feeType.NormalizedFeeTypeName = input.FeeTypeName.ToUpper();
            ObjectMapper.Map(input, feeType);
            await _feeTypeRepository.UpdateAsync(feeType);
        }
        
        public async Task DeleteFeeType(int input)
        {
            // First, find the FeeType entity
            var feeType = await _feeTypeRepository.FirstOrDefaultAsync(s => s.FeeTypeId == input);
    
            // Check if the entity exists
            if (feeType == null)
            {
                throw new EntityNotFoundException(typeof(FeeType), input);
            }

            // Perform the deletion
            await _feeTypeRepository.DeleteAsync(feeType);
        }

        
        
        //Payment Plan Management
        public async Task<List<PaymentPlanDto>> GetAllPaymentPlans()
    {
        var paymentPlans = await _paymentPlanRepository.GetAllListAsync();
        return ObjectMapper.Map<List<PaymentPlanDto>>(paymentPlans);
    }
        public async Task<PaymentPlanDto> CreatePaymentPlan(CreatePaymentPlanDto input)
        {
            var paymentPlan = ObjectMapper.Map<PaymentPlan>(input);
            paymentPlan.TenantId = (int)AbpSession.TenantId;
            paymentPlan.NormalizedPlanName = input.PlanName.ToUpper();
            await _paymentPlanRepository.InsertAsync(paymentPlan);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<PaymentPlanDto>(paymentPlan);
        }

        public async Task UpdatePaymentPlan(UpdatePaymentPlanDto input)
        {
            var paymentPlan = await _paymentPlanRepository.FirstOrDefaultAsync(p=>p.PaymentPlanId==input.Id);
            paymentPlan.NormalizedPlanName = input.PlanName.ToUpper();
            ObjectMapper.Map(input, paymentPlan);
            await _paymentPlanRepository.UpdateAsync(paymentPlan);
        }

        public async Task DeletePaymentPlan(EntityDto<int> input)
        {
            await _paymentPlanRepository.DeleteAsync(pp=>pp.PaymentPlanId==input.Id);
        }
        
    }
}
