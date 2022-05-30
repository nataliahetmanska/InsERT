using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_program
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ModelDef loaded = XMLLoader.LoadModelFromXml("D:\\studia\\semestr 6\\ZPI\\modeldanych\\InsERT.Moria.ModelDanych.ssdl"); // add path 
            List<ColumnDef> queryResult = loaded.findColumns("zamiennik", loaded.getTableDefByName("Asortymenty"));
            foreach (ColumnDef column in queryResult)
                Console.WriteLine(column);
            Console.WriteLine();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
