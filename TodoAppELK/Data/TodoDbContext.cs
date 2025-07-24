using Microsoft.EntityFrameworkCore;
using TodoAppELK.Models.Domain;

namespace TodoAppELK.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        public DbSet<Todo> Todos { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Todo entity konfigürasyonu
        //    modelBuilder.Entity<Todo>(entity =>
        //    {
        //        // Primary key
        //        entity.HasKey(e => e.Id);

        //        // Property konfigürasyonları
        //        entity.Property(e => e.Id)
        //            .ValueGeneratedOnAdd(); // PostgreSQL için otomatik ID

        //        entity.Property(e => e.Title)
        //            .HasMaxLength(50)
        //            .IsRequired();

        //        entity.Property(e => e.Description)
        //            .HasMaxLength(100);

        //        entity.Property(e => e.IsCompleted)
        //            .IsRequired()
        //            .HasDefaultValue(false);

        //        entity.Property(e => e.CreatedDate)
        //            .IsRequired()
        //            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        //        // Tablo adı (isteğe bağlı)
        //        entity.ToTable("Todos");
        //    });
        //}
    }
}
