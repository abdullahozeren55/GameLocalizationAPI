namespace MyFirstApi.DTOs;

public class LanguageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IsoCode { get; set; }
    // Dikkat: IsActive, AdminNote, Font bilgileri vs. YOK.
    // Sadece bunları göndermek istiyoruz.

    // Hatta Unity'ye kolaylık olsun diye ekstra bir alan uydurabiliriz:
    public string DisplayName { get; set; } // Örn: "Turkish (tr)"

    public List<GameStringDto> GameStrings { get; set; }
}