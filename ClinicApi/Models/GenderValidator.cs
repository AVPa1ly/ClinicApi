namespace ClinicApi.Models
{
    public static class GenderValidator
    {
        private static readonly string[] AllowedValues = { "male", "female", "other", "unknown" };

        public static bool GenderIsValid(string? gender) => gender is null || AllowedValues.Contains(gender);
    }
}
