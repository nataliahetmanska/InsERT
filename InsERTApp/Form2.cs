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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public static string server = "";
        public static string dbName = "";
        public static string sphereUser = "";
        public static string spherePass = "";
        public static string dbUser = "";
        public static string dbPass = "";


        private void button1_Click(object sender, EventArgs e)
        {
            server = serverTextBox.Text;
            dbName = ((string)databaseComboBox.SelectedItem).ToString();
            sphereUser = userTextBox.Text;
            spherePass = passwordTextBox.Text;
            dbUser = userDBTextBox.Text;
            dbPass = passwordDBTextBox.Text;

            Form3 form3 = new Form3();
            form3.Tag = this;
            form3.Show(this);
            Hide();
        }
    }
}
