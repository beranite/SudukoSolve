using System.Collections.Generic;

namespace Suduko
{
  public interface ICell
  {
    public int RowNo { get; }
    public int ColumnNo { get; }

    public string Id { get; }
  }

  public abstract class BaseCell : ICell
  {
    public int RowNo { get; }
    public int ColumnNo { get; }

    public string Id => this.CalculateId();

    protected BaseCell(int rowNo, int columnNo)
    {
      this.RowNo = rowNo;
      this.ColumnNo = columnNo;
    }

    public virtual string CalculateId()
    {
      return $"({RowNo},{ColumnNo})";
    }


    public override string ToString()
    {
      return $"{GetType().Name} {Id})";
    }
  }
}
  
