using Microsoft.EntityFrameworkCore;
using DisneyApi.Models;

namespace DisneyApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) {}

        public DbSet<Character> Characters {get; set;}
        public DbSet<Media> Medias {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Media>()
                .HasKey(m => new {m.Id, m.MediaType});
            
            modelBuilder.Entity<Character>()
                .HasMany(c => c.Medias)
                .WithMany(m => m.Characters)
                .UsingEntity<Dictionary<string, object>>(
                    "CharacterMedia",
                    j => j.HasOne<Media>()
                        .WithMany()
                        .HasForeignKey("MediId", "MediaType"),
                    j => j.HasOne<Character>()
                        .WithMany()
                        .HasForeignKey("CharacterId")
                );
            modelBuilder.Entity<Character>(entity =>
            {
                entity.Property(c => c.Films)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Default),
                        v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, System.Text.Json.JsonSerializerOptions.Default) ?? new string[0]);
                entity.Property(c => c.ShortFilms)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Default),
                        v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, System.Text.Json.JsonSerializerOptions.Default) ?? new string[0]);
                entity.Property(c => c.TvShows)
                    .HasConversion(
                        v => System.Text.Json.JsonSerializer.Serialize(v, System.Text.Json.JsonSerializerOptions.Default),
                        v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v, System.Text.Json.JsonSerializerOptions.Default) ?? new string[0]);
            });
        }
    }
}