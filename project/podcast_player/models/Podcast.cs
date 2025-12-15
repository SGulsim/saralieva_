namespace Project.models;

public class Podcast
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Author: {Author}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}";
    }
}