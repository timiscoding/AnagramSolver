using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace AnagramSolver;

/*
 * word list using size 95 (insane) on US, GB dictionaries, common variants and stripped diacritics, no special lists http://wordlist.aspell.net/dicts/
 * single word (1 gram) frequencies from 1980 to 2010 on "english" corpora https://github.com/orgtre/google-books-ngram-frequency 
 */

public static class Program
{

    private const int MaxWordLen = 9;
    
    private static Dictionary<string, ulong> _wordFrequencies = new();
    private static Dictionary<int, string[]> _wordGroups = new();

    private static void Main()
    {
        StartUi();
    }
    
    static void StartUi()
    {
        Application.Init();
        
        var menu = new MenuBar([
            new MenuBarItem("_File", new MenuItem[]
            {
                new("_Quit", null, () =>
                {
                    Application.RequestStop();
                })
            })
        ]);
        
        var myColorScheme = new ColorScheme()
        {
            Normal = Attribute.Make(Color.Gray, Color.Blue),
            Focus = Attribute.Make(Color.Gray, Color.Black),
            HotNormal = Attribute.Make(Color.Brown, Color.Black),
            HotFocus = Attribute.Make(Color.Blue, Color.DarkGray),
            Disabled = Attribute.Make(Color.DarkGray, Color.Black),
        };

        var mainWin = new Window("Anagram Solver")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 1
        };

        var debugWin = new Window("debug")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(50),
            Height = Dim.Fill()
        };

        var debug = new TextView()
        {
            X = 0,
            Y = 0,
            Width = 50,
            Height = 10
        };
        
        debugWin.Add(debug);
        
        var anagramView = new AnagramView(4, 9);
        var wosView = new WosView(4, 9);
        
        var tabView = new TabView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = myColorScheme
        };
        var anagramTab = new TabView.Tab("Anagram", anagramView);
        var wosTab = new TabView.Tab("Words on Stream", wosView);
        tabView.AddTab(anagramTab, andSelect: true);
        tabView.AddTab(wosTab, andSelect: false);

        var currentTabText = tabView.SelectedTab?.Text;
        StatusBar statusBar = new StatusBar(anagramView.StatusBar.Items);
        Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(100), args =>
        {
            if (tabView.SelectedTab?.Text != currentTabText)
            {
                debug.InsertText("tab changed\n");
                currentTabText = tabView.SelectedTab?.Text;
                if (currentTabText == "Anagram")
                {
                    statusBar.Items = anagramView.StatusBar.Items;
                }
                else
                {
                    statusBar.Items = wosView.StatusBar.Items;
                }
                statusBar.SetNeedsDisplay();
            }
            
            return true;
        });
        
        mainWin.Add(tabView);
        Application.Top.Add(menu, mainWin, statusBar);
        Application.Run();
        Application.Shutdown();
    }
}