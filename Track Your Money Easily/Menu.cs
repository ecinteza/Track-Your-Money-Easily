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
    public partial class Menu : Form
    {
        private void createFile(string path, string write)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path)){}
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(write);
                }
            }
        }
        public Menu()
        {
            InitializeComponent();
            string path = @".\Salary.txt";
            createFile(path, "0");
            path = @".\Savings.txt";
            createFile(path, "0");
            path = @".\Username.txt";
            createFile(path, "User");
            path = @".\Age.txt";
            createFile(path, "0");
            path = @".\Email.txt";
            createFile(path, "Please update your e-mail");
            path = @".\Monthly.txt";
            createFile(path, "");
            path = @".\Bills.txt";
            createFile(path, "");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Salary salary = new Salary();
            salary.Location = Location;
            salary.FormBorderStyle= FormBorderStyle.FixedDialog;
            this.Hide();
            salary.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Profile profile = new Profile();
            profile.Location = Location;
            this.Hide();
            profile.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Monthly monthly = new Monthly();
            monthly.Location = Location;
            this.Hide();
            monthly.ShowDialog();
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bills bills = new Bills();
            bills.Location = Location;
            this.Hide();
            bills.ShowDialog();
            this.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Reports reports = new Reports();
            reports.Location = Location;
            this.Hide(); 
            reports.ShowDialog();
            this.Show();
        }
    }
}
