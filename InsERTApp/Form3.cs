using System;
using System.Linq;
using System.Windows.Forms;

namespace CSV_program
{
    partial class Form3 : Form
    {
        public Form3()
        {
            modelDef = XMLLoader.LoadModelFromXml("D:\\InsERT.Moria.ModelDanych.ssdl");
            InitializeComponent();
        }

        public static TableDef table;
        public static ModelDef modelDef;
        private static string defaultItem = "InsERT.Moria.ModelDanych.Store.Asortymenty";

        private void button1_Click(object sender, EventArgs e)
        {
            table = (TableDef) catalog.SelectedItem;
            Form4 form4 = new Form4();
            form4.Tag = this;
            form4.Show(this);
            Hide();
        }

        private void Form3_Load_1(object sender, EventArgs e)
        {
            catalog.Items.AddRange(modelDef.PathToTable().Values.ToArray());

            if (modelDef.PathToTable().ContainsKey(defaultItem))
                catalog.SelectedItem = modelDef.PathToTable()[defaultItem];
        }
    }
}
