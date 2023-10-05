using System;
using System.Collections.Generic;
using System.Linq;

namespace Suduko.MasterSolver
{
  public enum ElementShape
  {
    Row, Column, Rectangle
  }

  public abstract class BaseElement<C> : BaseCell where C: BaseCell
  {
    public ElementShape Shape { get; }
    protected readonly Func<int, int, Cell> fetchCell;
    public int Entries { get; }
    protected List<C> elements = new List<C>();
    public List<C> Elements { get { return elements.ToList<C>(); } }
    protected int rows;
    protected int columns;
    


    protected BaseElement(ElementShape shape, int rowNo = 1, int columnNo = 1, int topValue = 9, Func<int, int, Cell> fetchCell = null) : base(rowNo, columnNo)
    {
      this.Shape = shape;
      this.Entries = topValue;
      this.fetchCell = fetchCell;
      var suggested = System.Math.Sqrt(Entries);
      rows = (int)suggested;
      columns = Entries / rows;
      this.Build();
    }

    void BuildBase(Func<IEnumerable<C>> build)
    {
      foreach (var cell in build())
      {
        this.elements.Add(cell);
      }
    }
    void Build()
    {
      switch (Shape)
      {
        case ElementShape.Rectangle:
          this.BuildBase(BuildRectangle);
          break;
        case ElementShape.Row:
          this.BuildBase(BuildRow);
          break;

        case ElementShape.Column:
          this.BuildBase(BuildColumn);
          break;
      }
    }

    IEnumerable<C> BuildColumn()
    {
      var list = new List<C>();

      ColumnLoop((r, c) =>
      {
        list.Add(AddElement(r, c, Entries));
      });
      return list;
    }

    IEnumerable<C> BuildRow()
    {
      var list = new List<C>();

      RowLoop((r, c) =>
      {
        list.Add(AddElement(r, c, Entries));
      });
      return list;
    }

    IEnumerable<C> BuildRectangle()
    {
      var list = new List<C>();

      RectangleLoop((r, c) =>
      {
        list.Add(AddElement(r, c, Entries));
      });
      return list;
    }

    protected void RowLoop(Action<int,int> action)
    {
      int rowNo = RowNo;
      int columnNo = 1;
      for (int i = 1; i <= Entries; i++)
      {
        action(rowNo,columnNo);
        columnNo++;        
      }
    }

    protected void ColumnLoop(Action<int, int> action)
    {
      int rowNo = 1;
      int columnNo = ColumnNo;
      for (int i = 1; i <= Entries; i++)
      {
        action(rowNo, columnNo);
        rowNo++;
      }
    }

    protected void RectangleLoop(Action<int, int> action)
    {
      int rowNo = 1;
      int columnNo = 1;
      for (int i = 1; i <= Entries; i++)
      {
        action(rowNo, columnNo);
        columnNo++;
        if (columnNo > rows)
        {
          rowNo++;
          columnNo = 1;
        }
      }
    }


    protected abstract C AddElement(int rowNo, int columnNo, int topValue = 9);

    /// <summary>
    /// Rows and columns from top perspective
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    protected abstract int InternalRow(int row, int column);
    protected abstract int InternalColumn(int row, int column);

    protected abstract int ExternalRow(int row);
    protected abstract int ExternalColumn(int column);

    public C Element(int row, int column)
    {
      var ir = InternalRow(row, column);
      var ic = InternalColumn(row, column);
      foreach (var element in elements)
      {
        if (element.RowNo == ir && element.ColumnNo == ic)
        {
          return element;
        }
      }

      return null;
    }
    
  }
}
  
