using System.Drawing;
using VSS.Wator.Original;

namespace VSS.Wator.Optimized {
  public class Fish : Animal {

    public override Color Color => Color.White;

    public Fish(OptimizedWatorWorld world, int position, int age)
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
