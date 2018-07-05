using NUnit.Framework;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class BoardTests
    {
        [Test]
        public void CreatesBoard()
        {
            // Arrange
            var dir = TestContext.CurrentContext.TestDirectory;

            // Act
            var board = new Board($"{dir}/Maps/Map1.txt", null);

            // Assert
            Assert.IsNotNull(board);
            Assert.AreNotEqual(0, board.ColumnCount);
            Assert.AreNotEqual(0, board.RowCount);
        }

        [Test]
        public void GetsPosition()
        {
            // Arrange
            var dir = TestContext.CurrentContext.TestDirectory;
            var board = new Board($"{dir}/Maps/Map1.txt", null);

            // Act
            var position = board.GetPosition(1, 2);

            // Assert
            Assert.AreEqual(1, position.Column);
            Assert.AreEqual(2, position.Row);
            Assert.NotNull(position.Tile);
        }
    }
}
