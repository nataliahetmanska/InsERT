using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return new CSVSferaMapItem(Form3.table.Name() + '.' + column.ColumnName(), index);
            }).ToList(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            csv.ExportExecute(true, false, getMapping());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            csv.ExportExecute(false, true, getMapping());
        }
    }
}