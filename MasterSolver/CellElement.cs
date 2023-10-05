using System.Text;

namespace Suduko.MasterSolver
{
  public class CellElement : CellListElement
  {
   
    public CellElement(ElementShape shape, int rowNo, int columnNo, int topValue = 9) : 
      base(shape, null, rowNo, columnNo, topValue)
    {  
    }

    protected override Cell AddElement(int rowNo, int columnNo, int topValue = 9)
    {
      var c = new Cell(this,rowNo, columnNo, topValue);
      var i = c.Id;
      return c;
    }

    public string Dump()
    {
      var sb = new StringBuilder();
      int row = 1;
      RectangleLoop((r, c) =>
      {
        if (r > row)
          sb.AppendLine();
        sb.Append(Element(r, c).Value);
      });
      return sb.ToString();
    }

    protected override int InternalRow(int row, int column)
    {
      return ((row - 1) % rows) + 1;
    }    

    protected override int InternalColumn(int row, int column)
    {
      return ((column - 1) % columns) + 1;
    }

    protected override int ExternalRow(int row)
    {
      return ((this.RowNo-1) * rows)+row;
    }

    protected override int ExternalColumn( int column)
    {
      return ((this.ColumnNo - 1) * columns) + column;
    }

  }
}
  
