using DocumentFormat.OpenXml.Drawing.Charts;
using System;
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
        }

        void Move()
        {
            Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            int neighbors = 0;
            Vector2 avgHeading = new Vector2(0, 0);
            Vector2 avgPos = new Vector2(0, 0);
            foreach (Boid b in World.boids)
            {
                if (b == this) continue;
                if ((b.pos - pos).Length() < vision)
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
    }
}
