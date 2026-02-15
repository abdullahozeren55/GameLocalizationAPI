namespace MyFirstApi.DTOs;

public class CreateGameStringDto
{
    //ID yok! GameLanguageId yok! Sadece kullanıcının girmesi gerekenler var.
    public required string Key { get; set; }
    public required string Value { get; set; }
}