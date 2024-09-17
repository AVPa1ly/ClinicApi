using ClinicApi.Entities.DateTimeFormats;
using ClinicApi.Models;

namespace ClinicApi.Entities.PrefixGenerator
{
    internal class ApGenerator<T> : PrefixGeneratorBase<T>
        where T : IDate
    {
        internal override IQueryable<T> GenerateWhere(IQueryable<T> query, IDateTimeFormat dateTime)
        {
            var difference = DateTime.Now - dateTime.MaxRespondingValue;
            var lowerBound = dateTime.MinRespondingValue.AddTicks(-difference.Ticks);
            var upperBound = dateTime.MaxRespondingValue.AddTicks(difference.Ticks);

            return query.Where(x => x.BirthDate >= lowerBound && x.BirthDate <= upperBound);
        }
    }
}
