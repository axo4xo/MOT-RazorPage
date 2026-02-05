using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MOT_RazorPage.Pages;

public class ArticlePage : PageModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    
    public void OnGetRead(int id)
    {
        var demo = new DemoModel();
        var _articles = demo.Articles;
        var article = _articles.FirstOrDefault(a => a.Id == id);
        if (article != null)
        {
            Title = article.Title;
            Content = article.Content;
            Author = article.Author;
        }
        else
        {
             Response.Redirect("/notfoudn");
        }
    }
}