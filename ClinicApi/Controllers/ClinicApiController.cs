using ClinicApi.Data;
using ClinicApi.Entities;
using ClinicApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ClinicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinicApiController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public ClinicApiController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
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
        public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
        {
            var patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id);

            if (patient is null)
            {
                return NotFound();
            }
            
            var patientDto = PatientConverter.ConvertToDto(patient);

            return Ok(patientDto);
        }

        /// <summary>
        /// Get PatientDtos by Date/Datetime intervals, matching the criteria
        /// </summary>
        /// <remarks>
        /// See <a href='https://www.hl7.org/fhir/search.html#date'>HERE</a> for date processing rules
        /// </remarks>
        /// <param name="date">Array of strings with prefixed Date/DateTime</param>
        /// <returns></returns>
        /// <response code="200">Success</response>
        /// <response code="404">No records found</response>
        /// <response code="400">Wrong argument(s)</response>
        [HttpGet(Name = "GetPatientByDateTimes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientDto>> GetPatientByDateTimes([FromQuery]string[] date)
        {
            var query = _db.Patients.Select(x => x);
            try
            {
                query = date.Aggregate(query, (current, d) => DateExpressionConverter.Convert(d, current));
                var patients = await query.ToListAsync();

                if (patients.IsNullOrEmpty())
                {
                    return NotFound();
                }

                var patientDtos = new List<PatientDto>(patients.Capacity);
                patientDtos.AddRange(patients.Select(PatientConverter.ConvertToDto));

                return Ok(patientDtos);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
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
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody]PatientDto patientDto)
        {
            var existingNameId = await _db.Patients
                .Where(x => x.Id == patientDto.Name.Id)
                .Select(p => p.Id)
                .SingleOrDefaultAsync();

            if (existingNameId != Guid.Empty)
            {
                ModelState.AddModelError("IdValueError", "Record with the following guid already exists");
                return BadRequest(ModelState);
            }

            if (!GenderValidator.GenderIsValid(patientDto.Gender, _config))
            {
                return InvalidGenreBadRequest();
            }

            var patient = PatientConverter.ConvertFromDto(patientDto);
            await _db.AddAsync(patient);
            await _db.SaveChangesAsync();

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
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == id);

            if (patient is null)
            {
                return NotFound();
            }

            _db.Patients.Remove(patient);
            await _db.SaveChangesAsync();
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
        ///                "family": "Сергеев",
        ///                "given": [
        ///                    "Сергей",
        ///                    "Сергеевич",
        ///                ]
        ///             }, 
        ///        "gender": "unknown",
        ///        "birthdate": "2024-05-14T18:25:43",
        ///        "active": false
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
        public async Task<IActionResult> UpdatePatient([FromBody]PatientDto patientDto)
        {
            if (!GenderValidator.GenderIsValid(patientDto.Gender, _config))
            {
                return InvalidGenreBadRequest();
            }

            var retrievedPatient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == patientDto.Name.Id);

            if (retrievedPatient is null)
            {
                return BadRequest();
            }

            var newPatientData = PatientConverter.ConvertFromDto(patientDto);

            _db.Attach(retrievedPatient);
            _db.Entry(retrievedPatient).CurrentValues.SetValues(newPatientData);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Patches patient record
        /// </summary>
        /// <remarks>
        /// See <a href='https://jsonpatch.com/'>HERE</a> for PATCH request details
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
        /// <param name="id">Guid</param>
        /// <param name="patchDto">PatchDto</param>
        /// <returns></returns>
        /// <response code="204">Successfully patched</response>
        /// <response code="400">API Error</response>
        [HttpPatch("{id:guid}", Name = "UpdatePartialPatient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialPatient(Guid id, JsonPatchDocument<PatientDto> patchDto)
        {
            var patient = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (patient is null)
            {
                return BadRequest();
            }

            var patientDto = PatientConverter.ConvertToDto(patient);
            patchDto.ApplyTo(patientDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patientUpdated = PatientConverter.ConvertFromDto(patientDto);

            if (!GenderValidator.GenderIsValid(patientUpdated.Gender, _config))
            {
                return InvalidGenreBadRequest();
            }

            _db.Update(patientUpdated);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private BadRequestObjectResult InvalidGenreBadRequest()
        {
            ModelState.AddModelError("GenderValueError", "Gender must have one of the following values: null | \"male\" | \"female\" | \"other\" | \"unknown\" ");
            return BadRequest(ModelState);
        }
    }
}
