namespace ClinicApi.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime CorrectLastDay(this DateTime dateTime) => dateTime.AddTicks(-1).AddDays(1);
    }
}
