using Terminal.Gui;

namespace AnagramSolver;

public class WosView : View
{
    public StatusBar StatusBar { get; }
    private Dictionary<int, string[]> _anagrams;
    public WosView(int minWordLen, int maxWordLen)
    {
        SolverForm form = new SolverForm()
        {
            X = Pos.Center() - 10,
            Y = 0
        };
        MyTables tables = new(minWordLen, maxWordLen, ["A-Z"])
        {
            X = 0,
            Y = Pos.Bottom(form) + 1
        };
        
        form.OnClear(() => tables.ClearTables());
           
        form.OnSolve((string pattern, int wordLen, string _) =>
        {
            tables.ClearTables();
            _anagrams = Wos.Solve(pattern, wordLen);

            for (int i = minWordLen; i <= wordLen; i++)
            {
                tables.ReplaceTableData(i, _anagrams[i]);
            }
        });

        form.OnFilter((string pattern, int wordLen, string filter) =>
        {
            if (!_anagrams.ContainsKey(wordLen)) return;
            tables.ClearTables();
            var filteredData = _anagrams[wordLen]
                .Where(word => filter.Length <= 0 || Anagram.ContainsAllLetters(word, filter)).ToArray();
            tables.ReplaceTableData(wordLen, filteredData);
        });
        
        form.OnFilterAnagrams((bool disableAnagrams, int wordLen) =>
        {
            if (_anagrams is null || !_anagrams.ContainsKey(wordLen)) return;
            tables.ClearTables();
            if (disableAnagrams)
            {
                var noAnagrams = _anagrams[wordLen]
                    .GroupBy(word => string.Concat(word.Order()))
                    .Select(group => group.First())
                    .Order()
                    .ToArray();
                tables.ReplaceTableData(wordLen, noAnagrams);
            }
            else
            {
                // bug: all words cleared in other tables
                var anagrams = _anagrams[wordLen].Order().ToArray();
                tables.ReplaceTableData(wordLen, anagrams);
            }
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
            new(Key.A, $"Dictionary words: {Wos.DictionaryWordCount:N0}", () => { }, () => false),
            new(Key.A, $"Word lengths: {Wos.DictionaryWordLengths:N0}", () => { }, () => false)
        ]);
        
        Add(form, tables);
    }
}