namespace MoogleEngine;


public static class Moogle
{
    private static string ContentPath = "/home/noel/Documents/moogle-main/Content";
    private static string[] filePaths = Directory.GetFiles(ContentPath, "*.txt"); // direccion de los archivos *.txt
    private static bool primeraBusqueda = true;
    private static int cantTxts = filePaths.Length;
    private static Dictionary<string, Dictionary<string, int>> d_TitlePalabraTf = new Dictionary<string, Dictionary<string, int>>(); // Key: titulo del doc, Value: dictionary (donde Key: palabra del query, Value: tf)
    private static Dictionary<string, double> d_PalabraIdf = new Dictionary<string, double>(); // Key: palabra del query, Value: idf
    private static Dictionary<string, string> d_TitleText = new Dictionary<string, string>();  // Key: titulo, Value: todo el texto (para dar el item.Title y item.Snippet)

    public static SearchResult Query(string query)
    {        
        string[] palabrasDelQuery = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool[] palabrasNoDebenAparecer = new bool[palabrasDelQuery.Length];  // en la posicion i, es true si la palabra i del query no debe aparecer
        bool[] palabrasSiDebenAparecer = new bool[palabrasDelQuery.Length];  // en la posicion i, es true si la palabra i del query debe aparecer
        OperadoresAparicion(palabrasDelQuery, palabrasNoDebenAparecer, palabrasSiDebenAparecer); // da valores correctos a los 3 arrays anteriores 

        SearchItem[] items = new SearchItem[cantTxts];  
        SearchResult.Inicializar(items, cantTxts); // se inicializa cada item de items, así no son null  

        if(primeraBusqueda)
        {
            Cargar_d_TitleText();
            Cargar_d_TitlePalabraTf();
        }

        Inicializar_d_PalabraIdf();   // hay que inicializar(renovar) d_PalabraIdf en cada búsqueda, porque depende del nuevo query
        Cargar_d_PalabraIdf(palabrasDelQuery);
        
        LLenarConTitleSnippetScore(items, palabrasDelQuery, palabrasNoDebenAparecer, palabrasSiDebenAparecer);

        primeraBusqueda = false;

        SearchResult result = new SearchResult(items, query);       
        result.OrdenaPorScore();
        return result;
    }

    private static int CantTxtsConPalabraX(string palabraX)
    {
        int contador = 0;
        foreach (KeyValuePair<string, Dictionary<string, int>> titlePalabraTf in d_TitlePalabraTf)
        {
            if (titlePalabraTf.Value.ContainsKey(palabraX)) contador++;
        }
        return contador;
    }

    private static void Cargar_d_TitleText()
    {
        foreach (string filepath in filePaths)
        {
            string tituloBad = Path.GetFileName(filepath);
            string titulo = tituloBad.Substring(0, tituloBad.Length-4); // se elimina el ".txt" 
            string texto = File.ReadAllText(filepath);
            if(!d_TitleText.ContainsKey(titulo))
            {
                d_TitleText.Add(titulo, texto);
            }
        }
    }

    private static void Cargar_d_TitlePalabraTf()    
    {
        foreach (KeyValuePair<string, string> titleText in d_TitleText)
        {
            Dictionary<string, int> d_PalabraTf = new Dictionary<string, int>();
            char[] separadores = { ' ', '.', ',', ';', ':', '{', '}', '[', ']', '"', '$', '!', '¡', '¿', '?', '%', '&', '/', '(', ')', '=', '-', '_', '@' };
            string[] palabrasDelText = titleText.Value.ToLower().Split(separadores, StringSplitOptions.RemoveEmptyEntries);
            foreach (string palabra in palabrasDelText)  
            {
                if (d_PalabraTf.ContainsKey(palabra)) d_PalabraTf[palabra]++;
                else d_PalabraTf.Add(palabra, 1);
            }
            d_TitlePalabraTf.Add(titleText.Key, d_PalabraTf);
        }
    }

    private static void Cargar_d_PalabraIdf(string[] palabras)
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

    private static void LLenarConTitleSnippetScore(SearchItem[] items, string[] palabras, bool[] palabrasNoDebenAparecer, bool[] palabrasSiDebenAparecer)
    {
        int i = 0;
        {
            foreach (KeyValuePair<string, Dictionary<string, int>> titlePalabraTf in d_TitlePalabraTf)
            {                
                string itemTitle = titlePalabraTf.Key;
                string itemSnippet = d_TitleText[titlePalabraTf.Key].Substring(0, Math.Min(350, d_TitleText[titlePalabraTf.Key].Length));//mejorable cambiando 0 por la palabra de mayor idf del query
                double itemScore = 0;
                
                for (int j = 0; j < palabras.Length; j++)
                {                             // si el texto contiene la palabra j
                    if (titlePalabraTf.Value.ContainsKey(palabras[j]))
                    {// itemScore +=             tf                    *          idf   
                        itemScore += titlePalabraTf.Value[palabras[j]] * d_PalabraIdf[palabras[j]];
                     // si la palabra j no debe aparecer(pero sí aparece por el if más externo), entonces el doc. no es válido
                        if(palabrasNoDebenAparecer[j]) items[i].docValido = false; 
                    }
                    else  // si el texto no contiene la palabra j,
                    {     // pero sí debe aparecer, entonces el doc. no es válido
                        if(palabrasSiDebenAparecer[j]) items[i].docValido = false;
                    }
                }
                if(items[i].docValido)    items[i] = new SearchItem(itemTitle, itemSnippet, itemScore);
                else/*si no es válido*/   items[i] = new SearchItem(" ", " ", -1);              
                i++;                
            }
        }
    }

    private static void Inicializar_d_PalabraIdf()
    {
        d_PalabraIdf = new Dictionary<string, double>();
    }

    private static void OperadoresAparicion(string[] palabrasDelQuery, bool[] palabraNoDebeAparecer, bool[] palabraSiDebeAparecer) 
    {
        for (int i = 0; i < palabrasDelQuery.Length; i++)
        {
            if(palabrasDelQuery[i][0] == '!') 
            {
                palabraNoDebeAparecer[i] = true;
                palabrasDelQuery[i] = palabrasDelQuery[i].Substring(1); //elimina caracter '!' de esa palabra del query
            }
            if(palabrasDelQuery[i][0] == '^') 
            {
                palabraSiDebeAparecer[i] = true;
                palabrasDelQuery[i] = palabrasDelQuery[i].Substring(1); //elimina caracter '^' de esa palabra del query
            }
        }
    }
}
