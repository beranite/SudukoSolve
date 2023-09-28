using System;
using System.Collections.Generic;

namespace Suduko
{

  public class ColumnElement : CellListElement
  {
    
    public int Column { get; }

    public ColumnElement(Func<int, int, Cell> fetchCell, int column, int topValue = 9) : 
      base(ElementShape.Column, fetchCell, 1, column, topValue: 9)
    {
      Column = column;
    }

   
    public string Dump()
    {
      return Element(1, 1).ToString();
    }

    protected override int InternalRow(int row, int column)
    {
    
      return row;
    }

    protected override int InternalColumn(int row, int column)
    {
      if (column != Column)
        throw new InvalidOperationException($"Wrong row {column}. Should be {Column}");

      return column - 1;
    }

    protected override Cell AddElement(int rowNo, int columnNo, int topValue = 9)
    {
      return fetchCell(rowNo, columnNo);
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
  
