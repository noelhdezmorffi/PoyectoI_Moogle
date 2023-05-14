namespace MoogleEngine;


public static class Moogle
{
    static string[] filePaths = Directory.GetFiles("/home/noel/Documents/moogle-main/Content", "*.txt"); // direccion de los archivos *.txt
    static int busquedasHechas = 0;
    static int cantTxts = filePaths.Length;
    static Dictionary<string, Dictionary<string, int>> d_TitlePalabraTf = new Dictionary<string, Dictionary<string, int>>(); // Key: titulo del doc, Value: dictionary (donde Key: palabra del query, Value: tf)
    static Dictionary<string, double> d_PalabraIdf = new Dictionary<string, double>(); // Key: palabra del query, Value: idf
    static Dictionary<string, string> d_TitleText = new Dictionary<string, string>();  // Key: titulo, Value: todo el texto (para dar el item.Title y item.Snippet)


    public static SearchResult Query(string query)
    {        
        string[] palabrasDelQuery = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        SearchItem[] items = new SearchItem[cantTxts];
        
        if(busquedasHechas == 0)
        {
        Cargar_d_TitleText();
        Cargar_d_TitlePalabraTf();                                
        }     
        Inicializar_d_PalabraIdf();   // hay que inicializarlo en cada búsqueda porque depende del query
        Cargar_d_PalabraIdf(palabrasDelQuery);
        SearchItem.Inicializar(items, cantTxts);
        LLenarConTitleSnippetScore(items, palabrasDelQuery);
        OrdenaPorScore(items);
        busquedasHechas++;
        return new SearchResult(items, query);       
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
    public static int CantTxtsConPalabraX(string palabraX)
    {
        int contador = 0;
        foreach (KeyValuePair<string, Dictionary<string, int>> titlePalabraTf in d_TitlePalabraTf)
        {
            if (d_TitlePalabraTf[titlePalabraTf.Key].ContainsKey(palabraX)) contador++;
        }
        return contador;
    }
    public static void Cargar_d_TitleText()
    {
        foreach (string filepath in filePaths)
        {
            string titulo = Path.GetFileName(filepath);
            string texto = File.ReadAllText(filepath);
            if(!d_TitleText.ContainsKey(titulo))
            {
            d_TitleText.Add(titulo, texto);
            }
        }
    }
    public static void Cargar_d_TitlePalabraTf()    
    {
        foreach (KeyValuePair<string, string> titleText in d_TitleText)
        {
            string titulo = titleText.Key;
            Dictionary<string, int> d_PalabraTf = new Dictionary<string, int>();
            char[] separadores = { ' ', '.', ',', ';', ':', '{', '}', '[', ']', '"', '$', '!', '¡', '¿', '?', '#', '%', '&', '/', '(', ')', '=', '-', '_', '@' };
            string[] palabrasDelText = d_TitleText[titulo].ToLower().Split(separadores, StringSplitOptions.RemoveEmptyEntries);
            foreach (string palabra in palabrasDelText)  
            {
                if (d_PalabraTf.ContainsKey(palabra)) d_PalabraTf[palabra]++;
                else d_PalabraTf.Add(palabra, 1);
            }
            d_TitlePalabraTf.Add(titulo, d_PalabraTf);
        }
    }
    public static void Cargar_d_PalabraIdf(string[] palabras)
    {
        foreach (string palabra in palabras)
        {
            double idf = Math.Log((cantTxts + 1 / (CantTxtsConPalabraX(palabra) + 1)), 2);
            if (!d_PalabraIdf.ContainsKey(palabra))
            {
                d_PalabraIdf.Add(palabra, idf);
            }
        }
    }
    public static void LLenarConTitleSnippetScore(SearchItem[] items, string[] palabras)
    {
        int i = 0;
        {
            foreach (KeyValuePair<string, Dictionary<string, int>> titlePalabraTf in d_TitlePalabraTf)
            {
                items[i].Title = titlePalabraTf.Key;
                for (int j = 0; j < palabras.Length; j++)
                {                             // si el texto contiene la palabra
                    if (d_TitlePalabraTf[titlePalabraTf.Key].ContainsKey(palabras[j]))
                    {                       //          tf                                  *     idf   
                        items[i].Score += d_TitlePalabraTf[titlePalabraTf.Key][palabras[j]] * d_PalabraIdf[palabras[j]];
                    }
                }                
                items[i].Snippet = d_TitleText[titlePalabraTf.Key].Substring(0, Math.Min(120, d_TitleText[titlePalabraTf.Key].Length));//mejorable con la palabra de mayor idf del query
                i++;
            }
        }
    }
    public static void Inicializar_d_PalabraIdf()
    {
        d_PalabraIdf = new Dictionary<string, double>();
    }
}
