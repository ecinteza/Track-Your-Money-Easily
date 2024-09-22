using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json;

namespace Track_Your_Money_Easily
{
    public partial class Reports : Form
    {
        private string username;
        private const string baseUrl = "http://localhost:5000";

        public Reports(Control parent, string username)
        {
            InitializeComponent();
            this.username = username;

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
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;

            textBox1.Text = DateTime.Now.ToString("MM");
            textBox2.Text = DateTime.Now.ToString("yyyy");
            checkBox1.Checked = true;

            chart1.Visible = false;
            chart2.Visible = false;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
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

        private async void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && textBox1.Text != string.Empty && textBox2.Text != string.Empty)
            {
                await FetchAndDisplayMonthlyData();
                await FetchAndDisplayBillsData();
            }

            if (checkBox2.Checked && textBox2.Text != string.Empty)
            {
                await FetchAndDisplayAnnualData();
                await FetchAndDisplayBillsData();
            }
        }

        private async Task FetchAndDisplayMonthlyData()
        {
            try
            {
                string month = textBox1.Text;
                string year = textBox2.Text;
                var salaryData = await FetchDataFromAPI<SalaryData>($"{baseUrl}/salaryStatements/{username}/salary");
                var savingsData = await FetchDataFromAPI<SavingsData>($"{baseUrl}/salaryStatements/{username}/savings");

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
                label14.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                label17.Visible = true;

                label3.Text = salaryData.salary.ToString();
                label4.Text = savingsData.savings.ToString();

                var monthlyData = await FetchDataFromAPI<List<BillingEntry>>($"{baseUrl}/monthlyBilling?user_name={username}");
                int monthlySum = monthlyData.Sum(entry => entry.price);

                DateTime startDate = new DateTime(int.Parse(year), int.Parse(month), 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                var billsData = await FetchDataFromAPI<List<Bill>>($"{baseUrl}/bills/range?user_name={username}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}");
                int billsSum = billsData.Sum(bill => bill.price);

                label7.Text = monthlySum.ToString();
                label9.Text = billsSum.ToString();

                int leftToSpend = salaryData.salary - savingsData.savings - monthlySum;
                label10.Text = leftToSpend.ToString();

                int savedUntilNow = salaryData.salary - monthlySum - billsSum;
                label12.Text = savedUntilNow.ToString();

                var (essentialSum, nonEssentialSum) = CalculateEssentialSpending(billsData);
                label15.Text = essentialSum.ToString();
                label16.Text = nonEssentialSum.ToString();

                DisplayChart();
                DisplayChart2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private async Task FetchAndDisplayAnnualData()
        {
            try
            {
                string year = textBox2.Text;
                var salaryData = await FetchDataFromAPI<SalaryData>($"{baseUrl}/salaryStatements/{username}/salary");
                var savingsData = await FetchDataFromAPI<SavingsData>($"{baseUrl}/salaryStatements/{username}/savings");

                int monthsToMultiply = (year == DateTime.Now.ToString("yyyy")) ? int.Parse(DateTime.Now.ToString("MM")) : 12;

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
                label14.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                label17.Visible = true;

                label3.Text = (salaryData.salary * monthsToMultiply).ToString();
                label4.Text = (savingsData.savings * monthsToMultiply).ToString();

                var monthlyData = await FetchDataFromAPI<List<BillingEntry>>($"{baseUrl}/monthlyBilling?user_name={username}");
                int monthlySum = monthlyData.Sum(entry => entry.price) * monthsToMultiply;

                DateTime startDate = new DateTime(int.Parse(year), 1, 1);
                DateTime endDate = (year == DateTime.Now.ToString("yyyy"))
                                   ? new DateTime(int.Parse(year), int.Parse(DateTime.Now.ToString("MM")), DateTime.Now.Day)
                                   : new DateTime(int.Parse(year), 12, 31);

                var billsData = await FetchDataFromAPI<List<Bill>>($"{baseUrl}/bills/range?user_name={username}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}");
                int billsSum = billsData.Sum(bill => bill.price);

                label7.Text = monthlySum.ToString();
                label9.Text = billsSum.ToString();

                int leftToSpend = (salaryData.salary * monthsToMultiply) - (savingsData.savings * monthsToMultiply) - monthlySum;
                label10.Text = leftToSpend.ToString();

                int savedUntilNow = (salaryData.salary * monthsToMultiply) - monthlySum - billsSum;
                label12.Text = savedUntilNow.ToString();

                var (essentialSum, nonEssentialSum) = CalculateEssentialSpending(billsData);
                label15.Text = essentialSum.ToString();
                label16.Text = nonEssentialSum.ToString();

                DisplayChart();
                DisplayChart2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message);
            }
        }

        private async Task FetchAndDisplayBillsData()
        {
            try
            {
                string month = textBox1.Text;
                string year = textBox2.Text;
                DateTime startDate = new DateTime(int.Parse(year), int.Parse(month), 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                var billsData = await FetchDataFromAPI<List<Bill>>(
                    $"{baseUrl}/bills/range?user_name={username}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}");

                int billSum = billsData.Sum(bill => bill.price);

                label9.Text = billSum.ToString();

                var (essentialSum, nonEssentialSum) = CalculateEssentialSpending(billsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bills data: " + ex.Message);
            }
        }

        private (int essentialSum, int nonEssentialSum) CalculateEssentialSpending(List<Bill> billsData)
        {
            int essentialSum = 0;
            int nonEssentialSum = 0;

            foreach (var bill in billsData)
            {
                if (IsEssential(bill.name))
                {
                    essentialSum += bill.price;
                }
                else
                {
                    nonEssentialSum += bill.price;
                }
            }

            return (essentialSum, nonEssentialSum);
        }

        private bool IsEssential(string billName)
        {
            var essentialCategories = new Dictionary<string, List<string>>
            {
                { "Housing", new List<string> { "rent", "mortgage", "property tax", "home insurance" } },
                { "Utilities", new List<string> { "electricity", "water", "gas", "sewage", "garbage", "internet", "phone", "cable" } },
                { "Groceries", new List<string> { "supermarket", "grocery", "food", "produce", "market", "meat", "dairy" } },
                { "Healthcare", new List<string> { "insurance", "doctor", "hospital", "medicine", "pharmacy", "clinic", "dental", "optical" } },
                { "Transportation", new List<string> { "fuel", "gasoline", "diesel", "bus", "subway", "train", "taxi", "rideshare", "uber", "lyft", "parking", "vehicle insurance" } },
                { "Education", new List<string> { "school", "college", "university", "tuition", "books", "supplies" } },
                { "Childcare", new List<string> { "daycare", "babysitter", "nanny", "childcare", "after-school program" } }
            };

            billName = billName.ToLower().Trim();

            foreach (var category in essentialCategories)
            {
                foreach (var keyword in category.Value)
                {
                    if (billName.Contains(keyword))
                    {
                        return true;
                    }
                }
            }

            var regexPatterns = new List<string>
            {
                @"\b(housing|rent|mortgage|property tax|home insurance)\b",
                @"\b(electricity|water|gas|sewage|garbage|internet|phone|cable)\b",
                @"\b(supermarket|grocery|food|produce|market|meat|dairy)\b",
                @"\b(insurance|doctor|hospital|medicine|pharmacy|clinic|dental|optical)\b",
                @"\b(fuel|gasoline|diesel|bus|subway|train|taxi|rideshare|uber|lyft|parking|vehicle insurance)\b",
                @"\b(school|college|university|tuition|books|supplies)\b",
                @"\b(daycare|babysitter|nanny|childcare|after-school program)\b"
            };

            foreach (var pattern in regexPatterns)
            {
                if (Regex.IsMatch(billName, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private async void buttonFetchBillsInRange_Click(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime((int.Parse(textBox1.Text)), (int.Parse(textBox2.Text)), 1);
            DateTime endDate = new DateTime(2024, 5, 31);
            await FetchAndDisplayBillsInRange(startDate, endDate);
        }

        private async Task FetchAndDisplayBillsInRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                var billsData = await FetchDataFromAPI<List<Bill>>(
                    $"{baseUrl}/bills/range?user_name={username}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}");

                int billSum = billsData.Sum(bill => bill.price);

                label9.Text = billSum.ToString();

                var (essentialSum, nonEssentialSum) = CalculateEssentialSpending(billsData);
                label15.Text = essentialSum.ToString();
                label16.Text = nonEssentialSum.ToString();

                int potentialSavings = nonEssentialSum;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bills data: " + ex.Message);
            }
        }

        private void DisplayChart()
        {
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
            chart1.Series[seriesname]["PieLabelStyle"] = "Disabled";

            chart1.Series[seriesname].Points.AddXY("Spendings", label9.Text);
            chart1.Series[seriesname].Points.AddXY("Subscriptions", label7.Text);
            chart1.Series[seriesname].Points.AddXY("Saved until now", label12.Text);
            chart1.ChartAreas[0].BackColor = Color.Transparent;
        }

        private void DisplayChart2()
        {
            chart2.Visible = true;
            chart2.BackColor = Color.Transparent;

            chart2.Series.Clear();
            chart2.Legends.Clear();

            chart2.Legends.Add("MyLegend");
            chart2.Legends[0].LegendStyle = LegendStyle.Table;
            chart2.Legends[0].Docking = Docking.Bottom;
            chart2.Legends[0].Alignment = StringAlignment.Center;
            chart2.Legends[0].Title = "Legend";
            chart2.Legends[0].BorderColor = Color.Black;

            string seriesname = "Serie";
            chart2.Series.Add(seriesname);
            chart2.Series[seriesname].ChartType = SeriesChartType.Pie;
            chart2.Series[seriesname]["PieLabelStyle"] = "Disabled";

            chart2.Series[seriesname].Points.AddXY("Essentials", label15.Text);
            chart2.Series[seriesname].Points.AddXY("Non-Essentials", label16.Text);
            chart2.Series[seriesname].Points.AddXY("Monthly Spendings", label7.Text);
            chart2.Series[seriesname].Points.AddXY("Save Expectations", (int.Parse(label3.Text) - int.Parse(label15.Text) - int.Parse(label16.Text) - int.Parse(label7.Text)));
            chart2.ChartAreas[0].BackColor = Color.Transparent;
        }

        private async Task<T> FetchDataFromAPI<T>(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
        }

        public class SalaryData
        {
            public int salary { get; set; }
        }

        public class SavingsData
        {
            public int savings { get; set; }
        }

        public class BillingEntry
        {
            public int price { get; set; }
        }

        public class Bill
        {
            public string name { get; set; }
            public int price { get; set; }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}
