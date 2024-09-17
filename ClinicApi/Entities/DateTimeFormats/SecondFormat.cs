namespace ClinicApi.Entities.DateTimeFormats
{
    public class SecondFormat : IDateTimeFormat
    {
        public SecondFormat(DateTime dateTime)
        {
            MinRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 999);
            MaxRespondingValue = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 999);
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
