using ClinicApi.Entities.DateTimeFormats;
using ClinicApi.Models;

namespace ClinicApi.Entities.PrefixGenerator
{
    internal abstract class PrefixGeneratorBase<T>
        where T : IDate
    {
        internal abstract IQueryable<T> GenerateWhere(IQueryable<T> query, IDateTimeFormat dateTime);
    }
}
