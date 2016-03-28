using System;
using System.Drawing;

namespace VSS.Wator.Original {
  public class Shark : Animal {

    public override Color Color => Color.Red;

    public Shark(OriginalWatorWorld world, int position, int energy)
      : base(world, position) {
      Energy = energy;
    }

    public override void ExecuteStep() {
      if (Moved) throw new InvalidProgramException("Tried to move a shark twice within one time step.");
      Age++;
      Energy--;

      var fish = World.SelectNeighborOfType<Fish>(Position);
      if (fish != -1) {
        Energy += World.Grid[fish].Energy;
        Move(fish);
      } else {
        var free = World.SelectFreeNeighbor(Position);
          if (free != -1) {
              Move(free);
          }
      }

      if (Energy >= World.SharkBreedEnergy) Spawn();
      if (Energy <= 0) World.Grid[Position] = null;
    }

    protected override void Spawn() {
      var free = World.SelectFreeNeighbor(Position);
        if (free == -1)
            return;
        var shark = new Shark(World, free, Energy / 2);
        Energy = Energy / 2;
    }
  }
}
