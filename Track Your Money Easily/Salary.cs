using System;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Track_Your_Money_Easily
{
    public partial class Salary : Form
    {
        private string username;
        private const string baseUrl = "http://localhost:5000";

        public Salary(Control parent, string username)
        {
            InitializeComponent();
            this.username = username;

            FetchSalaryAndSavings();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        private async void FetchSalaryAndSavings()
        {
            var client = new RestClient(baseUrl);

            var requestSalary = new RestRequest($"/salaryStatements/{username}/salary", Method.Get);
            var requestSavings = new RestRequest($"/salaryStatements/{username}/savings", Method.Get);

            try
            {
                var responseSalary = await client.ExecuteAsync(requestSalary);
                var responseSavings = await client.ExecuteAsync(requestSavings);

                if (responseSalary.IsSuccessful && responseSavings.IsSuccessful)
                {
                    var salaryJson = JObject.Parse(responseSalary.Content);
                    var savingsJson = JObject.Parse(responseSavings.Content);

                    label3.Text = salaryJson["salary"].ToString();
                    label7.Text = savingsJson["savings"].ToString();
                }
                else
                {
                    await CreateInitialSalaryAndSavings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CreateInitialSalaryAndSavings()
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest("/salaryStatements", Method.Post);
            request.AddJsonBody(new { salary = 0, savings = 0, name = username });

            try
            {
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    label3.Text = "0";
                    label7.Text = "0";
                }
                else
                {
                    MessageBox.Show("Failed to create initial salary and savings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var sal = textBox1.Text;
            if (sal.Length > 0)
            {
                int salaryN;
                bool isNumeric = int.TryParse(sal, out salaryN);
                if (isNumeric && salaryN > 0)
                {
                    var client = new RestClient(baseUrl);
                    var request = new RestRequest($"/salaryStatements/{username}", Method.Put);
                    request.AddJsonBody(new { salary = salaryN });

                    try
                    {
                        var response = await client.ExecuteAsync(request);

                        if (response.IsSuccessful)
                        {
                            label3.Text = salaryN.ToString();
                            label5.Text = "UPDATED\nSALARY";
                        }
                        else
                        {
                            label5.Text = "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    label5.Text = "ERROR";
                }
            }
            textBox1.Clear();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var sav = textBox2.Text;
            if (sav.Length > 0)
            {
                int savN;
                bool isNumeric = int.TryParse(sav, out savN);
                if (isNumeric && savN > 0)
                {
                    var client = new RestClient(baseUrl);
                    var request = new RestRequest($"/savings/{username}", Method.Put);
                    request.AddJsonBody(new { savings = savN });

                    try
                    {
                        var response = await client.ExecuteAsync(request);

                        if (response.IsSuccessful)
                        {
                            label7.Text = savN.ToString();
                            label5.Text = "UPDATED\nSAVINGS";
                        }
                        else
                        {
                            label5.Text = "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
