using DocumentFormat.OpenXml.Office.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EvoBoids
{
    public partial class EvoBoids : Form
    {
        public EvoBoids()
        {
            InitializeComponent();
        }

        Thread thread;
        Graphics g;
        Graphics fG;
        Bitmap bmp;
        Queue<DateTime> dateTimes = new Queue<DateTime>();

        Point previousMousePos = new Point(0, 0);
        Point cameraPosition = new Point(0, 0);

        private void EvoBoids_Load(object sender, EventArgs e)
        {
            Utility.width = Width;
            Utility.height = Height;
            bmp = new Bitmap(Width, Height);
            g = Graphics.FromImage(bmp);
            fG = CreateGraphics();
            thread = new Thread(Draw);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Draw()
        {
            World.Initialize();

            Pen outlinePen = new Pen(Color.Black);
            Font font = new Font("Arial", 16);
            SolidBrush textBrush = new SolidBrush(Color.Black);
            SolidBrush herbivoreBrush = new SolidBrush(Color.CornflowerBlue);
            SolidBrush carnivoreBrush = new SolidBrush(Color.OrangeRed);

            fG.Clear(Color.WhiteSmoke);
            while (true) {
                World.Update();

                g.Clear(Color.WhiteSmoke);
                foreach (Boid b in World.boids)
                {
                    int size = Settings.herbivoreSize;
                    if (b is Carnivore) size = Settings.carnivoreSize;
                    Vector2 head = b.pos +
                        size * new Vector2((float)Math.Cos(b.angle), (float)Math.Sin(b.angle));
                    Vector2 leftTail = b.pos +
                        size * new Vector2((float)Math.Cos(b.angle + Math.PI * 3 / 4), (float)Math.Sin(b.angle + Math.PI * 3 / 4));
                    Vector2 rightTail = b.pos +
                        size * new Vector2((float)Math.Cos(b.angle - Math.PI * 3 / 4), (float)Math.Sin(b.angle - Math.PI * 3 / 4));
                    Point[] pointArray = { 
                    new Point((int)head.X - cameraPosition.X, (int)head.Y - cameraPosition.Y), new Point((int)leftTail.X - cameraPosition.X, (int)leftTail.Y - cameraPosition.Y),
                    new Point((int)head.X - cameraPosition.X, (int)head.Y - cameraPosition.Y), new Point((int)rightTail.X - cameraPosition.X, (int)rightTail.Y - cameraPosition.Y),
                    new Point((int)rightTail.X - cameraPosition.X, (int)rightTail.Y - cameraPosition.Y), new Point((int)leftTail.X - cameraPosition.X, (int)leftTail.Y - cameraPosition.Y)};
                    if (b is Herbivore)
                    {
                        g.FillPolygon(herbivoreBrush, pointArray);
                    } else if (b is Carnivore)
                    {
                        g.FillPolygon(carnivoreBrush, pointArray);
                    }
                    g.DrawPolygon(outlinePen, pointArray);
                }

                //World.tree.show(g, cameraPosition);

                double avgTicks = 0;
                if (dateTimes.Count == 0) dateTimes.Enqueue(DateTime.Now);
                foreach (DateTime d in dateTimes)
                {
                    avgTicks += d.Ticks;
                }
                avgTicks /= dateTimes.Count;
                double deltaTime = DateTime.Now.Ticks - avgTicks;
                dateTimes.Enqueue(DateTime.Now);
                if (dateTimes.Count > 20)
                {
                    dateTimes.Dequeue();
                }
                int nH = 0;
                int nC = 0;
                foreach (Boid b in World.boids)
                {
                    if (b is Herbivore)
                    {
                        nH++;
                    } else
                    {
                        nC++;
                    }
                }
                g.DrawString(((int)(1/(deltaTime/100000000))).ToString()
                    + "  H: " + nH
                    + "  C: " + nC, font, textBrush, new Point(10, 10));
                fG.DrawImage(bmp, new Point(0, 0));
            }
        }
        
        public void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            // Console.WriteLine("start wait timer");
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
                // Console.WriteLine("stop wait timer");
            };

            while (timer1.Enabled)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void EvoBoids_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cameraPosition.X -= e.Location.X - previousMousePos.X;
                cameraPosition.Y -= e.Location.Y - previousMousePos.Y;
                if (cameraPosition.X < 0) cameraPosition.X = 0;
                if (cameraPosition.X > World.width-Width) cameraPosition.X = World.width-Width;
                if (cameraPosition.Y < 0) cameraPosition.Y = 0;
                if (cameraPosition.Y > World.height-Height) cameraPosition.Y = World.height-Height;
            }
            previousMousePos = e.Location;
        }
    }
}
