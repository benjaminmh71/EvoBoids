using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EvoBoids
{
    internal abstract class Boid
    {
        public int vision = 30;
        public int avoidance = 20;
        public double maxSpeed = 2;
        public double minSpeed = 1;
        public double turnSpeed = 0.1;
        public double avoidStrength = 10;
        public double alignStrength = 1;
        public double cohesionStrength = 0.01;

        public int deathTime = (int)(Settings.minLifetime * (1 + Utility.rand.NextDouble()));
        public int age = 0;
        public double energy = Settings.initialEnergy + Utility.rand.NextDouble() - 0.5;

        public Vector2 pos;
        public double angle;

        public Boid(Vector2 _pos)
        {
            pos = _pos;
            angle = Utility.rand.NextDouble() * 2 * Math.PI;
        }

        public void die()
        {
            World.killQueue.Enqueue(this);
        }

        public abstract void Update();
    }
}
