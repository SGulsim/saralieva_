using System.Reflection;
namespace CalculateSecret;
public class CalculateSecret
{
    private static void calculateSecret()
    {
        Console.WriteLine("скрытни метод");
    }
}

class Program
{
    static void Main()
    {
        var type = typeof(CalculateSecret);
        
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
        
        foreach (var methodInfo in methods)
        {
            if (methodInfo.Name == "calculateSecret")
            {
                Console.WriteLine($"метод: {methodInfo.Name}");
                methodInfo.Invoke(null, null);
                break;
            }
        }
    }
}