using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Project.Models;

public class Playlist
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public User Owner { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public ICollection<PlaylistEpisode> PlaylistEpisodes { get; set; } = new List<PlaylistEpisode>();
}
