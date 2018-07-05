using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class GnomeFightingTests
    {
        [Test] 
        public void CombinesGnomesCorrectly()
        {
            // Arrange
            var gnomeCreator = MockRepository.GenerateMock<IGnomeCreator>();
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnomeHandler = MockRepository.GenerateMock<IGnomeHandler>();
            var gnomes = new List<Gnome>
            {
                new Gnome(1, 1, 2, gnomeMovement),
                new Gnome(2, 1, 4, gnomeMovement),
                new Gnome(3, 1, 6, gnomeMovement),
                new Gnome(1, 2, 2, gnomeMovement),
            };
            gnomeCreator.Stub(x => x.GenerateGnomes(1, 3)).Return(gnomes);
            var logger = MockRepository.GenerateMock<ILogger>();
            var gnomeFighting = new GnomeFighting(gnomeHandler, logger);

            // Act
            var combinedGnomes = gnomeFighting.CombineGnomesOfSameGroup(gnomes, new BoardPosition());

            // Assert
            Assert.AreEqual(2, combinedGnomes.Count);
            Assert.AreEqual(1, combinedGnomes.Count(x => x.GnomeGroup == 1 && x.GnomeStrength == 12));
            Assert.AreEqual(1, combinedGnomes.Count(x => x.GnomeGroup == 2 && x.GnomeStrength == 2));
            logger.AssertWasCalled(x => x.LogLine(Arg<string>.Is.Anything), options => options.Repeat.Twice());
            gnomeHandler.AssertWasCalled(x => x.KillGnome(gnomes[1]), options => options.Repeat.Once());
            gnomeHandler.AssertWasCalled(x => x.KillGnome(gnomes[2]), options => options.Repeat.Once());
            gnomeHandler.AssertWasNotCalled(x => x.KillGnome(gnomes[0]));
            gnomeHandler.AssertWasNotCalled(x => x.KillGnome(gnomes[3]));
        }

        [Test]
        public void FightsGnomesCorrectly()
        {
            // Arrange
            var gnomeCreator = MockRepository.GenerateMock<IGnomeCreator>();
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnomeHandler = MockRepository.GenerateMock<IGnomeHandler>();
            var gnome1 = new Gnome(1, 1, 2, gnomeMovement);
            var gnome2 = new Gnome(1, 2, 4, gnomeMovement);
            var gnome3 = new Gnome(1, 3, 6, gnomeMovement);
            var gnome4 = new Gnome(1, 4, 2, gnomeMovement);
            var gnomes = new List<IGnome> {gnome1, gnome2, gnome3, gnome4};
            gnomeCreator.Stub(x => x.GenerateGnomes(1, 3)).Return(gnomes);
            var logger = MockRepository.GenerateMock<ILogger>();
            var gnomeFighting = new GnomeFighting(gnomeHandler, logger);

            // Act
            gnomeFighting.FightGnomesOfDifferentType(gnomes, new BoardPosition());

            // Assert
            gnomeHandler.AssertWasCalled(x => x.KillGnome(gnome1), options => options.Repeat.Once());
            gnomeHandler.AssertWasCalled(x => x.KillGnome(gnome2), options => options.Repeat.Once());
            gnomeHandler.AssertWasCalled(x => x.KillGnome(gnome4), options => options.Repeat.Once());
            gnomeHandler.AssertWasNotCalled(x => x.KillGnome(gnome3));
            logger.AssertWasCalled(x => x.LogLine(Arg<string>.Is.Anything), options => options.Repeat.Times(3));
        }
    }
}
