using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.ExceptionServices;

public interface ISchetsTool
{
    void MuisVast(SchetsControl s, Point p);
    void MuisDrag(SchetsControl s, Point p);
    void MuisLos(SchetsControl s, Point p);
    void Letter(SchetsControl s, char c, bool add_hist = true);
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
    public abstract void Letter(SchetsControl s, char c, bool add_hist = true);
}

public class TekstTool : StartpuntTool
{
    public override string ToString() { return "tekst"; }

    public override void MuisDrag(SchetsControl s, Point p) { }

    public override void Letter(SchetsControl s, char c, bool add_hist = true)
    {
        if (c >= 32)
        {
            Graphics gr = s.MaakBitmapGraphics();
            Font font = new Font("Tahoma", 40);
            string tekst = c.ToString();

            SizeF sz = 
            gr.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
            gr.DrawString   (tekst, font, kwast, 
                                            this.startpunt, StringFormat.GenericTypographic);
            // gr.DrawRectangle(Pens.Black, startpunt.X, startpunt.Y, sz.Width, sz.Height);

            grHist gr_hist = new grHist {
                p1 = this.startpunt, p2 = new Point(this.startpunt.X + (int)sz.Width, this.startpunt.Y + (int)sz.Height),
                brush = kwast, tekst = tekst, Actie = "DrawString"
            };

            startpunt.X += (int)sz.Width;

            s.Invalidate();
            if (add_hist)
                Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
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
    public static Pen MaakPen(Brush b, int dikte)
    {   Pen pen = new Pen(b, dikte);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        return pen;
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
    public override void Letter(SchetsControl s, char c, bool add_hist = true)
    {
    }
    public abstract void Bezig(Graphics g, Point p1, Point p2, bool add_hist = true);
        
    public virtual void Compleet(Graphics g, Point p1, Point p2, bool add_hist = true)
    {   this.Bezig(g, p1, p2);
    }
}

public class RechthoekTool : TweepuntTool
{
    public override string ToString() { return "kader"; }

    public override void Bezig(Graphics g, Point p1, Point p2, bool add_hist = true)
    {
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = kwast, Actie = "DrawRectangle"
        };

        g.DrawRectangle(MaakPen(kwast,3), TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }
}
    
public class VolRechthoekTool : RechthoekTool
{
    public override string ToString() { return "vlak"; }

    public override void Compleet(Graphics g, Point p1, Point p2, bool add_hist = true)
    {
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = kwast, Actie = "FillRectangle"
        };

        g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }
}

public class LijnTool : TweepuntTool
{
    public override string ToString() { return "lijn"; }

    public override void Bezig(Graphics g, Point p1, Point p2, bool add_hist = true)
    {
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = kwast, Actie = "DrawLine"
        };

        g.DrawLine(MaakPen(this.kwast,3), p1, p2);

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
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

    public override void Bezig(Graphics g, Point p1, Point p2, bool add_hist = false)
    {
        g.DrawLine(MaakPen(Brushes.White, 7), p1, p2);
        int pos = vindGeschiedenis(p1);  /* Plek in geschiedenis dat weggehaald moet worden */

        if (pos != -1)
        {
            Schets.BMveranderingen.Remove(Schets.BMveranderingen[pos]);  /* Verwijder item uit geschiedenis */
            //clear bitmap
            bouwBitmap(g);  /* Herbouw de bitmap */
        }
    }

    private int vindGeschiedenis(Point p)
    {
        for (int i = Schets.BMveranderingen.Count - 1; i >= 0 ; i--)
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

                    break;
                case "DrawRectangle":

                    break;
                case "FillRectangle":
                    
                    break;
                case "DrawLine":

                    break;
                case "DrawEllipse":

                    break;
                case "FillEllipse":

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

    public override void Bezig(Graphics g, Point p1, Point p2, bool add_hist = true)
    {
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = kwast, Actie = "DrawEllipse"
        };

        g.DrawEllipse(MaakPen(kwast, 3), TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }
}

public class VolCirkelTool : CirkelTool
{
    public override string ToString() { return "Cirkel"; }

    public override void Compleet(Graphics g, Point p1, Point p2, bool add_hist = true)
    {
        grHist gr_hist = new grHist {
            p1 = p1, p2 = p2, brush = kwast, Actie = "FillEllipse"
        };

        g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));

        if (add_hist)
            Schets.BMveranderingen.Add(gr_hist);  /* Voeg nieuwe graphics informatie toe aan geschiedenis */
    }
}