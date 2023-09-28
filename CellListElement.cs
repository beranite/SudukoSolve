using System;
using System.Linq;
using System.Collections.Generic;

namespace Suduko
{
  public abstract class CellListElement : BaseElement<Cell>
  {
    public static int CountOfSolve = 0;
    public bool Solved { get; private set; } = false;

    public List<Cell> Cells
    {
      get
      {
        return this.elements;
      }
    }


    List<int> allValues = new List<int>();
    List<int> values = new List<int>();

    public CellListElement(ElementShape shape, Func<int, int, Cell> fetchCell, int rowNo, int columnNo, int topValue = 9) :
      base(shape, rowNo, columnNo, topValue, fetchCell)
    {
      foreach (var cell in this.elements)
      {
        cell.ValueSet += ValueSet;
      }

      for (int i = 1; i <= Entries; i++)
      {
        allValues.Add(i);
      }
    }

    void ValueSet(Cell cell)
    {
      int number = cell.Value.Value;
      if (values.Contains(number))
      {
        throw new InvalidOperationException("{number} is already used");
      }
      values.Add(number);
      allValues.Remove(number);
      Solved = allValues.Count == 0;
      CountOfSolve++;
    }

    public bool Solve()
    {

      if (Solved)
        return false;

      bool change = false;
      foreach (var cell in Cells)
      {
        change |= cell.RemoveAllowedValues(values);
      }

      change |= SolveSingleField();
      FindDoubleField();
      FindDoubleFieldV2();
      UpdateRestrictions();
      CountOfSolve++;
      return change;
    }

    private bool SolveSingleField()
    {
      bool change = false;
      // See if a given value is only placed once
      foreach (var value in allValues.ToList())
      {
        Cell singleCell = null;
        foreach (var cell in Cells)
        {
          if (!cell.Value.HasValue)
          {
            if (cell.PeekAllowed(value))
            {
              if (singleCell != null)
              {
                singleCell = null;
                break;
              }
              singleCell = cell;
            }
          }
        }
        if (singleCell != null)
        {
          singleCell.SetValue(value);
          change = true;
        }
      }

      return change;
    }

    private void FindDoubleField()
    {
      var openedCells = Cells.Where(c => !c.Value.HasValue).ToList();
      // See if a given value is only placed once
      Cell firstCell = null;
      Cell secondCell = null;
      var doubleCells = Cells.Where(c => c.AllowedValues.Count == 2);
      if (doubleCells.Count() < 2)
        return;
      List<string> distincts = doubleCells.Select(dc => dc.AllowedValuesImage()).Distinct().ToList();
      List<string> pairImages = new List<string>();
      int count = 0;
      foreach (var d in distincts)
      {
        firstCell = null;
        secondCell = null;
        foreach (var cell in openedCells.Where(c => c.AllowedValuesImage() == d))
        {
          count++;
          if (firstCell == null)
            firstCell = cell;
          else
            if (secondCell == null)
          {
            secondCell = cell;
          }
          else
          {
            firstCell = null;
            secondCell = null;
            break;
          }
        }
        if (firstCell != null && secondCell != null)
          pairImages.Add(d);
      }
      foreach (var p in pairImages)
      {
        var pairCell = Cells.First(c => c.AllowedValuesImage() == p);
        foreach (var cell in Cells.Where(c => c.AllowedValuesImage() != p))
        {
          cell.RemoveAllowedValues(pairCell.AllowedValues);
        }
      }
    }

    class Pair
    {
      public int First;
      public int Second;
      public Pair(int first, int second)
      {
        First = first;
        Second = second;
      }

      public string Image
      {
        get { return $"{First}{Second}"; }
      }

      public List<int> Allowed
      {
        get
        {
          return new List<int> { First, Second };
        }
      }
      public override string ToString()
      {
        return Image;
      }
    }

    private void FindDoubleFieldV2()
    {
      var openedCells = Cells.Where(c => !c.Value.HasValue).ToList();
      // See if a given value is only placed once
      Cell firstCell = null;
      Cell secondCell = null;
      List<Pair> pairList = new List<Pair>();
      foreach (var value in this.allValues)
      {
        foreach (var value2 in this.allValues)
        {
          if (value == value2)
            continue;
          pairList.Add(new Pair(value, value2));
        }
      }
      var pairImages = new List<Pair>();
      int count = 0;
      foreach (var pair in pairList)
      {
        firstCell = null;
        secondCell = null;
        foreach (var cell in openedCells)
        {
          count++;
          if (cell.HasBoth(pair.First, pair.Second))
          {
            if (firstCell == null)
              firstCell = cell;
            else
              if (secondCell == null)
            {
              secondCell = cell;
            }
            else
            {
              firstCell = null;
              secondCell = null;
              break;
            }
          }
          else
          {
            if (cell.PeekAllowed(pair.First) || cell.PeekAllowed(pair.Second))
            {
              firstCell = null;
              secondCell = null;
              break;
            }
          }
        }
        if (firstCell != null && secondCell != null)
          pairImages.Add(pair);
      }
      foreach (var p in pairImages)
      {
        foreach (var cell in openedCells.Where(c => c.HasBoth(p.First,p.Second)))
        {
          cell.RemoveAllowedButValues(p.Allowed);
        }
      }
    }

    public Dictionary<int, List<int> > rowRestrictions = new Dictionary<int, List<int>>();
    public Dictionary<int, List<int>> columnRestrictions = new Dictionary<int,List<int>>();

    void UpdateRestrictions()
    {
      if (Shape != ElementShape.Rectangle)
        return;
      // check rows
      var allowedArray = allValues.ToList<int>();
      var rowContain = new Dictionary<int,List<int>>();
      var columnContain = new Dictionary<int,List<int>>();
      rowRestrictions.Clear();
      columnRestrictions.Clear();
      foreach (var suggestedValue in allowedArray)
      {
        int onlyRow = 0;
        int onlyColumn = 0;

        foreach (var cell in Cells)
        {
          if (cell.PeekAllowed(suggestedValue))
          {
            if (onlyRow == 0)
            {
              onlyRow = cell.RowNo;
            }
            else
            {
              if (onlyRow != cell.RowNo)
                onlyRow = -1;
            }

            if (onlyColumn == 0)
            {
              onlyColumn = cell.ColumnNo;
            }
            else
            {
              if (onlyColumn != cell.ColumnNo)
                onlyColumn = -1;
            }
          }
        }
        if (onlyRow > 0)
        {
          List<int> list;
          if (!rowRestrictions.TryGetValue(onlyRow, out list))
          {
            list = new List<int>();
            rowRestrictions.Add(onlyRow, list);
          }
          rowRestrictions[onlyRow].Add(suggestedValue);
        }
        if (onlyColumn > 0)
        {
          List<int> list;
          if (!columnRestrictions.TryGetValue(onlyColumn, out list))
          {
            list = new List<int>();
            columnRestrictions.Add(onlyColumn, list);
          }
          columnRestrictions[onlyColumn].Add(suggestedValue);
        }
      }
    }
  }
}
  
