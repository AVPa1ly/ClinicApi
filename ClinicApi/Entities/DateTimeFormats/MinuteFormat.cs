namespace ClinicApi.Entities.DateTimeFormats
{
    public class MinuteFormat : IDateTimeFormat
    {
        public MinuteFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 999);
            MaxRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 59, 999);
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
