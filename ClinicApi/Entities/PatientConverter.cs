using AutoMapper;
using ClinicApi.Models.Dto;
using ClinicApi.Models;

namespace ClinicApi.Entities
{
    public static class PatientConverter
    {
        public static Patient ConvertFromDto(PatientDto patientDto)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<PatientDto, Patient>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Name.Id))
                .ForMember("Use", opt => opt.MapFrom(src => src.Name.Use))
                .ForMember("Family", opt => opt.MapFrom(src => src.Name.Family))
                .ForMember("FirstName", opt => opt.MapFrom(src => src.Name.Given.FirstOrDefault()))
                .ForMember("MiddleName", opt => opt.MapFrom(src => src.Name.Given.LastOrDefault())));
            var mapper = new Mapper(config);
            return mapper.Map<PatientDto, Patient>(patientDto);
        }

        public static PatientDto ConvertToDto(Patient patient)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Patient, NameDto>()
                    .ForMember("Given", opt => opt.MapFrom(
                        src => new[] { src.FirstName, src.MiddleName }));
                cfg.CreateMap<Patient, PatientDto>()
                    .ForMember(dest => dest.Name, opt =>
                        opt.MapFrom(src => src));
            });
            var mapper = new Mapper(config);
            return mapper.Map<Patient, PatientDto>(patient);
        }
    }
}
