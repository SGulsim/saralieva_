using System.Text.Json.Serialization;

namespace Lab1.Models;

public class Person
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private int _age;

    [JsonPropertyName("firstName")]
    public string FirstName
    {
        get => _firstName;
        set => _firstName = ValidateRequired(value, nameof(FirstName));
    }

    [JsonPropertyName("lastName")]
    public string LastName
    {
        get => _lastName;
        set => _lastName = ValidateRequired(value, nameof(LastName));
    }

    [JsonPropertyName("age")]
    public int Age
    {
        get => _age;
        set => _age = value >= 0
            ? value
            : throw new ArgumentOutOfRangeException(nameof(Age), value, "возраст не может быть отрицательным, вы чего");
    }

    [JsonPropertyName("email")]
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("адрес электронной почты обязателен", nameof(Email));
            }

            if (!value.Contains('@', StringComparison.Ordinal))
            {
                throw new ArgumentException("адрес электронной почты должен содержать символ '@'", nameof(Email));
            }

            _email = value.Trim();
        }
    }

    [JsonIgnore]
    // чтобы пароли не попадали в сериализованный json
    public string? Password { get; set; }

    [JsonPropertyName("fullName")]
    public string FullName => $"{FirstName} {LastName}".Trim();

    [JsonPropertyName("isAdult")]
    public bool IsAdult => Age >= 18;

    private static string ValidateRequired(string? value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"значение свойства {propertyName} обязательно", propertyName);
        }

        return value.Trim();
    }
}
