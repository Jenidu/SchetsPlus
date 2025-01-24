using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class Schets
{
    public Bitmap bitmap;
    public static List<grHist> BMveranderingen = new List<grHist>();
    public static List<grHist> Undos = new List<grHist>();   
    public static List<grHist> BMnieuw = new List<grHist>();
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
    public void Exporteren_schets()
    {
        StringBuilder sb = new StringBuilder();
        foreach (grHist element in BMveranderingen)
        {
            sb.Append(element.ToString() + ";" + System.Environment.NewLine);
            
            //sb.Append(";");
        }
        string AlleElementen = sb.ToString();

        SaveFileDialog sfd = new SaveFileDialog();
        sfd.Filter = "Tekstfiles|*.txt|Alle files|*.*";
        sfd.Title = "Tekst opslaan als...";
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            StreamWriter writer = new StreamWriter(sfd.FileName);
            writer.Write(AlleElementen);
            writer.Close();
        }      

    }
    public void open_bestand()
    {
             


        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
        openFileDialog.Title = "Bestand openen";        

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
            BMveranderingen.Clear();

            Regex pointRegex = new Regex(@"\{X=(\d+),Y=(\d+)\}");
            Regex colorRegex = new Regex(@"Color \[(.+?)\]");

            StreamReader reader = new StreamReader(openFileDialog.FileName);

            string regel;
            while ((regel = reader.ReadLine()) != null)
            {
                string[] elements = regel.Split(',');

                grHist history = new grHist();

                if (pointRegex.IsMatch(elements[0]))
                {
                    Match match = pointRegex.Match(elements[0]);
                    int x1 = int.Parse(match.Groups[1].Value);
                    int y1 = int.Parse(match.Groups[2].Value);
                    history.p1 = new Point(x1, y1);
                    BMveranderingen.Add(history);
                }

                if (pointRegex.IsMatch(elements[1]))
                {
                    Match match = pointRegex.Match(elements[1]);
                    int x2 = int.Parse(match.Groups[1].Value);
                    int y2 = int.Parse(match.Groups[2].Value);
                    history.p2 = new Point(x2, y2);
                }

                // Parse color (if it exists)
                if (colorRegex.IsMatch(elements[2]))
                {
                    Match match = colorRegex.Match(elements[2]);
                    string colorName = match.Groups[1].Value;
                    history.brush = new SolidBrush(Color.FromName(colorName));
                }


                history.tekst = elements[3];




                history.Actie = elements[4];



                BMveranderingen.Add(history);
            }

            ElemBewerken.bouwBitmap(gr);



        }
    }




}

public class grHist
{
    public Point p1 { get; set; }
    public Point p2 { get; set; }
    public Brush brush { get; set; }
    public string tekst { get; set; }
    public string Actie { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(p1.ToString() + ",");
        sb.Append(p2.ToString() + ",");
        sb.Append(((SolidBrush)brush).Color.ToString() + ",");        
        sb.Append(tekst + ",");
        sb.Append(Actie);
        return sb.ToString();

    }

    
   
   

}