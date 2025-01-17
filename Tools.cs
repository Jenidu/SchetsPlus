using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.ExceptionServices;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c);
}

public class isHetOpgeslagen
{
    public bool opgeslagen;
    public void isOpgeslagen(bool x)
    {
        opgeslagen = x;

    }
    public bool CheckOpgeslagen
    {
        get
        {
            return opgeslagen;
        }
    }
}

public abstract class StartpuntTool : ISchetsTool
{
    protected Point startpunt;
    protected Brush kwast;

    public virtual void MuisVast(SchetsControl s, Point p)
    {   startpunt = p;
    }
    public virtual void MuisLos(SchetsControl s, Point p)
    {   kwast = new SolidBrush(s.PenKleur);
    }
    public abstract void MuisDrag(SchetsControl s, Point p);
    public abstract void Letter(SchetsControl s, char c);

    public static Pen MaakPen(Brush b, int dikte)
    {   Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
    }

    private Point[] OrderPoints(Point p1, Point p2)  /* Zorg ervoor dat p1 >= p2 */
    {
        Point[] p = new Point[2];

        p[0].X = p1.X < p2.X ? p1.X : p2.X;
        p[0].Y = p1.Y < p2.Y ? p1.Y : p2.Y;
        p[1].X = p1.X > p2.X ? p1.X : p2.X;
        p[1].Y = p1.Y > p2.Y ? p1.Y : p2.Y;

        return p;
    }

    public void TekenString(Graphics g, string tekst, bool add_hist)
    {
        Font font = new Font("Tahoma", 40);

        SizeF sz = 
        g.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
        g.DrawString   (tekst, font, kwast, this.startpunt, StringFormat.GenericTypographic);
        // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);

        grHist gr_hist = new grHist {
            p1 = this.startpunt, p2 = new Point(this.startpunt.X + (int)sz.Width, this.startpunt.Y + (int)sz.Height),
            brush = kwast, tekst = tekst, Actie = "DrawString"
        };

        startpunt.X += (int)sz.Width;

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void TekenRechtHoek(Graphics g, Point p1, Point p2, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = kwast, Actie = "DrawRectangle"
        };

        g.DrawRectangle(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void VulRechthoek(Graphics g, Point p1, Point p2, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = kwast, Actie = "FillRectangle"
        };Console.WriteLine(p1);Console.WriteLine(p2);

        g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void TekenLijn(Graphics g, Point p1, Point p2, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = kwast, Actie = "DrawLine"
        };

        g.DrawLine(MaakPen(this.kwast,3), p1, p2);

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void TekenEllips(Graphics g, Point p1, Point p2, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = kwast, Actie = "DrawEllipse"
        };

        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void VulEllips(Graphics g, Point p1, Point p2, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = kwast, Actie = "FillEllipse"
        };

        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            string tekst = c.ToString();

            TekenString(gr, tekst, true);
            s.Invalidate();
        }
    }
}

public abstract class TweepuntTool : StartpuntTool
{
    public static Rectangle Punten2Rechthoek(Point p1, Point p2)
    {
        return new Rectangle( new Point(Math.Min(p1.X,p2.X), Math.Min(p1.Y,p2.Y))
                            , new Size (Math.Abs(p1.X-p2.X), Math.Abs(p1.Y-p2.Y))
                            );
    }
    public override void MuisVast(SchetsControl s, Point p)
    {   base.MuisVast(s, p);
        kwast = Brushes.Gray;
    }
    public override void MuisDrag(SchetsControl s, Point p)
    {   s.Refresh();
        this.Bezig(s.CreateGraphics(), this.startpunt, p);
    }
    public override void MuisLos(SchetsControl s, Point p)
    {   base.MuisLos(s, p);
        this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
        s.Invalidate();
    }
    public override void Letter(SchetsControl s, char c)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2)
    {   this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2){
        TekenRechtHoek(g, p1, p2, false);
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2){
        VulRechthoek(g, p1, p2, true);
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2){
        TekenLijn(g, p1, p2, true);
    }
}

public class PenTool : LijnTool
{
    public override string ToString() { return "pen"; }

    public override void MuisDrag(SchetsControl s, Point p)
    {   this.MuisLos(s, p);
        this.MuisVast(s, p);
    }
}

public class GumTool : PenTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        // g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        Console.WriteLine(startpunt);
        int pos = vindGeschiedenis(startpunt);  /* Vind de plek in geschiedenis dat weggehaald moet worden */

        if (pos != -1)
        {   Console.WriteLine($"Gevonden {pos}");
            g.FillRectangle(Brushes.White, Schets.BMveranderingen[pos].p1.X, Schets.BMveranderingen[pos].p1.Y,
                                           Schets.BMveranderingen[pos].p2.X+1, Schets.BMveranderingen[pos].p2.Y+1);  /* Clear bitmap */
            Schets.BMveranderingen.Remove(Schets.BMveranderingen[pos]);  /* Verwijder item uit geschiedenis */
            bouwBitmap(g);  /* Herbouw de bitmap */
        }
    }

    private int vindGeschiedenis(Point p)
    {   Console.WriteLine($"Count: {Schets.BMveranderingen.Count}");
        for (int i = Schets.BMveranderingen.Count - 1; i >= 0 ; i--)  /* Vind laatste Bitmap element dat overlapt met Point p */
        {
            if (Schets.BMveranderingen[i].p1.X <= p.X && Schets.BMveranderingen[i].p1.Y <= p.Y &&
            Schets.BMveranderingen[i].p2.X <= p.X && Schets.BMveranderingen[i].p2.Y <= p.Y)
                return i;
        }
        return -1;  /* De gegeven positie zit niet in de geschiedenis */
    }

    private void bouwBitmap(Graphics g)
    {
        for (int i = 0; i < Schets.BMveranderingen.Count; i++)
        {
            switch (Schets.BMveranderingen[i].Actie)
            {
                case "DrawString":
                    TekenString(g, Schets.BMveranderingen[i].tekst, false);
                    break;
                case "DrawRectangle":
                    TekenRechtHoek(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, false);
                    break;
                case "FillRectangle":
                    VulRechthoek(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, false);
                    break;
                case "DrawLine":
                    TekenLijn(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, false);
                    break;
                case "DrawEllipse":
                    TekenEllips(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, false);
                    break;
                case "FillEllipse":
                    VulEllips(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, false);
                    break;
                default:
                    Console.WriteLine($"Incorrect history of '{Schets.BMveranderingen[i].Actie}'");
                    break;
            }
        }
    }
}

public class CirkelTool : TweepuntTool
{
    public override string ToString() { return "Open"; }

    public override void Bezig(Graphics g, Point p1, Point p2){
        TekenEllips(g, p1, p2, true);
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "Cirkel"; }

    public override void Compleet(Graphics g, Point p1, Point p2){
        VulEllips(g, p1, p2, true);
    }
}
