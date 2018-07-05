using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class GnomeHandlerTests
    {
        [Test]
        public void PlacesGnomesOnBoard()
        {
            // Arrange
            var logger = MockRepository.GenerateMock<ILogger>();
            var dir = TestContext.CurrentContext.TestDirectory;
            var board = new Board($"{dir}/Maps/Map1.txt", logger);
            var positions = new Dictionary<IGnome, BoardPosition>();
            var gnomeHandler = new GnomeHandler(board, positions, logger);
            var gnomes = new List<Gnome>
            {
                new Gnome(1, 1, 2, null),
                new Gnome(2, 1, 4, null),
                new Gnome(1, 2, 2, null),
                new Gnome(2, 2, 4, null),
            };

            // Act
            gnomeHandler.PlaceGnomesOnBoard(gnomes);

            // Assert
            // assert all positions are unique
            Assert.AreEqual(4, positions.Count());
            Assert.AreEqual(true, positions.Values.All(x => 1 == positions.Values.Count(y => y.Column == x.Column && y.Row == x.Row)));
            
            // assert all gnomes on valid tiles
            Assert.AreEqual(true, positions.Values.All(x => x.Tile.CanMoveThrough));

            // assert all gnomes are unique
            Assert.AreEqual(true, positions.Keys.All(x => 1 == gnomes.Count(y => y == x)));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MultipleGnomesSurvive(bool expectMultipleSurvive)
        {
            // Arrange
            var positions = new Dictionary<IGnome, BoardPosition>
            {
                {MockRepository.GenerateMock<IGnome>(), new BoardPosition()}
            };
            if (expectMultipleSurvive)
            {
                positions.Add(MockRepository.GenerateMock<IGnome>(), new BoardPosition());
            }
            var gnomeHandler = new GnomeHandler(null, positions, null);

            // Act
            var multipleSurvive = gnomeHandler.MultipleGnomesSurvive();

            // Assert
            Assert.AreEqual(expectMultipleSurvive, multipleSurvive);
        }

        [Test]
        public void GnomesMoved()
        {
            // Arrange
            var gnome1 = MockRepository.GenerateMock<IGnome>();
            var gnome2 = MockRepository.GenerateMock<IGnome>();
            var positions = new Dictionary<IGnome, BoardPosition>
            {
                {gnome1, new BoardPosition()},
                {gnome2, new BoardPosition()},
            };
            var board = MockRepository.GenerateMock<IBoard>();
            var logger = MockRepository.GenerateMock<ILogger>();
            var gnomeHandler = new GnomeHandler(board, positions, logger);

            // Act
            gnomeHandler.MoveGnomes();

            // Assert
            gnome1.AssertWasCalled(x => x.Move(), options => options.Repeat.Once());
            gnome2.AssertWasCalled(x => x.Move(), options => options.Repeat.Once());
        }

        [Test]
        public void GnomesKilled()
        {
            // Arrange
            var gnome1 = MockRepository.GenerateMock<IGnome>();
            var gnome2 = MockRepository.GenerateMock<IGnome>();
            var positions = new Dictionary<IGnome, BoardPosition>
            {
                {gnome1, new BoardPosition()},
                {gnome2, new BoardPosition()},
            };
            var gnomeHandler = new GnomeHandler(null, positions, null);

            // Act
            gnomeHandler.KillGnome(gnome1);

            // Assert
            Assert.AreEqual(1, positions.Count);
            Assert.AreEqual(true, positions.ContainsKey(gnome2));
            Assert.AreEqual(false, positions.ContainsKey(gnome1));
        }

        [Test]
        public void GetsGnomesAtPosition()
        {
            // Arrange
            var gnome1 = MockRepository.GenerateMock<IGnome>();
            var gnome2 = MockRepository.GenerateMock<IGnome>();
            var gnome3 = MockRepository.GenerateMock<IGnome>();
            var gnome4 = MockRepository.GenerateMock<IGnome>();
            var boardPosition1 = new BoardPosition(1, 1, null);
            var boardPosition2 = new BoardPosition(1, 2, null);
            var boardPosition3 = new BoardPosition(2, 1, null);
            var positions = new Dictionary<IGnome, BoardPosition>
            {
                {gnome1, boardPosition1},
                {gnome2, boardPosition2},
                {gnome3, boardPosition3},
                {gnome4, boardPosition1},
            };
            var gnomeHandler = new GnomeHandler(null, positions, null);

            // Act
            var gnomesAtPosition = gnomeHandler.GetGnomesAtPosition(boardPosition1);

            // Assert
            Assert.AreEqual(2, gnomesAtPosition.Count());
            Assert.AreEqual(1, gnomesAtPosition.Count(x => x == gnome1));
            Assert.AreEqual(1, gnomesAtPosition.Count(x => x == gnome4));
        }
    }
}
