using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Project.Models;

public class Podcast
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RssFeedUrl { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int? CategoryId { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public Category? Category { get; set; }
    
    public string Language { get; set; } = "ru";
    public DateTime LastUpdatedAt { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    
    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Description: {Description}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}";
    }
}
