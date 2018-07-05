using System.Collections.Generic;
using System.Linq;

namespace GnomeWars
{
    public interface IGnomeCreator
    {
        IEnumerable<IGnome> GenerateGnomes(int numTypes, int numPerType);
    }

    public class GnomeCreator : IGnomeCreator
    {
        private readonly GnomeMovement gnomeMovement;

        public GnomeCreator(GnomeMovement gnomeMovement)
        {
            this.gnomeMovement = gnomeMovement;
        }

        public IEnumerable<IGnome> GenerateGnomes(int numTypes, int numPerType)
        {
            var gnomes = new List<IGnome>();
            var gnomeStrengths = GenerateGnomeStrengths(numPerType).ToArray();
            for (var gnomeGroup = 1; gnomeGroup <= numTypes; gnomeGroup++)
            {
                for (var id = 1; id <= numPerType; id++)
                {
                    gnomes.Add(new Gnome(id, gnomeGroup, gnomeStrengths[id - 1], gnomeMovement));
                }
            }

            return gnomes;
        }

        private IEnumerable<int> GenerateGnomeStrengths(int numPerType)
        {
            var strengths = new List<int>();
            var strength = 4;
            for (var i = 1; i <= numPerType; i++)
            {
                strengths.Add(strength);
                if (strength > 1)
                {
                    strength--;
                }
            }

            return strengths;
        }
    }
}
