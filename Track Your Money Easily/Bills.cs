using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Track_Your_Money_Easily
{
    public partial class Bills : Form
    {
        private string username;
        private const string baseUrl = "http://localhost:5000";

        public Bills(Control parent, string username)
        {
            InitializeComponent();
            this.username = username;

            label4.Text = "";
            textBox3.Text = DateTime.Now.ToString("dd");
            textBox4.Text = DateTime.Now.ToString("MM");
            textBox5.Text = DateTime.Now.ToString("yyyy");

            FetchBills();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;

            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
        }

        private async void FetchBills()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{baseUrl}/bills?user_name={username}");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var bills = JsonConvert.DeserializeObject<List<Bill>>(responseBody);

                    listView1.Items.Clear(); // Clear previous entries
                    foreach (var bill in bills)
                    {
                        ListViewItem item = new ListViewItem(bill.name);  // Ensure case sensitivity
                        item.SubItems.Add(bill.price.ToString());
                        item.SubItems.Add(bill.date.ToString("yyyy-MM-dd"));
                        item.Tag = bill.ID; // Store the ID in the Tag property
                        listView1.Items.Add(item);
                    }
                }
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                // No bills found for the user
                listView1.Items.Clear();
                label4.Text = "No bills found.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching bills: " + ex.Message);
            }
        }

        private string GenerateID(string name, string userName, string date, int price)
        {
            return $"{name}-{userName}-{date}-{price}";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Any(Char.IsLetter))
            {
                label4.Text = "Invalid naming.";
                textBox1.Clear();
                textBox2.Clear();
                textBox1.Focus();
                return;
            }

            if (textBox1.Text != string.Empty)
            {
                if (textBox1.Text.Length > 30)
                {
                    label4.Text = "Name too long (max 30 chars).";
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox1.Focus();
                    return;
                }

                if (textBox2.Text != string.Empty)
                {
                    int price;
                    bool isNumeric = int.TryParse(textBox2.Text, out price);
                    if (isNumeric && price > 0)
                    {
                        if (textBox3.Text != string.Empty && textBox4.Text != string.Empty && textBox5.Text != string.Empty)
                        {
                            DateTime temp;
                            if (!DateTime.TryParse($"{textBox5.Text}-{textBox4.Text}-{textBox3.Text}", out temp))
                            {
                                label4.Text = "Invalid date.";
                                textBox3.Text = DateTime.Now.ToString("dd");
                                textBox4.Text = DateTime.Now.ToString("MM");
                                textBox5.Text = DateTime.Now.ToString("yyyy");
                                textBox1.Focus();
                                return;
                            }

                            if (temp > DateTime.Now)
                            {
                                label4.Text = "You cannot put a date that is in the future.";
                                textBox3.Text = DateTime.Now.ToString("dd");
                                textBox4.Text = DateTime.Now.ToString("MM");
                                textBox5.Text = DateTime.Now.ToString("yyyy");
                                textBox1.Focus();
                                return;
                            }

                            var newBill = new Bill
                            {
                                date = temp,
                                name = textBox1.Text,
                                price = price,
                                user_name = username,
                                ID = GenerateID(textBox1.Text, username, temp.ToString("yyyy-MM-dd"), price)
                            };

                            try
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    var json = JsonConvert.SerializeObject(newBill);
                                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    HttpResponseMessage response = await client.PostAsync($"{baseUrl}/bills", content);
                                    response.EnsureSuccessStatusCode();

                                    label4.Text = $"{textBox1.Text} of price {textBox2.Text} added";
                                    ListViewItem item = new ListViewItem(textBox1.Text);
                                    item.SubItems.Add(textBox2.Text);
                                    item.SubItems.Add(temp.ToString("yyyy-MM-dd"));
                                    item.Tag = newBill.ID; // Store the ID in the Tag property
                                    listView1.Items.Add(item);
                                    textBox1.Clear();
                                    textBox2.Clear();
                                    textBox1.Focus();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error adding bill entry: " + ex.Message);
                            }
                        }

                    }
                    else
                    {
                        label4.Text = "Invalid price.";
                    }
                }
            }
            else
            {
                label4.Text = "No name specified.";
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string selectedID = selectedItem.Tag.ToString();

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/bills/{selectedID}");
                        response.EnsureSuccessStatusCode();

                        label4.Text = $"{selectedItem.Text} removed";
                        listView1.Items.Remove(selectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting bill entry: " + ex.Message);
                }
            }
            else
            {
                label4.Text = "No item selected to delete.";
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var query = new StringBuilder($"{baseUrl}/bills/filter?user_name={username}");

                    if (!string.IsNullOrEmpty(textBox8.Text))
                    {
                        int day;
                        if (int.TryParse(textBox8.Text, out day) && day > 0 && day <= 31)
                        {
                            query.Append($"&day={day}");
                        }
                        else
                        {
                            label4.Text = "Invalid day in filter.";
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(textBox7.Text))
                    {
                        int month;
                        if (int.TryParse(textBox7.Text, out month) && month > 0 && month <= 12)
                        {
                            query.Append($"&month={month}");
                        }
                        else
                        {
                            label4.Text = "Invalid month in filter.";
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(textBox6.Text))
                    {
                        int year;
                        if (int.TryParse(textBox6.Text, out year) && year > 1970 && year <= DateTime.Now.Year)
                        {
                            query.Append($"&year={year}");
                        }
                        else
                        {
                            label4.Text = "Invalid year in filter.";
                            return;
                        }
                    }

                    HttpResponseMessage response = await client.GetAsync(query.ToString());
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var bills = JsonConvert.DeserializeObject<List<Bill>>(responseBody);

                    foreach (var bill in bills)
                    {
                        ListViewItem item = new ListViewItem(bill.name);  // Ensure case sensitivity
                        item.SubItems.Add(bill.price.ToString());
                        item.SubItems.Add(bill.date.ToString("yyyy-MM-dd"));
                        item.Tag = bill.ID; // Store the ID in the Tag property
                        listView1.Items.Add(item);
                    }

                    label4.Text = "Filter applied";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering bills: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            FetchBills();
            label4.Text = "Filter removed";
        }

        public class Bill
        {
            public DateTime date { get; set; }
            public string name { get; set; }
            public int price { get; set; }
            public string user_name { get; set; }
            public string ID { get; set; }
        }
    }
}
