using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.Pkcs;
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
            sb.Append(element.ToString() + System.Environment.NewLine);

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

            StreamReader reader = new StreamReader(openFileDialog.FileName);
            string regel;

            while ((regel = reader.ReadLine()) != null)
            {
                string[] elements = regel.Split(',');

                int position = elements[0].IndexOf("=");
                int x1 = int.Parse(elements[0].Substring(position + 1));

                int lengte = elements[1].Length;
                position = elements[1].IndexOf("=");
                int y1 = int.Parse(elements[1].Substring(position + 1, lengte - (position + 2)));

                position = elements[2].IndexOf("=");
                int x2 = int.Parse(elements[2].Substring(position + 1));

                lengte = elements[3].Length;
                position = elements[3].IndexOf("=");
                int y2 = int.Parse(elements[3].Substring(position + 1, lengte - (position + 2)));

                lengte = elements[4].Length;
                position = elements[4].IndexOf("[");
                string color = elements[4].Substring(position + 1, lengte - (position + 2));
                Color kleur = Color.FromName(color);
                Console.WriteLine(color);

                grHist new_element = new grHist {
                    p1 = new Point(x1, y1), p2 = new Point(x2, y2), brush = new SolidBrush(kleur), tekst = elements[5], Actie = elements[6]
                };


                BMveranderingen.Add(new_element);
            }
            StringBuilder sb = new StringBuilder();
            foreach (grHist element in BMveranderingen)
            {
                sb.Append(element.ToString() + System.Environment.NewLine);

                //sb.Append(";");
            }
            string AlleElementen = sb.ToString();
            Console.WriteLine(AlleElementen);
            Schoon();
            Graphics gr = Graphics.FromImage(bitmap);
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
        sb.Append(p1.ToString() + ";");
        sb.Append(p2.ToString() + ";");
        sb.Append(((SolidBrush)brush).Color.ToString() + ";");        
        sb.Append(tekst + ";");
        sb.Append(Actie);
        return sb.ToString();

    }

    
      

}