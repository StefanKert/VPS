using System;
using System.Drawing;

namespace VSS.Wator.Original {
  public class Fish : Animal {

    public override Color Color => Color.White;

    public Fish(OriginalWatorWorld world, int position, int age)
      : base(world, position) {
      Energy = world.InitialFishEnergy;
      Age = age;
    }

    public override void ExecuteStep() {
      if (Moved) throw new InvalidProgramException("Tried to move a fish twice in one time step.");
      Age++;

      var free = World.SelectFreeNeighbor(Position);

      if (free != -1) Move(free);
      if (Age >= World.FishBreedTime) Spawn();
    }

    protected override void Spawn() {
      var free = World.SelectFreeNeighbor(Position);
        if (free == -1)
            return;
        var fish = new Fish(World, free, 0);
        Age -= World.FishBreedTime;
    }
  }
}
