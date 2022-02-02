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
            menu = new ToolStripMenuItem("About");
            menu.DropDownItems.Add("Over \"Schets\"", null, this.about);
            menu.DropDownItems.Add("Info", null, this.info);
            menuStrip.Items.Add(menu);
        }
        private void about(object o, EventArgs ea)
        {   MessageBox.Show("Schets modified\n(c) Finn & Wouter,  UU 2022"
                           , "Over \"Schets\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }
        private void info(object o, EventArgs ea)
        {
            MessageBox.Show("De gum tool verwijdert bij alleen klikken het onderste element, dus wat als eerst getekend is \nOm tekst te verwijderen moet je de gum tool op de eerste letter gebruiken.\n", "Info");
        }
        public void nieuw(object sender, EventArgs e)
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

            SchetsWin sw = new SchetsWin();
            sw.MdiParent = this;
            sw.Show();

            // Select an existing file and pass it to the reader
            FileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
                sw.Read(dlg.FileName);
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
