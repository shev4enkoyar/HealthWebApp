using HealthWebApp.DAL.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWebApp.DAL.Models
{
    public class Specialization : BaseModel
    {
        [Required]
        public string Name { get; set; }

        public virtual List<Doctor> Doctors { get; set; }
    }
}
