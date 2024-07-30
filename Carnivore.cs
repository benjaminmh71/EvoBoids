using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EvoBoids
{
    internal class Carnivore : Boid
    {
        double huntStrength = 1;
        int digestionTimer = 0;

        public Carnivore(Vector2 _pos) : base(_pos)
        {
            //maxSpeed = 2.1;
            //minSpeed = 1.05;
        }

        public override void Update()
        {
            Move();

            if (digestionTimer > 0)
            {
                digestionTimer--;
                maxSpeed = 1;
                minSpeed = 0.5;
            } else
            {
                maxSpeed = 2;
                minSpeed = 1;
            }

            energy += Settings.carnivoreEnergyGain;
            age++;
            if (energy < 0 || age > deathTime)
            {
                die();
            }
            if (energy > Settings.carnivoreReproductionThreshhold)
            {
                reproduce();
                energy = Settings.initialEnergy;
            }
        }

        void Move()
        {
            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            int neighbors = 0;
            Vector2 avgPos = new Vector2(0, 0);
            ArrayList nearbyBoids = new ArrayList();
            World.tree.getBoids(pos.X, pos.Y, vision, nearbyBoids);
            foreach (Boid b in nearbyBoids)
            {
                if (b == this) continue;
                if ((b.pos - pos).Length() < vision)
                {
                    if (b is Herbivore)
                    {
                        neighbors++;
                        avgPos += b.pos - pos;
                        if ((b.pos - pos).Length() < Settings.carnivoreSize)
                        {
                            eat(b);
                        }
                    } else if (b is Carnivore)
                    {
                        velocity -= (float)avoidStrength * (b.pos - pos);
                    }
                }
            }
            if (neighbors > 0)
            {
                velocity += (float)huntStrength * avgPos / neighbors;
            }
            double newAngle = Math.Atan2(velocity.Y, velocity.X);
            double speed = maxSpeed - Math.Abs(newAngle - angle) / Math.PI * (maxSpeed - minSpeed);
            if (newAngle - angle > turnSpeed)
            {
                angle += turnSpeed;
            }
            else if (angle - newAngle > turnSpeed)
            {
                angle -= turnSpeed;
            }
            else
            {
                angle = newAngle;
            }
            pos += (float)speed * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            if (pos.X < 0)
            {
                pos.X += World.width;
            }
            if (pos.X > World.width)
            {
                pos.X -= World.width;
            }
            if (pos.Y < 0)
            {
                pos.Y += World.height;
            }
            if (pos.Y > World.height)
            {
                pos.Y -= World.height;
            }
        }

        void reproduce()
        {
            Carnivore offspring = new Carnivore(pos);
            offspring.pos.X += (float)Utility.rand.NextDouble() * 10;
            offspring.pos.Y += (float)Utility.rand.NextDouble() * 10;
            World.spawnQueue.Enqueue(offspring);
        }

        void eat(Boid b)
        {
            if (digestionTimer > 0) return;
            b.die();
            energy += Settings.herbivoreEatenEnergy;
            digestionTimer += Settings.digestionTime;
        }
    }
}
