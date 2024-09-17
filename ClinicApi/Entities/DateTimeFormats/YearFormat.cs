using ClinicApi.Extensions;

namespace ClinicApi.Entities.DateTimeFormats
{
    public class YearFormat : IDateTimeFormat
    {
        public YearFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, 1, 1);
            MaxRespondingValue = new DateTime(dateTime.Year, 12, 31).CorrectLastDay();
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
