using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOT_RazorPage.Pages;

/// <summary>
/// PageModel demonstrující CRUD operace s články:
/// 1. Seznam článků s tlačítkem "Edituj" (odkaz s ID)
/// 2. Načtení dat článku podle ID (OnGet s parametrem)
/// 3. Předvyplnění formuláře daty článku
/// 4. Odeslání změn přes OnPost (model binding na DTO)
/// 5. Uložení změněných dat
/// </summary>
public class DemoModel : PageModel
{
    // ===== SIMULOVANÁ "DATABÁZE" =====
    // V reálné aplikaci by data byla v databázi (Entity Framework, Dapper, ...)
    // Static = data přetrvávají mezi requesty (pro demo účely)
    private static List<Article> _articles = new()
    {
        new Article { Id = 1, Title = "Úvod do ASP.NET", Content = "ASP.NET Core je moderní framework...", Author = "Jan Novák" },
        new Article { Id = 2, Title = "Razor Pages základy", Content = "Razor Pages je page-based model...", Author = "Petra Svobodová" },
        new Article { Id = 3, Title = "Entity Framework", Content = "ORM pro práci s databází...", Author = "Martin Dvořák" },
    };

    // ===== SEZNAM ČLÁNKŮ PRO ZOBRAZENÍ =====
    public List<Article> Articles => _articles;

    // ===== DTO PRO EDITACI (Data Transfer Object) =====
    // [BindProperty] zajistí automatické mapování dat z formuláře
    // Při POST requestu se data z <input asp-for="..."> namapují sem
    [BindProperty]
    public ArticleDto Input { get; set; } = new();

    // Zpráva pro uživatele (např. "Článek byl uložen")
    public string? Message { get; set; }

    // Příznak, zda jsme v režimu editace
    public bool IsEditing { get; set; }

    // ===== HANDLER: OnGet =====
    // Voláno při běžném načtení stránky (bez parametru)
    // URL: /Demo
    public void OnGet()
    {
        // Jen zobrazíme seznam článků, žádná editace
        IsEditing = false;
    }

    // ===== HANDLER: OnGetEdit =====
    // Voláno při kliknutí na "Edituj" u článku
    // Parametr 'id' přijde z URL: /Demo?handler=Edit&id=1
    public IActionResult OnGetEdit(int id)
    {
        // Najdeme článek podle ID
        var article = _articles.FirstOrDefault(a => a.Id == id);

        if (article == null)
        {
            Message = "Článek nenalezen!";
            return Page();
        }

        // Přepneme do režimu editace
        IsEditing = true;

        // Předvyplníme formulář daty z článku
        // Mapování Entity -> DTO
        Input = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            Author = article.Author
        };

        return Page();
    }

    // ===== HANDLER: OnPost =====
    // Voláno po odeslání formuláře (method="post")
    // Data z formuláře jsou automaticky v Input díky [BindProperty]
    public IActionResult OnPost()
    {
        // Validace - kontrola povinných polí pomocí ModelState
        if (!ModelState.IsValid)
        {
            Message = "Formulář obsahuje chyby!";
            IsEditing = true;
            return Page();
        }

        // Najdeme existující článek nebo vytvoříme nový
        var article = _articles.FirstOrDefault(a => a.Id == Input.Id);

        if (article != null)
        {
            // UPDATE - aktualizace existujícího článku
            // Mapování DTO -> Entity
            article.Title = Input.Title;
            article.Content = Input.Content;
            article.Author = Input.Author;
            Message = $"Článek #{article.Id} byl úspěšně aktualizován!";
        }
        else
        {
            // INSERT - vytvoření nového článku
            var newArticle = new Article
            {
                Id = _articles.Max(a => a.Id) + 1,
                Title = Input.Title,
                Content = Input.Content,
                Author = Input.Author
            };
            _articles.Add(newArticle);
            Message = $"Nový článek #{newArticle.Id} byl vytvořen!";
        }

        // Po uložení se vrátíme do režimu seznamu
        IsEditing = false;
        return Page();
    }

    // ===== HANDLER: OnGetNew =====
    // Pro vytvoření nového článku (prázdný formulář)
    public void OnGetNew()
    {
        IsEditing = true;
        Input = new ArticleDto(); // Prázdný DTO pro nový článek
    }
}

// ===== ENTITA (reprezentuje data v databázi) =====
public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
}

// ===== DTO (Data Transfer Object) =====
// Reprezentuje data přenášená mezi formulářem a serverem
// Může mít validační atributy, odlišnou strukturu od Entity
public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
}
