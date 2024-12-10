using System.Globalization;
using CsvHelper;

namespace AnagramSolver;

public static class Anagram
{
    private const string GoogleFreqPath = "dictionaries/1grams_english2.csv";
    private const string WordListPath = "dictionaries/word-list.txt";
    private static Dictionary<int, string[]> _wordGroups = LoadWordList();
    private static Dictionary<string, ulong> _wordFrequencies = BuildWordFreqDictionary();
    private const int MinWordLen = 4;

    public static int DictionaryWordCount => _wordGroups.Values.Sum(words => words.Length);
    public static int DictionaryWordLengths => _wordGroups.Count;
    public static int FrequencyWordsCount => _wordFrequencies.Count;
    
    static Dictionary<int, string[]> LoadWordList()
    {
        Console.Write("[Anagram] Loading word list...");
        string[] lines = File.ReadAllLines(WordListPath);
        var res =
            (from word in lines
                where !word.Contains('\'') && word.Length <= 12
                group word by word.Length into wgroup
                select new KeyValuePair<int, string[]>(wgroup.Key, wgroup.ToArray())).ToDictionary();
        Console.WriteLine("DONE");
        return res;
    }
    
    static Dictionary<string, ulong> BuildWordFreqDictionary()
    {
        Console.Write("[Anagram] Loading word freq dictionary...");
        using var reader = new StreamReader(GoogleFreqPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var anonType = new
        {
            ngram = string.Empty,
            freq = default(ulong),
            cumshare = default(float)
        };
        var records = csv.GetRecords(anonType);
        
        var res = (from row in records
                select new KeyValuePair<string, ulong>(row.ngram, row.freq)).ToDictionary(x => x.Key, x => x.Value);

        Console.WriteLine("DONE");
        return res;
    }
    
    public static Dictionary<int, string[]> Solve(string pattern, int wordLen)
    {
        var res = new Dictionary<int, string[]>();
        if (pattern.Length <= 0) return res;

        for (int i = MinWordLen; i <= wordLen; i++)
        {
            var len = i;
            res[i] =_wordGroups[len]
                .Where(word => MatchesPattern(word, pattern, len))
                .OrderByUsageFrequency().ToArray();
        }

        return res;
    }
    
    static IEnumerable<string> OrderByUsageFrequency(this IEnumerable<string> words)
    {
        var query =
            from word in words
            let freq = _wordFrequencies.GetValueOrDefault<string, ulong>(word, 0)
            orderby freq descending
            where freq != 0
            select word;

        return query.Take(150);
    }
    
    public static bool MatchesPattern(string source, string pattern, int length)
    {
        if (source.Length != length) return false;

        int wildCards = pattern.Count(c => c == '?');
        foreach (char c in source)
        {
            int index = pattern.IndexOf(c);
            if (index < 0)
            {
                if (wildCards <= 0) return false;
                wildCards--;
            }
            else
            {
                pattern = pattern.Remove(index, 1);
            }
        }
        
        return wildCards == 0;
    }
    
    public static bool ContainsAllLetters(string source, string target)
    {
        var targetFrequency = new Dictionary<char, int>();
        foreach (char c in target)
        {
            targetFrequency[c] = targetFrequency.GetValueOrDefault(c) + 1;
        }

        foreach (var kvp in targetFrequency)
        {
            int count = source.Split(kvp.Key).Length - 1;
            if (count < kvp.Value)
            {
                return false;
            }
        }

        return true;
    }
}