using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GnomeWars
{
    public interface IBoard
    {
        int RowCount { get; }
        int ColumnCount { get; }
        BoardPosition GetPosition(int column, int row);
        void PrintBoard(IDictionary<IGnome, BoardPosition> gnomePositions);
    }

    public class Board : IBoard
    {
        public int RowCount { get; }
        public int ColumnCount { get; }

        // assuming there won't be more than 5 types of gnome
        private readonly ConsoleColor[] colours = new[]
        {
            ConsoleColor.Magenta,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Cyan,
            ConsoleColor.Blue
        };
        private readonly ILogger logger;
        private readonly BoardPosition[,] board;

        public Board(string filepath, ILogger logger)
        {
            this.logger = logger;
            var rows = File.ReadAllLines(filepath);
            RowCount = rows.Length;
            ColumnCount = rows.First().Length;
            board = new BoardPosition[ColumnCount, RowCount];
            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    var tile = Tile.Create(rows[row][column]);
                    board[column, row] = new BoardPosition(column, row, tile);
                }
            }
        }

        public BoardPosition GetPosition(int column, int row)
        {
            return board[column, row];
        }

        // The characters of the board are printed out in batches of the same colour
        // to reduce the number of calls made to the console (which are slow) to avoid flickering
        // in the display when the characters are not printed out quickly enough
        public void PrintBoard(IDictionary<IGnome, BoardPosition> gnomePositions)
        {
            logger.WaitThenClear();
            var currentColour = ConsoleColor.White;
            var currentString = "";
            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    if (board[column, row].Tile.TileType == TileType.Wall)
                    {
                        AddToOutputLine("#", ConsoleColor.White, ref currentColour, ref currentString);
                        continue;
                    }

                    var gnomesAtPosition = gnomePositions.Where(x => x.Value.Column == column && x.Value.Row == row).ToList();
                    var gnomeCount = gnomesAtPosition.Count;
                    if (gnomeCount == 0)
                    {
                        AddToOutputLine(" ", ConsoleColor.White, ref currentColour, ref currentString);
                    }
                    else if (gnomeCount == 1)
                    {
                        var gnome = gnomesAtPosition.Single().Key;
                        var colour = colours[(gnome.GnomeGroup - 1) % colours.Length];
                        AddToOutputLine(gnome.GnomeStrength.ToString(), colour, ref currentColour, ref currentString);
                    }
                    else
                    {
                        AddToOutputLine("X", ConsoleColor.Red, ref currentColour, ref currentString);
                    }
                }
                AddToOutputLine($"{Environment.NewLine}", currentColour, ref currentColour, ref currentString);
            }
            logger.LogWithColour($"{currentString}{Environment.NewLine}", currentColour);
        }

        private void AddToOutputLine(string output, ConsoleColor colour, ref ConsoleColor currentColour, ref string currentString)
        {
            if (currentColour == colour)
            {
                currentString += output;
            }
            else
            {
                logger.LogWithColour(currentString, currentColour);
                currentString = output;
                currentColour = colour;
            }
        }
    }
}
