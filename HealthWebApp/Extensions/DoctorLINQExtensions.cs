using HealthWebApp.DAL.Models;
using System.Linq;

namespace HealthWebApp.Extensions
{
    public static class DoctorLINQExtensions
    {
        public static object SelectForEdit<T>(this IQueryable<Doctor> value)
        {
            return value.Select(x => new { x.Id, x.FullName, x.CabinetId, x.SpecializationId, x.SectorId });
        }
    }
}
