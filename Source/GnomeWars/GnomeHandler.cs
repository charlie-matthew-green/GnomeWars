using System;
using System.Collections.Generic;
using System.Linq;

namespace GnomeWars
{
    public interface IGnomeHandler
    {
        void PlaceGnomesOnBoard(IEnumerable<IGnome> gnomes);
        bool MultipleGnomesSurvive();
        void MoveGnomes();
        void KillGnome(IGnome gnome);
        IGnome GetWinner();
        IEnumerable<BoardPosition> GetDistinctPositions();
        IEnumerable<IGnome> GetGnomesAtPosition(BoardPosition position);
    }

    public class GnomeHandler : IGnomeHandler
    {
        private readonly IDictionary<IGnome, BoardPosition> gnomePositions;
        private readonly ILogger logger;
        private readonly Random random;
        private readonly IBoard board;

        public GnomeHandler(IBoard board, IDictionary<IGnome, BoardPosition> gnomePositions, ILogger logger)
        {
            this.board = board;
            this.gnomePositions = gnomePositions;
            this.logger = logger;
            random = new Random(DateTime.UtcNow.Millisecond);
        }

        public void PlaceGnomesOnBoard(IEnumerable<IGnome> gnomes)
        {
            foreach (var gnome in gnomes)
            {
                var possiblePositions = new List<(int column, int row)>();
                for (var column = 0; column < board.ColumnCount; column++)
                {
                    for (var row = 0; row < board.RowCount; row++)
                    {
                        possiblePositions.Add((column, row));
                    }
                }

                while (possiblePositions.Count > 0)
                {
                    var positionIndex = random.Next(0, possiblePositions.Count);
                    var position = possiblePositions[positionIndex];
                    var boardPosition = board.GetPosition(position.column, position.row);
                    if (boardPosition.Tile.CanMoveThrough && NoGnomesAtThisPosition(boardPosition))
                    {
                        gnomePositions.Add(gnome, boardPosition);
                        break;
                    }
                    possiblePositions.RemoveAt(positionIndex);
                }

                if (possiblePositions.Count == 0)
                {
                    throw new Exception("Not enough space to place all gnomes at unique positions on the board!");
                }
            }
            board.PrintBoard(gnomePositions);
        }

        public bool MultipleGnomesSurvive()
        {
            return gnomePositions.Count > 1;
        }

        public void MoveGnomes()
        {
            foreach (var gnome in gnomePositions.Select(x => x.Key).ToList())
            {
                gnome.Move();
            }
            board.PrintBoard(gnomePositions);
        }

        public void KillGnome(IGnome gnome)
        {
            gnomePositions.Remove(gnome);
        }

        public IGnome GetWinner()
        {
            return gnomePositions.Single().Key;
        }

        public IEnumerable<BoardPosition> GetDistinctPositions()
        {
            return gnomePositions.Values.Distinct();
        }

        public IEnumerable<IGnome> GetGnomesAtPosition(BoardPosition position)
        {
            return gnomePositions
                .Where(x => x.Value.Column == position.Column && x.Value.Row == position.Row)
                .Select(x => x.Key)
                .ToList();
        }

        private bool NoGnomesAtThisPosition(BoardPosition position)
        {
            return gnomePositions.Values.All(x => !(x.Column == position.Column && x.Row == position.Row));
        }
    }
}
