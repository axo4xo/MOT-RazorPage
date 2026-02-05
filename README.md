# 🎓 Razor Pages - Maturitní Příprava

Tento projekt demonstruje základní koncepty **ASP.NET Core Razor Pages** pro maturitní zkoušku.

---

## 📚 Co jsou Razor Pages?

**Razor Pages** je framework pro tvorbu webových aplikací v ASP.NET Core. Je to zjednodušená alternativa k MVC (Model-View-Controller), ideální pro stránky zaměřené na konkrétní funkci.

### Klíčové charakteristiky:
- **Page-based** (stránkově orientovaný) přístup
- Každá stránka má svůj vlastní **PageModel** (code-behind)
- Kombinuje HTML s C# pomocí **Razor syntaxe**
- Používá **Tag Helpers** pro propojení HTML s backendem

---

## 🎯 Demonstrovaný scénář

Projekt ukazuje **editaci článků** - typický CRUD scénář:

1. **Seznam článků** s tlačítkem "Edituj" (`<a>` s ID článku)
2. **Načtení dat** článku podle ID (handler s parametrem)
3. **Předvyplnění formuláře** daty článku
4. **Odeslání změn** přes POST (binding na DTO)
5. **Uložení** nových/změněných dat

---

## 🏗️ Struktura Razor Page

Každá Razor stránka se skládá ze dvou souborů:

```
Pages/
├── Demo.cshtml        ← VIEW (HTML + Razor syntaxe)
└── Demo.cshtml.cs     ← PageModel (C# logika)
```

### Demo.cshtml (View)
```html
@page                           ← Povinná direktiva - označuje to jako Razor Page
@model DemoModel                ← Propojení s PageModel třídou

<h1>@Model.Message</h1>         ← Přístup k datům z PageModel
```

### Demo.cshtml.cs (PageModel)
```csharp
public class DemoModel : PageModel    // Dědí z PageModel
{
    [BindProperty]                    // Automatický binding z formuláře
    public ArticleDto Input { get; set; }

    public void OnGet() { }           // Handler pro GET
    public void OnPost() { }          // Handler pro POST
}
```

---

## 🔗 1. Odkaz s ID článku (asp-page-handler + asp-route-*)

Každý článek má tlačítko "Edituj" - je to `<a>` tag s ID článku:

```html
<a asp-page="/Demo" 
   asp-page-handler="Edit" 
   asp-route-id="@article.Id" 
   class="btn btn-primary">
    Edituj
</a>
```

**Výsledná URL:** `/Demo?handler=Edit&id=1`

### Co dělají jednotlivé atributy:
| Atribut | Hodnota | Význam |
|---------|---------|--------|
| `asp-page` | `/Demo` | Cílová Razor stránka |
| `asp-page-handler` | `Edit` | Zavolá `OnGetEdit()` místo `OnGet()` |
| `asp-route-id` | `@article.Id` | Přidá `?id=1` do URL |

---

## 📥 2. Načtení dat článku (Handler s parametrem)

Handler `OnGetEdit` přijímá parametr `id` z URL:

```csharp
// URL: /Demo?handler=Edit&id=1
// ASP.NET automaticky extrahuje id z query stringu
public IActionResult OnGetEdit(int id)
{
    // Najdeme článek podle ID
    var article = _articles.FirstOrDefault(a => a.Id == id);

    if (article == null)
    {
        return NotFound();
    }

    // Předvyplníme DTO daty z článku
    Input = new ArticleDto
    {
        Id = article.Id,
        Title = article.Title,
        Content = article.Content,
        Author = article.Author
    };

    return Page();
}
```

---

## 📝 3. Předvyplnění formuláře (asp-for)

Tag Helper `asp-for` propojuje formulářová pole s modelem:

```html
<form method="post">
    <!-- Skryté pole pro ID -->
    <input type="hidden" asp-for="Input.Id"/>

    <!-- Textové pole pro název -->
    <label asp-for="Input.Title">Název:</label>
    <input asp-for="Input.Title" class="form-control"/>

    <!-- Textarea pro obsah -->
    <textarea asp-for="Input.Content"></textarea>

    <button type="submit">Uložit</button>
</form>
```

### Co `asp-for` automaticky generuje:

```html
<!-- Před zpracováním (Razor): -->
<input asp-for="Input.Title"/>

<!-- Po zpracování (HTML co dostane prohlížeč): -->
<input type="text" 
       id="Input_Title" 
       name="Input.Title" 
       value="Úvod do ASP.NET"/>
```

**Klíčové výhody asp-for:**
1. **Automatické `name`** - správný formát pro binding
2. **Automatické `id`** - pro propojení s `<label>`
3. **Automatické `value`** - předvyplnění z modelu
4. **Type inference** - `type="number"` pro int, atd.

---

## 🔄 4. Model Binding s [BindProperty]

`[BindProperty]` zajišťuje automatické mapování dat z formuláře:

```csharp
public class DemoModel : PageModel
{
    // Data z formuláře se automaticky namapují sem
    [BindProperty]
    public ArticleDto Input { get; set; } = new();

    public IActionResult OnPost()
    {
        // Po odeslání formuláře jsou data v Input
        // Input.Title obsahuje hodnotu z <input asp-for="Input.Title">
        // Input.Content obsahuje hodnotu z <textarea asp-for="Input.Content">

        // Validace
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Uložení...
        return Page();
    }
}
```

### Jak binding funguje:

