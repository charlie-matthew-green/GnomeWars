using System.Linq;
using NUnit.Framework;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class GnomeCreatorTests
    {
        [Test]
        public void GeneratesGnomes()
        {
            // Arrange
            var gnomeCreator = new GnomeCreator(null);

            // Act
            var gnomes = gnomeCreator.GenerateGnomes(2, 7);

            // Assert
            Assert.AreEqual(14, gnomes.Count());
            var group1Gnomes = gnomes.Where(x => x.GnomeGroup == 1);
            var group2Gnomes = gnomes.Where(x => x.GnomeGroup == 2);
            Assert.AreEqual(7, group1Gnomes.Count());
            Assert.AreEqual(7, group2Gnomes.Count());

            // check all gnomes in a group have unique ids
            Assert.AreEqual(true, group1Gnomes.All(x => group1Gnomes.Count(y => y.Id == x.Id) == 1));
            Assert.AreEqual(true, group2Gnomes.All(x => group2Gnomes.Count(y => y.Id == x.Id) == 1));

            // check gnome strength
            var strengths = new[] {4, 3, 2, 1, 1, 1, 1};
            foreach (var strength in strengths.Where(x => x != 1))
            {
                Assert.AreEqual(1, group1Gnomes.Count(x => x.GnomeStrength == strength));
                Assert.AreEqual(1, group2Gnomes.Count(x => x.GnomeStrength == strength));
            }
            Assert.AreEqual(4, group1Gnomes.Count(x => x.GnomeStrength == 1));
            Assert.AreEqual(4, group2Gnomes.Count(x => x.GnomeStrength == 1));
        }
    }
}
