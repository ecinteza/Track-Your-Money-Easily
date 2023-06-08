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
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Track_Your_Money_Easily
{
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            textBox1.Text = DateTime.Now.ToString("MM");
            textBox2.Text = DateTime.Now.ToString("yyyy");
            checkBox1.Checked = true;

            chart1.Visible = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                checkBox1.Checked = false;
                textBox1.Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked)
            {
                checkBox2.Checked = false;
                textBox1.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && textBox1.Text != string.Empty && textBox2.Text != string.Empty) 
            {
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                string salary = System.IO.File.ReadAllText(@".\Salary.txt");
                string savings = System.IO.File.ReadAllText(@".\Savings.txt");
                label3.Text = salary;
                label4.Text = savings;


                int monthlySum = 0;
                string path = @".\Monthly.txt";
                using (StreamReader read = new StreamReader(path))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                        {
                            string[] items = line.Split('|');
                            monthlySum += int.Parse(items[1]);
                        }
                    }
                }
                label7.Text = monthlySum.ToString();

                int billSum = 0;
                path = @".\Bills.txt";
                using (StreamReader read = new StreamReader(path))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                        {
                            string[] items = line.Split('|');
                            if (items[2] == textBox1.Text && items[3] == textBox2.Text) billSum += int.Parse(items[5]);
                        }
                    }
                }

                label9.Text = billSum.ToString();

                int lefttospend = int.Parse(salary)-int.Parse(savings)-monthlySum-billSum;
                label10.Text = lefttospend.ToString();

                int saveduntilnow = int.Parse(salary) - monthlySum - billSum;
                label12.Text = saveduntilnow.ToString();

                chart1.Visible = true;
                chart1.BackColor = Color.Transparent;

                chart1.Series.Clear();
                chart1.Legends.Clear();

                chart1.Legends.Add("MyLegend");
                chart1.Legends[0].LegendStyle = LegendStyle.Table;
                chart1.Legends[0].Docking = Docking.Bottom;
                chart1.Legends[0].Alignment = StringAlignment.Center;
                chart1.Legends[0].Title = "Legend";
                chart1.Legends[0].BorderColor = Color.Black;

                string seriesname = "Serie";
                chart1.Series.Add(seriesname);
                chart1.Series[seriesname].ChartType = SeriesChartType.Pie;

                chart1.Series[seriesname].Points.AddXY("Spendings", label9.Text);
                chart1.Series[seriesname].Points.AddXY("Subscriptions", label7.Text);
                chart1.Series[seriesname].Points.AddXY("Saved until now", label12.Text);
                chart1.ChartAreas[0].BackColor = Color.Transparent;
            }

            if (checkBox2.Checked && textBox1.Text != string.Empty && textBox2.Text != string.Empty)
            {
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                label8.Visible = true;
                label9.Visible = true;
                label10.Visible = true;
                label11.Visible = true;
                label12.Visible = true;
                label13.Visible = true;
                string salary = System.IO.File.ReadAllText(@".\Salary.txt");
                string savings = System.IO.File.ReadAllText(@".\Savings.txt");
                int multiply = 0;
                if (textBox1.Text == DateTime.Now.ToString("yyyy"))
                {
                    multiply = int.Parse(DateTime.Now.ToString("MM"));
                }
                else
                {
                    multiply = 12;
                }
                salary = (int.Parse(salary)*multiply).ToString();
                savings = (int.Parse(savings)*multiply).ToString();
                label3.Text = salary;
                label4.Text = savings;

                int monthlySum = 0;
                string path = @".\Monthly.txt";
                using (StreamReader read = new StreamReader(path))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                        {
                            string[] items = line.Split('|');
                            monthlySum += int.Parse(items[1]);
                        }
                    }
                }
                monthlySum = monthlySum * multiply;
                label7.Text = monthlySum.ToString();

                int billSum = 0;
                path = @".\Bills.txt";
                using (StreamReader read = new StreamReader(path))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        if (line != string.Empty)
                        {
                            string[] items = line.Split('|');
                            if (items[3] == textBox2.Text) billSum += int.Parse(items[5]);
                        }
                    }
                }

                label9.Text = billSum.ToString();

                int lefttospend = int.Parse(salary) - int.Parse(savings) - monthlySum - billSum;
                label10.Text = lefttospend.ToString();

                int saveduntilnow = int.Parse(salary) - monthlySum - billSum;
                label12.Text = saveduntilnow.ToString();

                chart1.Visible = true;
                chart1.BackColor = Color.Transparent;
                chart1.Series.Clear();
                chart1.Legends.Clear();

                chart1.Legends.Add("MyLegend");
                chart1.Legends[0].LegendStyle = LegendStyle.Table;
                chart1.Legends[0].Docking = Docking.Bottom;
                chart1.Legends[0].Alignment = StringAlignment.Center;
                chart1.Legends[0].Title = "Legend";
                chart1.Legends[0].BorderColor = Color.Black;

                string seriesname = "Serie";
                chart1.Series.Add(seriesname);
                chart1.Series[seriesname].ChartType = SeriesChartType.Pie;

                chart1.Series[seriesname].Points.AddXY("Spendings", label9.Text);
                chart1.Series[seriesname].Points.AddXY("Subscriptions", label7.Text);
                chart1.Series[seriesname].Points.AddXY("Saved until now", label12.Text);
                chart1.ChartAreas[0].BackColor = Color.Transparent;
            }

        }
    }
}
