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
    public partial class Monthly : Form
    {
        private string username;
        private const string baseUrl = "http://localhost:5000";

        public Monthly(Control parent, string username)
        {
            InitializeComponent();
            this.username = username;
            label4.Text = "";

            FetchMonthlyBilling();

            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = parent.Size;
            this.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
        }

        private async void FetchMonthlyBilling()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync($"{baseUrl}/monthlyBilling?user_name={username}");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var billingEntries = JsonConvert.DeserializeObject<List<BillingEntry>>(responseBody);

                    listView1.Items.Clear(); // Clear previous entries
                    foreach (var entry in billingEntries)
                    {
                        ListViewItem item = new ListViewItem(entry.name);  // Ensure case sensitivity
                        item.SubItems.Add(entry.price.ToString());
                        item.SubItems.Add(entry.user_name);
                        item.Tag = entry.ID; // Store the ID in the Tag property
                        listView1.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching monthly billing data: " + ex.Message);
            }
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
                if (textBox1.Text.Length > 32)
                {
                    label4.Text = "Name too long (max 32 chars).";
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
                        var newBilling = new BillingEntry
                        {
                            name = textBox1.Text, // Ensure lowercase keys
                            price = price,
                            user_name = username,
                            ID = $"{textBox1.Text}-{username}"
                        };

                        try
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                var json = JsonConvert.SerializeObject(newBilling);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");
                                HttpResponseMessage response = await client.PostAsync($"{baseUrl}/monthlyBilling", content);
                                response.EnsureSuccessStatusCode();

                                label4.Text = $"{textBox1.Text} of price {textBox2.Text} added";
                                ListViewItem item = new ListViewItem(textBox1.Text);
                                item.SubItems.Add(textBox2.Text);
                                item.SubItems.Add(username);
                                item.Tag = newBilling.ID; // Store the ID in the Tag property
                                listView1.Items.Add(item);
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox1.Focus();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error adding billing entry: " + ex.Message);
                        }
                    }
                    else
                    {
                        label4.Text = "Invalid price.";
                    }
                }
                else
                {
                    label4.Text = "No price specified.";
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
                        HttpResponseMessage response = await client.DeleteAsync($"{baseUrl}/monthlyBilling/{selectedID}");
                        response.EnsureSuccessStatusCode();

                        label4.Text = $"{selectedItem.Text} removed";
                        listView1.Items.Remove(selectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting billing entry: " + ex.Message);
                }
            }
            else
            {
                label4.Text = "No item selected to delete.";
            }
        }

        private async void button3_Click(object sender, EventArgs e) // New method for updating entries
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                string selectedID = selectedItem.Tag.ToString();
                string newName = textBox1.Text;
                string newPriceText = textBox2.Text;
                int newPrice;

                if (!int.TryParse(newPriceText, out newPrice) || newPrice <= 0)
                {
                    label4.Text = "Invalid price.";
                    return;
                }

                try
                {
                    var updatedBilling = new BillingEntry
                    {
                        name = newName,
                        price = newPrice,
                        user_name = username,
                        ID = $"{newName}-{username}"
                    };

                    using (HttpClient client = new HttpClient())
                    {
                        var json = JsonConvert.SerializeObject(updatedBilling);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PutAsync($"{baseUrl}/monthlyBilling/{selectedID}", content);
                        response.EnsureSuccessStatusCode();

                        label4.Text = $"{selectedItem.Text} updated to {newName} with price {newPriceText}";
                        selectedItem.SubItems[0].Text = newName;
                        selectedItem.SubItems[1].Text = newPriceText;
                        selectedItem.Tag = updatedBilling.ID;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating billing entry: " + ex.Message);
                }
            }
            else
            {
                label4.Text = "No item selected to update.";
            }
        }

        public class BillingEntry
        {
            public string name { get; set; } // Ensure lowercase keys
            public int price { get; set; }
            public string user_name { get; set; }
            public string ID { get; set; } // ID is no longer calculated, it will be set directly
        }
    }
}
