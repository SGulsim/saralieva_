using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Project.Models;

public class Episode
{
    public int Id { get; set; }
    public int PodcastId { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public Podcast Podcast { get; set; } = null!;
    
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int DurationInSeconds { get; set; }
    public string AudioFileUrl { get; set; } = string.Empty;
    public long? FileSizeInBytes { get; set; }
    public int ProgressInSeconds { get; set; }
    public bool IsDownloaded { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    [JsonIgnore]
    [ValidateNever]
    public ICollection<PlaylistEpisode> PlaylistEpisodes { get; set; } = new List<PlaylistEpisode>();
}
