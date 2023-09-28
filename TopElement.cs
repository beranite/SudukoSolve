using System;
using System.Linq;
using System.Text;

namespace Suduko
{

  public class TopElement : BaseElement<CellElement>
  {

    public bool Solved { get; private set; } = false;

    public TopElement(ElementShape shape, int topValue = 9) : base(shape, topValue: 9)
    {  
    }

    protected override CellElement AddElement(int rowNo, int columnNo, int topValue = 9)
    {
      return new CellElement(Shape, rowNo, columnNo, topValue);
    }

    public Cell FetchCell(int rowNo, int columnNo)
    {
      return Element(rowNo, columnNo).Element(rowNo, columnNo);
    }

      public void SetCell(int rowNo, int columnNo, int value)
    {
      var cell = FetchCell(rowNo, columnNo);
      cell.SetValue(value);
    }

    public bool Solve(Action<string> action)
    {
      int solved = this.elements.Count((C) => C.Solved);
      
      Solved = solved == Entries;
      if (Solved)
        return false;
      bool change = false;
      foreach (var element in this.elements )
      {
        var lastChange = element.Solve();
        change |= lastChange;
        if (lastChange) action($"Element ({element.RowNo}, {element.ColumnNo})");
      }
      this.SolveSingleRowColumn(action);
      return false;
    }

    public bool SolveSingleRowColumn(Action<string> action)
    {
      bool change = false;
      foreach (var element in this.elements)
      {
        foreach (var row in element.rowRestrictions.Keys)
        {
          var removeAllowedValues = element.rowRestrictions[row];
          foreach (var el2 in this.elements)
          {
            if (el2.RowNo == element.RowNo && el2.ColumnNo != element.ColumnNo)
            {
              foreach (var cell in el2.Cells.Where(c => c.RowNo == row))
              {
                var lastChange = cell.RemoveAllowedValues(removeAllowedValues);
                change |= lastChange;
                if (lastChange) action($"Restriction Element ({element.RowNo}, {element.ColumnNo}, {removeAllowedValues})");
              }
            }
          }
        }

        foreach (var column in element.columnRestrictions.Keys)
        {
          var removeAllowedValues = element.columnRestrictions[column];
          foreach (var el2 in this.elements)
          {
            if (el2.ColumnNo == element.ColumnNo && el2.RowNo != element.RowNo)
            {
              foreach (var cell in el2.Cells.Where(c => c.ColumnNo == column))
              {
                var lastChange = cell.RemoveAllowedValues(removeAllowedValues);
                change |= lastChange;
                if (lastChange) action($"Restriction Element ({element.RowNo}, {element.ColumnNo}, {removeAllowedValues})");

              }
            }
          }
        }
        

      }
      return change;
    }

    public string Dump()
    {
      var sb = new StringBuilder();
      for (int i = 1; i <= Entries; i++)
      {
        for (int j = 1; j <= Entries; j++)
        {
          var c = FetchCell(i, j);
          if (c.Value == null)
            sb.Append("0");
          else
            sb.Append(c.Value);
        }
        sb.AppendLine();
      }
      return sb.ToString();
    }

    protected override int InternalRow(int row, int column)
    {
      return (row -1) / rows+1;
    }

    protected override int InternalColumn(int row, int column)
    {
      return (column - 1) / columns + 1;
    }

    protected override int ExternalRow(int row)
    {
      throw new NotImplementedException();
    }

    protected override int ExternalColumn(int column)
    {
      throw new NotImplementedException();
    }
  }
}
  
