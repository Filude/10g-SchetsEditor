using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }
        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }
        public void Roteer()
        {
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }


        public void Read(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            char[] separators = { ' ' };
            string line, color;
            int p1, p2;

            while ((line = sr.ReadLine()) != null)
            {
                string[] r = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                if (r.Length > 2)
                {
                    switch (r[0])
                    {
                        case ("PenTool"):
                            color = r[1]; p1 = Int32.Parse(r[2]); p2 = Int32.Parse(r[3]);
                            Console.WriteLine(r[0] + " " + r[1] + " " + r[2] + " " + r[3]);
                            break;
                        case ("LijnTool"):
                            color = r[1]; p1 = Int32.Parse(r[2]); p2 = Int32.Parse(r[3]);
                            break;
                        case ("RechthoekTool"):
                            color = r[1]; p1 = Int32.Parse(r[2]); p2 = Int32.Parse(r[3]);
                            break;
                        case ("VolRechthoekTool"):
                            color = r[1]; p1 = Int32.Parse(r[2]); p2 = Int32.Parse(r[3]);
                            break;
                        case ("GumTool"):
                            p1 = Int32.Parse(r[1]); p2 = Int32.Parse(r[2]);
                            break;
                        case ("TekstTool"):
                            color = r[1]; p1 = Int32.Parse(r[2]);
                            break;
                    }
                }
            }
        }


    }
}
