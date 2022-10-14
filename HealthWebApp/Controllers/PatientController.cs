using HealthWebApp.DAL;
using HealthWebApp.DAL.Models;
using HealthWebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public PatientController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region API

        /// <summary>
        /// Patient Object Add Method
        /// </summary>
        /// <param name="patient">Patient object</param>
        /// <returns>Ok if success</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add(Patient patient)
        {
            if (patient.Id != 0)
                return BadRequest();

            if (!CheckRelationId(patient))
                return BadRequest("Value(s) from the linked table(s) does not exist");

            await dbContext.Patients.AddAsync(patient);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Patient Object Editing Method
        /// </summary>
        /// <param name="doctor">Patient object</param>
        /// <returns>Ok if success</returns>
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Patient patient)
        {
            if (patient == null)
                return BadRequest();

            if (!dbContext.Patients.Any(x => x.Id == patient.Id))
                return BadRequest($"Patient with id:{patient.Id} not found");

            if (!CheckRelationId(patient))
                return BadRequest("Value(s) from the linked table(s) does not exist");

            dbContext.Patients.Update(patient);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Removing a patient from a database table
        /// </summary>
        /// <param name="patientId">Patient identifier</param>
        /// <returns>Ok if the entry is deleted, otherwise BadRequest</returns>
        [HttpGet("delete/{patientId}")]
        public async Task<IActionResult> Delete(int patientId)
        {
            var patient = dbContext.Patients.FirstOrDefault(x => x.Id == patientId);
            if (patient == null)
                return BadRequest($"Patient with id:{patientId} not found");
            dbContext.Patients.Remove(patient);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Method for getting patient object for editing by its id
        /// </summary>
        /// <param name="patientId">Patient identifier</param>
        /// <returns>Returns the patient object for editing</returns>
        [HttpGet("get/{patientId}")]
        public object Get(int patientId)
        {
            return dbContext.Patients.Where(x => x.Id == patientId).SelectForEdit<object>();
        }

        /// <summary>
        /// Getting all patients paginated and sorted by column
        /// </summary>
        /// <param name="sortBy">Column name for sorting</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of entries per page</param>
        /// <returns>Entries whose number is equal to pageSize (or 10 by default)</returns>
        [HttpGet("getlist")]
        public IEnumerable<object> GetList(string sortBy = "id", int page = 1, int pageSize = 10)
        {
            sortBy = sortBy.ToLower();
            if (!CheckSortNames(sortBy))
                return null;
            Dictionary<string, Func<Patient, object>> sortParameters = new Dictionary<string, Func<Patient, object>>()
            {
                {"id", x => x.Id },
                {"lastname", x => x.LastName},
                {"firstname", x => x.FirstName },
                {"patronymic", x => x.Patronymic },
                {"address", x => x.Address },
                {"birthday", x => x.Birthday },
                {"sex", x => x.Sex },
                {"sector", x => x.Sector.Number },
            };

            return dbContext.Patients.AsQueryable()
                .Include(x => x.Sector)
                .OrderBy(sortParameters[sortBy])
                .Select(x =>
                new
                {
                    x.LastName,
                    x.FirstName,
                    x.Patronymic,
                    x.Address,
                    x.Birthday,
                    x.Sex,
                    Sector = x.Sector.Number
                })
                .Pagination(page, pageSize);
        }
        #endregion

        #region Util methods

        /// <summary>
        /// Method for checking references to other tables
        /// </summary>
        /// <param name="patient">Patient object</param>
        /// <returns>True if correct</returns>
        private bool CheckRelationId(Patient patient)
        {
            if (!dbContext.Sectors.Any(x => x.Id == patient.SectorId))
                return false;
            return true;
        }

        /// <summary>
        /// Method for checking if a column name is in valid names
        /// </summary>
        /// <param name="sortBy">Column name for sorting</param>
        /// <returns>True if found in the list</returns>
        private bool CheckSortNames(string sortBy)
        {
            string[] names = { "id", "lastname", "firstname", "patronymic", "address", "birthday", "sex", "sector" };
            foreach (var item in names)
            {
                if (sortBy.Equals(item))
                    return true;
            }
            return false;
        }
        #endregion
    }
}
