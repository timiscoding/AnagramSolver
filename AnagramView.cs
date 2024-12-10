using Terminal.Gui;

namespace AnagramSolver;

public class AnagramView : View
{
    private Dictionary<int, string[]> _anagrams;

    public StatusBar StatusBar { get; }

    public AnagramView(int minWordLen, int maxWordLen)
    {
        SolverForm form = new SolverForm()
        {
            X = Pos.Center() - 10,
            Y = 0
        };
        MyTables tables = new(minWordLen, maxWordLen, ["Most-Least Frequent", "A-Z"])
        {
            X = 0,
            Y = Pos.Bottom(form) + 1
        };
        
        form.OnClear(() => tables.ClearTables());
           
        form.OnSolve((string pattern, int wordLen, string _) =>
        {
            tables.ClearTables();
            _anagrams = Anagram.Solve(pattern, wordLen);

            for (int i = minWordLen; i <= wordLen; i++)
            {
                tables.ReplaceTableData(i, CreateTableData(_anagrams[i]));
            }
        });

        form.OnFilter((string pattern, int wordLen, string filter) =>
        {
            if (!_anagrams.ContainsKey(wordLen)) return;
            tables.ClearTables();
            var filteredData = _anagrams[wordLen]
                .Where(word => filter.Length <= 0 || Anagram.ContainsAllLetters(word, filter)).ToArray();
            tables.ReplaceTableData(wordLen, CreateTableData(filteredData));
        });

        tables.OnCellActivated(args =>
        {
            var cell = args.Table.Rows[args.Row][args.Col].ToString();
            form.Pattern.Text = cell;
            form.WordLen.Text = "";
            form.Filter.Text = "";
            form.SolveButton.OnClicked();
        });
        
        StatusBar = new StatusBar([
            new StatusItem(Key.CtrlMask | Key.S, "~^S~ Solve", form.SolveButton.OnClicked),
            new StatusItem(Key.CtrlMask | Key.G, "~^G~ Clear", form.ClearButton.OnClicked),
            new StatusItem(Key.AltMask | Key.D1, "~\u23251~ Pattern", () => form.Pattern.SetFocus()),
            new StatusItem(Key.AltMask | Key.D2, "~\u23252~ Word length", () => form.WordLen.SetFocus()),
            new StatusItem(Key.AltMask | Key.D3, "~\u23253~ Filter", () => form.Filter.SetFocus()),
            new StatusItem(Key.A, $"Dictionary words: {Anagram.DictionaryWordCount:N0}", () => { },  () => false),
            new StatusItem(Key.A, $"Word lengths: {Anagram.DictionaryWordLengths:N0}", () => { },  () => false),
            new StatusItem(Key.A, $"Frequency words: {Anagram.FrequencyWordsCount:N0}", () => { },  () => false)
        ]);

        Add(form, tables);
    }
    
    private Object[][] CreateTableData(string[] wordsByFreq)
    {
        var sorted = wordsByFreq.Order().ToArray();
        return wordsByFreq.Select((d, i) => new Object[] { d, sorted[i] }).ToArray();
    }
}