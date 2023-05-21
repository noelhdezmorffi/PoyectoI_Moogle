namespace MoogleEngine;

public class SearchResult
{
    private SearchItem[] items;

    public SearchResult(SearchItem[] items, string suggestion="")
    {
        if (items == null) {
            throw new ArgumentNullException("items");
        }

        this.items = items;
        this.Suggestion = suggestion;
    }

    public static void Inicializar(SearchItem[] items,int n)
    {
        for (int i = 0; i < n; i++)
        {
            items[i] = new SearchItem(" ", " ", 0);
        }
    }

    public static void OrdenaPorScore(SearchItem[] items)
    {
        for (int k = 1; k < items.Length; k++)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                SearchItem temp = items[i];
                if (items[i].Score < items[i + 1].Score)
                {
                    items[i] = items[i + 1];
                    items[i + 1] = temp;
                }
            }
        }
    }

    public SearchResult() : this(new SearchItem[0]) {

    }

    public string Suggestion { get; private set; }

    public IEnumerable<SearchItem> Items() {
        return this.items;
    }

    public int Count { get { return this.items.Length; } }
}
