using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Splines
{
    public partial class Form1 : Form
    {

        public List<PointF> points = new List<PointF>();
        private Bitmap curBmp;
        public Color pointColor = Color.Gray;
        public Color linesColor = Color.LightGray;
        public Color bezierColor = Color.Blue;
        Graphics g;
        private PointF pToMove;
        private int pToMoveIndex = -1;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            curBmp = pictureBox1.Image as Bitmap;
            g = Graphics.FromImage(pictureBox1.Image);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (pToMoveIndex != -1)
            {
                return;
            }
            if (e.Button == MouseButtons.Left)
            {
                points.Add(new PointF(e.X, e.Y));
                DrawBez();
            }
            else
            {
                if (curBmp.GetPixel(e.X,e.Y).ToArgb() == pointColor.ToArgb())
                {
                    int i, j = 0;
                    double min = double.MaxValue;
                    for (i = 0; i < points.Count; ++i)
                    {
                        double m;
                        if ((m = Math.Sqrt(Math.Pow(e.X - points.ElementAt(i).X, 2) + Math.Pow(e.Y - points.ElementAt(i).Y, 2))) < min)
                        {
                            min = m;
                            j = i;
                        }
                    }
                    points.RemoveAt(j);
                    DrawBez();
                    pictureBox1.Refresh();
                }
            }
        }

        private void CurveDrawing(List<PointF> curList)
        {
            
            var tempList = new List<PointF>(curList);
            
            if (curList.Count > 4)
            {
                int count_to_add = 0;
                if (curList.Count % 2 == 1)
                    count_to_add = 1;
                else
                    count_to_add = 0;
                for (int i = 1; i <= count_to_add; ++i)
                    tempList.Add(curList[curList.Count - 1]);
                
                for (int cur = 2; cur < tempList.Count - 3; cur += 2)
                {
                    var p = new PointF((tempList[cur].X + tempList[cur + 1].X) / 2, (tempList[cur].Y + tempList[cur + 1].Y) / 2);
                    tempList.Insert(cur + 1, p);
                    ++cur;
                }
            }
            
            int curBezierPoint = 0;
            while (curBezierPoint + 3 < tempList.Count)
            {
                PointF point1 = tempList[curBezierPoint];
                PointF point2 = tempList[curBezierPoint + 1];
                PointF point3 = tempList[curBezierPoint + 2];
                PointF point4 = tempList[curBezierPoint + 3];
                curBezierPoint += 3;
                for (double t = 0.0; t <= 1.0; t += 0.001)
                {
                    double x =  Math.Pow(1 - t, 3) * point1.X + 
                                3 * t * Math.Pow(1 - t, 2) * point2.X + 
                                3 * Math.Pow(t, 2) * (1 - t) * point3.X + 
                                Math.Pow(t, 3) * point4.X;
                    double y =  Math.Pow(1 - t, 3) * point1.Y + 
                                3 * t * Math.Pow(1 - t, 2) * point2.Y + 
                                3 * Math.Pow(t, 2) * (1 - t) * point3.Y + 
                                Math.Pow(t, 3) * point4.Y;
                    curBmp.SetPixel((int)x, (int)y, bezierColor);
                }
            }
            pictureBox1.Refresh();
        }

        private void DrawBez()
        {     
            g.Clear(Color.White);
            for (int i = 0; i < points.Count - 1; ++i)
            {
                g.FillEllipse(new SolidBrush(pointColor), points[i].X - 5, points[i].Y - 5, 10, 10);
                g.DrawLine(new Pen(linesColor), points[i], points[i + 1]);
            }
            g.FillEllipse(new SolidBrush(pointColor), points[points.Count - 1].X - 5, points[points.Count - 1].Y - 5, 10, 10);
            if (points.Count >= 4)
            {
                CurveDrawing(points);
            }
            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
            points.Clear();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pToMove = new Point(0, 0);
                for (int i = 0; i < points.Count; i++)
                    if (Math.Abs(points[i].X - e.X) <= 5 && Math.Abs(points[i].Y - e.Y) <= 5)
                    {
                        pToMove = points[i];
                        pToMoveIndex = i;
                    }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pToMoveIndex != -1)
            {
                PointF newPoint = new PointF(e.X, e.Y);
                points.RemoveAt(pToMoveIndex);
                points.Insert(pToMoveIndex, newPoint);
                pToMoveIndex = -1;
                DrawBez();
            }
        }
    }
}
