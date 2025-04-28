using Microsoft.EntityFrameworkCore;
using ChessServer.Models;

namespace ChessServer.Data;

public class AppDbContext : DbContext
{
    // Таблицы базы данных
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<MatchmakingEntry> MatchmakingQueue { get; set; }

    // Конструктор с настройками
    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Настройка моделей (опционально)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity => {
            entity.HasIndex(u => u.Username).IsUnique(); // Уникальный Username
        });

        modelBuilder.Entity<Game>(entity => {
            entity.Property(g => g.Status)
                .HasConversion<string>(); // Хранить enum как строку в БД
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(
                "Server=localhost;Database=chess_server_db;User=chess_admin;Password=;",
                ServerVersion.AutoDetect("Server=localhost;Database=ChessDb;User=root;Password=jk3670x5ru3et9789f789rq8798;")
            );
        }
    }

}
