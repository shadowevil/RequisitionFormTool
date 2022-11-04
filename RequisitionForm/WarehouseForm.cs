using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace RequisitionForm
{
    public partial class WarehouseForm : Form
    {
        private const string CategoriesLocation = ".\\Data\\Warehouse\\Categories.txt";
        private const string UnitsLocation = ".\\Data\\Warehouse\\Units.txt";
        private const string DeliveryTypeLocation = ".\\Data\\Warehouse\\DeliveryType.txt";
        private const string ProductListLocation = ".\\Data\\Warehouse\\ProductList.txt";
        private const string XMLTemplateLocation = ".\\Data\\Warehouse\\form.xml";

        private List<Product> ProductList;
        private List<PlantLocation> plantLocations;
        private List<string> DeliveryType;

        public WarehouseForm()
        {
            InitializeComponent();
            
            this.DoubleBuffered = true;
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel2, new object[] { true });
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, ProductListBox, new object[] { true });
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, selectedItems, new object[] { true });
            
            this.SizeChanged += Form1_SizeChanged;
            ProductListBox.EditingControlShowing += ProductListBox_EditingControlShowing;
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            plantLocationListBox.SelectedValueChanged += plantLocationListBox_SelectedIndexChanged;

            LoadInformation();

            LoadFormInformation();
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1_SizeChanged(sender, e);
            if (tabControl1.TabPages[tabControl1.SelectedIndex].Name == "formTabPage")
            {
                foreach (DataGridViewRow dgvr in ProductListBox.Rows)
                {
                    int check = -1;
                    Int32.TryParse(dgvr.Cells["Column5"].Value as string, out check);
                    if (check <= 0 || (dgvr.Cells["Column5"].Value as string) == string.Empty) continue;
                    selectedItems.Rows.Add(new object[]
                    {
                        dgvr.Cells[0].Value,
                        dgvr.Cells[1].Value,
                        dgvr.Cells[2].Value,
                        dgvr.Cells[3].Value,
                        dgvr.Cells[4].Value
                    });
                }
            }
        }

        private void ProductListBox_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column5_KeyPress);
            if(ProductListBox.CurrentCell.ColumnIndex == 4)
            {
                TextBox tb = e.Control as TextBox;
                if(tb != null)
                {
                    tb.KeyPress += Column5_KeyPress;
                }
            }
        }

        private void Column5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LoadFormInformation()
        {
            foreach(Product product in ProductList)
            {
                string unit = Product.UNITDictionary.First(x => x.Value == product.OrderUNIT).Key;

                ProductListBox.Rows.Add(new object[]
                {
                    Product.CategoryDictionary.Where(x => x.Value == product.category).First().Key,
                    product.ItemNum,
                    product.description,
                    unit
                });
            }

            foreach(PlantLocation pl in plantLocations)
            {
                plantLocationListBox.Items.Add(pl);
            }

            foreach(string s in DeliveryType)
            {
                DeliveryMethodBox.Items.Add(s);
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            panel1.Location = new Point(((tabControl1.TabPages["formTabPage"].Width - panel1.Width) / 2) - 2, 6);
            panel1.Height = tabControl1.TabPages["formTabPage"].Height - 6;

            selectedItems.Height = panel1.Height - panel2.Height - 2;
        }

        private void LoadInformation()
        {
            Product.CategoryDictionary = new Dictionary<string, int>();

            using (StreamReader sr = new StreamReader(File.OpenRead(CategoriesLocation)))
            {
                int lineNum = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    List<string> cats = new List<string>();
                    cats.Add(line.Substring(0, line.IndexOf(" = ")));
                    int min = line.IndexOf(" = ") + 3;
                    int max = line.Length;
                    cats.Add(line.Substring(min, max - min));
                    int num = -1;
                    Int32.TryParse(cats[0], out num);
                    if(num == -1)
                    {
                        MessageBox.Show("Error -1:\n\nError Categories file failure at line: " + lineNum, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-1);
                    }
                    Product.CategoryDictionary.Add(cats[1], num);

                    lineNum++;
                }
            }

            Product.UNITDictionary = new Dictionary<string, int>();

            using (StreamReader sr = new StreamReader(File.OpenRead(UnitsLocation)))
            {
                int lineNum = 0;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    List<string> units = new List<string>();
                    units.Add(line.Substring(0, line.IndexOf(" = ")));
                    int min = line.IndexOf(" = ") + 3;
                    int max = line.Length;
                    units.Add(line.Substring(min, max - min));
                    int num = -1;
                    Int32.TryParse(units[0], out num);
                    if (num == -1)
                    {
                        MessageBox.Show("Error -1:\n\nError Units file failure at line: " + lineNum, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-1);
                    }
                    Product.UNITDictionary.Add(units[1], num);

                    lineNum++;
                }
            }

            plantLocations = PlantLocation.LoadPlantLocations();

            DeliveryType = new List<string>();
            using (StreamReader sr = new StreamReader(File.OpenRead(DeliveryTypeLocation)))
            {
                int lineNum = 0;
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    DeliveryType.Add(line);
                    lineNum++;
                }
            }

            ProductList = new List<Product>();

            using (StreamReader sr = new StreamReader(File.OpenRead(ProductListLocation)))
            {
                int lineNum = 0;
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    string[] lineSplitByTab = line.Split('\t');

                    Product p = new Product();

                    Product.CategoryDictionary.TryGetValue(lineSplitByTab[0], out p.category);
                    if(p.category == -1)
                    {
                        MessageBox.Show("Error -1:\n\nUnable to parse ProductList file at line " + lineNum, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-1);
                    }
                    p.ItemNum = lineSplitByTab[1];
                    p.description = lineSplitByTab[2];

                    Product.UNITDictionary.TryGetValue(lineSplitByTab[3], out p.OrderUNIT);
                    if (p.category == -1)
                    {
                        MessageBox.Show("Error -2:\n\nUnable to parse ProductList file at line " + lineNum, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(-2);
                    }

                    ProductList.Add(p);

                    lineNum++;
                }
            }
        }

        private void plantLocationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(plantLocationListBox.SelectedItem == null) return;
            PlantLocation pl = plantLocationListBox.SelectedItem as PlantLocation;
            lblSelectedLocation.Text = pl.PlantNum + "\n"
                + pl.CityState + "\n"
                + pl.plantEMail + "\n"
                + pl.PlantPhoneNumber;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in ProductListBox.Rows)
            {
                row.Cells["Column5"].Value = "";
            }

            selectedItems.Rows.Clear();
            plantLocationListBox.SelectedIndex = -1;
            plantLocationListBox.Text = "(Select location)";
            DeliveryMethodBox.SelectedIndex = -1;
            DeliveryMethodBox.Text = "(Select preferred delivery method)";
            txtSpecialInstructions.Text = "";
            lblSelectedLocation.Text = "";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (plantLocationListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Plant location not set. Please specify a location.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DeliveryMethodBox.SelectedIndex == -1)
            {
                MessageBox.Show("Plant Delivery Method not set. Please specify a method of delivery.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selectedItems.Rows.Count <= 0)
            {
                MessageBox.Show("Unable to save, no items selected", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".xml";
            sfd.Filter = "XML File (.xml)|*.xml|All Files (*.*)|*.*";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\";
            sfd.RestoreDirectory = true;
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            sfd.FileName = new string(("Warehouse Requisition Form " + DateTime.Now.ToString("MM/dd/yyyy")).Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
            sfd.FileName += ".xml";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string[] copyXMLFile = File.ReadAllLines(XMLTemplateLocation);
                List<string> newXMLFile = new List<string>();
                using (StreamWriter sw = new StreamWriter(File.Create(sfd.FileName)))
                {
                    int rowNum = 0;
                    int lineNum = 0;
                    KeyValuePair<bool, string> flag = new KeyValuePair<bool, string>(false, null);
                    int flagNum = -1;
                    PlantLocation pl = null;
                    foreach (string line in copyXMLFile)
                    {
                        newXMLFile.Add(line);
                        if(line.Contains("<Data ss:Type=\"String\">Date:"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Date:");
                            flagNum = lineNum + 1;
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Plant Location:"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Plant Location:");
                            flagNum = lineNum + 1;
                            pl = (plantLocationListBox.SelectedItem as PlantLocation);
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Plant Email:"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Plant Email:");
                            flagNum = lineNum + 1;
                            pl = (plantLocationListBox.SelectedItem as PlantLocation);
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Plant Phone #:"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Plant Phone #:");
                            flagNum = lineNum + 1;
                            pl = (plantLocationListBox.SelectedItem as PlantLocation);
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Preferred Method of Delivery:"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Preferred Method of Delivery:");
                            flagNum = lineNum + 1;
                            pl = (plantLocationListBox.SelectedItem as PlantLocation);
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Enter Special Instructions in box below"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Enter Special Instructions in box below");
                            flagNum = lineNum + 3;
                            pl = (plantLocationListBox.SelectedItem as PlantLocation);
                        }
                        if(line.Contains("<Data ss:Type=\"String\">Order Quantity"))
                        {
                            flag = new KeyValuePair<bool, string>(true, "<Data ss:Type=\"String\">Order Quantity");
                            flagNum = lineNum + 1;
                            pl = null;
                        }

                        if(flag.Key && flagNum == lineNum)
                        {
                            if(flag.Value.Contains("Date:"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + DateTime.Now.ToString() + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;

                            } else if(flag.Value.Contains("Plant Location:"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                if (pl == null)
                                {
                                    MessageBox.Show("Error -1:\n\nError issue with plant location, not set.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(-1);
                                }
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + pl.CityState + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                            } else if(flag.Value.Contains("Plant Email:"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                if (pl == null)
                                {
                                    MessageBox.Show("Error -1:\n\nError issue with Plant e-mail, not set.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(-1);
                                }
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + pl.plantEMail + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                            } else if(flag.Value.Contains("Plant Phone #:"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                if(pl == null)
                                {
                                    MessageBox.Show("Error -1:\n\nError issue with Plant phone number, not set.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(-1);
                                }
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + pl.PlantPhoneNumber + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                            } else if(flag.Value.Contains("Preferred Method of Delivery:"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                if(pl == null)
                                {
                                    MessageBox.Show("Error -1:\n\nError issue with Delivery method, not set.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(-1);
                                }
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + DeliveryMethodBox.Items[DeliveryMethodBox.SelectedIndex] + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                            } else if(flag.Value.Contains("Enter Special Instructions in box below"))
                            {
                                int index = line.IndexOf("</Cell>");
                                newXMLFile[lineNum] = line.Substring(0, index);
                                int leng = newXMLFile[lineNum].Length;
                                if(pl == null)
                                {
                                    MessageBox.Show("Error -1:\n\nError issue with special instructions, data not set.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Environment.Exit(-1);
                                }
                                newXMLFile[lineNum] += "<Data ss:Type=\"String\">" + txtSpecialInstructions.Text + "</Data>";
                                newXMLFile[lineNum] += line.Substring(index, line.Length - index);
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                            } else if(flag.Value.Contains("Order Quantity"))
                            {
                                foreach (DataGridViewRow dgvr in selectedItems.Rows)
                                {
                                    List<string> addToFile = new List<string>();
                                    addToFile.Add("   <Row ss:AutoFitHeight=\"0\">");
                                    addToFile.Add("    <Cell><Data ss:Type=\"String\">" + dgvr.Cells[0].Value + "</Data></Cell>");
                                    if ((dgvr.Cells[1].Value as string) == "NOI") addToFile.Add("    <Cell><Data ss:Type=\"String\">" + dgvr.Cells[1].Value + "</Data></Cell>");
                                    else addToFile.Add("    <Cell><Data ss:Type=\"Number\">" + dgvr.Cells[1].Value + "</Data></Cell>");
                                    addToFile.Add("    <Cell><Data ss:Type=\"String\">" + dgvr.Cells[2].Value + "</Data></Cell>");
                                    addToFile.Add("    <Cell><Data ss:Type=\"String\">" + dgvr.Cells[3].Value + "</Data></Cell>");
                                    addToFile.Add("    <Cell><Data ss:Type=\"Number\">" + dgvr.Cells[4].Value + "</Data></Cell>");
                                    addToFile.Add("   </Row>");

                                    foreach(string s in addToFile)
                                    {
                                        newXMLFile.Add(s);
                                    }

                                    lineNum += 7;
                                    rowNum++;
                                }
                                flag = new KeyValuePair<bool, string>(false, null);
                                flagNum = -1;
                                pl = null;
                                continue;
                            }
                        }

                        lineNum++;
                    }

                    foreach (string l in newXMLFile)
                    {
                        if(l.Contains("<Table ss:ExpandedColumnCount=\"8\" ss:ExpandedRowCount=\""))
                        {
                            int index = l.IndexOf("12\" x:FullColumns=\"1\"");
                            int index2 = l.IndexOf("\" x:FullColumns=\"1\"");
                            int num = 0;
                            Int32.TryParse(l.Substring(index, index2 - index), out num);
                            int ExpandedRowCount = num + rowNum + 1;
                            string line = l.Substring(0, index) + ExpandedRowCount.ToString() +
                                l.Substring(index + (ExpandedRowCount.ToString().Length), index2 - index - (ExpandedRowCount.ToString().Length)) +
                                "\" x:FullColumns=\"1\"";
                            sw.WriteLine(line);
                            continue;
                        }

                        sw.WriteLine(l);
                        if (l.Contains("</Workbook>")) break;
                    }
                }
            }
        }
    }
}
