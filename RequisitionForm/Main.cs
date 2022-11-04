using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RequisitionForm
{
    public partial class Main : Form
    {
        private Thread WarehouseFormThread;
        private Thread PurchaseFormThread;

        public Main()
        {
            InitializeComponent();

            panel2.Click += Panel2_Click;
            pictureBox2.Click += Panel2_Click;
            label1.Click += Panel2_Click;


            panel1.Click += Panel1_Click;
            pictureBox1.Click += Panel1_Click;
            lblPurchasing.Click += Panel1_Click;
        }

        private void Panel2_Click(object sender, EventArgs e)
        {
            WarehouseFormThread = new Thread(new ThreadStart(delegate
            {
                Application.Run(new WarehouseForm());
            }));
            WarehouseFormThread.SetApartmentState(ApartmentState.STA);
            WarehouseFormThread.Start();
        }

        private void Panel1_Click(object sender, EventArgs e)
        {
            PurchaseFormThread = new Thread(new ThreadStart(delegate
            {
                Application.Run(new PurchaseForm());
            }));
            PurchaseFormThread.SetApartmentState(ApartmentState.STA);
            PurchaseFormThread.Start();
        }
    }
}
