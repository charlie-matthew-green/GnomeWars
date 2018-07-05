using System;
using System.Collections.Generic;
using System.Linq;

namespace GnomeWars
{
    public interface IGnome
    {
        int Id { get; }
        int GnomeGroup { get; }
        int GnomeStrength { get; set; }
        void Move();
    }

    public class Gnome : IGnome
    {
        private readonly IGnomeMovement gnomeMovement;
        private readonly Random random = new Random(DateTime.UtcNow.Millisecond);
        private Direction? lastDirection;

        public int Id { get; }
        public int GnomeGroup { get; }
        public int GnomeStrength { get; set; }

        public Gnome(int id, int gnomeGroup, int gnomeStrength, IGnomeMovement gnomeMovement)
        {
            this.gnomeMovement = gnomeMovement;
            Id = id;
            GnomeGroup = gnomeGroup;
            GnomeStrength = gnomeStrength;
        }

        public void Move()
        {
            if (gnomeMovement.IsFrozen(this))
            {
                return;
            }
            var hasMoved = false;
            var possibleDirections = new List<Direction>
            {
                Direction.North,
                Direction.East,
                Direction.South,
                Direction.West
            };
            if (lastDirection != null)
            {
                hasMoved = gnomeMovement.TryMove(this, lastDirection.Value);
                possibleDirections.Remove(lastDirection.Value);
            }
            while (!hasMoved)
            {
                var indexNextDirection = possibleDirections.Count == 1 ? 0 : random.Next(0, possibleDirections.Count);
                hasMoved = gnomeMovement.TryMove(this, possibleDirections[indexNextDirection]);
                lastDirection = possibleDirections[indexNextDirection];
                possibleDirections.Remove(possibleDirections[indexNextDirection]);
                if (!hasMoved && !possibleDirections.Any())
                {
                    throw new Exception("Error - it is not possible for the gnome to move in any direction");
                }
            }
        }
    }
}
