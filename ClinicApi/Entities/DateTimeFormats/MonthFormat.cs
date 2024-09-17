using ClinicApi.Extensions;

namespace ClinicApi.Entities.DateTimeFormats
{
    public class MonthFormat : IDateTimeFormat
    {
        public MonthFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, dateTime.Month, 1);
            MaxRespondingValue = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month)).CorrectLastDay();
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
