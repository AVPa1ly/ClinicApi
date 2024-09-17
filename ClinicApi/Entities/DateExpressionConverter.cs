using System.Globalization;
using System.Text.RegularExpressions;
using ClinicApi.Entities.DateTimeFormats;
using ClinicApi.Entities.PrefixGenerator;
using ClinicApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace ClinicApi.Entities
{
    public static class DateExpressionConverter
    {
        public static IQueryable<Patient> Convert(string expression, IQueryable<Patient> query)
        {
            var prefixGenerator = ProcessPrefix(expression);
            var dateTime = ProcessDateTime(expression);
            return prefixGenerator.GenerateWhere(query, dateTime);
        }

        private static PrefixGeneratorBase<Patient> ProcessPrefix(string expression)
        {
            var prefix = CheckPrefix(expression);
            return GetPrefixGenerator(prefix, expression);
        }

        private static PrefixGeneratorBase<Patient> GetPrefixGenerator(string prefix, string expression)
        {
            switch (prefix)
            {
                case "ap":
                    return new ApGenerator<Patient>();
                case "eq":
                    return new EqGenerator<Patient>();
                case "ge":
                    return new GeGenerator<Patient>();
                case "gt":
                case "sa":
                    return new GtGenerator<Patient>();
                case "le":
                    return new LeGeneratorx<Patient>();
                case "lt":
                case "eb":
                    return new LtGenerator<Patient>();
                case "ne":
                    return new NeGenerator<Patient>();
                default:
                    throw new ArgumentException($"Two-letter prefix is not valid: '{prefix}' in '{expression}'");
            }
        }

        private static string CheckPrefix(string expression)
        {
            var matches = RegexRepository.PrefixRegex.Matches(expression);
            if (matches.IsNullOrEmpty())
            {
                throw new ArgumentException($"Letter prefix not found in '{expression}'");
            }
            return matches.First().Value;
        }

        private static IDateTimeFormat ProcessDateTime(string expression)
        {
            var regexLength = RegexRepository.Collection.Length;
            for (var i = 0; i < regexLength; i++)
            {
                var currentPattern = RegexRepository.Collection[i];
                var regex = new Regex(currentPattern);
                var matches = regex.Matches(expression);

                if (matches.IsNullOrEmpty())
                {
                    continue;
                }

                var trimmedDateTime = expression[RegexRepository.PrefixMaxLength..];

                // 'yyyy' throws error via DateTime.Parse
                var dateTime = currentPattern == RegexRepository.YRegex 
                    ? new DateTime(int.Parse(trimmedDateTime), 1, 1)
                    : DateTime.Parse(trimmedDateTime);

                return GetDateTimeFormatByMatch(currentPattern, dateTime);
            }

            throw new ArgumentException($"Invalid DateTime format in '{expression}'");
        }

        private static IDateTimeFormat GetDateTimeFormatByMatch(string matchedPattern, DateTime dateTime)
        {
            switch (matchedPattern)
            {
                case RegexRepository.MsRegex:
                    return new MillisecondFormat(dateTime);
                case RegexRepository.SecRegex:
                    return new SecondFormat(dateTime);
                case RegexRepository.MinRegex:
                    return new MinuteFormat(dateTime);
                case RegexRepository.HrRegex:
                    return new HourFormat(dateTime);
                case RegexRepository.DRegex:
                    return new DayFormat(dateTime);
                case RegexRepository.MoRegex:
                    return new MonthFormat(dateTime);
                case RegexRepository.YRegex:
                    return new YearFormat(dateTime);
                default:
                    throw new ArgumentException("Invalid DateTime");
            }
        }
    }
}
