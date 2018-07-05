using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace GnomeWars.Tests
{
    public class GameTests
    {
        [Test]
        public void CheckSetUp()
        {
            // Arrange
            var gnomes = new List<Gnome>
            {
                new Gnome(1, 1, 1, null)
            };
            var gnomeCreator = MockRepository.GenerateMock<IGnomeCreator>();
            gnomeCreator.Stub(x => x.GenerateGnomes(1, 1)).Return(gnomes);
            var gnomeHandler = MockRepository.GenerateMock<IGnomeHandler>();
            var gnomeFighting = MockRepository.GenerateMock<IGnomeFighting>();
            var logger = MockRepository.GenerateMock<ILogger>();

            // Act
            var game = new Game(1, 1, gnomeCreator, gnomeHandler, gnomeFighting, logger);

            // Assert
            gnomeCreator.AssertWasCalled(x => x.GenerateGnomes(1, 1));
            gnomeHandler.AssertWasCalled(x => x.PlaceGnomesOnBoard(gnomes));
        }

        [Test]
        public void PlaysGame()
        {
            // Arrange
            var gnomes = new List<IGnome>
            {
                new Gnome(1, 1, 1, null)
            };
            var gnomeCreator = MockRepository.GenerateMock<IGnomeCreator>();
            gnomeCreator.Stub(x => x.GenerateGnomes(1, 1)).Return(gnomes);
            var gnomeHandler = MockRepository.GenerateMock<IGnomeHandler>();
            gnomeHandler.Stub(x => x.MultipleGnomesSurvive()).Return(true).Repeat.Once();
            var position = new BoardPosition(1, 1, null);
            gnomeHandler.Stub(x => x.GetDistinctPositions()).Return(new List<BoardPosition> {position});
            gnomeHandler.Stub(x => x.GetGnomesAtPosition(position)).Return(gnomes);
            gnomeHandler.Stub(x => x.GetWinner()).Return(gnomes[0]);
            var gnomeFighting = MockRepository.GenerateMock<IGnomeFighting>();
            gnomeFighting.Stub(x => x.CombineGnomesOfSameGroup(gnomes, position)).Return(gnomes);
            var logger = MockRepository.GenerateMock<ILogger>();
            var game = new Game(1, 1, gnomeCreator, gnomeHandler, gnomeFighting, logger);

            // Act
            game.Play();

            // Assert
            gnomeHandler.AssertWasCalled(x => x.MoveGnomes(), options => options.Repeat.Once());
            gnomeFighting.AssertWasCalled(x => x.CombineGnomesOfSameGroup(gnomes, position));
            gnomeFighting.AssertWasCalled(x => x.FightGnomesOfDifferentType(gnomes, position));
            gnomeHandler.AssertWasCalled(x => x.GetWinner());
        }
    }
}
