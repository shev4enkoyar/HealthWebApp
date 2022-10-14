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
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public DoctorController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region API
        /// <summary>
        /// Doctor Object Add Method
        /// </summary>
        /// <param name="doctor">Doctor object</param>
        /// <returns>Ok if success</returns>
        [HttpPost("add")]
        public async Task<IActionResult> Add(Doctor doctor)
        {
            if (doctor.Id != 0)
                return BadRequest();

            if (!CheckRelationId(doctor))
                return BadRequest("Value(s) from the linked table(s) does not exist");

            await dbContext.Doctors.AddAsync(doctor);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Doctor Object Editing Method
        /// </summary>
        /// <param name="doctor">Doctor object</param>
        /// <returns>Ok if success</returns>
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            if (doctor == null)
                return BadRequest();

            if (!dbContext.Doctors.Any(x => x.Id == doctor.Id))
                return BadRequest($"Doctor with id:{doctor.Id} not found");

            if (!CheckRelationId(doctor))
                return BadRequest("Value(s) from the linked table(s) does not exist");

            dbContext.Doctors.Update(doctor);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Removing a doctor from a database table
        /// </summary>
        /// <param name="doctorId">Doctor identifier</param>
        /// <returns>Ok if the entry is deleted, otherwise BadRequest</returns>
        [HttpGet("delete/{doctorId}")]
        public async Task<IActionResult> Delete(int doctorId)
        {
            var doctor = dbContext.Doctors.FirstOrDefault(x => x.Id == doctorId);
            if (doctor == null)
                return BadRequest($"Doctor with id:{doctorId} not found");
            dbContext.Doctors.Remove(doctor);
            await dbContext.SaveChangesAsync();
            return Ok("Successfully");
        }

        /// <summary>
        /// Method for getting doctor object for editing by its id
        /// </summary>
        /// <param name="doctorId">Doctor identifier</param>
        /// <returns>Returns the doctor object for editing</returns>
        [HttpGet("get/{doctorId}")]
        public object Get(int doctorId)
        {
            return dbContext.Doctors.Where(x => x.Id == doctorId).SelectForEdit<object>();
        }

        /// <summary>
        /// Getting all doctors paginated and sorted by column
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

            Dictionary<string, Func<Doctor, object>> sortParameters = new Dictionary<string, Func<Doctor, object>>()
            {
                {"id", x => x.Id },
                {"fullname", x => x.FullName },
                {"cabinet", x => x.Cabinet.Number },
                {"specialization", x => x.Specialization.Name },
                {"sector", x => x.Sector?.Number }
            };

            return dbContext.Doctors.AsQueryable()
                .Include(x => x.Cabinet)
                .Include(x => x.Specialization)
                .Include(x => x.Sector).ToList()
                .OrderBy(sortParameters[sortBy])
                .Select(x =>
                new
                {
                    x.FullName,
                    Cabinet = x.Cabinet.Number,
                    Specialization = x.Specialization.Name,
                    Sector = x.Sector?.Number
                })
                .Pagination(page, pageSize);
        }
        #endregion

        #region Util methods
        /// <summary>
        /// Method for checking references to other tables
        /// </summary>
        /// <param name="doctor">Doctor object</param>
        /// <returns>True if correct</returns>
        private bool CheckRelationId(Doctor doctor)
        {
            if (!dbContext.Cabinets.Any(x => x.Id == doctor.CabinetId))
                return false;
            if (!dbContext.Specializations.Any(x => x.Id == doctor.SpecializationId))
                return false;
            if (doctor.SectorId != null)
                if (!dbContext.Sectors.Any(x => x.Id == doctor.SectorId))
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
            string[] names = { "id", "fullname", "cabinet", "specialization", "sector" };
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
