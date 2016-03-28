using System;
using System.Drawing;

namespace VSS.Wator.Original {
  public class Shark : Animal {

    public override Color Color => Color.Red;

    public Shark(OriginalWatorWorld world, Point position, int energy)
      : base(world, position) {
      Energy = energy;
    }

    public override void ExecuteStep() {
      if (Moved) throw new InvalidProgramException("Tried to move a shark twice within one time step.");
      Age++;
      Energy--;

      Point fish = World.SelectNeighbor(typeof(Fish), Position);
      if (fish.X != -1) {
        Energy += World.Grid[fish.X, fish.Y].Energy;
        Move(fish);
      } else {
        Point free = World.SelectNeighbor(null, Position);
        if (free.X != -1) Move(free);
      }

      if (Energy >= World.SharkBreedEnergy) Spawn();
      if (Energy <= 0) World.Grid[Position.X, Position.Y] = null;
    }

    protected override void Spawn() {
      Point free = World.SelectNeighbor(null, Position);
      if (free.X != -1) {
        Shark shark = new Shark(World, free, Energy / 2);
        Energy = Energy / 2;
      }
    }
  }
}
