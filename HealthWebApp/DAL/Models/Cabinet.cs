using HealthWebApp.DAL.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWebApp.DAL.Models
{
    public class Cabinet : BaseModel
    {
        [Required]
        public int Number { get; set; }

        public virtual List<Doctor> Doctors { get; set; }
    }
}
