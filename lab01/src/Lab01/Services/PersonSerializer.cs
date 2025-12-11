using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Lab1.Models;

namespace Lab1.Services;

public class PersonSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // чтобы легче читать было
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        // если ниче нету, то не записываем
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        // чтобы кириллица не экранировалась
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string SerializeToJson(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        return JsonSerializer.Serialize(person, JsonOptions);
    }

    public Person DeserializeFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON-строка не может быть пустой", nameof(json));
        }

        var person = JsonSerializer.Deserialize<Person>(json, JsonOptions)
                     ?? throw new InvalidOperationException("не удалось десериализовать объект Person, грустно");

        return person;
    }

    // синхронно сохраняем
    public void SaveToFile(Person person, string filePath)
    {
        ArgumentNullException.ThrowIfNull(person);
        ValidateFilePath(filePath);

        var json = SerializeToJson(person);

        using var manager = new FileResourceManager(filePath);
        manager.OpenForWriting();
        manager.WriteLine(json);
    }

    public Person LoadFromFile(string filePath)
    {
        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("файл с данными не найден", filePath);
        }
        // читаем файл через file resource manager 
        using var manager = new FileResourceManager(filePath);
        manager.OpenForReading();
        var json = manager.ReadAllText();

        return DeserializeFromJson(json);
    }

    // асинхронно сохраняем (другая версия)
    public async Task SaveToFileAsync(Person person, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(person);
        ValidateFilePath(filePath);

        var json = SerializeToJson(person);

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(filePath))!);

        await using var stream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4096,
            useAsync: true);

        await using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        await writer.WriteAsync(json.AsMemory(), cancellationToken);
        await writer.FlushAsync();
    }

    public async Task<Person> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("файл с данными не найден", filePath);
        }

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true);

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var json = await reader.ReadToEndAsync(cancellationToken);

        return DeserializeFromJson(json);
    }

    // тож самое только для списков
    public void SaveListToFile(List<Person> people, string filePath)
    {
        ArgumentNullException.ThrowIfNull(people);
        ValidateFilePath(filePath);

        var json = JsonSerializer.Serialize(people, JsonOptions);

        using var manager = new FileResourceManager(filePath);
        manager.OpenForWriting();
        manager.WriteLine(json);
    }

    public List<Person> LoadListFromFile(string filePath)
    {
        ValidateFilePath(filePath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("файл со списком Person не найден", filePath);
        }

        using var manager = new FileResourceManager(filePath);
        manager.OpenForReading();
        var json = manager.ReadAllText();

        var people = JsonSerializer.Deserialize<List<Person>>(json, JsonOptions);

        return people ?? new List<Person>();
    }

    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("путь к файлу не может быть пустым", nameof(filePath));
        }
    }
}
