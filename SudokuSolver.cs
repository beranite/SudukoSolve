using System;

namespace Suduko
{
  public class SudokuSolver
  {
    private static int N = 9;
    private static int count = 0;

    public int[,] Board { get; }

    public SudokuSolver(int[,] board)
    {
      Board = board;
    }

    bool SolveBoard()
    {
      return SolveSudoku(Board);
  }

    bool IsSafe(int[,] board,int row, int col, int num)
    {
      // Check if the number is already present in the row or column
      for (int x = 0; x < N; x++)
      {
        if (board[row, x] == num || board[x, col] == num)
          return false;
      }

      // Check if the number is already present in the 3x3 grid
      int startRow = row - row % 3;
      int startCol = col - col % 3;
      for (int i = 0; i < 3; i++)
      {
        for (int j = 0; j < 3; j++)
        {
          if (board[i + startRow, j + startCol] == num)
            return false;
        }
      }

      return true;
    }



    bool SolveSudoku(int[,] board)
    {
      int row = -1, col = -1;
      bool isEmpty = true;
      count++;
      // Find an empty cell
      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          if (board[i, j] == 0)
          {
            row = i;
            col = j;
            isEmpty = false;
            break;
          }
        }
        if (!isEmpty)
          break;
      }

      // No empty cell, puzzle solved
      if (isEmpty)
        return true;

      // Try placing numbers from 1 to 9
      for (int num = 1; num <= N; num++)
      {
        if (IsSafe(board, row, col, num))
        {
          board[row, col] = num;

          if (SolveSudoku(board))
            return true;

          board[row, col] = 0; // Backtrack
        }
      }

      return false;
    }

    static void PrintBoard(int[,] board)
    {
      for (int r = 0; r < N; r++)
      {
        for (int c = 0; c < N; c++)
        {
          Console.Write(board[r, c] + " ");
        }
        Console.WriteLine();
      }
    }

    public static void Solve()
    {
      int[,] board = new int[,]
      {
            {5, 3, 0, 0, 7, 0, 0, 0, 0},
            {6, 0, 0, 1, 9, 5, 0, 0, 0},
            {0, 9, 8, 0, 0, 0, 0, 6, 0},
            {8, 0, 0, 0, 6, 0, 0, 0, 3},
            {4, 0, 0, 8, 0, 3, 0, 0, 1},
            {7, 0, 0, 0, 2, 0, 0, 0, 6},
            {0, 6, 0, 0, 0, 0, 2, 8, 0},
            {0, 0, 0, 4, 1, 9, 0, 0, 5},
            {0, 0, 0, 0, 8, 0, 0, 7, 9}
      };

      int[,] evilBoard = new int[,]
{
            {8, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 3, 6, 0, 0, 0, 0, 0},
            {0, 7, 0, 0, 9, 0, 2, 0, 0},
            {0, 5, 0, 0, 0, 7, 0, 0, 0},
            {0, 0, 0, 0, 4, 5, 7, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 3, 0},
            {0, 0, 1, 0, 0, 0, 0, 6, 8},
            {0, 0, 8, 5, 0, 0, 0, 1, 0},
            {0, 9, 0, 0, 0, 0, 4, 0, 0}
};
      var ss = new SudokuSolver(evilBoard);
      if (ss.SolveBoard())
      {
        Console.WriteLine("Solved Sudoku:");
        PrintBoard(evilBoard);
      }
      else
      {
        Console.WriteLine("No solution exists.");
      }
    }
  }
}