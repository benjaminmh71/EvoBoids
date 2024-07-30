using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EvoBoids
{
    internal static class World
    {
        public static HashSet<Boid> boids = new HashSet<Boid>();
        public static Queue<Boid> spawnQueue = new Queue<Boid>();
        public static Queue<Boid> killQueue = new Queue<Boid>();
        public static Quadtree tree = new Quadtree(width/2, height/2, width, height);
        public static double energy = 0;
        public static int width = 2000;
        public static int height = 2000;

        public static void Initialize()
        {
            for (int i = 0; i < 1000; i++)
            {
                boids.Add(new Herbivore(new Vector2(Utility.rand.Next() % 
                    width, Utility.rand.Next() % height)));
            }

            for (int i = 0; i < 50; i++)
            {
                boids.Add(new Carnivore(new Vector2(Utility.rand.Next() %
                    width, Utility.rand.Next() % height)));
            }

            foreach (Boid b in boids)
            {
                b.energy += Utility.rand.NextDouble() * 2;
                b.age += (int) (Utility.rand.NextDouble() * Settings.minLifetime);
            }
        }

        public static void Update()
        {
            energy = Settings.availableEnergy;

            tree = new Quadtree(width / 2, height / 2, width, height);
            foreach (Boid b in boids) 
            {
                tree.addBoid(b);
            }
            foreach (Boid b in boids)
            {
                b.Update();
            }
            while (spawnQueue.Count > 0)
            {
                boids.Add(spawnQueue.Dequeue());
            }
            while (killQueue.Count > 0)
            {
                boids.Remove(killQueue.Dequeue());
            }
        }
    }
}
