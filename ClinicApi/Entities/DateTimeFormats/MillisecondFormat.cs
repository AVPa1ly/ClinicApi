namespace ClinicApi.Entities.DateTimeFormats
{
    public class MillisecondFormat : IDateTimeFormat
    {
        public MillisecondFormat(DateTime dateTime)
        {
            var exactTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                dateTime.Second, dateTime.Millisecond);
            MinRespondingValue = exactTime;
            MaxRespondingValue = exactTime;
        }

        public DateTime MinRespondingValue { get; set; }
        public DateTime MaxRespondingValue { get; set; }
    }
}
