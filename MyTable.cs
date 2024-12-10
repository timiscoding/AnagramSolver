using System.Data;
using Terminal.Gui;

namespace AnagramSolver;

public class MyTable
{
    public readonly DataTable Data = new();
    public readonly TableView View;
    public readonly Label Header;
    
    public int WordLength;
    
    public MyTable(int wordLen, Pos x, Pos y, Dim width, Dim height, string[] columns)
    {
        WordLength = wordLen;
        Data.Columns.Add("#");
        foreach (var column in columns)
        {
            Data.Columns.Add(column);
        }

        View = new TableView()
        {
            X = x,
            Y = y + 1,
            Width = width,
            Height = height
        };
        
        Header = new Label($"{wordLen} LETTERS")
        {
            X = x,
            Y = y,
            Width = 20,
            Height = 1
        };

        View.Table = Data;
    }

    public void ReplaceRows(Object[] rows)
    {
        Data.Clear();
        int i = 1;
        foreach (var row in rows)
        {
            if (row is object[] arr)
            {
                Data.Rows.Add([i++, ..arr]);
            }
            else
            {
                Data.Rows.Add([i++, row]);
            }
        }
        View.Redraw(View.Bounds);
    }

    public void ClearRows()
    {
        Data.Clear();
        View.Redraw(View.Bounds);
    }
}