using Microsoft.EntityFrameworkCore;
using MyFirstApi.Models;

namespace MyFirstApi.Data;

// DbContext'ten miras alarak bu class'ı "Veritabanı Merkezi" yapıyoruz
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Veritabanında "Languages" adında bir tablo olsun ve "GameLanguage" kalıbını kullansın
    public DbSet<GameLanguage> Languages { get; set; }
    public DbSet<GameString> GameStrings { get; set; }
}