using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Podcast> Podcasts { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<PlaylistEpisode> PlaylistEpisodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка Podcast
        modelBuilder.Entity<Podcast>(entity =>
        {
            entity.ToTable("podcasts");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(2000);
            
            entity.Property(e => e.RssFeedUrl)
                .HasColumnName("rss_feed_url")
                .HasMaxLength(500)
                .IsRequired();
            
            entity.Property(e => e.CoverImageUrl)
                .HasColumnName("cover_image_url")
                .HasMaxLength(500);
            
            entity.Property(e => e.CategoryId)
                .HasColumnName("category_id");
            
            entity.Property(e => e.Language)
                .HasColumnName("language")
                .HasMaxLength(10)
                .HasDefaultValue("ru");
            
            entity.Property(e => e.LastUpdatedAt)
                .HasColumnName("last_updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            entity.Property(e => e.IsFavorite)
                .HasColumnName("is_favorite")
                .HasDefaultValue(false);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.HasIndex(e => e.Name)
                .HasDatabaseName("idx_podcasts_name");
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Podcasts)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Настройка Episode
        modelBuilder.Entity<Episode>(entity =>
        {
            entity.ToTable("episodes");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.PodcastId)
                .HasColumnName("podcast_id")
                .IsRequired();
            
            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(500)
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(5000);
            
            entity.Property(e => e.PublishedAt)
                .HasColumnName("published_at")
                .IsRequired();
            
            entity.Property(e => e.DurationInSeconds)
                .HasColumnName("duration_in_seconds")
                .HasDefaultValue(0);
            
            entity.Property(e => e.AudioFileUrl)
                .HasColumnName("audio_file_url")
                .HasMaxLength(1000)
                .IsRequired();
            
            entity.Property(e => e.FileSizeInBytes)
                .HasColumnName("file_size_in_bytes");
            
            entity.Property(e => e.ProgressInSeconds)
                .HasColumnName("progress_in_seconds")
                .HasDefaultValue(0);
            
            entity.Property(e => e.IsDownloaded)
                .HasColumnName("is_downloaded")
                .HasDefaultValue(false);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.HasIndex(e => e.PodcastId)
                .HasDatabaseName("idx_episodes_podcast_id");
            
            entity.HasIndex(e => e.PublishedAt)
                .HasDatabaseName("idx_episodes_published_at");
            
            entity.HasOne(e => e.Podcast)
                .WithMany(p => p.Episodes)
                .HasForeignKey(e => e.PodcastId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Настройка User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.Email)
                .HasColumnName("email")
                .HasMaxLength(256)
                .IsRequired();
            
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(500)
                .IsRequired();
            
            entity.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.LastName)
                .HasColumnName("last_name")
                .HasMaxLength(100);
            
            entity.Property(e => e.DefaultPlaybackSpeed)
                .HasColumnName("default_playback_speed")
                .HasDefaultValue(1.0);
            
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<int>()
                .HasDefaultValue(Models.Role.User)
                .IsRequired();
            
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("idx_users_email");
        });

        // Настройка Playlist
        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.ToTable("playlists");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasMaxLength(1000);
            
            entity.Property(e => e.OwnerId)
                .HasColumnName("owner_id")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.HasIndex(e => e.OwnerId)
                .HasDatabaseName("idx_playlists_owner_id");
            
            entity.HasOne(e => e.Owner)
                .WithMany()
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Настройка Category
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
            
            entity.Property(e => e.Icon)
                .HasColumnName("icon")
                .HasMaxLength(100);
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName("idx_categories_name");
        });

        // Настройка PlaylistEpisode (промежуточная таблица)
        modelBuilder.Entity<PlaylistEpisode>(entity =>
        {
            entity.ToTable("playlist_episodes");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityColumn();
            
            entity.Property(e => e.PlaylistId)
                .HasColumnName("playlist_id")
                .IsRequired();
            
            entity.Property(e => e.EpisodeId)
                .HasColumnName("episode_id")
                .IsRequired();
            
            entity.Property(e => e.Order)
                .HasColumnName("order")
                .HasDefaultValue(0);
            
            entity.Property(e => e.AddedAt)
                .HasColumnName("added_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();
            
            entity.HasIndex(e => new { e.PlaylistId, e.EpisodeId })
                .IsUnique()
                .HasDatabaseName("idx_playlist_episodes_unique");
            
            entity.HasOne(e => e.Playlist)
                .WithMany(p => p.PlaylistEpisodes)
                .HasForeignKey(e => e.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Episode)
                .WithMany(ep => ep.PlaylistEpisodes)
                .HasForeignKey(e => e.EpisodeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

