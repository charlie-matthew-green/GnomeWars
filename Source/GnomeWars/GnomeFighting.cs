using System;
using System.Collections.Generic;
using System.Linq;

namespace GnomeWars
{
    public interface IGnomeFighting
    {
        List<IGnome> CombineGnomesOfSameGroup(IEnumerable<IGnome> gnomesAtPosition, BoardPosition position);
        void FightGnomesOfDifferentType(IList<IGnome> gnomes, BoardPosition position);
    }

    public class GnomeFighting : IGnomeFighting
    {
        private readonly ILogger logger;
        private readonly IGnomeHandler gnomeHandler;
        private readonly Random random = new Random(DateTime.UtcNow.Millisecond);

        public GnomeFighting(IGnomeHandler gnomeHandler, ILogger logger)
        {
            this.logger = logger;
            this.gnomeHandler = gnomeHandler;
        }

        public List<IGnome> CombineGnomesOfSameGroup(IEnumerable<IGnome> gnomesAtPosition, BoardPosition position)
        {
            var groupedGnomes = gnomesAtPosition.GroupBy(x => x.GnomeGroup, (key, value) => value.ToList());
            var combinedGnomesPerGroup = new List<IGnome>();
            foreach (var gnomeGroup in groupedGnomes)
            {
                while (gnomeGroup.Count() > 1)
                {
                    var combinedStrength = gnomeGroup[0].GnomeStrength + gnomeGroup[1].GnomeStrength;
                    PrintMergeResult(gnomeGroup, position, combinedStrength);
                    gnomeHandler.KillGnome(gnomeGroup[1]);
                    gnomeGroup.Remove(gnomeGroup[1]);
                    gnomeGroup[0].GnomeStrength = combinedStrength;
                }

                combinedGnomesPerGroup.Add(gnomeGroup[0]);
            }

            return combinedGnomesPerGroup;
        }

        public void FightGnomesOfDifferentType(IList<IGnome> gnomes, BoardPosition position)
        {
            while (gnomes.Count > 1)
            {
                int winnerIndex;
                int loserIndex;

                if (gnomes[0].GnomeStrength > gnomes[1].GnomeStrength)
                {
                    winnerIndex = 0;
                    loserIndex = 1;
                }
                else if (gnomes[1].GnomeStrength > gnomes[0].GnomeStrength)
                {
                    winnerIndex = 1;
                    loserIndex = 0;
                }
                else
                {
                    winnerIndex = random.Next(0, 1);
                    loserIndex = winnerIndex == 1 ? 0 : 1;
                }

                gnomeHandler.KillGnome(gnomes[loserIndex]);
                PrintFightResult(gnomes, position, winnerIndex, loserIndex);
                gnomes.Remove(gnomes[loserIndex]);
            }
        }

        
        private void PrintMergeResult(IList<IGnome> gnomeGroup, BoardPosition occupiedPosition, int combinedStrength)
        {
            logger.LogLine(
                $"Gnome {gnomeGroup[0].Id} from team {gnomeGroup[0].GnomeGroup}, gnome {gnomeGroup[1].Id} from team {gnomeGroup[1].GnomeGroup} " +
                $"have met at ({occupiedPosition.Column},{occupiedPosition.Row}) and combined into a strength {combinedStrength} gnome.");
            logger.Wait();
        }

        private void PrintFightResult(IList<IGnome> mergedGnomes, BoardPosition occupiedPosition, int winnerIndex, int loserIndex)
        {
            logger.LogLine(
                $"Gnome {mergedGnomes[winnerIndex].Id} from team {mergedGnomes[winnerIndex].GnomeGroup}, gnome {mergedGnomes[loserIndex].Id} from team {mergedGnomes[loserIndex].GnomeGroup} " +
                $"have fought at ({occupiedPosition.Column},{occupiedPosition.Row}) and gnome {mergedGnomes[winnerIndex].Id} from team {mergedGnomes[winnerIndex].GnomeGroup} was victorious.");
            logger.Wait();
        }
    }
}
