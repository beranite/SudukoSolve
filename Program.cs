using System;
using System.IO;
using Suduko.MasterSolver;

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

    static string sudokoWorst =
@"
800000000
003600000
070090200
050007000
000045700
000100030
001000068
008500010
090000400
";

    static string sudokoWorst2 =
@"
005300000
800000020
070010500
400005300
010070006
003200080
060500009
004000030
000009700
";

    static string sudokoWorstSolved =
@"
812753649
943682175
675491283
154237896
369845721
287169534
521974368
438526917
796318452";

    static void Main(string[] args)
    {
      SudokuSolver.Solve();

      string lastDump = "";
      var masterSolver = new Suduko.MasterSolver.MasterSolver(sudokoWorst, ElementShape.Rectangle,
        (h, t) =>
        {
          Console.WriteLine(t);
          var curDump = (h != null) ? h.Dump() : null;
          if (h != null && (curDump != lastDump))
          {             
            Console.WriteLine(h.Dump());
          }
          lastDump = curDump;
          Console.WriteLine(CellListElement.CountOfSolve);
        }, 9);
      var head = masterSolver.Solve();

      var dump = head.Dump();
      Console.WriteLine(head.Dump());          
    }    
 }
}
