using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Track_Your_Money_Easily
{
    public partial class Bills : Form
    {
        public Bills()
        {
            InitializeComponent();
            label4.Text = "";
            textBox3.Text = DateTime.Now.ToString("dd");
            textBox4.Text = DateTime.Now.ToString("MM");
            textBox5.Text = DateTime.Now.ToString("yyyy");
            string path = @".\Bills.txt";
            using (StreamReader read = new StreamReader(path))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    if (line != string.Empty)
                    {
                        string[] items = line.Split('|');
                        ListViewItem item = new ListViewItem(items[0]);
                        item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                        item.SubItems.Add(items[4]);
                        item.SubItems.Add(items[5]);

                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
                            if (!DateTime.TryParse(textBox3.Text + "/" + textBox4.Text + "/" + textBox5.Text, out temp))
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

                            string path = @".\Bills.txt";
                            var lastline = "";
                            var id = "";
                            try
                            {
                                var linez = File.ReadAllLines(path);
                                foreach(string line in linez)
                                {
                                    if (line.Length > 1) lastline = line;
                                }
                                id = (int.Parse(lastline.Split('|')[0]) + 1).ToString();
                            } catch
                            {
                                id = "1";
                            }
                            using (StreamWriter sw = File.AppendText(path))
                            {

                                sw.WriteLine('\n' + id + "|" + textBox3.Text + "|" + textBox4.Text + "|" + textBox5.Text + "|" + textBox1.Text + "|" + textBox2.Text + '\n');
                            }
                            label4.Text = textBox1.Text + " of price " + textBox2.Text + " added";
                            ListViewItem item = new ListViewItem(id);
                            item.SubItems.Add(textBox3.Text + "/" + textBox4.Text + "/" + textBox5.Text);
                            item.SubItems.Add(textBox1.Text);
                            item.SubItems.Add(textBox2.Text);
                            listView1.Items.Add(item);
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox1.Focus();
                        }
                        
                    } else
                    {
                        label4.Text = "Invalid price.";
                    }
                }
            } else
            {
                label4.Text = "No name specified.";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string line_to_delete = listView1.SelectedItems[0].Text + "|" + listView1.SelectedItems[0].SubItems[1].Text.Replace("/", "|") + "|" + listView1.SelectedItems[0].SubItems[2].Text + "|" + listView1.SelectedItems[0].SubItems[3].Text;
                label4.Text = listView1.SelectedItems[0].SubItems[2].Text + " of price " + listView1.SelectedItems[0].SubItems[3].Text + " from " + listView1.SelectedItems[0].SubItems[1].Text + " removed";
                string path = @".\Bills.txt";
                List<string> lines = new List<string>();
                int del = 0;
                string[] allines = File.ReadAllLines(path);
                foreach (string line in allines)
                {
                    string theline = line;
                    if (theline.Length < 1)
                    {
                        continue;
                    }
                    if (String.Compare(theline, line_to_delete) == 0)
                    {
                        del = 1;
                        continue;
                    }

                    if (del == 1)
                    {
                        string id = theline.Split('|')[0];
                        theline = theline.Replace(id + "|", (int.Parse(theline.Split('|')[0])-1).ToString() + "|");
                        if (listView1.Items.Count > 1)
                            listView1.FindItemWithText(id).Text = (int.Parse(id) - 1).ToString();
                        
                    }

                    lines.Add(theline);
                }

                File.WriteAllLines(path, lines);
                listView1.Items.Remove(listView1.SelectedItems[0]);
            }
            else
            {
                label4.Text = "No item selected to delete.";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            string path = @".\Bills.txt";
            using (StreamReader read = new StreamReader(path))
            {
                if (textBox8.Text != string.Empty && (int.Parse(textBox8.Text) <=0 || int.Parse(textBox8.Text)>31))
                {
                    label4.Text = "Invalid day in filter.";
                    return;
                }
                if (textBox7.Text != string.Empty && (int.Parse(textBox7.Text) <= 0 || int.Parse(textBox7.Text) > 12))
                {
                    label4.Text = "Invalid month in filter.";
                    return;
                }
                if (textBox6.Text != string.Empty && (int.Parse(textBox6.Text) <= 1970 || int.Parse(textBox6.Text) > int.Parse(DateTime.Now.ToString("yyyy"))))
                {
                    label4.Text = "Invalid year in filter.";
                    return;
                }
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    if (line != string.Empty)
                    {
                        string[] items = line.Split('|');

                        if (textBox8.Text != string.Empty && textBox7.Text != string.Empty && textBox6.Text != string.Empty)
                        {
                            if (items[1] == textBox8.Text && items[2] == textBox7.Text && items[3] == textBox6.Text)
                            {
                                ListViewItem item = new ListViewItem(items[0]);
                                item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                                item.SubItems.Add(items[4]);
                                item.SubItems.Add(items[5]);

                                listView1.Items.Add(item);
                            }
                        }

                        else if (textBox7.Text != string.Empty && textBox6.Text != string.Empty)
                        {
                            if (items[2] == textBox7.Text && items[3] == textBox6.Text && textBox8.Text == string.Empty)
                            {
                                ListViewItem item = new ListViewItem(items[0]);
                                item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                                item.SubItems.Add(items[4]);
                                item.SubItems.Add(items[5]);

                                listView1.Items.Add(item);
                            }
                        }

                        else if (textBox7.Text != string.Empty && textBox8.Text == string.Empty)
                        {
                            if (items[2] == textBox7.Text)
                            {
                                ListViewItem item = new ListViewItem(items[0]);
                                item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                                item.SubItems.Add(items[4]);
                                item.SubItems.Add(items[5]);

                                listView1.Items.Add(item);
                            }
                        }

                        else if (textBox6.Text != string.Empty && textBox8.Text == string.Empty)
                        {
                            if (items[3] == textBox6.Text)
                            {
                                ListViewItem item = new ListViewItem(items[0]);
                                item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                                item.SubItems.Add(items[4]);
                                item.SubItems.Add(items[5]);

                                listView1.Items.Add(item);
                            }
                        }

                        else
                        {
                            label4.Text = "Invalid Filter. Only [DD/MM/YYYY | MM/YYYY | MM | YYYY] formats allowed.";
                            return;
                        }
                    }
                }
                label4.Text = "Filter applied";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            label4.Text = "Filter removed";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            string path = @".\Bills.txt";
            using (StreamReader read = new StreamReader(path))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    if (line != string.Empty)
                    {
                        string[] items = line.Split('|');
                        ListViewItem item = new ListViewItem(items[0]);
                        item.SubItems.Add(items[1] + "/" + items[2] + "/" + items[3]);
                        item.SubItems.Add(items[4]);
                        item.SubItems.Add(items[5]);

                        listView1.Items.Add(item);
                    }
                }
            }
        }
    }
}
