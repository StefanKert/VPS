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
          Age++;
          var free = World.SelectFreeNeighbor(Position);
          if (free == -1)
              return;
          Move(free);
          if (Age >= World.FishBreedTime)
              Spawn();
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
