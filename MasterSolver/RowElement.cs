using System;
using System.Collections.Generic;

namespace Suduko.MasterSolver
{

    public class RowElement : CellListElement
  {

    public int Row { get; }

    public RowElement(Func<int,int,Cell> fetchCell, int row, int topValue = 9) : 
      base(ElementShape.Row, fetchCell, row, 1, topValue)
    {      
      Row = row;
    }

   
    public string Dump()
    {
      return Element(1, 1).ToString();
    }

    protected override int InternalRow(int row, int column)
    {
      if (row != Row)
        throw new InvalidOperationException($"Wrong row {row}. Should be {Row}");
      return Row;
    }

    protected override int InternalColumn(int row, int column)
    {
      return column;
    }

    protected override Cell AddElement(int rowNo, int columnNo, int topValue = 9)
    {
      return fetchCell(rowNo, columnNo);
    }

    protected override int ExternalRow(int column)
    {
      throw new NotImplementedException();
    }

    protected override int ExternalColumn(int column)
    {
      throw new NotImplementedException();
    }
  }
}
  
