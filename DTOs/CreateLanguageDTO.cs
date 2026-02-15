namespace MyFirstApi.DTOs;

public class CreateLanguageDto
{
    public required string Name { get; set; }
    public required string IsoCode { get; set; }
    // GameStrings listesi BURADA YOK! O yüzden Scalar bizden kelime istemeyecek.
}