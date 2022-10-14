using HealthWebApp.DAL.Models.Base;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWebApp.DAL.Models
{
    public class Doctor : BaseModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [ForeignKey(nameof(Cabinet))]
        public int CabinetId { get; set; }

        [Required]
        [ForeignKey(nameof(Specialization))]
        public int SpecializationId { get; set; }

        [ForeignKey(nameof(Sector))]
        public int? SectorId { get; set; }


        [JsonIgnore]
        public virtual Cabinet Cabinet { get; set; }
        public virtual Specialization Specialization { get; set; }
        public virtual Sector Sector { get; set; }
    }
}
