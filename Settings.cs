using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoBoids
{
    internal static class Settings
    {
        // World settings:
        public const int availableEnergy = 1000;

        // Creature settings:
        public const int carnivoreSize = 10;
        public const int herbivoreSize = 8;

        public const double initialEnergy = 2.0;
        public const double carnivoreEnergyGain = -0.0025;
        public const double herbivoreEnergyGain = 0.0035;
        public const double carnivoreReproductionThreshhold = 5.0;
        public const double herbivoreReproductionThreshhold = 5.0;
        public const double herbivoreEatenEnergy = 1;
        public const int digestionTime = 100;

        public const int minLifetime = 1500;
    }
}
