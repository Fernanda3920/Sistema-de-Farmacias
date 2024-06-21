using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
      
        private void button2_Click(object sender, EventArgs e)
        {

            CiudadForm ciudadForm = new CiudadForm();

            this.Hide();

            ciudadForm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PropietarioForm propietarioForm = new PropietarioForm();
            this.Hide();
            propietarioForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FarmaciaForm farmaciaForm = new FarmaciaForm();
            this.Hide();
            farmaciaForm.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MedicamentoForm medicamentoForm = new MedicamentoForm();
            this.Hide();
            medicamentoForm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ClienteForm clienteForm = new ClienteForm();
            this.Hide();
            clienteForm.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TicketForm ticketForm = new TicketForm();
            this.Hide();
            ticketForm.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CostosForm costosForm = new CostosForm();   
            this.Hide();
            costosForm.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

       
    }
}
