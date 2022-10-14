using HealthWebApp.DAL.Models;
using System.Linq;

namespace HealthWebApp.Extensions
{
    public static class PatientLINQExtensions
    {
        public static object SelectForEdit<T>(this IQueryable<Patient> value)
        {
            return value.Select(x => new { x.Id, x.LastName, x.FirstName, x.Patronymic, x.Address, x.Birthday, x.Sex, x.SectorId });
        }
    }
}
