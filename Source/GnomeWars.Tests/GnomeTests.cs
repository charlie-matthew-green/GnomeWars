using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace GnomeWars.Tests
{
    [TestFixture]
    public class GnomeTests
    {
        [Test]
        public void InitiallyMovesInRandomDirection()
        {
            // Arrange
            const int timesRun = 10000;
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            gnomeMovement.Stub(x => x.IsFrozen(Arg<Gnome>.Is.Anything)).Return(false); 
            
            var northCount = 0;
            var eastCount = 0;
            var southCount = 0;
            var westCount = 0;

            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.North)))
                .Do((Func<IGnome, Direction, bool>) ((g, d) => { 
                    northCount++;
                    return true;
                }));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.East)))
                .Do((Func<IGnome, Direction, bool>) ((g, d) => { 
                    eastCount++;
                    return true;
                }));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.South)))
                .Do((Func<IGnome, Direction, bool>) ((g, d) => { 
                    southCount++;
                    return true;
                }));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.West)))
                .Do((Func<IGnome, Direction, bool>) ((g, d) => { 
                    westCount++;
                    return true;
                }));

            // Act
            for (var i = 0; i < timesRun; i++)
            {
                var gnome = new Gnome(1, 1, 1, gnomeMovement);
                gnome.Move();
            }

            // Assert
            var lowerLimit = 2000;
            var upperLimit = 3000;
            Assert.AreEqual(timesRun, northCount + eastCount + southCount + westCount);
            Assert.AreEqual(true, northCount > lowerLimit && northCount < upperLimit);
            Assert.AreEqual(true, eastCount > lowerLimit && eastCount < upperLimit);
            Assert.AreEqual(true, southCount > lowerLimit && southCount < upperLimit);
            Assert.AreEqual(true, westCount > lowerLimit && westCount < upperLimit);
        }

        [Test]
        public void ChangesDirectionIfBlocked()
        {
            // Arrange
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomeMovement.Stub(x => x.IsFrozen(gnome)).Return(false);
            Direction? firstDirectionChosen = null;
            var firstDirectionCount = 0;
            var otherDirectionCount = 0;

            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.North)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckDifferentDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.East)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckDifferentDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.South)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckDifferentDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.West)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckDifferentDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));


            // Act
            gnome.Move();
            gnome.Move();

            // Assert
            Assert.AreEqual(1, firstDirectionCount);
            Assert.AreEqual(1, otherDirectionCount);
        }

        [Test]
        public void DoesntMoveIfFrozen()
        {
            // Arrange
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomeMovement.Stub(x => x.IsFrozen(gnome)).Return(true); 

            // Act
            gnome.Move();

            // Assert
            gnomeMovement.AssertWasNotCalled(x => x.TryMove(Arg<Gnome>.Is.Equal(gnome), Arg<Direction>.Is.Anything));
        }

        [Test]
        public void DoesntTryDirectionMultipleTimes()
        {
            // Arrange
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomeMovement.Stub(x => x.IsFrozen(gnome)).Return(false); 
            gnomeMovement.Stub(x => x.TryMove(gnome, Direction.North)).Return(false);
            gnomeMovement.Stub(x => x.TryMove(gnome, Direction.East)).Return(false);
            gnomeMovement.Stub(x => x.TryMove(gnome, Direction.South)).Return(false);
            gnomeMovement.Stub(x => x.TryMove(gnome, Direction.West)).Return(false);

            // Act & Assert
            Assert.Throws<Exception>(gnome.Move);
            gnomeMovement.AssertWasCalled(x => x.TryMove(gnome, Direction.North), options => options.Repeat.Once());
            gnomeMovement.AssertWasCalled(x => x.TryMove(gnome, Direction.East), options => options.Repeat.Once());
            gnomeMovement.AssertWasCalled(x => x.TryMove(gnome, Direction.South), options => options.Repeat.Once());
            gnomeMovement.AssertWasCalled(x => x.TryMove(gnome, Direction.West), options => options.Repeat.Once());
        }

        [Test]
        public void ContinuesDirectionUntilBlocked()
        {
            // Arrange
            var gnomeMovement = MockRepository.GenerateMock<IGnomeMovement>();
            var gnome = new Gnome(1, 1, 1, gnomeMovement);
            gnomeMovement.Stub(x => x.IsFrozen(gnome)).Return(false);

            Direction? firstDirectionChosen = null;
            var firstDirectionCount = 0;
            var otherDirectionCount = 0;

            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.North)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckSameDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.East)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckSameDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.South)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckSameDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));
            gnomeMovement.Stub(x => x.TryMove(Arg<Gnome>.Is.Anything, Arg<Direction>.Is.Equal(Direction.West)))
                .Do((Func<IGnome, Direction, bool>)((g, d) => CheckSameDirection(d, ref firstDirectionChosen, ref firstDirectionCount, ref otherDirectionCount)));

            // Act
            for (var i = 0; i < 100; i++)
            {
                gnome.Move();
            }

            // Assert
            Assert.AreEqual(firstDirectionCount, 100);
            Assert.AreEqual(otherDirectionCount, 0);
        }

        // ensures the gnome must move in another direction after the first direction chosen
        private bool CheckDifferentDirection(Direction currentDirection, ref Direction? firstDirectionChosen, ref int firstDirectionCount, ref int otherDirectionCount)
        {
            if (firstDirectionChosen == null)
            {
                firstDirectionChosen = currentDirection;
                return true;
            }

            if (currentDirection == firstDirectionChosen.Value)
            {
                firstDirectionCount++;
                return false;
            }

            otherDirectionCount++;
            return true;
        }

        // checks that gnome is still moving in the same direction as initially chosen
        private bool CheckSameDirection(Direction currentDirection, ref Direction? firstDirectionChosen, ref int firstDirectionCount, ref int otherDirectionCount)
        {
            if (firstDirectionChosen == null)
            {
                firstDirectionChosen = currentDirection;
            }

            if (currentDirection == firstDirectionChosen.Value)
            {
                firstDirectionCount++;
            }
            else
            {
                otherDirectionCount++;
            }
            return true;
        }
    }
}
