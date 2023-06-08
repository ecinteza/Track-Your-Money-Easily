using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Track_Your_Money_Easily
{
    public partial class Profile : Form
    {
        public Profile()
        {
            InitializeComponent();
            label5.Text = File.ReadAllText(@".\Username.txt");
            label6.Text = File.ReadAllText(@".\Age.txt");
            label8.Text = File.ReadAllText(@".\Email.txt");
            using (Image image = Image.FromFile(@".\ProfilePicture.jpg"))
            {
                pictureBox1.Image = new Bitmap(image);
                image.Dispose();
            }
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string status = string.Empty;
            var name = textBox1.Text;
            var age = textBox2.Text;
            var email = textBox3.Text;
            if (name.Length > 0) 
            { 
                if (Regex.IsMatch(name, @"^[a-zA-Z]+$") && name.Length <= 32)
                {
                    using (StreamWriter writer = new StreamWriter(@".\Username.txt"))
                    {
                        writer.WriteLine(name);
                        label5.Text = name;
                        status = "NAME UPDATED\n";
                    }
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
                    using (StreamWriter writer = new StreamWriter(@".\Age.txt"))
                    {
                        writer.WriteLine(age);
                        label6.Text = age;
                        status += "AGE UPDATED\n";
                    }
                }
                else
                {
                    status += "ERROR INVALID AGE\n";
                }
                textBox2.Clear();
            }
            if (email.Length> 0)
            {
                if (IsValidEmail(email) && email.Length <= 41)
                {
                    using (StreamWriter writer = new StreamWriter(@".\Email.txt"))
                    {
                        writer.WriteLine(email);
                        label8.Text = email;
                        status += "EMAIL UPDATED\n";
                    }
                }
                else
                {
                    status += "ERROR INVALID EMAIL\n";
                }
                textBox3.Clear();
            }
            label12.Text = status;
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Open Image file";
                fileOpen.Filter = "JPG Files (*.jpg)| *.jpg";

                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    using (Image image = Image.FromFile(fileOpen.FileName))
                    {
                        try
                        {
                            File.Delete(@".\ProfilePicture.jpg");
                            resizeImage(image, new Size(155, 155)).Save(@".\ProfilePicture.jpg");
                            var img = new Bitmap(@".\ProfilePicture.jpg");
                            img.Dispose();
                            image.Dispose();
                            using (Image imeg = Image.FromFile(@".\ProfilePicture.jpg"))
                            {
                                pictureBox1.Image = new Bitmap(imeg);
                                imeg.Dispose();
                            }
                            label12.Text = "IMAGE UPDATED";
                        }
                        catch
                        {
                            label12.Text = "AN ERROR OCCURED.";
                        }

                    }
                }
                fileOpen.Dispose();
            } catch
            {
                label12.Text = "AN ERROR OCCURED.";
            }
        }
    }
}
