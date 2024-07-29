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

        public Carnivore(Vector2 _pos) : base(_pos)
        {
            maxSpeed = 2.2;
            minSpeed = 1.1;
        }

        public override void Update()
        {
            Move();
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

        void eat(Boid b)
        {
            b.die();
        }
    }
}
