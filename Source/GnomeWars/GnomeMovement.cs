using System;
using System.Collections.Generic;
using System.Linq;

namespace GnomeWars
{
    public interface IGnomeMovement
    {
        bool IsFrozen(IGnome gnome);
        bool TryMove(IGnome gnome, Direction direction);
    }

    public class GnomeMovement : IGnomeMovement
    {
        private readonly IBoard board;
        private readonly IDictionary<IGnome, BoardPosition> gnomePositions;

        public GnomeMovement(IBoard board, IDictionary<IGnome, BoardPosition> gnomePositions)
        {
            this.board = board;
            this.gnomePositions = gnomePositions;
        }

        // don't move gnome if another gnome has collided with it
        public bool IsFrozen(IGnome gnome)
        {
            var position = gnomePositions[gnome];
            var gnomesAtPosition = gnomePositions.Where(x => x.Value.Column == position.Column && x.Value.Row == position.Row).ToList();
            return gnomesAtPosition.Count > 1;
        }

        public bool TryMove(IGnome gnome, Direction direction)
        {
            var position = gnomePositions[gnome];
            switch (direction)
            {
                case Direction.North: return TryMove(gnome, position.Row - 1, position.Column);
                case Direction.East: return TryMove(gnome, position.Row, position.Column + 1);
                case Direction.South: return TryMove(gnome, position.Row + 1, position.Column);
                case Direction.West: return TryMove(gnome, position.Row, position.Column - 1);
                default: throw new Exception($"Unexpected direction {direction}");
            }
        }

        private bool TryMove(IGnome gnome, int row, int column)
        {
            var newPosition = board.GetPosition(column, row);
            if (newPosition.Tile.CanMoveThrough)
            {
                gnomePositions[gnome] = newPosition;
                return true;
            }

            return false;
        }
    }
}
