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
        public Menu(string username) // Add a parameter to the constructor to accept the username
        {
            InitializeComponent();

            tabControl1 = new TabControl();
            Profile profile = new Profile(tabPage1, username); // Pass the username here
            profile.TopLevel = false;
            profile.Show();
            tabPage1.Controls.Add(profile);

            Salary salary = new Salary(tabPage2, username);
            salary.TopLevel = false;
            salary.Show();
            tabPage2.Controls.Add(salary);

            Monthly monthly = new Monthly(tabPage3, username);
            monthly.TopLevel = false;
            monthly.Show();
            tabPage3.Controls.Add(monthly);

            Bills bills = new Bills(tabPage4, username);
            bills.TopLevel = false;
            bills.Show();
            tabPage4.Controls.Add(bills);

            Reports reports = new Reports(tabPage5, username);
            reports.TopLevel = false;
            reports.Show();
            tabPage5.Controls.Add(reports);
        }
    }
}
