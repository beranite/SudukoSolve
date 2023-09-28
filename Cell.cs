using System;
using System.Linq;
using System.Collections.Generic;

namespace Suduko
{
  public class Cell : BaseCell
  {
    public int? Value { get; private set; }


     List<int> allowedValues = new List<int>();

    public List<int> AllowedValues
    {
      get
      {
        return allowedValues.ToList();
      }
    }

    public string AllowedValuesImage()
    {
      string s = "";
      foreach (var entry in allowedValues)
      {
        s += entry.ToString();
      }
      return s;
    }

    public ICell Parent { get; }

    public bool PeekAllowed(int value)
    {
      return allowedValues.Contains(value);
    }

    public bool HasBoth(int first, int second)
    {
      if (this.Value.HasValue)
        return false;
      return (this.PeekAllowed(first) && this.PeekAllowed(second));
    }

    public Cell(ICell parent, int rowNo, int columnNo, int topValue = 9) : base(rowNo, columnNo)
    {
      for (int i=1;i<=topValue;i++)
      {
        allowedValues.Add(i);
      }
      Value = null;
      Parent = parent;
    }

    public delegate void CellChange(Cell cell);
    public event CellChange ValueSet;
    public bool SetValue(int suggestedValue)
    {
      if (allowedValues.Contains(suggestedValue))
      {
        this.Value = suggestedValue;
        allowedValues = new List<int> { this.Value.Value };
        ValueSet?.Invoke(this);
        return true;
      }
      return false;
    }

    public bool RemoveAllowedValues(IEnumerable<int> suggestedValues)
    {
      if (suggestedValues is null)
      {
        throw new ArgumentNullException(nameof(suggestedValues));
      }

      if (this.Value != null)
        return false;
      bool found = false;
      foreach (var suggestedValue in suggestedValues)
      {
        if (allowedValues.Contains(suggestedValue))
        {
          allowedValues.Remove(suggestedValue);
          found = true;
        }
      }
      if (found)
      {
        if (allowedValues.Count == 1)
        {
          this.SetValue(allowedValues.FirstOrDefault<int>());
        }
      }
      return found;
    }

    public bool RemoveAllowedButValues(IEnumerable<int> suggestedValues)
    {
      if (suggestedValues is null)
      {
        throw new ArgumentNullException(nameof(suggestedValues));
      }

      if (this.Value != null)
        return false;
      bool found = false;
      foreach (var av in allowedValues.ToList())
      {
        if (!suggestedValues.Contains(av))
        {
          allowedValues.Remove(av);
          found = true;
        }
      }
      if (found)
      {
        if (allowedValues.Count == 1)
        {
          this.SetValue(allowedValues.FirstOrDefault<int>());
        }
      }
      return found;
    }

    public override string ToString()
    {

      return $"({Id}) - {this.Value} - {AllowedValuesImage()}";
    }

    public override string CalculateId()
    {
      var id = $"({RowNo},{ColumnNo})";
      if (Parent == null)
        return id;
      return $"{Parent.Id}{id}";
    }
  }
}
