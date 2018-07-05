using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class GnomeMovementTests
    {
        private ITile corridorTile;
        private ITile wallTile;
        private BoardPosition startPosition;

        [SetUp]
        public void SetUp()
        {
            corridorTile = MockRepository.GenerateMock<ITile>();
            corridorTile.Stub(x => x.TileType).Return(TileType.Corridor);
            corridorTile.Stub(x => x.CanMoveThrough).Return(true);
            
            wallTile = MockRepository.GenerateMock<ITile>();
            wallTile.Stub(x => x.TileType).Return(TileType.Wall);
            wallTile.Stub(x => x.CanMoveThrough).Return(false);

            startPosition = new BoardPosition(1, 1, corridorTile);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsFrozenTest(bool shouldFreeze)
        {
            // Arrange
            var gnomePositions = new Dictionary<IGnome, BoardPosition>();
            var board = MockRepository.GenerateMock<IBoard>();
            var gnomeMovement = new GnomeMovement(board, gnomePositions);
            var gnome1 = new Gnome(1, 1, 1, gnomeMovement);
            var gnome2 = new Gnome(1, 1, 1, gnomeMovement);
            gnomePositions.Add(gnome1, new BoardPosition(0, 0, null));
            gnomePositions.Add(gnome2, new BoardPosition(0, shouldFreeze ? 0 : 1, null));

            // Act
            var isFrozen = gnomeMovement.IsFrozen(gnome1);

            // Assert
            Assert.AreEqual(shouldFreeze, isFrozen);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryMoveNorth(bool isBlocked)
        {
            // Arrange
            var gnomePositions = new Dictionary<IGnome, BoardPosition>();
            var board = MockRepository.GenerateMock<IBoard>();
            var gnomeMovement = new GnomeMovement(board, gnomePositions);
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomePositions.Add(gnome, startPosition);
            var northPosition = new BoardPosition(1, 0, isBlocked ? wallTile : corridorTile);
            board.Stub(x => x.GetPosition(1, 0)).Return(northPosition);

            // Act
            var couldMove = gnomeMovement.TryMove(gnome, Direction.North);

            // Assert
            Assert.AreEqual(isBlocked, !couldMove);
            var expectedPosition = isBlocked ? startPosition : northPosition;
            Assert.AreEqual(expectedPosition, gnomePositions[gnome]);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryMoveEast(bool isBlocked)
        {
            // Arrange
            var gnomePositions = new Dictionary<IGnome, BoardPosition>();
            var board = MockRepository.GenerateMock<IBoard>();
            var gnomeMovement = new GnomeMovement(board, gnomePositions);
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomePositions.Add(gnome, startPosition);
            var eastPosition = new BoardPosition(2, 1, isBlocked ? wallTile : corridorTile);
            board.Stub(x => x.GetPosition(2, 1)).Return(eastPosition);

            // Act
            var couldMove = gnomeMovement.TryMove(gnome, Direction.East);

            // Assert
            Assert.AreEqual(isBlocked, !couldMove);
            var expectedPosition = isBlocked ? startPosition : eastPosition;
            Assert.AreEqual(expectedPosition, gnomePositions[gnome]);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryMoveSouth(bool isBlocked)
        {
            // Arrange
            var gnomePositions = new Dictionary<IGnome, BoardPosition>();
            var board = MockRepository.GenerateMock<IBoard>();
            var gnomeMovement = new GnomeMovement(board, gnomePositions);
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomePositions.Add(gnome, startPosition);
            var southPosition = new BoardPosition(1, 2, isBlocked ? wallTile : corridorTile);
            board.Stub(x => x.GetPosition(1, 2)).Return(southPosition);

            // Act
            var couldMove = gnomeMovement.TryMove(gnome, Direction.South);

            // Assert
            Assert.AreEqual(isBlocked, !couldMove);
            var expectedPosition = isBlocked ? startPosition : southPosition;
            Assert.AreEqual(expectedPosition, gnomePositions[gnome]);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryMoveWest(bool isBlocked)
        {
            // Arrange
            var gnomePositions = new Dictionary<IGnome, BoardPosition>();
            var board = MockRepository.GenerateMock<IBoard>();
            var gnomeMovement = new GnomeMovement(board, gnomePositions);
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomePositions.Add(gnome, startPosition);
            var westPosition = new BoardPosition(0, 1, isBlocked ? wallTile : corridorTile);
            board.Stub(x => x.GetPosition(0, 1)).Return(westPosition);

            // Act
            var couldMove = gnomeMovement.TryMove(gnome, Direction.West);

            // Assert
            Assert.AreEqual(isBlocked, !couldMove);
            var expectedPosition = isBlocked ? startPosition : westPosition;
            Assert.AreEqual(expectedPosition, gnomePositions[gnome]);
        }
    }
}
