using System;
using System.Collections.Generic;

namespace Suduko
{
  public class Head 
  {
    public TopElement topElement { get; }

    public List<RowElement> rows = new List<RowElement>();
    public List<ColumnElement> columns = new List<ColumnElement>();

    Action<Head,string> action;

    public Head(ElementShape shape, Action<Head,string> action, int topValue = 9) 
    {
      this.action = action;
      topElement = new TopElement(shape, topValue);
      this.AddRowsAndColumns();
    }

    public void Solve()
    {
      

      bool change;
      do
      {
        change = DoResolve();

      }
      while (change);
    }

    public bool DoResolve()
    {
      bool change = false;
      bool lastChange = false;
      foreach (var row in rows)
      {
        lastChange = row.Solve();
        change |= lastChange;
        if (lastChange) action(this, $"row {row.RowNo}");
      }
      foreach (var column in columns)
      {
        lastChange = column.Solve();
        change |= lastChange;
        if (lastChange) action(this, $"Column {column.ColumnNo}");
      }

      lastChange = topElement.Solve((t) => action(this, t));
      change |= lastChange;
      return change;
    }




    void AddRowsAndColumns()
    {
      for (int i = 1;i<=topElement.Entries;i++)
      {
        rows.Add(new RowElement(topElement.FetchCell, i, topElement.Entries));
        columns.Add(new ColumnElement(topElement.FetchCell, i, topElement.Entries));
      }
    }

    public void SetCell(int rowNo, int columnNo, int value)
    {
      topElement.SetCell(rowNo, columnNo, value);
    }

    public string Dump()
    {
      return topElement.Dump();
    }
  }
}
  
