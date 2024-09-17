namespace ClinicApi.Entities.DateTimeFormats
{
    internal interface IDateTimeFormat
    {
        internal DateTime MinRespondingValue { get; set; }
        internal DateTime MaxRespondingValue { get; set; }
    }
}
