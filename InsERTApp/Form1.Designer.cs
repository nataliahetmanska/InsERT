
namespace CSV_program
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.separatorTextBox = new System.Windows.Forms.TextBox();
            this.headerSizeTextBox = new System.Windows.Forms.TextBox();
            this.encodingComboBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(143, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(347, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Proszę o wpisanie informacji na temat pliku *.csv";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Ścieżka do pliku";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 272);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Użyty separator";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 345);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Liczba wierszy w nagłówku";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 413);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(144, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Sposób kodowania";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(367, 196);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(171, 26);
            this.pathTextBox.TabIndex = 5;
            this.pathTextBox.Text = "D:\\Asortyment.csv";
            // 
            // separatorTextBox
            // 
            this.separatorTextBox.Location = new System.Drawing.Point(367, 266);
            this.separatorTextBox.Name = "separatorTextBox";
            this.separatorTextBox.Size = new System.Drawing.Size(171, 26);
            this.separatorTextBox.TabIndex = 6;
            this.separatorTextBox.Text = ";";
            // 
            // headerSizeTextBox
            // 
            this.headerSizeTextBox.Location = new System.Drawing.Point(367, 342);
            this.headerSizeTextBox.Name = "headerSizeTextBox";
            this.headerSizeTextBox.Size = new System.Drawing.Size(171, 26);
            this.headerSizeTextBox.TabIndex = 7;
            this.headerSizeTextBox.Text = "2";
            // 
            // encodingComboBox
            // 
            this.encodingComboBox.FormattingEnabled = true;
            this.encodingComboBox.Items.AddRange(new object[] {
            "windows-1250",
            "UTF-8",
            "HTML",
            "URL",
            "Base64",
            "Hex",
            "ASCII"});
            this.encodingComboBox.Location = new System.Drawing.Point(367, 404);
            this.encodingComboBox.Name = "encodingComboBox";
            this.encodingComboBox.Size = new System.Drawing.Size(171, 28);
            this.encodingComboBox.TabIndex = 8;
            this.encodingComboBox.Text = "windows-1250";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(444, 549);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 41);
            this.button1.TabIndex = 9;
            this.button1.Text = "Dalej >";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 643);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.encodingComboBox);
            this.Controls.Add(this.headerSizeTextBox);
            this.Controls.Add(this.separatorTextBox);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Sposób kodowania";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.TextBox separatorTextBox;
        private System.Windows.Forms.TextBox headerSizeTextBox;
        private System.Windows.Forms.ComboBox encodingComboBox;
        private System.Windows.Forms.Button button1;
    }
}

