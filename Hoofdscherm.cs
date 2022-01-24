using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {   this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "Schets editor";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }
        private void maakFileMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);

            // Adds an option to the drop down menu to open an existing drawing
            menu.DropDownItems.Add("Open", null, this.open);

            menu.DropDownItems.Add("Exit", null, this.afsluiten);

            menuStrip.Items.Add(menu);
        }
        private void maakHelpMenu()
        {   ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menuStrip.Items.Add(menu);
        }
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("Schets versie 1.0\n(c) UU Informatica 2010"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {   SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }
        private void afsluiten(object sender, EventArgs e)
        {   this.Close();
        }

        // Open an existing drawing
        private void open(object sender, EventArgs e)
        {
            Schets schets = new Schets();

            // Select an existing file and pass it to the reader
            FileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
                schets.Read(dlg.FileName);
                

            /*
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show(); 
            */
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Hoofdscherm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.DoubleBuffered = true;
            this.Name = "Hoofdscherm";
            this.ResumeLayout(false);

        }
    }
}
