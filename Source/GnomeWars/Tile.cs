using System;

namespace GnomeWars
{
    public enum TileType
    {
        Wall,
        Corridor
    }

    public interface ITile
    {
        TileType TileType { get; }
        bool CanMoveThrough { get; }
    }

    public struct Tile : ITile
    {
        private Tile(TileType tileType, bool canMoveThrough)
        {
            TileType = tileType;
            CanMoveThrough = canMoveThrough;
        }

        public TileType TileType { get; }
        public bool CanMoveThrough { get; }

        public static Tile Create(char tileChar)
        {
            switch (tileChar)
            {
                case '#': return new Tile(TileType.Wall, false);
                case ' ': return new Tile(TileType.Corridor, true);
                default: throw new Exception($"Unexpected character '{tileChar}', cannot determine tile.");
            }
        }
    }
}
