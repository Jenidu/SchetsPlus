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

    public void TekenString(Graphics g, Point p, Brush Kwast, string tekst, bool add_hist)
    {
        Font font = new Font("Tahoma", 40);

        SizeF sz = 
        g.MeasureString(tekst, font, p, StringFormat.GenericTypographic);
        g.DrawString   (tekst, font, Kwast, p, StringFormat.GenericTypographic);
        // g.DrawRectangle(Pens.GreenYellow, p.X, p.Y, sz.Width, sz.Height);

        grHist gr_hist = new grHist {
            p1 = new Point(p.X, p.Y), p2 = new Point(p.X + (int)sz.Width, p.Y + (int)sz.Height),
            brush = Kwast, tekst = tekst, Actie = "DrawString"
        };

        startpunt.X += (int)sz.Width;

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }

    public void TekenRechtHoek(Graphics g, Point p1, Point p2, Brush Kwast, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = Kwast, Actie = "DrawRectangle"
        };

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */

        g.DrawRectangle(MaakPen(Kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public void VulRechthoek(Graphics g, Point p1, Point p2, Brush Kwast, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = Kwast, Actie = "FillRectangle"
        };Console.WriteLine(p1);Console.WriteLine(p2);

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */

        g.FillRectangle(Kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public void TekenLijn(Graphics g, Point p1, Point p2, Brush Kwast, bool add_hist)
    {
        // Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = Kwast, Actie = "DrawLine"
        };

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */

        g.DrawLine(MaakPen(Kwast,3), p1, p2);
    }

    public void TekenEllips(Graphics g, Point p1, Point p2, Brush Kwast, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = Kwast, Actie = "DrawEllipse"
        };

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */

        g.DrawEllipse(MaakPen(Kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));
    }

    public void VulEllips(Graphics g, Point p1, Point p2, Brush Kwast, bool add_hist)
    {
        Point[] p = OrderPoints(p1, p2);
        grHist gr_hist = new grHist {
            p1 = p[0], p2 = p[1], brush = Kwast, Actie = "FillEllipse"
        };

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */

        g.FillEllipse(Kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
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

            TekenString(gr, startpunt, kwast, tekst, true);
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
        TekenRechtHoek(g, p1, p2, kwast, false);
    }

    public override void Compleet(Graphics g, Point p1, Point p2){
        TekenRechtHoek(g, p1, p2, kwast, true);
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2){
        VulRechthoek(g, p1, p2, kwast, true);
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2){
        TekenLijn(g, p1, p2, kwast, false);
    }

    public override void Compleet(Graphics g, Point p1, Point p2){
        TekenLijn(g, p1, p2, kwast, true);
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

public class GumTool : TweepuntTool
{
    public override string ToString() { return "gum"; }

    public override void Bezig(Graphics g, Point p1, Point p2)
    {
        Console.WriteLine(startpunt);
        int pos = vindGeschiedenis(startpunt);  /* Vind de plek in geschiedenis dat weggehaald moet worden */

        if (pos != -1)
        {   Console.WriteLine($"Gevonden {pos}");
            g.FillRectangle(Brushes.White, Schets.BMveranderingen[pos].p1.X - 2, Schets.BMveranderingen[pos].p1.Y - 2,
                                           Schets.BMveranderingen[pos].p2.X, Schets.BMveranderingen[pos].p2.Y);  /* Clear bitmap */
            Schets.BMveranderingen.Remove(Schets.BMveranderingen[pos]);  /* Verwijder item uit geschiedenis */
            bouwBitmap(g);  /* Herbouw de bitmap */
        }
    }

    private int vindGeschiedenis(Point p)
    {   Console.WriteLine($"Count: {Schets.BMveranderingen.Count}");
        for (int i = Schets.BMveranderingen.Count - 1; i >= 0 ; i--)  /* Vind laatste Bitmap element dat overlapt met Point p */
        {
// Console.WriteLine($"{Schets.BMveranderingen[i].p1.X} <= {p.X} {Schets.BMveranderingen[i].p1.Y} <= {p.Y} {Schets.BMveranderingen[i].p2.X} >= {p.X} {Schets.BMveranderingen[i].p2.Y} >= {p.Y}");
            switch (Schets.BMveranderingen[i].Actie)
            {
                case "FillRectangle" or "DrawString":
                    if (BoundingBox.FillRectangle(p, Schets.BMveranderingen[i])) return i;
                    else break;
                case "DrawRectangle":
                    if (BoundingBox.DrawRectangle(p, Schets.BMveranderingen[i])) return i;
                    else break;
                case "DrawLine":
                    if (BoundingBox.DrawLine(p, Schets.BMveranderingen[i])) return i;
                    else break;
                case "DrawEllipse":
                    if (BoundingBox.DrawEllipse(p, Schets.BMveranderingen[i])) return i;
                    else break;
                case "FillEllipse":
                    if (BoundingBox.FillEllipse(p, Schets.BMveranderingen[i])) return i;
                    else break;
                default:
                    return i;
            }
        }
        return -1;  /* De gegeven positie zit niet in de geschiedenis */
    }

    private void bouwBitmap(Graphics g)
    {   Console.WriteLine($"New count: {Schets.BMveranderingen.Count}");
        for (int i = 0; i < Schets.BMveranderingen.Count; i++)
        {
            switch (Schets.BMveranderingen[i].Actie)
            {
                case "DrawString":
                    TekenString(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].brush, Schets.BMveranderingen[i].tekst, false);
                    break;
                case "DrawRectangle":
                    TekenRechtHoek(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, Schets.BMveranderingen[i].brush, false);
                    break;
                case "FillRectangle":
                    VulRechthoek(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, Schets.BMveranderingen[i].brush, false);
                    break;
                case "DrawLine":
                    TekenLijn(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, Schets.BMveranderingen[i].brush, false);
                    break;
                case "DrawEllipse":
                    TekenEllips(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, Schets.BMveranderingen[i].brush, false);
                    break;
                case "FillEllipse":
                    VulEllips(g, Schets.BMveranderingen[i].p1, Schets.BMveranderingen[i].p2, Schets.BMveranderingen[i].brush, false);
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
        TekenEllips(g, p1, p2, kwast, false);
    }

    public override void Compleet(Graphics g, Point p1, Point p2){
        TekenEllips(g, p1, p2, kwast, true);
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "Cirkel"; }

    public override void Compleet(Graphics g, Point p1, Point p2){
        VulEllips(g, p1, p2, kwast, true);
    }
}

public static class BoundingBox
{
    private static readonly int mar = 10;  /* Bounding box margin */

    public static bool FillRectangle(Point p, grHist gr_hist)
    {
        return gr_hist.p1.X <= p.X && gr_hist.p1.Y <= p.Y &&
               gr_hist.p2.X >= p.X && gr_hist.p2.Y >= p.Y;
    }

    public static bool DrawRectangle(Point p, grHist gr_hist)
    {
        int minX = Math.Min(gr_hist.p1.X, gr_hist.p2.X);
        int maxX = Math.Max(gr_hist.p1.X, gr_hist.p2.X);
        int minY = Math.Min(gr_hist.p1.Y, gr_hist.p2.Y);
        int maxY = Math.Max(gr_hist.p1.Y, gr_hist.p2.Y);

        return (Math.Abs(p.X - minX) <= mar || Math.Abs(p.X - maxX) <= mar) && (p.Y >= minY && p.Y <= maxY) ||
               (Math.Abs(p.Y - minY) <= mar || Math.Abs(p.Y - maxY) <= mar) && (p.X >= minX && p.X <= maxX);
    }

    public static bool DrawLine(Point p, grHist gr_hist)
    {
        // (y - y1) / (y2 - y1) == (x - x1) / (x2 - x1)
        double dx = gr_hist.p2.X - gr_hist.p1.X;
        double dy = gr_hist.p2.Y - gr_hist.p1.Y;

        if (Math.Abs(dy * (p.X - gr_hist.p1.X) - dx * (p.Y - gr_hist.p1.Y)) / Math.Sqrt(dx * dx + dy * dy) > mar*2)  /* Check if the point is on the line */
            return false;

        return p.X >= Math.Min(gr_hist.p1.X, gr_hist.p2.X) && p.X <= Math.Max(gr_hist.p1.X, gr_hist.p2.X) &&  /* De lijn is niet de lengte van het scherm */
               p.Y >= Math.Min(gr_hist.p1.Y, gr_hist.p2.Y) && p.Y <= Math.Max(gr_hist.p1.Y, gr_hist.p2.Y);
    }

    public static bool DrawEllipse(Point p, grHist gr_hist)
    {
        double mid_x = (gr_hist.p1.X + gr_hist.p2.X) / (double)2;
        double mid_y = (gr_hist.p1.Y + gr_hist.p2.Y) / (double)2;
        double radX = Math.Abs(gr_hist.p2.X - gr_hist.p1.X) / (double)2;
        double radY = Math.Abs(gr_hist.p2.Y - gr_hist.p1.Y) / (double)2;

        double normal_x = (p.X - mid_x) / radX;  /* Afstand van midden van 0 tot 1 */
        double normal_y = (p.Y - mid_y) / radY;

        double ellip = normal_x * normal_x + normal_y * normal_y;

        return Math.Abs(ellip - 1) <= mar / Math.Sqrt(radX * radY);
    }

    public static bool FillEllipse(Point p, grHist gr_hist)
    {
        double mid_x = (gr_hist.p1.X + gr_hist.p2.X) / (double)2;
        double mid_y = (gr_hist.p1.Y + gr_hist.p2.Y) / (double)2;
        double radX = Math.Abs(gr_hist.p2.X - gr_hist.p1.X) / (double)2;
        double radY = Math.Abs(gr_hist.p2.Y - gr_hist.p1.Y) / (double)2;

        double normal_x = (p.X - mid_x) / radX;  /* Afstand van midden van 0 tot 1 */
        double normal_y = (p.Y - mid_y) / radY;

        return normal_x * normal_x + normal_y * normal_y <= 1;
    }
}
