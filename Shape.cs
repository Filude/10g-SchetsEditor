using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SchetsEditor
{
    public class Shape
    {
        List<Point> DragPoints = new List<Point>();

        List<char> Chars = new List<char>();
        public Point Startpoint { get; set; }
        public Point Endpoint { get; set; }
        public Color c { get; set; }

        public ISchetsTool Tool;



        public void AddDragPoint(Point DragPoint)
        {
            DragPoints.Add(DragPoint);
        }
        public void AddLetter(char c)
        {
            Chars.Add(c);
        }
        public void Load(SchetsControl s)
        {
            s.SetPenKleur = c;
            Tool.MuisVast(s, Startpoint);
            foreach (Point DragPoint in DragPoints)
            {
                Tool.MuisDrag(s, DragPoint);
            }
            Tool.MuisLos(s, Endpoint);
            foreach (char c in Chars)
            {
                Tool.Letter(s, c);
            }

        }
    }

}
