using Lab03.Collections;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("SimpleList<int>");
var list = new SimpleList<int> { 1, 2, 3 };
list.Add(4);
list.Insert(1, 42);
Console.WriteLine("содержимое: " + string.Join(", ", list));
Console.WriteLine($"элемент по индексу 1: {list[1]}");
list.RemoveAt(2);
Console.WriteLine("после RemoveAt(2): " + string.Join(", ", list));
Console.WriteLine();

Console.WriteLine("DoublyLinkedList<string>");
var linked = new DoublyLinkedList<string>();
linked.Add("mama");
linked.Add("papa");
linked.Add("babushka");
linked.Insert(1, "sestra");
Console.WriteLine("содержимое: " + string.Join(", ", linked));
linked.Remove("papa");
Console.WriteLine("После Remove(\"papa\"): " + string.Join(", ", linked));
Console.WriteLine();

Console.WriteLine("SimpleDictionary<string, int>");
var dict = new SimpleDictionary<string, int>();
dict.Add("one", 1);
dict.Add("two", 2);
dict["three"] = 3;
Console.WriteLine("пары:");
foreach (var pair in dict)
{
    Console.WriteLine($"{pair.Key} => {pair.Value}");
}

if (dict.TryGetValue("two", out var value))
{
    Console.WriteLine($"значение по ключу \"two\": {value}");
}

dict.Remove("one");
Console.WriteLine("после удаления ключа \"one\":");
foreach (var pair in dict)
{
    Console.WriteLine($"{pair.Key} => {pair.Value}");
}