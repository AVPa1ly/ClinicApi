using ClinicApi.Entities.DateTimeFormats;
using ClinicApi.Models;

namespace ClinicApi.Entities.PrefixGenerator
{
    internal class NeGenerator<T> : PrefixGeneratorBase<T>
        where T : IDate
    {
        internal override IQueryable<T> GenerateWhere(IQueryable<T> query, IDateTimeFormat dateTime)
        {
            return query.Where(x => x.BirthDate < dateTime.MinRespondingValue || x.BirthDate > dateTime.MaxRespondingValue);
        }
    }
}
