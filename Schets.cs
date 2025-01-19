using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

public class Schets
{
    public Bitmap bitmap;
    public static List<grHist> BMveranderingen = new List<grHist>();
    public static List<grHist> Undos = new List<grHist>();
    public isHetOpgeslagen isHetOpgeslagen = new isHetOpgeslagen();

    public Schets()
    {
        bitmap = new Bitmap(1, 1);
    }
    public Graphics BitmapGraphics
    {
        get { return Graphics.FromImage(bitmap); }
    }
    public void VeranderAfmeting(Size sz)
    {
        if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
        {
            Bitmap nieuw = new Bitmap( Math.Max(sz.Width,  bitmap.Size.Width)
                                     , Math.Max(sz.Height, bitmap.Size.Height)
                                     );
            Graphics gr = Graphics.FromImage(nieuw);
            gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
            gr.DrawImage(bitmap, 0, 0);
            bitmap = nieuw;
        }
    }
    public void Teken(Graphics gr)
    {
        gr.DrawImage(bitmap, 0, 0);
    }
    public void Schoon()
    {
        Graphics gr = Graphics.FromImage(bitmap);
        gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        BMveranderingen.Clear();
    }
    public void Roteer()
    {
        bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
    }
    public void ExporterenmetFormaat(ImageFormat format)
    {
        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Title = "Plaatje opslaan als...";
        bitmap.Save($@"file.{format}", format);          
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            bitmap.Save(sfd.FileName, format);
            isHetOpgeslagen.opgeslagen = true;
        }
    }
}

public struct grHist
{
    public Point p1 { get; set; }
    public Point p2 { get; set; }
    public Brush brush { get; set; }
    public string tekst { get; set; }
    public string Actie { get; set; }
}