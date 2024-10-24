using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Models;

namespace BusinessLayer
{
    public class BusinessProfile : Profile
    {
        public BusinessProfile()
        {
            CreateMapper();
        }

        private void CreateMapper()
        {
            CreateMap<DataEntity, BaseInfo>();
            CreateMap<Company, CompanyInfo>();

            CreateMap<Employee, EmployeeInfo>()
            .ForMember(dest => dest.OccupationName, opt => opt.MapFrom(src => src.Occupation))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.LastModifiedDateTime, opt => opt.MapFrom(src => src.LastModified));

            CreateMap<EmployeeInfo, Employee>()
                .ForMember(dest => dest.Occupation, opt => opt.MapFrom(src => src.OccupationName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.LastModified, opt => opt.MapFrom(src => src.LastModifiedDateTime));

            CreateMap<ArSubledger, ArSubledgerInfo>();
        }
    }

}