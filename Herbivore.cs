using DocumentFormat.OpenXml.Drawing.Charts;
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
    internal class Herbivore : Boid
    {
        public Herbivore(Vector2 _pos) : base(_pos)
        {
        }

        public override void Update()
        {
            Move();
            if (World.energy > 0)
            {
                energy += Settings.herbivoreEnergyGain;
                World.energy -= 1;
            }
            age++;
            if (energy < 0 || age > deathTime)
            {
                die();
            }
            if (energy > Settings.herbivoreReproductionThreshhold)
            {
                reproduce();
                energy = Settings.initialEnergy;
            }
        }

        void Move()
        {
            if (angle > Math.PI)
            {
                angle -= Math.PI * 2;
            }
            if (angle < -Math.PI)
            {
                angle += Math.PI * 2;
            }
            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            int neighbors = 0;
            Vector2 avgHeading = new Vector2(0, 0);
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
                        if ((b.pos - pos).Length() < avoidance)
                        {
                            velocity -= (float)avoidStrength * (b.pos - pos);
                        }
                        neighbors++;
                        avgHeading += new Vector2((float)Math.Cos(b.angle), (float)Math.Sin(b.angle));
                        avgPos += b.pos - pos;
                    }
                }
            }
            if (neighbors > 0)
            {
                velocity += (float)alignStrength * avgHeading / neighbors;
                velocity += (float)cohesionStrength * avgPos / neighbors;
            }
            double newAngle = Math.Atan2(velocity.Y, velocity.X);
            double speed = maxSpeed - Math.Abs(newAngle - angle)/Math.PI * (maxSpeed-minSpeed);
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
            Herbivore offspring = new Herbivore(pos);
            offspring.pos.X += (float)Utility.rand.NextDouble() * 10;
            offspring.pos.Y += (float)Utility.rand.NextDouble() * 10;
            World.spawnQueue.Enqueue(offspring);
        }
    }
}
