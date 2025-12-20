using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Project.Models;

public class PlaylistEpisode
{
    public int Id { get; set; }
    public int PlaylistId { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public Playlist Playlist { get; set; } = null!;
    
    public int EpisodeId { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public Episode Episode { get; set; } = null!;
    
    public int Order { get; set; }
    public DateTime AddedAt { get; set; }
}

