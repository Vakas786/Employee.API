using AutoMapper;
using Employee.API.DTOs;
using Employee.API.Models;

namespace Employee.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity -> DTO
            CreateMap<Models.Employee, EmployeeDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            // DTO -> Entity
            CreateMap<CreateEmployeeDto, Models.Employee>();

            CreateMap<UpdateEmployeeDto, Models.Employee>();

            // Department
            CreateMap<Department, Department>();
        }
    }
}