using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class SchetsWin : Form //in form zit dat kruisje
{   
    MenuStrip menuStrip;
    SchetsControl schetscontrol;
    ISchetsTool huidigeTool;
    Panel paneel;
    bool vast;

    private void veranderAfmeting(object o, EventArgs ea)
    {
        schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
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
        if (schetscontrol.schets.isHetOpgeslagen.CheckOpgeslagen == false)
        {
            string message = "Do you want to close this window?";
            string title = "close window";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
        else {
            this.Close();
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
                                , new CirkelTool()
                                , new VolCirkelTool()
                                };
        String[] deKleuren = { "Black", "Red", "Green", "Blue", "Yellow", "Magenta", "Cyan" };

        this.ClientSize = new Size(700, 500);
        huidigeTool = deTools[0];

        schetscontrol = new SchetsControl();
       
        schetscontrol.Location = new Point(64, 10);
        schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                    {   vast=true;  
                                        huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                    };
        schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                    {   if (vast)
                                        huidigeTool.MuisLos (schetscontrol, mea.Location);
                                        vast = false; 
                                    };
        schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                    {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                    };
        this.Controls.Add(schetscontrol);

        menuStrip = new MenuStrip();
        menuStrip.Visible = false;
        this.Controls.Add(menuStrip);
        this.maakFileMenu();
        this.maakExporterenMenu();
        this.maakToolMenu(deTools);
        this.maakActieMenu(deKleuren);
        this.maakToolButtons(deTools);
        this.maakActieButtons(deKleuren);
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }

    private void maakFileMenu()
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("File");
        menu.MergeAction = MergeAction.MatchOnly;
        menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
        menu.DropDownItems.Add("Open", null, schetscontrol.open_bestand);
        menuStrip.Items.Add(menu);
    }

    private void maakToolMenu(ICollection<ISchetsTool> tools)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
        foreach (ISchetsTool tool in tools)
        {   ToolStripItem item = new ToolStripMenuItem();
            item.Tag = tool;
            item.Text = tool.ToString();
            item.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            item.Click += this.klikToolMenu;
            menu.DropDownItems.Add(item);
        }
        menuStrip.Items.Add(menu);
    }

    private void maakActieMenu(String[] kleuren)
    {   
        ToolStripMenuItem menu = new ToolStripMenuItem("Actie");
        menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
        menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
        ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
        foreach (string k in kleuren)
            submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
        menu.DropDownItems.Add(submenu);
        menuStrip.Items.Add(menu);
    }

    private void maakExporterenMenu()
    {
        ToolStripMenuItem menu = new ToolStripMenuItem("Opslaan");        
        menu.DropDownItems.Add("PNG", null, schetscontrol.Exporteren_Png);        
        menu.DropDownItems.Add("JPG", null, schetscontrol.Exporteren_Jpeg);        
        menu.DropDownItems.Add("BMP", null, schetscontrol.Exporteren_Bmp);
        menu.DropDownItems.Add("Schets-formaat", null, schetscontrol.Exporteren_schets);
        menuStrip.Items.Add(menu);
    }

    private void maakToolButtons(ICollection<ISchetsTool> tools)
    {
        int t = 0;
        foreach (ISchetsTool tool in tools)
        {
            RadioButton b = new RadioButton();
            b.Appearance = Appearance.Button;
            b.Size = new Size(45, 62);
            b.Location = new Point(10, 10 + t * 62);
            b.Tag = tool;
            b.Text = tool.ToString();
            b.Image = new Bitmap($"../../../Icons/{tool.ToString()}.png");
            b.TextAlign = ContentAlignment.TopCenter;
            b.ImageAlign = ContentAlignment.BottomCenter;
            b.Click += this.klikToolButton;
            this.Controls.Add(b);
            if (t == 0) b.Select();
            t++;
        }
    }

    private void maakActieButtons(String[] kleuren)
    {   
        paneel = new Panel(); this.Controls.Add(paneel);
        paneel.Size = new Size(600, 24);
            
        Button clear = new Button(); paneel.Controls.Add(clear);
        clear.Text = "Clear";
        clear.Location = new Point( 0, 0);
        clear.Click += schetscontrol.Schoon;
            
        Button rotate = new Button(); paneel.Controls.Add(rotate);
        rotate.Text = "Rotate";
        rotate.Location = new Point( 80, 0);
        rotate.Click += schetscontrol.Roteer;
           
        Label penkleur = new Label(); paneel.Controls.Add(penkleur);
        penkleur.Text = "Penkleur:"; 
        penkleur.Location = new Point(180, 3); 
        penkleur.AutoSize = true;

        Button undo = new Button(); paneel.Controls.Add(undo);
        undo.Text = "Undo";
        undo.Location = new Point(440, 0);
        undo.Click += schetscontrol.Undo;

        Button redo = new Button(); paneel.Controls.Add(redo);
        redo.Text = "Redo";
        redo.Location = new Point(520, 0);
        redo.Click += schetscontrol.Redo;

        ComboBox cbb = new ComboBox(); paneel.Controls.Add(cbb);
        cbb.Location = new Point(240, 0); 
        cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
        cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
        foreach (string k in kleuren)
            cbb.Items.Add(k);
        cbb.SelectedIndex = 0;
    }
   
}