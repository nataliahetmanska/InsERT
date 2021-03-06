using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace CSV_program
{
    public partial class Form4 : Form
    {
        
        public Form4()
        {
            InitializeComponent();
        }

        private List<Tuple<int, ComboBox>> csvColumnIndexToComboBox = new List<Tuple<int, ComboBox>>();
        private CSV_tools csv;

        private void Form4_Load(object sender, EventArgs e)
        {

            string amtString = Form1.amtString;
            string encoding = Form1.encoding;
            string filePath = Form1.filePath;
            string strSeparator = Form1.strSeparator;
            string server = Form2.server;
            string dbName = Form2.dbName;
            string sphereUser = Form2.sphereUser;
            string spherePass = Form2.spherePass;
            string dbUser = Form2.dbUser;
            string dbPass = Form2.dbPass;

            int amtHeaders = Convert.ToInt32(amtString);

            String[] listSeparator = new String[] { strSeparator};


            csv = new CSV_tools(filePath, amtHeaders, listSeparator, encoding, server, dbName, sphereUser, spherePass, dbUser, dbPass);

            List<string> fullHeaders = csv.getHeaders();

            ComboBox[] ComboBoxes = new ComboBox[amtHeaders];

            wrapper.RowCount = fullHeaders.Count;

            for (int i = 0; i < fullHeaders.Count; i++)
            {
                string columnName = fullHeaders[i];
                List<ColumnDef> modelColumns = Form3.modelDef.findColumns(columnName, Form3.table);

                ComboBox comboBox = new ComboBox();
                comboBox.Items.AddRange(modelColumns.ToArray());
                comboBox.Size = new Size(220, 30);

                if (modelColumns.Count > 0)
                    comboBox.SelectedIndex = 0;
                
                Label label = new Label();
                label.Size = new Size(220, 30);
                label.BorderStyle = BorderStyle.None;

                label.Text = columnName;
                wrapper.Controls.Add(label, 0, i);
                wrapper.Controls.Add(comboBox, 1, i);

                csvColumnIndexToComboBox.Add(Tuple.Create(i, comboBox));
            }
        }

        private List<CSVSferaMapItem> getMapping()
        {
            return csvColumnIndexToComboBox.Select(tuple =>
            {
                int index = tuple.Item1;
                ColumnDef column = ((ColumnDef) tuple.Item2.SelectedItem);
                return new CSVSferaMapItem(Form3.table.Name() + '.' + column.ColumnName(), index, CSVSferaMapItem.MAP_TYPE_ENTITY);
            }).ToList(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            csv.ExportExecute(true, false, getMapping());
        }

        private void button2_Click(object sender, EventArgs e)
        {

            List<CSVSferaMapItem> mapList = new List<CSVSferaMapItem>();
            mapList.Add(new CSVSferaMapItem("Asortyment.Symbol", 1, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.Nazwa", 2, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.Opis", 3, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.CenaEwidencyjna", 22, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.SklepInternetowy", 48, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.WyliczenieZWartosci", 48, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.Numer", 48, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.TerminWaznosciKontrola", 41, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.TerminWaznosciLiczbaDni", 42, CSVSferaMapItem.MAP_TYPE_ENTITY));
            mapList.Add(new CSVSferaMapItem("Asortyment.Producent.1.NazwaSkrocona", 25, CSVSferaMapItem.MAP_TYPE_REL));
            mapList.Add(new CSVSferaMapItem("Asortyment.Odbiorca.2.Uwagi", 27, CSVSferaMapItem.MAP_TYPE_REL));
            mapList.Add(new CSVSferaMapItem("Asortyment.Dostawca.1.Uwagi", 26, CSVSferaMapItem.MAP_TYPE_REL));
            mapList.Add(new CSVSferaMapItem("Asortyment.Odbiorca.2.NazwaSkrocona", 28, CSVSferaMapItem.MAP_TYPE_REL));

            csv.ExportExecute(false, true, mapList);
            //csv.ExportExecute(false, true, getMapping());
        }
    }
}