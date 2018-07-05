using System.Linq;

namespace GnomeWars
{
    public class Game
    {
        private readonly IGnomeFighting gnomeFighting;
        private readonly ILogger logger;
        private readonly IGnomeHandler gnomeHandler;

        public Game(int numTypesOfGnome, int numGnomesPerType, IGnomeCreator gnomeCreator, IGnomeHandler gnomeHandler,
            IGnomeFighting gnomeFighting, ILogger logger)
        {
            this.gnomeFighting = gnomeFighting;
            this.logger = logger;
            this.gnomeHandler = gnomeHandler;
            var gnomes = gnomeCreator.GenerateGnomes(numTypesOfGnome, numGnomesPerType);
            gnomeHandler.PlaceGnomesOnBoard(gnomes);
        }

        public void Play()
        {
            LogIntroduction();
            logger.Wait();
            while (gnomeHandler.MultipleGnomesSurvive())
            {
                gnomeHandler.MoveGnomes();
                FightGnomes();
            }
            var gnomeWinner = gnomeHandler.GetWinner();
            logger.LogLine($"The gnome wars are over! Gnome team {gnomeWinner.GnomeGroup} is victorious, long live gnomes of team {gnomeWinner.GnomeGroup}.");
            logger.Wait();
        }

        private void FightGnomes()
        {
            foreach (var position in gnomeHandler.GetDistinctPositions().ToList())
            {
                var gnomesAtPosition = gnomeHandler.GetGnomesAtPosition(position);
                var combinedGnomesPerGroup = gnomeFighting.CombineGnomesOfSameGroup(gnomesAtPosition, position);
                gnomeFighting.FightGnomesOfDifferentType(combinedGnomesPerGroup, position);
            }
        }

        private void LogIntroduction()
        {
            logger.LogLine("");
            logger.LogLine("Welcome to Gnome Wars! A few things:");
            logger.LogLine("");
            logger.LogLine("1. Each gnome is represented by a number which is the value of its strength");
            logger.LogLine("2. The colour indicates which team the gnome belongs to. All gnomes in the same team are the same colour.");
            logger.LogLine("");
            logger.LogLine("Now let the battle commence!");
            logger.LogLine("");
        }
    }
}
