using HealthWebApp.DAL.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWebApp.DAL.Models
{
    public class Patient : BaseModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Patronymic { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        [ForeignKey(nameof(Sector))]
        public int SectorId { get; set; }


        public virtual Sector Sector { get; set; }
    }
}
