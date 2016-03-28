using System;
using System.Drawing;

namespace VSS.Wator.Original {
  public class Fish : Animal {

    public override Color Color => Color.White;

    public Fish(OriginalWatorWorld world, Point position, int age)
      : base(world, position) {
      Energy = world.InitialFishEnergy;
      Age = age;
    }

    public override void ExecuteStep() {
      if (Moved) throw new InvalidProgramException("Tried to move a fish twice in one time step.");
      Age++;

      Point free = World.SelectNeighbor(null, Position);

      if (free.X != -1) Move(free);
      if (Age >= World.FishBreedTime) Spawn();
    }

    protected override void Spawn() {
      Point free = World.SelectNeighbor(null, Position);
      if (free.X != -1) {
        Fish fish = new Fish(World, free, 0);
        Age -= World.FishBreedTime;
      }
    }
  }
}
