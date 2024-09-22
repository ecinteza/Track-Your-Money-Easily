using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace Track_Your_Money_Easily
{
    public partial class Login : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public Login()
        {
            InitializeComponent();
            this.button1.Click += new System.EventHandler(this.button1_Click);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var name = textBox1.Text;
            var password = textBox2.Text;
            var confirmPassword = textBox3.Text;
            var isRegister = textBox3.Visible;

            if (isRegister && password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            if (name.Contains(" "))
            {
                MessageBox.Show("Username must not contain spaces");
                return;
            }

            var userData = new
            {
                name = name,
                password = password,
                age = 18,  // Dummy data for age, email and image for login, following that the user modifies it according to their real data
                email = "dummy@example.com",
                image = "dummy.jpg"
            };

            try
            {
                var json = JsonConvert.SerializeObject(userData);  // Use JsonConvert to serialize the object to JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response;
                if (isRegister)
                {
                    response = await client.PostAsync("http://127.0.0.1:5000/register", content);
                    this.checkBox1.Checked = false;
                }
                else
                {
                    response = await client.PostAsync("http://127.0.0.1:5000/login", content);
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);  // Use JsonConvert to deserialize the JSON response

                //MessageBox.Show(result["message"]);

                if (result["message"] == "Login successful")
                {
                    runMenu(name);  // Call runMenu if login is successful and pass the username
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (label3.Visible == false)
            {
                textBox3.Visible = true;
                label3.Visible = true;
                button1.Text = "Register";
            }
            else
            {
                textBox3.Visible = false;
                label3.Visible = false;
                button1.Text = "Login";
            }
        }

        private void runMenu(string username)
        {
            Menu menuForm = new Menu(username);
            this.Hide();
            menuForm.ShowDialog();
            this.Close();
        }
    }
}
