using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Security.Cryptography;

namespace EvoBoids
{
    internal class Quadtree
    {
        double x;
        double y;
        double width;
        double height;
        int capacity = 4;
        Quadtree topLeft;
        Quadtree topRight;
        Quadtree bottomLeft;
        Quadtree bottomRight;
        bool divided = false;
        ArrayList boids;

        public Quadtree(double _x, double _y, double _width, double _height)
        {
            x = _x;
            y = _y;
            width = _width;
            height = _height;
            boids = new ArrayList();
        }

        public void addBoid(Boid b)
        {
            Vector2 p = b.pos;
            if (p.X < x-width/2 
                || p.X > x+width/2
                || p.Y < y-height/2
                || p.Y > y+height/2)
            {
                return;
            }
            if (boids.Count < capacity)
            {
                boids.Add(b);
                return;
            } else
            {
                if (!divided)
                {
                    subdivide();
                    divided = true;
                }

                topLeft.addBoid(b);
                topRight.addBoid(b);
                bottomLeft.addBoid(b);
                bottomRight.addBoid(b);
            }
        }

        public void subdivide() 
        {
            topLeft = new Quadtree(x - width / 4, y - height / 4, width / 2, height / 2);
            topRight = new Quadtree(x + width / 4, y - height / 4, width / 2, height / 2);
            bottomLeft = new Quadtree(x - width / 4, y + height / 4, width / 2, height / 2);
            bottomRight = new Quadtree(x + width / 4, y + height / 4, width / 2, height / 2);
        }

        public void getBoids(double boidX, double boidY, double range, ArrayList list)
        {
            if (!isOverlapping(boidX, boidY, range))
            {
                return;
            }

            foreach (Boid b in boids)
            {
                if ((b.pos - new Vector2((float)boidX, (float)boidY)).Length() < range)
                {
                    list.Add(b);
                }
            }

            if (divided)
            {
                topLeft.getBoids(boidX, boidY, range, list);
                topRight.getBoids(boidX, boidY, range, list);
                bottomLeft.getBoids(boidX, boidY, range, list);
                bottomRight.getBoids(boidX, boidY, range, list);
            }
        }

        public bool isOverlapping(double boidX, double boidY, double range) {
            return !(boidX - range/2 > x + width/2 ||
                boidX + range/2 < x - width/2 ||
                boidY - range/2 > y + height/2 ||
                boidY + range/2 < y - height/2);
        }

        public void show(Graphics g, Point cameraPosition)
        {
            Pen testPen = new Pen(Color.Black);
            g.DrawRectangle(testPen, new Rectangle(new Point((int)x-cameraPosition.X-(int)width/2, (int)y-cameraPosition.Y-(int)height/2), 
                new Size(new Point((int)width, (int)height))));
            
            if (divided)
            {
                topLeft.show(g, cameraPosition);
                topRight.show(g, cameraPosition);
                bottomLeft.show(g, cameraPosition);
                bottomRight.show(g, cameraPosition);
            }
        }
    }
}
