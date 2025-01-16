using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class SchetsControl : UserControl
{
    public Schets schets;
    private Color penkleur;

    public Color PenKleur
    {
        get { return penkleur; }
    }
    public Schets Schets
    {
        get { return schets; }
    }
    public SchetsControl()
    {
        this.BorderStyle = BorderStyle.Fixed3D;
        this.schets = new Schets();
        this.Paint += this.teken;
        this.Resize += this.veranderAfmeting;
        this.veranderAfmeting(null, null);
    }
    protected override void OnPaintBackground(PaintEventArgs e)
    {
    }
    private void teken(object o, PaintEventArgs pea)
    {
        schets.Teken(pea.Graphics);
        schets.isHetOpgeslagen.isOpgeslagen(false);
    }
    private void veranderAfmeting(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(this.ClientSize);
        this.Invalidate();
    }
    public Graphics MaakBitmapGraphics()
    {
        Graphics g = schets.BitmapGraphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        return g;
        
    }
    public void Schoon(object o, EventArgs ea)
    {
        schets.Schoon();
        // Schets.BMveranderingen.Clear();
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }
    public void Exporteren_Png(object o, EventArgs ea)//kan dit effici�nter? misschien met switch?
    {
        schets.ExporterenmetFormaat(ImageFormat.Png);
    }
    public void Exporteren_Jpeg(object o, EventArgs ea)
    {
        schets.ExporterenmetFormaat(ImageFormat.Jpeg);
    }
    public void Exporteren_Bmp(object o, EventArgs ea)
    {
        schets.ExporterenmetFormaat(ImageFormat.Bmp);
    }
    public void VeranderKleur(object obj, EventArgs ea)
    {
        string kleurNaam = ((ComboBox)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }
    public void VeranderKleurViaMenu(object obj, EventArgs ea)
    {
        string kleurNaam = ((ToolStripMenuItem)obj).Text;
        penkleur = Color.FromName(kleurNaam);
    }
}