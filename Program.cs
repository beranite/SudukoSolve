using System;
using System.IO;

namespace Suduko
{
  class Program
  {
    static string sudokoMed =
@"
072000089
009800016
831094700
405020003
900047105
003108020
000009230
084035000
000081540";
    static string sudokoExpert =
@"
000015069
000060500
001000003
830000004
040006802
000200000
084000000
000031006
000000700
";

    static string sudokoEvil =
@"
060470000
407300005
020000400
200000000
304010900
000009006
000500080
901030600
070000000
";

    static string sudokoEvil2 =
@"
020490306
000003000
700000500
090160004
002000090
000080000
000002003
010008000
005310600
";

    static void Main(string[] args)
    {
      Console.WriteLine("Hello World!");
      var head = new Head(ElementShape.Rectangle, 
        (h,t) => { 
          Console.WriteLine(t);  
          Console.WriteLine(h.Dump());
          Console.WriteLine(CellListElement.CountOfSolve);
        }, 9);
      ReadStringToMatrix(sudokoEvil2, 9,head.SetCell);
      
      Console.WriteLine(" ");
      head.Solve();
      var dump = head.Dump();
      Console.WriteLine(head.Dump());

    }

    static void ReadStringToMatrix(string sudoko, int topValue, Action<int,int,int> setCell)
    {
      int row = 1;
      int column = 1;
      using (var sr = new StringReader(sudoko))
      {
        int next;
        while (sr.Read()> 0)
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
