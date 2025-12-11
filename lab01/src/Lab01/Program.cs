using Lab1.Models;
using Lab1.Services;

var person = new Person
{
    FirstName = "Гульсимэнэк",
    LastName = "Саралиева",
    Age = 18,
    Email = "gulsim.saralieva@russia.ru",
    Password = "Mandarin_2007"
};

var serializer = new PersonSerializer();
var baseDirectory = AppContext.BaseDirectory;

var singlePersonPath = Path.Combine(baseDirectory, "person.json");
var peoplePath = Path.Combine(baseDirectory, "people.json");
var notesPath = Path.Combine(baseDirectory, "notes.txt");

// Синхронная сериализация одного объекта
serializer.SaveToFile(person, singlePersonPath);
var loadedPerson = serializer.LoadFromFile(singlePersonPath);

Console.WriteLine($"загружено: {loadedPerson.FullName}, взрослый: {loadedPerson.IsAdult}");

// Работа со списком
var people = new List<Person>
{
    person,
    new Person
    {
        FirstName = "Машка",
        LastName = "Рязанова",
        Age = 7,
        Email = "mashka.ryazanova@russia.ru"
    }
};

serializer.SaveListToFile(people, peoplePath);
var loadedPeople = serializer.LoadListFromFile(peoplePath);

Console.WriteLine($"в файле {peoplePath} найдено записей: {loadedPeople.Count}");

// Асинхронная работа
var asyncFilePath = Path.Combine(baseDirectory, "person_async.json");
await serializer.SaveToFileAsync(person, asyncFilePath);
var asyncLoaded = await serializer.LoadFromFileAsync(asyncFilePath);
Console.WriteLine($"асинхронно загружен: {asyncLoaded.FullName}");

// Работа с FileResourceManager
using (var resourceManager = new FileResourceManager(notesPath))
{
    resourceManager.OpenForWriting();
    resourceManager.WriteLine("лог сессии:");
    resourceManager.WriteLine(DateTimeOffset.Now.ToString("u"));
}

using (var resourceManager = new FileResourceManager(notesPath))
{
    resourceManager.AppendText("добавленная строка без перевода строки");
    resourceManager.OpenForReading();
    var log = resourceManager.ReadAllText();
    Console.WriteLine($"содержимое notes.txt:{Environment.NewLine}{log}");
}