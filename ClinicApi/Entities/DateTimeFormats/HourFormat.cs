namespace ClinicApi.Entities.DateTimeFormats
{
    public class HourFormat : IDateTimeFormat
    {
        public HourFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0);
            MaxRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 59, 59, 999);
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
