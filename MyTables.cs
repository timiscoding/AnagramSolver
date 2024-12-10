using Terminal.Gui;

namespace AnagramSolver;

public class MyTables : View
{
    public Dictionary<int, MyTable> Tables = new();
    public int MinWordLen;
    public int MaxWordLen;

    public MyTables(int minWordLength, int maxWordLength, string[] columns)
    {
        Width = Dim.Fill(1);
        Height = Dim.Fill(1);
        
        MinWordLen = minWordLength;
        MaxWordLen = maxWordLength;
        for (int i = minWordLength; i <= maxWordLength; i++)
        {
            Tables[i] = new MyTable(i, i == minWordLength ? 0 : Pos.Right(Tables[i - 1].View) + 2, 0, 40, Dim.Fill(), columns);
        }
        
        foreach (var table in Tables.Values)
        {
            Add(table.View, table.Header);
        }
    }

    public void OnCellActivated(Action<TableView.CellActivatedEventArgs> onCellActivated)
    {
        for (int i = MinWordLen; i <= MaxWordLen; i++)
        {
            Tables[i].View.CellActivated += onCellActivated;
        }
    }
    
    public void ClearTables()
    {
        foreach (var table in Tables.Values)
            table.Data.Rows.Clear();
    }

    public void ReplaceTableData(int wordLen, object[] data)
    {
            if (wordLen > MaxWordLen) return;
            Tables[wordLen].ReplaceRows(data);
    }
}