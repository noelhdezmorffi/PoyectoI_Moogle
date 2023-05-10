namespace MoogleEngine;

public class SearchItem
{
    public SearchItem(string title, string snippet, double score)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    public string Title { get; set; }

    public string Snippet { get; set; }

    public double Score { get; set; }

    public bool docValido = false; // para funcionalidades opcionales
}