using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
        private bool button1Hover = false;
        private bool button2Hover = false;
        private Pen buttonBorder;
        private Pen buttonBorderHover;

        public Main()
        {
            InitializeComponent();

            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel1, new object[] { true });
            typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel2, new object[] { true });

            this.MouseMove += Main_MouseMove;

            panel2.Click += Panel2_Click;
            pictureBox2.Click += Panel2_Click;
            label1.Click += Panel2_Click;

            panel1.Click += Panel1_Click;
            pictureBox1.Click += Panel1_Click;
            lblPurchasing.Click += Panel1_Click;

            panel2.Paint += Panel2_Paint;
            panel1.Paint += Panel2_Paint;

            buttonBorder = new Pen(Brushes.Black, 1.0f);
            buttonBorderHover = new Pen(Brushes.Orange, 1.0f);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Point relativePoint = this.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            e.Graphics.DrawString(relativePoint.X + ":" + relativePoint.Y, DefaultFont, Brushes.Black, relativePoint.X, relativePoint.Y - 10.0f);
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            Point relativePoint = this.PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            Rectangle r = panel1.Bounds;
            Rectangle r2 = panel2.Bounds;

            if (r.Contains(relativePoint) && !button1Hover)
            {
                button1Hover = true;
                panel1.Refresh();
            } else if (!r.Contains(relativePoint) && button1Hover)
            {
                button1Hover = false;
                panel1.Refresh();
            }

            if (r2.Contains(relativePoint) && !button2Hover)
            {
                button2Hover = true;
                panel2.Refresh();
            }
            else if (!r2.Contains(relativePoint) && button2Hover)
            {
                button2Hover = false;
                panel2.Refresh();
            }
        }

        private void Panel2_Paint(object sender, PaintEventArgs e)
        {
            Pen p = buttonBorder;
            if ((sender as Panel).Name == "panel2" && button2Hover) p = buttonBorderHover;
            if ((sender as Panel).Name == "panel1" && button1Hover) p = buttonBorderHover;
            e.Graphics.DrawRectangle(p, new Rectangle(0, 0, (sender as Panel).Width - 1, (sender as Panel).Height - 1));
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
