using System.Net.Http.Json;
using ClinicApi.Models.Dto;

namespace TestPostRequests;

internal class Program
{
    private static readonly HttpClient Client = new HttpClient();
    private const string Uri = "https://localhost:7076/api/ClinicApi/";

    private static readonly string[] FamilyNames = { "Иванов", "Ильин", "Константинов", "Петров" };
    private static readonly string[] FirstNames = { "Иван", "Илья", "Константин", "Петр" };
    private static readonly string[] MiddleNames = { "Иванович", "Ильич", "Константинович", "Петрович" };
    private static readonly string[] Usages = { "official", "unofficial", "alias", "temporary" };
    private static readonly string[] Genders = { "male", "female", "other", "unknown" };

    private const int RequiredAmount = 100;

    private static DateTime RandomDay(Random gen)
    {
        var start = new DateTime(2023, 1, 1);
        var range = (DateTime.Today - start).Days;
        return start.AddDays(gen.Next(range));
    }

    private static async Task Main(string[] args)
    {
        var rand = new Random();
        var dataArrayLength = FamilyNames.Length;

        for (var i = 0; i < RequiredAmount; i++)
        {
            var nameDto = new NameDto
            {
                Id = new Guid(),
                Use = Usages[rand.Next(dataArrayLength)],
                Family = FamilyNames[rand.Next(dataArrayLength)],
                Given = new[]
                {
                    FirstNames[rand.Next(dataArrayLength)],
                    MiddleNames[rand.Next(dataArrayLength)]
                }
            };

            var patientDto = new PatientDto
            {
                Name = nameDto,
                Active = rand.Next(2) == 0,
                BirthDate = RandomDay(rand),
                Gender = Genders[rand.Next(dataArrayLength)]
            };

            using var response = await Client.PostAsJsonAsync(Uri, patientDto);
        }
    }
}