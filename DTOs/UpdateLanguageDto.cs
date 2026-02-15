namespace MyFirstApi.DTOs;

public class UpdateLanguageDto
{
    public required string Name { get; set; }
    public required string IsoCode { get; set; }
    public required bool IsActive { get; set; } // Admin dili pasife çekmek isteyebilir
}