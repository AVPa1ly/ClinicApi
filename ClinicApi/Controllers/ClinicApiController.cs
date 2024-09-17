using AutoMapper;
using ClinicApi.Data;
using ClinicApi.Models;
using ClinicApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ClinicApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Get PatientDto by Guid
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">Record not found</response>
        [HttpGet("{id:guid}", Name = "GetPatient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PatientDto> GetPatient(Guid id)
        {
            var patient = _db.Patients.FirstOrDefault(x => x.Id == id);

            if (patient is null)
            {
                return NotFound();
            }
            
            var patientDto = ConvertToDto(patient);

            return Ok(patientDto);
        }

        /// <summary>
        /// Add new patient 
        /// </summary>
        /// <remarks>
        /// Request example:
        /// 
        ///     POST
        ///     {
        ///        "name":
        ///             {
        ///                "id": "d8ff176f-bd0a-4b8e-b329-871952e32e1f",
        ///                "use": "official",
        ///                "family": "Иванов",
        ///                "given": [
        ///                    "Иван",
        ///                    "Иванович",
        ///                ]
        ///             }, 
        ///        "gender": "male",
        ///        "birthdate": "2024-09-16T23:13:51",
        ///        "active": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="patientDto">PatientDto</param>
        /// <returns></returns>
        /// <response code="201">Successfully created</response>
        /// <response code="400">Duplicate Guid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PatientDto> CreatePatient([FromBody]PatientDto patientDto)
        {
            var existingNameId = _db.Patients
                .Where(x => x.Id == patientDto.Name.Id)
                .Select(p => p.Id)
                .SingleOrDefault();

            if (existingNameId != Guid.Empty)
            {
                ModelState.AddModelError("IdValueError", "Record with the following guid already exists");
                return BadRequest(ModelState);
            }

            if (!GenderValidator.GenderIsValid(patientDto.Gender))
            {
                return InvalidGenreBadRequest();
            }

            var patient = ConvertFromDto(patientDto);
            _db.Add(patient);
            _db.SaveChanges();

            return CreatedAtRoute("GetPatient", new { id = patientDto.Name.Id }, patientDto);
        }

        /// <summary>
        /// Delete patient by Guid
        /// </summary>
        /// <param name="id">Guid</param>
        /// <returns></returns>
        /// <response code="204">Successfully deleted</response>
        /// <response code="400">Record not found</response>
        [HttpDelete("{id:guid}", Name = "DeletePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeletePatient(Guid id)
        {
            var patient = _db.Patients.FirstOrDefault(x => x.Id == id);

            if (patient is null)
            {
                return NotFound();
            }

            _db.Patients.Remove(patient);
            _db.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Full patient record update
        /// </summary>
        /// <remarks>
        /// Request example:
        /// 
        ///     PUT
        ///     {
        ///        "name":
        ///             {
        ///                "id": "d8ff176f-bd0a-4b8e-b329-871952e32e1f",
        ///                "use": "official",
        ///                "family": "Иванов",
        ///                "given": [
        ///                    "Иван",
        ///                    "Иванович",
        ///                ]
        ///             }, 
        ///        "gender": "male",
        ///        "birthdate": "2024-09-16T23:13:51",
        ///        "active": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="patientDto">PatientDto</param>
        /// <returns></returns>
        /// <response code="204">Successfully updated</response>
        /// <response code="400">API Error</response>
        [HttpPut(Name = "UpdatePatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePatient([FromBody]PatientDto patientDto)
        {
            if (!GenderValidator.GenderIsValid(patientDto.Gender))
            {
                return InvalidGenreBadRequest();
            }

            var retrievedPatient = _db.Patients.FirstOrDefault(x => x.Id == patientDto.Name.Id);

            if (retrievedPatient is null)
            {
                return BadRequest();
            }

            var newPatientData = ConvertFromDto(patientDto);

            _db.Attach(retrievedPatient);
            _db.Entry(retrievedPatient).CurrentValues.SetValues(newPatientData);
            _db.SaveChanges();
            return NoContent();
        }

        /// <summary>
        /// Patches patient record
        /// </summary>
        /// <remarks>
        /// See https://jsonpatch.com/.
        /// 
        /// Request example:
        /// 
        ///     PATCH
        ///     {
        ///        "path": "/gender",
        ///        "op": "replace",
        ///        "value": "unknown"
        ///     }
        /// 
        /// </remarks>
        /// <param name="patientDto">PatientDto</param>
        /// <returns></returns>
        /// <response code="204">Successfully patched</response>
        /// <response code="400">API Error</response>
        [HttpPatch("{id:guid}", Name = "UpdatePartialPatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialPatient(Guid id, JsonPatchDocument<PatientDto> patchDto)
        {
            var patient = _db.Patients.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (patient is null)
            {
                return BadRequest();
            }

            var patientDto = ConvertToDto(patient);
            patchDto.ApplyTo(patientDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patientUpdated = ConvertFromDto(patientDto);

            _db.Update(patientUpdated);
            _db.SaveChanges();

            return NoContent();
        }

        private static Patient ConvertFromDto(PatientDto patientDto)
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

        private static PatientDto ConvertToDto(Patient patient)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Patient, NameDto>()
                    .ForMember("Given", opt => opt.MapFrom(
                        src => new [] { src.FirstName, src.MiddleName }));
                cfg.CreateMap<Patient, PatientDto>()
                    .ForMember(dest => dest.Name, opt =>
                        opt.MapFrom(src => src));
            });
            var mapper = new Mapper(config);
            return mapper.Map<Patient, PatientDto>(patient);
        }

        private BadRequestObjectResult InvalidGenreBadRequest()
        {
            ModelState.AddModelError("GenderValueError", "Gender must have one of the following values: null | \"male\" | \"female\" | \"other\" | \"unknown\" ");
            return BadRequest(ModelState);
        }
    }
}
