using System;
using System.Drawing;

namespace VSS.Wator.Original {
    public class Shark: Animal
    {

        public override Color Color => Color.Red;

        public Shark(OriginalWatorWorld world, int position, int energy): base(world, position) {
            Energy = energy;
        }

        public override void ExecuteStep() {
            Age++;
            Energy--;
            int freeField;
            var fish = World.SelectNeighborOfType<Fish>(Position, out freeField);
            if (fish != -1) {
                Energy += World.Grid[fish].Energy;
                Move(fish);
            }
            else if (freeField != -1)
                Move(freeField);

            if (fish != -1 || freeField != -1) {
                if (Energy >= World.SharkBreedEnergy)
                    Spawn();
            }
            if (Energy <= 0)
                World.Grid[Position] = null;
        }

        protected override void Spawn() {
            var free = World.SelectFreeNeighbor(Position);
            if (free == -1)
                return;
            Energy = Energy/2;
            var shark = new Shark(World, free, Energy);
        }
    }
}
