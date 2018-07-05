namespace GnomeWars
{
    public struct BoardPosition
    {
        public int Column { get; }
        public int Row { get; }
        public ITile Tile { get; }

        public BoardPosition(int column, int row, ITile tile)
        {
            Column = column;
            Row = row;
            Tile = tile;
        }
    }
}
