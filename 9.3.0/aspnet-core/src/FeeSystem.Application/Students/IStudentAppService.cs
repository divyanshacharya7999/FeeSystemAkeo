using Abp.Application.Services;
using FeeSystem.Students.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeeSystem.Students
{
    public interface IStudentAppService : IApplicationService
    {
        Task<List<StudentDto>> GetAllStudents();
        Task<StudentDto> GetStudentById(string id);
        Task<StudentDto> CreateStudent(CreateStudentDto input);
        Task<StudentDto> UpdateStudent(UpdateStudentDto input);
        Task DeleteStudent(string id);
        Task<List<StudentFeeDto>> GetFeesForStudent(string studentId);
        Task PayFee(string studentId, int feeId, double amountPaid, string paymentMethod, int discount = 0);

        // Class Management
        Task<ClassDto> GetClassAsync(int id);
        Task<List<ClassDto>> GetAllClassesAsync();
        Task CreateOrUpdateClassAsync(CreateClassDto input);
        Task DeleteClassAsync(int id);
    }
}
