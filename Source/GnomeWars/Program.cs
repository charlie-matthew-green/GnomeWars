using System;
using System.Collections.Generic;

namespace GnomeWars
{
    class Program
    {
        /// <summary>
        /// Limitations/Assumptions of the program:
        ///
        /// 1. Assuming that the board will be read in as a text file, with hashes as walls, spaces as corridors,
        /// and a continous wall surrounding the boundary. The map should also be rectangular.
        /// 
        /// 2. There are some limitations because of the choice to represent gnomes as the number which is their strength,
        /// in the colour which is their team. First, because of the way this is displayed if gnomes combine into
        /// double digit strength then this throws off the alignment in the display so it doesn't look as good.
        /// Secondly I've made the assumption there won't be more than 5 teams, and so only implemented a colour scheme
        /// for up to 5 teams.
        ///
        /// 3. Assuming if gnomes of same strength fight it should be random which gnome wins. Also assuming it should be
        /// a fair fight with equal distribution of strengths on both teams.
        ///
        /// 4. Assuming gnomes should continue in the direction they started in until they are blocked
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var logger = new Logger();

            try
            {
                var floorPlanFilepath = args[0];
                var numTypesOfGnome = int.Parse(args[1]);
                var numGnomesPerType = int.Parse(args[2]);
                var board = new Board(floorPlanFilepath, logger);
                var gnomePositions = new Dictionary<IGnome, BoardPosition>();
                var gnomeHandler = new GnomeHandler(board, gnomePositions, logger);
                var gnomeMovement = new GnomeMovement(board, gnomePositions);
                var gnomeCreator = new GnomeCreator(gnomeMovement);
                var gnomeFighting = new GnomeFighting(gnomeHandler, logger);
                var coordinator = new Game(numTypesOfGnome, numGnomesPerType, gnomeCreator, gnomeHandler, gnomeFighting, logger);
                coordinator.Play();
            }
            catch (Exception e)
            {
                logger.LogLine($"Fatal error occurred: {e}");
            }
        }
    }
}
