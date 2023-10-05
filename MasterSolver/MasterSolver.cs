using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Suduko.MasterSolver
{
    public class MasterSolver
    {
        public string Sudoko { get; }
        public Action<Head, string> Action { get; }
        public int TopValue { get; }

        public MasterSolver(string sudoko, ElementShape shape, Action<Head, string> action, int topValue = 9)
        {
            Sudoko = sudoko;
            Action = action;
            TopValue = topValue;
        }

        public Head Solve()
        {
            var cellSetList = new List<CellSet>();
            var head = Create(cellSetList);
            head.Solve();

            return DoSolve(head.CellValueList.First(), cellSetList);
        }

        public Head DoSolve(CellValue topCellValue, List<CellSet> cellSetList)
        {
            foreach (var v in topCellValue.AllowedValues)
            {
                var newCellSetList = new List<CellSet>(cellSetList)
        {
          new CellSet(topCellValue.RowNo, topCellValue.ColumnNo, v)
        };

                var head = Create(newCellSetList);

                try
                {
                    if (head.Solve())
                        return head;
                    var cellValueList = head.CellValueList;
                    if (cellValueList.Count > 0)
                        DoSolve(cellValueList.First(), newCellSetList);
                    else
                        return head;
                }
                catch (CellListElementException)
                { }
            }
            return null;
        }

        public Head Create(List<CellSet> cellSetList)
        {
            var head = new Head(ElementShape.Rectangle, Action, 9);
            ReadStringToMatrix(Sudoko, head.SetCell);
            foreach (var cellSet in cellSetList)
            {
                Action(null, $"set cell {cellSet}");
                head.SetCell(cellSet.RowNo, cellSet.ColumnNo, cellSet.CellValue);
            }
            return head;
        }
        static void ReadStringToMatrix(string sudoko, Action<int, int, int> setCell)
        {
            int row = 1;
            int column = 1;
            using (var sr = new StringReader(sudoko))
            {
                int next;
                while (sr.Read() > 0)
                {
                    next = sr.Peek();
                    if (next >= 48 && next < 58)
                    {
                        var number = next - 48;
                        setCell(row, column, number);
                        column++;
                        if (column > 9)
                        {
                            column = 1;
                            row++;
                        }
                    }
                }
            }
        }
    }
}
