using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Suduko.MasterSolver
{
  public class CellSet
  {
    public int RowNo { get; }
    public int ColumnNo { get; }
    public int CellValue { get; }

    public CellSet(int rowNo, int columnNo, int cellValue)
    {
      this.RowNo = rowNo;
      this.ColumnNo = columnNo;
      this.CellValue = cellValue;
    }

    public override string ToString()
    {
      return $"({RowNo},{ColumnNo}) - {CellValue}";
    }
  }

  public class CellValue : BaseCell, IComparable<CellValue>
  {
    public int? Value { get; private set; }
    public List<int> AllowedValues = new List<int>();

    public CellValue(int rowNo, int columnNo, int? value, List<int> allowedValues) : base(rowNo, columnNo)
    {
      this.Value = value;
      this.AllowedValues = allowedValues.ToList();
    }

    public int CompareTo([AllowNull] CellValue other)
    {
      if (other.AllowedValues.Count == AllowedValues.Count)
      {
        return 0;
      }
      return (other.AllowedValues.Count < AllowedValues.Count) ? 1 : -1;
    }
    public string AllowedValuesImage()
    {
      string s = "";
      foreach (var entry in AllowedValues)
      {
        s += entry.ToString();
      }
      return s;
    }
    public override string ToString()
    {
      return $"({Id}) - {this.Value} - {AllowedValuesImage()}";
    }
  }

  /// <summary>
  /// Represents a cell in a grid.
  /// </summary>
  public class Cell : BaseCell
  {
    /// <summary>
    /// The value of the cell.
    /// </summary>
    public int? Value { get; private set; }

    private List<int> allowedValues = new List<int>();

    /// <summary>
    /// Gets the list of allowed values for the cell.
    /// </summary>
    public List<int> AllowedValues
    {
      get
      {
        return allowedValues.ToList();
      }
    }

    /// <summary>
    /// Creates a copy of the current cell value.
    /// </summary>
    public CellValue Copy()
    {
      return new CellValue(this.ExternalRowNo(), this.ExternalColumnNo(), this.Value, this.AllowedValues);
    }

    /// <summary>
    /// Resets the cell value.
    /// </summary>
    public void ResetValue(CellValue cv)
    {
      this.Value = cv.Value;
      this.allowedValues = cv.AllowedValues.ToList();
    }

    /// <summary>
    /// Returns a string representation of the allowed values.
    /// </summary>
    public string AllowedValuesImage()
    {
      string s = "";
      foreach (var entry in allowedValues)
      {
        s += entry.ToString();
      }
      return s;
    }

    /// <summary>
    /// The parent cell.
    /// </summary>
    public ICell Parent { get; }

    /// <summary>
    /// Checks if a value is allowed in the cell.
    /// </summary>
    public bool PeekAllowed(int value)
    {
      return allowedValues.Contains(value);
    }

    /// <summary>
    /// Checks if both values are allowed in the cell.
    /// </summary>
    public bool HasBoth(int first, int second)
    {
      if (this.Value.HasValue)
        return false;
      return (this.PeekAllowed(first) && this.PeekAllowed(second));
    }

    /// <summary>
    /// Initializes a new instance of the Cell class.
    /// </summary>
    public Cell(ICell parent, int rowNo, int columnNo, int topValue = 9) : base(rowNo, columnNo)
    {
      for (int i = 1; i <= topValue; i++)
      {
        allowedValues.Add(i);
      }
      Value = null;
      Parent = parent;
    }

    /// <summary>
    /// Delegate for handling cell changes.
    /// </summary>
    public delegate void CellChange(Cell cell);

    /// <summary>
    /// Event that occurs when the cell value is set.
    /// </summary>
    public event CellChange ValueSet;

    /// <summary>
    /// Sets the value of the cell.
    /// </summary>
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

    /// <summary>
    /// Returns the external row number of the cell.
    /// </summary>
    public int ExternalRowNo()
    {
      return (Parent.RowNo - 1) * 3 + this.RowNo;
    }

    /// <summary>
    /// Returns the external column number of the cell.
    /// </summary>
    public int ExternalColumnNo()
    {
      return (Parent.ColumnNo - 1) * 3 + this.ColumnNo;
    }

    /// <summary>
    /// Removes the specified values from the list of allowed values.
    /// </summary>
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
      if (allowedValues.Count == 0)
        throw new CellListElementException(nameof(allowedValues));
      return found;
    }

    /// <summary>
    /// Removes all values from the list of allowed values except the specified ones.
    /// </summary>
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

    /// <summary>
    /// Returns a string that represents the current cell.
    /// </summary>
    public override string ToString()
    {
      return $"({Id}) - {this.Value} - {AllowedValuesImage()}";
    }

    /// <summary>
    /// Calculates the ID of the cell.
    /// </summary>
    public override string CalculateId()
    {
      var id = $"({RowNo},{ColumnNo})";
      if (Parent == null)
        return id;
      return $"{Parent.Id}{id}";
    }
  }
}
