using System.Text.Encodings.Web;
using System.Text.Json;
using Lab1.Models;
using Lab1.Services;
using Xunit;

namespace Lab01.Tests;

public class PersonSerializerTests : IDisposable
{
    private readonly string _tempDirectory;
    private readonly PersonSerializer _serializer = new();

    public PersonSerializerTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"Lab01Tests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
    }

    [Fact]
    public void SerializeToJson_ИспользуетCamelCase_ИНеВключаетПароль()
    {
        var person = CreatePerson();
        var json = _serializer.SerializeToJson(person);

        Assert.Contains("\"firstName\": \"Гульсимэнэк\"", json);
        Assert.Contains("\"lastName\": \"Саралиева\"", json);
        Assert.Contains("\"age\": 18", json);
        Assert.DoesNotContain("Password", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SerializeToJson_БезNamingPolicy_ИспользуетJsonPropertyNameАтрибуты()
    {
        var person = CreatePerson();

        var json = JsonSerializer.Serialize(person, new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

        Assert.Contains("\"firstName\":", json);
        Assert.Contains("\"lastName\":", json);
        Assert.Contains("\"age\":", json);
        Assert.Contains("\"fullName\":", json);
        Assert.Contains("\"isAdult\":", json);
    }

    [Fact]
    public void DeserializeFromJson_КорректноВосстанавливаетОбъект()
    {
        const string json = """
        {
          "firstName": "Маша",
          "lastName": "Рязанова",
          "age": 7,
          "email": "masha@example.com"
        }
        """;

        var person = _serializer.DeserializeFromJson(json);

        Assert.Equal("Маша", person.FirstName);
        Assert.Equal("Рязанова", person.LastName);
        Assert.Equal(7, person.Age);
        Assert.Equal("masha@example.com", person.Email);
        Assert.False(person.IsAdult);
    }

    [Fact]
    public void DeserializeFromJson_ПустаяСтрока_ГенерируетArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _serializer.DeserializeFromJson("  "));
    }

    [Fact]
    public void SaveToFile_А_ЗатемLoadFromFile_ВозвращаетЭквивалентныйОбъект()
    {
        var person = CreatePerson();
        var path = GetTempFilePath("person.json");

        _serializer.SaveToFile(person, path);
        var restored = _serializer.LoadFromFile(path);

        Assert.Equal(person.FirstName, restored.FirstName);
        Assert.Equal(person.LastName, restored.LastName);
        Assert.Equal(person.Age, restored.Age);
        Assert.Equal(person.Email, restored.Email);
        Assert.True(restored.IsAdult);
    }

    [Fact]
    public async Task SaveToFileAsync_И_LoadFromFileAsync_РаботаютКорректно()
    {
        var person = CreatePerson();
        var path = GetTempFilePath("person_async.json");

        await _serializer.SaveToFileAsync(person, path);
        var restored = await _serializer.LoadFromFileAsync(path);

        Assert.Equal(person.FullName, restored.FullName);
    }

    [Fact]
    public void LoadFromFile_ОтсутствующийФайл_ГенерируетFileNotFoundException()
    {
        var path = GetTempFilePath("missing.json");

        Assert.Throws<FileNotFoundException>(() => _serializer.LoadFromFile(path));
    }

    [Fact]
    public void SaveListToFile_И_LoadListFromFile_СохраняютКоллекцию()
    {
        var people = new List<Person>
        {
            CreatePerson(),
            new Person
            {
                FirstName = "Маша",
                LastName = "Рязанова",
                Age = 7,
                Email = "masha@example.com"
            }
        };

        var path = GetTempFilePath("people.json");

        _serializer.SaveListToFile(people, path);
        var restored = _serializer.LoadListFromFile(path);

        Assert.Equal(2, restored.Count);
        Assert.Equal("Гульсимэнэк Саралиева", restored[0].FullName);
        Assert.Equal("Маша Рязанова", restored[1].FullName);
    }

    [Fact]
    public void LoadListFromFile_ЕслиJsonNull_ВозвращаетПустойСписок()
    {
        var path = GetTempFilePath("nullpeople.json");
        File.WriteAllText(path, "null");

        var restored = _serializer.LoadListFromFile(path);

        Assert.Empty(restored);
    }

    [Fact]
    public void SerializeToJson_НулевойPerson_ВызываетArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _serializer.SerializeToJson(null!));
    }

    [Fact]
    public async Task SaveToFileAsync_ОтменаЧерезToken_ПрерываетОперацию()
    {
        var person = CreatePerson();
        var path = GetTempFilePath("cancelled.json");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(
            () => _serializer.SaveToFileAsync(person, path, cts.Token));
    }

    private static Person CreatePerson() => new()
    {
        FirstName = "Гульсимэнэк",
        LastName = "Саралиева",
        Age = 18,
        Email = "gulsim.saralieva@russia.ru",
        Password = "Mandarin_2007"
    };

    private string GetTempFilePath(string fileName) => Path.Combine(_tempDirectory, fileName);

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }
}
