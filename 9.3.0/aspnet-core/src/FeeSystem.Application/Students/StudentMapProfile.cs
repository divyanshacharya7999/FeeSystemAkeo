using AutoMapper;
using FeeManagementSystem.Classes;
using FeeManagementSystem.Fees;
using FeeManagementSystem.FeeTypes;
using FeeManagementSystem.PaymentPlans;
using FeeManagementSystem.Payments;
using FeeManagementSystem.StudentFees;
using FeeManagementSystem.Students;
using FeeSystem.Fees.Dto;
using FeeSystem.Students.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeeSystem.Students
{
    public class StudentMapProfile : Profile
    {
        public StudentMapProfile()
        {
            // Student mappings
            CreateMap<CreateStudentDto, Student>();
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Class.ClassName));

            CreateMap<StudentDto, Student>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<UpdateStudentDto, Student>();
            // StudentFee mappings
            CreateMap<StudentFee, StudentFeeDto>();

            // Class mappings
            // Map from Class to ClassDto
            CreateMap<Class, ClassDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClassId))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassName));

            // Map from ClassDto to Class
            CreateMap<ClassDto, Class>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) 
                .ForMember(dest => dest.ClassId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassName));

            // Map from CreateUpdateClassDto to Class
            CreateMap<CreateClassDto, Class>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id during creation
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassName));

            // Map from Class to CreateUpdateClassDto
            CreateMap<Class, CreateClassDto>()
                .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.ClassName));


            CreateMap<StudentFee, StudentFeeDto>()
            .ForMember(dest => dest.FeeHeading, opt => opt.MapFrom(src => src.Fee.FeeHeading))
            .ForMember(dest => dest.AmountPerPeriod, opt => opt.MapFrom(src => src.AmountPerPeriod))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.PaymentPlanName, opt => opt.MapFrom(src => src.PaymentPlan.PlanName));
            //// Fee mappings
            //CreateMap<Fee, FeeDto>();
            //CreateMap<CreateUpdateFeeDto, Fee>();

            //// Payment mappings
            //CreateMap<Payment, PaymentDto>();
            //CreateMap<CreateUpdatePaymentDto, Payment>();

            //// FeeType mappings
            //CreateMap<FeeType, FeeTypeDto>();
            //CreateMap<CreateUpdateFeeTypeDto, FeeType>();

            //// PaymentPlan mappings
            //CreateMap<PaymentPlan, PaymentPlanDto>();
            //CreateMap<CreateUpdatePaymentPlanDto, PaymentPlan>();
        }
    }
}
