using ClinicApi.Extensions;

namespace ClinicApi.Entities.DateTimeFormats
{
    public class DayFormat : IDateTimeFormat
    {
        public DayFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            MaxRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).CorrectLastDay();
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
