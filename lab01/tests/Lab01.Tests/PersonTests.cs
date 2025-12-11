using System.Text.Json;
using System.Text.Json.Serialization;
using Lab1.Models;
using Xunit;

namespace Lab01.Tests;

public class PersonTests
{
    [Fact]
    public void FullName_СобираетсяИзИмениИФамилии_СУчётомTrim()
    {
        var person = new Person
        {
            FirstName = "  Иван ",
            LastName = "   Петров  ",
            Age = 20,
            Email = "ivan.petrov@example.com"
        };

        Assert.Equal("Иван Петров", person.FullName);
    }

    [Theory]
    [InlineData(17, false)]
    [InlineData(18, true)]
    [InlineData(35, true)]
    public void IsAdult_ВозвращаетКорректноеЗначение(int age, bool expected)
    {
        var person = new Person
        {
            FirstName = "Иван",
            LastName = "Петров",
            Age = age,
            Email = "ivan.petrov@example.com"
        };

        Assert.Equal(expected, person.IsAdult);
    }

    [Fact]
    public void Age_ОтрицательноеЗначение_ГенерируетArgumentOutOfRangeException()
    {
        var person = new Person
        {
            FirstName = "Иван",
            LastName = "Петров",
            Email = "ivan.petrov@example.com"
        };

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => person.Age = -1);
        Assert.Contains("возраст не может быть отрицательным", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Email_ПустоеЗначение_ГенерируетArgumentException(string? invalidEmail)
    {
        var person = new Person
        {
            FirstName = "Иван",
            LastName = "Петров"
        };

        var ex = Assert.Throws<ArgumentException>(() => person.Email = invalidEmail!);
        Assert.Contains("адрес электронной почты обязателен", ex.Message);
    }

    [Fact]
    public void Email_БезСобачки_ГенерируетArgumentException()
    {
        var person = new Person
        {
            FirstName = "Иван",
            LastName = "Петров"
        };

        var ex = Assert.Throws<ArgumentException>(() => person.Email = "ivan.petrov");
        Assert.Contains("должен содержать символ '@'", ex.Message);
    }

    [Fact]
    public void Password_НеСериализуетсяВСетиJson()
    {
        var person = new Person
        {
            FirstName = "Иван",
            LastName = "Петров",
            Age = 30,
            Email = "ivan.petrov@example.com",
            Password = "SuperSecret"
        };

        var json = JsonSerializer.Serialize(person, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.DoesNotContain("password", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void JsonPropertyName_Атрибуты_СоответствуютОжидаемымИменам()
    {
        var expected = new Dictionary<string, string>
        {
            { nameof(Person.FirstName), "firstName" },
            { nameof(Person.LastName), "lastName" },
            { nameof(Person.Age), "age" },
            { nameof(Person.Email), "email" },
            { nameof(Person.FullName), "fullName" },
            { nameof(Person.IsAdult), "isAdult" }
        };

        foreach (var (propertyName, expectedJsonName) in expected)
        {
            var property = typeof(Person).GetProperty(propertyName)
                           ?? throw new InvalidOperationException($"Свойство {propertyName} не найдено.");

            var attribute = property.GetCustomAttributes(typeof(JsonPropertyNameAttribute), inherit: false)
                                    .Cast<JsonPropertyNameAttribute>()
                                    .SingleOrDefault();

            Assert.NotNull(attribute);
            Assert.Equal(expectedJsonName, attribute!.Name);
        }
    }
}