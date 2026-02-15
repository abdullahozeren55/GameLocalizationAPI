using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyFirstApi.Models
{
    public class GameString
    {
        public int Id { get; set; }

        [Required]
        public required string Key { get; set; } // Örn: "main_menu_button"

        [Required]
        public required string Value { get; set; } // Örn: "Ana Menü"

        // --- İLİŞKİ KISMI (Bağlantı Kabloları) ---

        // Bu kelime hangi dile ait?
        public int GameLanguageId { get; set; }

        // O dilin detaylarına ulaşmak istersem diye
        [JsonIgnore] // Döngüye girmesin diye
        public GameLanguage? Language { get; set; }
    }
}
