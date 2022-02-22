using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Crystal_game
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            frame = new Bitmap(Width, Height);
            g = CreateGraphics();
            gF = Graphics.FromImage(frame);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    new Crystal(gm, gF, i, j, rand.Next(5));
        }
        Bitmap frame;
        Graphics g, gF;
        GameManager gm = new GameManager();
        Random rand = new Random();

        void Render()
        {
            gF.Clear(Color.FromArgb(170, 170, 170));
            foreach (Crystal q in gm.gObjs) q.Render();

            g.DrawImage(frame, 0, 0);
        }
        void Step()
        {
            foreach (var q in gm.gObjs)
                q.Step();
            for (int i = 0; i<9;i++ )
                if(gm.crs[i,0] == null)
                    new Crystal(gm, gF, i, 0, rand.Next(5));

        }
        void Delete()
        {
            foreach (Crystal q in gm.delList)
                DelEl(q);
            gm.delList.Clear();

        }
        void DelEl(Crystal c)
        {
            gm.gObjs.Remove(c);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (gm.crs[i, j] == c) gm.crs[i, j] = null;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (gm.crs[e.X / 100, e.Y / 100] != null)
                gm.crs[e.X / 100, e.Y / 100].Tap();
            else
                gm.selectCrystal = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Step();
            Delete();
            Render();
        }
    }
    class Crystal
    {
        public Crystal(GameManager gm, Graphics gf, int x, int y, int type)
        {
            gm.gObjs.Add(this);
            g = gf;
            this.gm = gm;
            this.pos = new Point(x * 100, 0);
            target = new Point(x * 100, y * 100);
            this.type = type;
            gm.crs[x, y] = this;
        }
        public Crystal app;
        readonly int speed = 3;
        public Point pos, target;
        Graphics g;
        GameManager gm;
        int type;
        public bool isMoving() => pos != target;
        public static double Dist(Point c1, Point c2) => Math.Sqrt(Math.Pow(c1.X - c2.X, 2) + Math.Pow(c1.Y - c2.Y, 2));

        public void Tap()
        {
            if (!isMoving())
            {
                if (gm.selectCrystal == null)
                    gm.selectCrystal = this;
                else if (Dist(pos, gm.selectCrystal.pos) > 100)
                    gm.selectCrystal = this;
                else
                {
                    target = gm.selectCrystal.pos;
                    gm.selectCrystal.target = pos;
                    app = gm.selectCrystal;
                    gm.selectCrystal.app = this;
                    gm.selectCrystal = null;
                }
            }
            else gm.selectCrystal = null;
        }

        public int xDest()
        {
            if (target.X - pos.X > speed) return 1;
            else if (target.X - pos.X < -speed) return -1;
            else return 0;
        }
        public int yDest()
        {
            if (target.Y - pos.Y > speed) return 1;
            else if (target.Y - pos.Y < -speed) return -1;
            else return 0;
        }
        public void Step()
        {
            if (Dist(target, pos) >= speed * 2)
            {
                pos.X += xDest() * speed;
                pos.Y += yDest() * speed;
            }
            else
                pos = target;
            if (!isMoving())
            {
                gm.crs[pos.X / 100, pos.Y / 100] = this;
                try
                {
                    if (gm.crs[pos.X / 100, pos.Y / 100 - 1] != null && gm.crs[pos.X / 100, pos.Y / 100 + 1] != null)
                        if (gm.crs[pos.X / 100, pos.Y / 100 - 1].type == type && gm.crs[pos.X / 100, pos.Y / 100 + 1].type == type)
                        {
                            gm.delList.Add(gm.crs[pos.X / 100, pos.Y / 100 - 1]);
                            gm.delList.Add(this);
                            gm.delList.Add(gm.crs[pos.X / 100, pos.Y / 100 + 1]);
                        }

                    if (gm.crs[pos.X / 100, pos.Y / 100 + 1] == null)
                    {
                        gm.crs[pos.X / 100, pos.Y / 100 + 1]= this;
                        gm.crs[pos.X / 100, pos.Y / 100] = null;
                        target = new Point(pos.X, pos.Y + 100);
                    }

                }
                catch { }





            }

            if (!isMoving() && app != null)
            {
                (target, app.target) = (app.target, target);
                app.app = null;
                app = null;
            }
        }

        public void Render()
        {
            g.DrawImage(gm.sprites[type], pos.X, pos.Y, 100, 100);
            if (gm.selectCrystal == this)
                g.DrawRectangle(new Pen(Color.Red), pos.X, pos.Y, 100, 100);
        }

    }
    class GameManager
    {
        public Crystal[,] crs = new Crystal[9, 9];
        public List<Crystal> gObjs = new List<Crystal>();
        public List<Crystal> delList = new List<Crystal>();
        public Bitmap[] sprites = {Resources.sprite0, Resources.sprite1, Resources.sprite2, Resources.sprite3, Resources.sprite4 };
        public Crystal selectCrystal;
    }
    

}
