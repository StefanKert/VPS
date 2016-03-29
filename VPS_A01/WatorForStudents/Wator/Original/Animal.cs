using System.Drawing;

namespace VSS.Wator.Original
{
    // base class for animals (fish & sharks)
    public abstract class Animal
    {

        // the world that this animal lives in
        // the animal can check neighboring cells
        public OriginalWatorWorld World { get; private set; }

        // position of the animal in the world (x/y position)
        public int Position { get; private set; }

        public int IterationIdentifier { get; set; }

        // the age of the animal (only relevant for fish)
        public int Age { get; protected set; }

        // the energy of the animal
        // sharks need to eat fish to increase energy
        // the energy of a fish is constant
        public int Energy { get; protected set; }

        // boolean flag that indicates wether an animal has moved in the current iteration
        public bool Moved => IterationIdentifier == World.Iteration;

        // the color of the enimal (e.g. fish=white, shark=red)
        public abstract Color Color { get; }


        // ctor: create a new animal on the specified position of the given world
        public Animal(OriginalWatorWorld world, int position) {
            World = world;
            Position = position;
            Age = 0;
            IterationIdentifier = -1;
            Energy = 0;
            // place the new animal in the world
            World.Grid[position] = this;
        }

        // move the animal to a given position
        // does not check if the position can be reached by the animal
        protected void Move(int destination) {
            IterationIdentifier = World.Iteration;
            World.Grid[Position] = null;
            World.Grid[destination] = this;
            Position = destination;
        }

        // execute one simulation step for this animal 
        // animal behaviour is implemented in the specific classes (fish, shark)
        public abstract void ExecuteStep();

        // animals can spawn to create new children
        // specific spawning behaviour of animal is implemented in the specific classes
        protected abstract void Spawn();
    }
}
