using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Track_Your_Money_Easily
{
    public partial class Profile : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private string username;
        private string imagePath; // Add a class-level variable to store image path

        public Profile(Control parent, string username)
        {
            InitializeComponent();
            this.username = username;
            this.imagePath = string.Empty; // Initialize the image path
            this.Load += async (s, e) => await LoadUserProfile(username);

            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        private async Task LoadUserProfile(string username)
        {
            try
            {
                var response = await client.GetAsync($"http://127.0.0.1:5000/userProfile/{username}");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var userProfile = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

                label5.Text = userProfile["name"];
                label6.Text = userProfile["age"];
                label8.Text = userProfile["email"];

                imagePath = userProfile["image"]; // Store the image path
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading user profile: {ex.Message}");
            }
        }

        private async Task UpdateUserProfile(string name, string age, string email, string imagePath)
        {
            var userProfile = new Dictionary<string, string>
            {
                { "name", name },
                { "age", age },
                { "email", email },
                { "image", imagePath }
            };

            try
            {
                var json = JsonConvert.SerializeObject(userProfile);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"http://127.0.0.1:5000/userProfile/{username}", content);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                MessageBox.Show(result["message"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating user profile: {ex.Message}");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string status = string.Empty;
            var name = textBox1.Text;
            var age = textBox2.Text;
            var email = textBox3.Text;

            if (name.Length > 0)
            {
                if (Regex.IsMatch(name, @"^[a-zA-Z]+$") && name.Length <= 32)
                {
                    label5.Text = name;
                    status = "NAME UPDATED\n";
                }
                else
                {
                    status = "ERROR INVALID NAME\n";
                }
                textBox1.Clear();
            }

            if (age.Length > 0)
            {
                int intage;
                bool isNumeric = int.TryParse(age, out intage);
                if (isNumeric && intage > 0 && intage <= 100)
                {
                    label6.Text = age;
                    status += "AGE UPDATED\n";
                }
                else
                {
                    status += "ERROR INVALID AGE\n";
                }
                textBox2.Clear();
            }

            if (email.Length > 0)
            {
                if (IsValidEmail(email) && email.Length <= 41)
                {
                    label8.Text = email;
                    status += "EMAIL UPDATED\n";
                }
                else
                {
                    status += "ERROR INVALID EMAIL\n";
                }
                textBox3.Clear();
            }

            label12.Text = status;
            await UpdateUserProfile(name, age, email, imagePath);
        }

        bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
    }
}
