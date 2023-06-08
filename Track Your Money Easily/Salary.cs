using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Track_Your_Money_Easily
{
    public partial class Salary : Form
    {
        public Salary()
        {
            InitializeComponent();
            label3.Text = File.ReadAllText(@".\Salary.txt");
            label7.Text = File.ReadAllText(@".\Savings.txt");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sal = textBox1.Text;
            if (sal.Length > 0)
            {
                int salaryN;
                bool isNumeric = int.TryParse(sal, out salaryN);
                if (isNumeric && salaryN > 0)
                {
                    using (StreamWriter writer = new StreamWriter(@".\Salary.txt"))
                    {
                        writer.WriteLine(salaryN.ToString());
                        label3.Text = salaryN.ToString();
                        label5.Text = "UPDATED\nSALARY";
                    }
                }
                else
                {
                    label5.Text = "ERROR";
                }
            }
            textBox1.Clear();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sav = textBox2.Text;
            if (sav.Length > 0)
            {
                int savN;
                bool isNumeric = int.TryParse(sav, out savN);
                if (isNumeric && savN > 0)
                {
                    using (StreamWriter writer = new StreamWriter(@".\Savings.txt"))
                    {
                        writer.WriteLine(savN.ToString());
                        label7.Text = savN.ToString();
                        label5.Text = "UPDATED\nSAVINGS";
                    }
                }
                else
                {
                    label5.Text = "ERROR";
                }
            }
            textBox2.Clear();
        }
    }
}
