namespace AnagramSolver;

public class Wos
{
    private const string WordListPath = "dictionaries/wos_word_list2.txt";
    private static Dictionary<int, string[]> _wordGroups = LoadWordList();
    public static int DictionaryWordCount => _wordGroups.Values.Sum(words => words.Length);
    public static int DictionaryWordLengths => _wordGroups.Count;
    private static int MinWordLen = 4;
    static Dictionary<int, string[]> LoadWordList()
    {
        Console.Write($"[WOS] Loading word list...");
        string[] lines = File.ReadAllLines(WordListPath);
        var res =
            (from word in lines
                group word by word.Length into wgroup
                select new KeyValuePair<int, string[]>(wgroup.Key, wgroup.ToArray())).ToDictionary();
        Console.WriteLine("DONE");
        return res;
    }
    
    public static Dictionary<int, string[]> Solve(string pattern, int wordLen, bool filterAnagrams = false)
    {
        var res = new Dictionary<int, string[]>();
        if (pattern.Length <= 0) return res;

        for (int i = MinWordLen; i <= wordLen; i++)
        {
            var len = i;
            var wordGroup = _wordGroups.GetValueOrDefault(len, new string[0]);
            var query = wordGroup
                .Where(word => Anagram.MatchesPattern(word, pattern, len));
            res[i] = query.Order().ToArray();
        }

        return res;
    }
}