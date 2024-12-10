using System.Text.RegularExpressions;
using Terminal.Gui;

namespace AnagramSolver;

public class SolverForm : View
{
    public Button ClearButton { get; }
    public TextField Filter { get; }

    public Label FilterLabel { get; }

    public TextField Pattern { get; }

    public Label PatternLabel { get; }

    public Button SolveButton { get; }

    public TextField WordLen { get; }

    public Label WordLenLabel { get; }
    
    public Label FilterAnagramsLabel { get; }
    public CheckBox FilterAnagrams { get; }
    
    private Label Debug { get; }

    public SolverForm()
    {
        Width = 42;
        Height = 10;
        
        PatternLabel = new("Pattern: (0)")
        {
            X = 0,
            Y = 0,
            Width = 20,
            Height = 1
        };
        
        Pattern = new("")
        {
            X = 0,
            Y = 1,
            Width = 20,
            Height = 1
        };
        
        WordLenLabel = new("Word length:")
        {
            X = 0,
            Y = 3,
            Width = 20,
            Height = 1
        };
        
        WordLen = new()
        {
            X = 0,
            Y = 4,
            Width = 20,
            Height = 1
        };
        
        FilterLabel = new("Filter:")
        {
            X = 0,
            Y = 6,
            Width = 20,
            Height = 1
        };
        
        Filter = new("")
        {
            X = 0,
            Y = 7,
            Width = 20,
            Height = 1
        };

        FilterAnagramsLabel = new Label("Disable anagrams:")
        {
            X = Pos.Right(Filter) + 2,
            Y = Pos.Y(FilterLabel),
            Width = 20,
            Height = 1
        };

        FilterAnagrams = new CheckBox()
        {
            X = Pos.X(FilterAnagramsLabel),
            Y = Pos.Y(Filter),
            Width = 10,
            Height = 1
        };
        
        SolveButton = new("_Solve")
        {
            X = 0,
            Y = 9,
            Width = 10,
            Height = 1
        };
        
        ClearButton = new("_Clear")
        {
            X = Pos.Right(SolveButton) + 2,
            Y = Pos.Y(SolveButton),
            Width = 10,
            Height = 1
        };

        Debug = new Label("Debug")
        {
            X = Pos.Right(ClearButton) + 2,
            Y = Pos.Y(ClearButton),
            Height = 1,
            Width = 20
        };
        
        Pattern.TextChanged += _ =>
        {
            PatternLabel.Text = $"Pattern: ({Pattern.Text.Length})";
        };
        WordLen.TextChanging += args =>
        {
            if (!Regex.IsMatch(args.NewText.ToString() ?? string.Empty, @"^\d*$"))
            {
                args.Cancel = true;
            }
        };

        Add(PatternLabel, Pattern, WordLenLabel, WordLen, FilterLabel, Filter, FilterAnagramsLabel, FilterAnagrams, SolveButton, ClearButton);
    }

    public void OnClear(Action onClear)
    {
        ClearButton.Clicked += () =>
        {
            ResetTextFields();
            Pattern.SetFocus();
            onClear?.Invoke();
        };
    }

    public void OnSolve(Action<string, int, string> onSolve)
    {
        SolveButton.Clicked += () =>
        {
            (string p, int wl, string f) = ParseFields(Pattern, WordLen, Filter);
            onSolve?.Invoke(p, wl, f);
        };
    }

    public void OnFilter(Action<string, int, string> onFilter)
    {
        Filter.TextChanged += _ =>
        {
            (string p, int wl, string f) = ParseFields(Pattern, WordLen, Filter);
            onFilter?.Invoke(p, wl, f);
        };
    }

    public void OnFilterAnagrams(Action<bool, int> onFilterAnagrams)
    {
        FilterAnagrams.Toggled += _ =>
        {
            (string p, int wl, string f) = ParseFields(Pattern, WordLen, Filter);
            onFilterAnagrams?.Invoke(FilterAnagrams.Checked, wl);
        };
    }

    private (string pattern, int wordLen, string filter) ParseFields(TextField pattern, TextField wordLen,
        TextField filter)
    {
        string p = pattern.Text.ToString() ?? string.Empty;
        string wl = wordLen.Text.ToString() ?? string.Empty;
        string f = filter.Text.ToString() ?? string.Empty;

        var parsed = int.TryParse(wl, out int len);
        len = parsed ? len : p.Length;

        return (p.Trim(), len, f.Trim());
    }

    private void ResetTextFields()
    {
        Pattern.Text = "";
        WordLen.Text = "";
        Filter.Text = "";
    }
}