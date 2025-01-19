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
        this.Invalidate();
    }
    public void Roteer(object o, EventArgs ea)
    {
        schets.VeranderAfmeting(new Size(this.ClientSize.Height, this.ClientSize.Width));
        schets.Roteer();
        this.Invalidate();
    }

    public void Undo(object o, EventArgs ea)
    {
        if (Schets.BMveranderingen.Count >= 1)  /* Er valt iets terug te zetten */
        {
            Graphics gr = Graphics.FromImage(Schets.bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, Schets.bitmap.Width, Schets.bitmap.Height);
            Schets.Undos.Add(Schets.BMveranderingen[Schets.BMveranderingen.Count-1]);
            Schets.BMveranderingen.Remove(Schets.BMveranderingen[Schets.BMveranderingen.Count-1]);  /* Verwijder item uit geschiedenis */
            ElemBewerken.bouwBitmap(gr);  /* Herbouw de bitmap */
            this.Invalidate();
        }
    }
    public void Redo(object o, EventArgs ea)
    {
        if (Schets.Undos.Count >= 1)  /* Er valt iets terug te zetten */
        {
            Graphics gr = Graphics.FromImage(Schets.bitmap);
            Schets.BMveranderingen.Add(Schets.Undos[Schets.Undos.Count-1]);
            Schets.Undos.Remove(Schets.Undos[Schets.Undos.Count-1]);  /* Verwijder item uit geschiedenis */
            ElemBewerken.bouwBitmap(gr);  /* Herbouw de bitmap */
            this.Invalidate();
        }
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