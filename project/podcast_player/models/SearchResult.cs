namespace Project.Models;

public class SearchResult
{
    public string Query { get; set; } = string.Empty;
    public List<Podcast> Podcasts { get; set; } = new();
    public List<Episode> Episodes { get; set; } = new();
}

