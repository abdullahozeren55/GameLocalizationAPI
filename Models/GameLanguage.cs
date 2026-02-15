using MyFirstApi.Models;
using System.ComponentModel.DataAnnotations;

public class GameLanguage
{
    [Required(ErrorMessage = "ID zorunludur.")]
    [Range(1, 1000, ErrorMessage = "ID 1 ile 1000 arasında olmalıdır.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Dil adı boş olamaz.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Dil adı 2-20 karakter olmalıdır.")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "ISO Kodu zorunludur.")]
    [RegularExpression(@"^[a-z]{2}$", ErrorMessage = "ISO Kodu tam olarak 2 küçük harf olmalıdır (ör: tr, en).")]
    public required string IsoCode { get; set; }

    // Bu veri SADECE backend'de kalmalı, Unity bunu görmemeli!
    public string AdminNote { get; set; } = "Bu dili sakın silmeyin, patron kızar.";

    // Bir dilin birden fazla kelimesi olabilir
    public List<GameString> GameStrings { get; set; } = new List<GameString>();

    public bool IsActive { get; set; }
}