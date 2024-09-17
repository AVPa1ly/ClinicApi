namespace ClinicApi.Entities
{
    public static class GenderValidator
    {
        public static bool GenderIsValid(string? gender, IConfiguration config)
        {
            var allowedValues = config.GetSection("allowedUseValues").Get<string[]>();
            return gender is null || allowedValues.Contains(gender);
        }
    }
}
