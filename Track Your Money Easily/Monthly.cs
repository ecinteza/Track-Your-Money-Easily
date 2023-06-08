using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Track_Your_Money_Easily
{
    public partial class Monthly : Form
    {
        public Monthly()
        {
            InitializeComponent();
            label4.Text = "";
            string path = @".\Monthly.txt";
            using (StreamReader read = new StreamReader(path))
            {
                string line;
                while ((line = read.ReadLine()) != null)
                {
                    if (line != string.Empty)
                    {
                        string[] items = line.Split('|');
                        ListViewItem item = new ListViewItem(items[0]);
                        item.SubItems.Add(items[1]);
                        listView1.Items.Add(item);
                    }
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                        string path = @".\Monthly.txt";
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine('\n' + textBox1.Text + "|" + textBox2.Text + '\n');
                        }
                        label4.Text =textBox1.Text + " of price " + textBox2.Text + " added";
                        ListViewItem item = new ListViewItem(textBox1.Text);
                        item.SubItems.Add(textBox2.Text);
                        listView1.Items.Add(item);
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox1.Focus();
                    } else
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string line_to_delete = listView1.SelectedItems[0].Text + "|" + listView1.SelectedItems[0].SubItems[1].Text;
                label4.Text = listView1.SelectedItems[0].Text + " of price " + listView1.SelectedItems[0].SubItems[1].Text + " removed";
                string path = @".\Monthly.txt";
                List<string> lines = new List<string>();

                string[] allines = File.ReadAllLines(path);
                foreach (string line in allines)
                {
                    if (String.Compare(line, line_to_delete) == 0 || line.Length < 1)
                    {
                        continue;
                    }
                    
                    lines.Add(line);
                }

                File.WriteAllLines(path, lines);
                listView1.Items.Remove(listView1.SelectedItems[0]);
            } else
            {
                label4.Text = "No item selected to delete.";
            }
        }
    }
}
