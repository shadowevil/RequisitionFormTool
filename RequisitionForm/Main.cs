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

        private TransparentPanel button1;
        private TransparentPanel button2;

        public Main()
        {
            InitializeComponent();

            button1 = new TransparentPanel("panel1", new Rectangle(8, 8, 162, 249));
            this.Controls.Add(button1);
            this.Controls["panel1"].BringToFront();
            button1.Click += Button1_Click;

            button2 = new TransparentPanel("panel2", new Rectangle(296, 8, 162, 249));
            this.Controls.Add(button2);
            this.Controls["panel2"].BringToFront();
            button2.Click += Button2_Click;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            WarehouseFormThread = new Thread(new ThreadStart(delegate
            {
                Application.Run(new WarehouseForm());
            }));
            WarehouseFormThread.SetApartmentState(ApartmentState.STA);
            WarehouseFormThread.Start();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            PurchaseFormThread = new Thread(new ThreadStart(delegate
            {
                Application.Run(new PurchaseForm());
            }));
            PurchaseFormThread.SetApartmentState(ApartmentState.STA);
            PurchaseFormThread.Start();
        }
    }

    public class TransparentPanel : Panel
    {
        private bool isMouseEnter = false;

        public TransparentPanel(string name, Rectangle rect) : base()
        {
            this.Name = name;
            this.Visible = true;
            this.Location = new Point(rect.X, rect.Y);
            this.Width = rect.Width;
            this.Height = rect.Height;
            this.Bounds = rect;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isMouseEnter = true;
            this.Cursor = Cursors.Hand;
            this.Refresh();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isMouseEnter = false;
            this.Cursor = Cursors.Default;
            this.Refresh();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen((isMouseEnter ? Brushes.OrangeRed : Brushes.Black), 1.0f), 0, 0, this.Width - 1, this.Height - 1);
            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
    }
}
