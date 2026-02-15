using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Bu lazım!
using MyFirstApi.Data;
using MyFirstApi.DTOs;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguagesController : ControllerBase
{
    private readonly AppDbContext _context; // Boru hattımız

    // Constructor: Sistem ayağa kalkarken "Al sana veritabanı anahtarı" der.
    public LanguagesController(AppDbContext context)
    {
        _context = context;
    }

    // --- BURADAN SONRA METOTLARI GÜNCELLEYECEĞİZ ---

    [HttpGet]
    public async Task<IActionResult> GetAllLanguages()
    {
        var languages = await _context.Languages
            .Include(l => l.GameStrings) // Kelimeleri de dahil et
            .Select(l => new LanguageDto
            {
                Id = l.Id,
                Name = l.Name,
                IsoCode = l.IsoCode,
                DisplayName = $"{l.Name} ({l.IsoCode})",

                // Kelimeleri tek tek DTO'ya çeviriyoruz
                GameStrings = l.GameStrings.Select(s => new GameStringDto
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value
                }).ToList()
            })
            .ToListAsync();

        return Ok(languages);
    }

    [HttpPost]
    public async Task<IActionResult> AddLanguage([FromBody] CreateLanguageDto dto)
    {
        // 1. Kuryeden (DTO) gelen temiz verileri alıp, 
        // Veritabanının anladığı asıl kalıba (GameLanguage) aktarıyoruz.
        var newLanguage = new GameLanguage
        {
            Name = dto.Name,
            IsoCode = dto.IsoCode,
            // IsActive'i varsayılan olarak true yapıyorum.
            IsActive = true,
            // GameStrings listesi zaten modelin içinde "new List<GameString>()" olarak 
            // tanımlı olduğu için bizim burada boş liste oluşturmamıza gerek yok.
        };

        // 2. Artık asıl modelimizi veritabanına ekleyebiliriz
        _context.Languages.Add(newLanguage);
        await _context.SaveChangesAsync();

        return Ok(newLanguage); // Şimdilik sadece eklenen dili dönüyoruz, kelimeleri boş olacak çünkü yeni dil ekledik.
    }

    [HttpDelete("{isoCode}")] // api/languages/tr
    public async Task<IActionResult> DeleteLanguage(string isoCode)
    {
        // Include ile o dile ait kelimeleri de RAM'e çekiyoruz ki veritabanı şelale silmeyi tetikleyebilsin
        var language = await _context.Languages
            .Include(l => l.GameStrings)
            .FirstOrDefaultAsync(l => l.IsoCode.ToLower() == isoCode.ToLower());

        if (language == null) return NotFound("Silinecek dil bulunamadı.");

        // Dili siliyoruz. EF Core akıllı olduğu için "Aha, bunun altındaki kelimeler de yetim kaldı, onları da sileyim" diyecek.
        _context.Languages.Remove(language);
        await _context.SaveChangesAsync();

        return Ok($"'{language.Name}' dili ve ona ait tüm kelimeler başarıyla yok edildi.");
    }

    [HttpPut("{isoCode}")] //api/languages/tr adresine PUT isteği atacağız
    public async Task<IActionResult> UpdateLanguage(string isoCode, [FromBody] UpdateLanguageDto dto)
    {
        // 1. Dili URL'den gelen ISO koduna göre bul
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.IsoCode.ToLower() == isoCode.ToLower());

        // 2. Bulamazsan hata fırlat
        if (language == null) return NotFound("Güncellenecek dil bulunamadı.");

        // 3. Bulduğun asıl modelin içindeki verileri, DTO'dan gelen yeni verilerle ez
        language.Name = dto.Name;
        language.IsoCode = dto.IsoCode;
        language.IsActive = dto.IsActive;

        // 4. Değişiklikleri kaydet
        await _context.SaveChangesAsync();

        return Ok(language);
    }

    [HttpGet("search")] // Adresi: api/languages/search olacak
    public async Task<IActionResult> SearchLanguages([FromQuery] string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return BadRequest("Lütfen bir arama kelimesi girin.");
        }

        // SQL: SELECT * FROM Languages WHERE Name LIKE '%keyword%'
        var result = await _context.Languages
                                 .Where(x => x.Name.Contains(keyword))
                                 .ToListAsync();

        if (result.Count == 0)
        {
            return NotFound("Böyle bir dil bulunamadı.");
        }

        return Ok(result);
    }

    [HttpPost("{isoCode}/strings")]
    public async Task<IActionResult> AddString(string isoCode, [FromBody] CreateGameStringDto dto) // Dikkat: Artık GameString değil, CreateGameStringDto alıyoruz!
    {
        var language = await _context.Languages.FirstOrDefaultAsync(l => l.IsoCode.ToLower() == isoCode.ToLower());
        if (language == null) return NotFound("Dil bulunamadı.");

        // Dışarıdan gelen o sade veriyi (DTO), veritabanı formatına (Model) kod içinde BİZ çeviriyoruz:
        var newString = new GameString
        {
            Key = dto.Key,
            Value = dto.Value,
            GameLanguageId = language.Id // Arka planda gizlice bağlıyoruz
            // Id = ... yazmıyoruz çünkü veritabanı kendisi +1 yapacak!
        };

        _context.GameStrings.Add(newString);
        await _context.SaveChangesAsync();
        return Ok(newString);
    }

    [HttpGet("{isoCode}/export")] // Örn: api/languages/tr/export
    public async Task<IActionResult> ExportForGame(string isoCode)
    {
        // 1. Dili ISO koduna göre bul ve sadece aktif olanı al
        // Unity mantığı: FindObjectOfType<GameLanguage>().Where(name == "tr")
        var language = await _context.Languages
            .Include(l => l.GameStrings)
            .FirstOrDefaultAsync(l => l.IsoCode.ToLower() == isoCode.ToLower());

        // 2. Dil yoksa "404 Not Found" dön
        if (language == null)
            return NotFound($"Hata: '{isoCode}' kodlu dil sistemde bulunamadı.");

        // 3. Dil pasife alınmışsa (Örn: Çevirisi bitmemiş) oyuna gönderme
        if (!language.IsActive)
            return BadRequest("Bu dil şu anda yapım aşamasında, oyuna çekilemez.");

        // 4. ŞOV KISMI (Data Shaping):
        // Unity'nin tam olarak istediği format: Dictionary<string, string>
        // Sadece "Key" ve "Value" alıyoruz. ID'leri, isimleri çöpe atıyoruz.
        var gameDictionary = language.GameStrings.ToDictionary(
            s => s.Key,
            s => s.Value
        );

        // 5. Tertemiz veriyi yolla
        return Ok(gameDictionary);
    }

    [HttpGet("{isoCode}/strings")] // Adres api/languages/tr/strings?page=1&pageSize=50
    public async Task<IActionResult> GetStringsForAdmin(
        string isoCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // 1. Önce URL'den gelen "tr" koduna sahip dilin GERÇEK ID'sini bulalım
        var language = await _context.Languages
            .FirstOrDefaultAsync(l => l.IsoCode.ToLower() == isoCode.ToLower());

        // Yoksa direkt şutla
        if (language == null) return NotFound($"'{isoCode}' kodlu dil bulunamadı.");

        // BÜYÜ BURADA: Kullanıcı "tr" girdi ama biz veritabanında arama yaparken 
        // bilgisayarın en sevdiği şey olan "language.Id" (Örn: 1) ile arama yapıyoruz. Işık hızında!

        // 2. Toplam kelime sayısını bul
        var totalItems = await _context.GameStrings.CountAsync(s => s.GameLanguageId == language.Id);

        // 3. Toplam sayfa sayısını hesapla
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // 4. Sayfalama yaparak kelimeleri çek
        var strings = await _context.GameStrings
            .Where(s => s.GameLanguageId == language.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new GameStringDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value
            })
            .ToListAsync();

        // 5. Yanıtı paketle yolla
        var response = new
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Data = strings
        };

        return Ok(response);
    }
}