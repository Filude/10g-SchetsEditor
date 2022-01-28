using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        public List<Shape> Shapes = new List<Shape>();
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        // Event handler for save file dialogue
        private void opslaan(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text Files|*.txt";
            dlg.Title = "Sla je tekening op";
            dlg.ShowDialog();

            // Continue if the specified file name is not an empty string
            if (dlg.FileName != "")
            {
                // Write each line from the Shapes list to text file
                StreamWriter sw = new StreamWriter(dlg.FileName);
                foreach (Shape s in Shapes)
                {
                    string result = s.Tool.ToString() + s.c + " " + s.Startpoint.X + " " + s.Startpoint.Y + " " + s.Endpoint.X + " " + s.Endpoint.Y + " ";
                    string r = "";

                    foreach (Point p in s.DragPoints)
                    {
                        r = r + p.X.ToString() + " " + p.Y.ToString() + " ";
                    }

                    result = result + r;

                    result = result.Replace("Color", "");
                    result = result.Replace("[", "");
                    result = result.Replace("]", "");

                    sw.WriteLine(result);    
                }
                sw.Close();
            }
        }

        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan", "White"
                                 };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            
            Shape TempShape = new Shape();
            bool ShapeNew = true;
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {
                                           

                                           // Temporary shape to write to
                                           TempShape = new Shape();

                                           vast = true;
                                           huidigeTool.MuisVast(schetscontrol, mea.Location);
                                           TempShape.Startpoint = mea.Location;
                                           TempShape.Tool = huidigeTool;
                                           TempShape.c = schetscontrol.PenKleur;
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       { if (vast)
                                           {
                                               huidigeTool.MuisDrag(schetscontrol, mea.Location);
                                               TempShape.AddDragPoint(mea.Location);
                                               if (huidigeTool.ToString() == "Gum")
                                               {
                                                   // Check for overlap if the current tool is the eraser
                                                   CheckOverlap(mea.Location);
                                               }
                                                }

                                       };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
                                     { if (vast)
                                         {
                                             huidigeTool.MuisLos(schetscontrol, mea.Location);
                                             TempShape.Endpoint = mea.Location;
                                             ShapeNew = !ShapeNew;
                                             if (!ShapeNew)
                                             {
                                                 Shapes.Add(TempShape);
                                                 ShapeNew = true;
                                             }
                                         }
                                         vast = false;

                                     };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
                                      { huidigeTool.Letter(schetscontrol, kpea.KeyChar);
                                          Shape LastShape = Shapes[Shapes.Count - 1];
                                          if (LastShape.Tool.ToString() == "Text")
                                          {
                                              LastShape.AddLetter(kpea.KeyChar);
                                          }         
                                      };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;

            // Added a drop down menu item for saving current drawing
            menu.DropDownItems.Add("Opslaan", null, this.opslaan);

            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            { ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 45);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                //b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetscontrol.Schoon;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += schetscontrol.Roteer;
            paneel.Controls.Add(b);


            // Load list button
            b = new Button();
            b.Text = "Load List";
            b.Location = new Point(160, 0);
            b.Click += LoadList;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = "Penkleur:";
            l.Location = new Point(180, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SchetsWin
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Name = "SchetsWin";
            this.ResumeLayout(false);

        }
        // Load all shapes given by a list of shapes
        public void LoadList(object obj, EventArgs ea)
        {
            foreach (Shape s in Shapes)
            {
                s.Load(schetscontrol);
            }

        }

        // Checks for overlapping shapes. Not finished, unfortunately
        public void CheckOverlap(Point p)
        {
            Console.WriteLine("Tried to check for overlap at"+ p); // debugging
            foreach (Shape s in Shapes)
            {
                string str = s.Tool.ToString();
                Console.WriteLine(str);
                switch(str)
                    {
                    case "Lijn":
                        {
                            Console.WriteLine("Checked a Lijn");
                            break;
                        }
                    case "Vlak":
                        {
                            Console.WriteLine("Checked a Vlak");
                            if (s.Startpoint.X < p.X && p.X < s.Endpoint.X && s.Startpoint.Y < p.Y && p.Y < s.Endpoint.Y)
                            {
                                Console.WriteLine("Deleted Vlak");
                                Shapes.Remove(s);
                                
                                foreach (Shape s2 in Shapes)
                                {
                                    s2.Load(schetscontrol);
                                }
                            }
                            break;
                        }
                    }
                break;
            }
        }

        // This function reads a txt file for drawing instructions
        public void Read(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            char[] separators = { ' ' };
            string line;

            // Ensure the line is not empty
            while ((line = sr.ReadLine()) != null)
            {
                string[] r = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                Shape shape = new Shape();

                if (r.Length > 4)
                {
                    switch (r[0]) // Checks all possible Tool instructions
                    {
                        case ("Pen"):
                            int a1 = Int32.Parse(r[2]);
                            int b1 = Int32.Parse(r[3]);
                            int a2 = Int32.Parse(r[4]);
                            int b2 = Int32.Parse(r[5]);

                            for (int z = 6; z < r.Length; z = z + 2)
                            {
                                shape.DragPoints.Add(new Point(Int32.Parse(r[z]), Int32.Parse(r[z+1])));
                            }
                            

                            // Add instructions to a Shape object
                            shape.Tool = new PenTool();
                            shape.c = Color.FromName(r[1]);
                            shape.Startpoint = new Point(a1, b1);
                            shape.Endpoint = new Point(a2, b2);

                            Shapes.Add(shape);
                            break;

                        case ("Lijn"):
                            int c1 = Int32.Parse(r[2]);
                            int d1 = Int32.Parse(r[3]);
                            int c2 = Int32.Parse(r[4]);
                            int d2 = Int32.Parse(r[5]);

                            shape.Tool = new LijnTool();
                            shape.c = Color.FromName(r[1]);
                            shape.Startpoint = new Point(c1, d1);
                            shape.Endpoint = new Point(c2, d2);

                            Shapes.Add(shape);
                            break;

                        case ("Kader"):
                            int e1 = Int32.Parse(r[2]);
                            int f1 = Int32.Parse(r[3]);
                            int e2 = Int32.Parse(r[4]);
                            int f2 = Int32.Parse(r[5]);

                            shape.Tool = new RechthoekTool();
                            shape.c = Color.FromName(r[1]);
                            shape.Startpoint = new Point(e1, f1);
                            shape.Endpoint = new Point(e2, f2);

                            Shapes.Add(shape);
                            break;

                        case ("Vlak"):
                            int g1 = Int32.Parse(r[2]);
                            int h1 = Int32.Parse(r[3]);
                            int g2 = Int32.Parse(r[4]);
                            int h2 = Int32.Parse(r[5]);

                            shape.Tool = new VolRechthoekTool();
                            shape.c = Color.FromName(r[1]);
                            shape.Startpoint = new Point(g1, h1);
                            shape.Endpoint = new Point(g2, h2);

                            Shapes.Add(shape);
                            break;

                        case ("Tekst"):
                            int i = Int32.Parse(r[2]);
                            int j = Int32.Parse(r[3]);

                            shape.Tool = new TekstTool();
                            shape.c = Color.FromName(r[1]);
                            shape.Startpoint = new Point(i, j);
                            foreach (char c in r[4])
                            {
                                shape.Chars.Add(c);
                            }

                            Shapes.Add(shape);
                            break;
                    }       
                }
            }
        }
    }
}
