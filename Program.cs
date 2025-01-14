using System;
using System.Windows.Forms;

static class Program
{
    [STAThreadAttribute]
    static void Main() //hiermee begint het programma
    {
        Application.Run(new SchetsEditor()); 
    }
}