```
1. Uživatel vyplní formulář
           ↓
2. Prohlížeč odešle POST s daty:
   Input.Id=1
   Input.Title=Nový název
   Input.Content=Nový obsah
           ↓
3. [BindProperty] namapuje data do Input:
   Input.Id = 1
   Input.Title = "Nový název"
   Input.Content = "Nový obsah"
           ↓
4. OnPost() může pracovat s daty v Input
```

---

## 💾 5. DTO (Data Transfer Object)

**DTO** je třída reprezentující data přenášená mezi formulářem a serverem:

```csharp
// DTO - pro přenos dat z/do formuláře
public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
}

// Entity - reprezentuje data v databázi
public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime CreatedAt { get; set; }  // Může mít více polí
}
```

### Proč používat DTO?
1. **Bezpečnost** - nevystavujeme všechna pole entity
2. **Validace** - můžeme přidat validační atributy
3. **Flexibilita** - struktura může být jiná než entity

---

## 🔄 Životní cyklus požadavku

### GET Request (načtení článku):
```
1. Uživatel klikne na "Edituj" (odkaz s id=1)
           ↓
2. Prohlížeč → GET /Demo?handler=Edit&id=1
           ↓
3. ASP.NET najde OnGetEdit(int id)
           ↓
4. Handler načte článek a naplní Input (DTO)
           ↓
5. Razor vyrenderuje formulář s předvyplněnými daty
           ↓
6. HTML odpověď → prohlížeč zobrazí stránku
```

### POST Request (uložení článku):
```
1. Uživatel klikne "Uložit" ve formuláři
           ↓
2. Prohlížeč → POST /Demo (s daty z formuláře)
           ↓
3. [BindProperty] namapuje data do Input
           ↓
4. ASP.NET zavolá OnPost()
           ↓
5. Handler validuje a uloží data
           ↓
6. Stránka se přerenderuje se zprávou o úspěchu
```

---

## 📊 Přehled Handler metod

| Metoda | HTTP | Kdy se volá | URL příklad |
|--------|------|-------------|-------------|
| `OnGet()` | GET | Běžné načtení | `/Demo` |
| `OnGetEdit(int id)` | GET | Editace článku | `/Demo?handler=Edit&id=1` |
| `OnGetNew()` | GET | Nový článek | `/Demo?handler=New` |
| `OnPost()` | POST | Odeslání formuláře | `/Demo` (POST) |

---

## 🏷️ Tag Helpers - přehled

| Tag Helper | Použití | Příklad |
|------------|---------|---------|
| `asp-page` | Odkaz na stránku | `<a asp-page="/Demo">` |
| `asp-page-handler` | Volání handleru | `<a asp-page-handler="Edit">` |
| `asp-route-*` | Parametry do URL | `<a asp-route-id="5">` |
| `asp-for` | Binding na model | `<input asp-for="Input.Title">` |

---

## 🚀 Spuštění projektu

```bash
# Obnovení závislostí
dotnet restore

# Spuštění
dotnet run

# Nebo s hot reload
dotnet watch run
```

Aplikace poběží na adrese zobrazené v terminálu (např. `http://localhost:5000`).

---

## 📁 Struktura projektu

```
MOT-RazorPage/
├── Pages/
│   ├── Demo.cshtml           ← Stránka s editací článků
│   ├── Demo.cshtml.cs        ← PageModel s handlery
│   ├── Shared/
│   │   └── _Layout.cshtml    ← Společný layout
│   ├── _ViewImports.cshtml   ← Importy Tag Helpers
│   └── _ViewStart.cshtml     ← Výchozí layout
├── wwwroot/                  ← Statické soubory (CSS, JS)
├── Program.cs                ← Vstupní bod aplikace
└── README.md                 ← Tento soubor
```

---

## 🎯 Shrnutí pro zkoušku

| Koncept | Popis |
|---------|-------|
| **Razor Pages** | Stránkově orientovaný framework, alternativa k MVC |
| **PageModel** | C# třída s logikou stránky (dědí z `PageModel`) |
| **Handlers** | Metody `OnGet()`, `OnPost()`, `OnGetXxx()` pro HTTP požadavky |
| **[BindProperty]** | Automatické mapování dat z formuláře na C# objekt |
| **asp-for** | Tag Helper pro binding formulářových polí |
| **asp-page-handler** | Volání konkrétního handleru z odkazu |
| **asp-route-*** | Předání parametrů do URL/handleru |
| **DTO** | Data Transfer Object - třída pro přenos dat |

---

## ✅ Co projekt demonstruje

| Požadavek | Implementace |
|-----------|--------------|
| Odkaz s ID článku | `<a asp-page-handler="Edit" asp-route-id="@article.Id">` |
| Načtení dat podle ID | `OnGetEdit(int id)` handler |
| Předvyplnění formuláře | `asp-for="Input.Title"` + data v DTO |
| Binding na DTO | `[BindProperty] ArticleDto Input` |
| Uložení změn | `OnPost()` handler s validací |

---

## 📖 Další zdroje

- [Microsoft Docs - Razor Pages](https://learn.microsoft.com/cs-cz/aspnet/core/razor-pages/)
- [ASP.NET Core Tag Helpers](https://learn.microsoft.com/cs-cz/aspnet/core/mvc/views/tag-helpers/intro)
- [Model Binding](https://learn.microsoft.com/cs-cz/aspnet/core/mvc/models/model-binding)
