using System.Text.RegularExpressions;

namespace ClinicApi.Entities
{
    public static class RegexRepository
    {
        public const string MsRegex = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{4}$";
        public const string SecRegex = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$";
        public const string MinRegex = @"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}$";
        public const string HrRegex = @"\d{4}-\d{2}-\d{2}T\d{2}$";
        public const string DRegex = @"\d{4}-\d{2}-\d{2}$";
        public const string MoRegex = @"\d{4}-\d{2}$";
        public const string YRegex = @"\d{4}$";

        public const int PrefixMaxLength = 2;
        public static readonly Regex PrefixRegex = new($"^[a-z]{{{PrefixMaxLength}}}");

        public static readonly string[] Collection = { MsRegex, SecRegex, MinRegex, HrRegex, DRegex, MoRegex, YRegex };
    }
}
