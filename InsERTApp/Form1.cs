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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string filePath = "";
        public static string amtString = "";
        public static string strSeparator = "";
        public static string encoding = "";

        private void button1_Click(object sender, EventArgs e)
        {

            filePath = pathTextBox.Text;

            int amtHeaders;
            amtHeaders = int.Parse(headerSizeTextBox.Text);

            amtString = (amtHeaders).ToString();
            strSeparator = separatorTextBox.Text;
            encoding = ((string)encodingComboBox.SelectedItem).ToString();

            Form2 form2 = new Form2();
            form2.Tag = this;
            form2.Show(this);
            Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
