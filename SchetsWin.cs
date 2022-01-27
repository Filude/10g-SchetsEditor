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
        List<Shape> Shapes = new List<Shape>();
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

        // Testing
        List<string> list = new List<string> ();

        // Event handler for save file dialogue
        private void opslaan(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text Files|*.txt";
            dlg.Title = "Sla je tekening op";
            dlg.ShowDialog();

            // Testing 
            list.Add("LijnTool Blue 50 30 100 100");
            list.Add("RechthoekTool Red 100 200 300 400");

            // Continue if the specified file name is not an empty string
            if (dlg.FileName != "")
            {
                // Write each line from the given list to text file
                StreamWriter sw = new StreamWriter(dlg.FileName);
                foreach (string line in list)
                {
                    sw.WriteLine(line);    
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
                                    , new NewGumTool()
                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan"
                                 };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            
            Shape TempShape = new Shape();
            bool ShapeNew = true;
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {
                                           


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
                                                 Console.WriteLine("Added shape to List");
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
        private void LoadList(object obj, EventArgs ea)
        {
            foreach (Shape s in Shapes)
            {
                s.Load(schetscontrol);
            }

        }

    }
}
