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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Sistema
{
    public partial class CostosForm : Form
    {
        FuncionesBasicas instanciaCostos = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";

        public CostosForm()
        {
            InitializeComponent();
            CargarDatosCosto(dataGridView1);
            this.Load += CostosForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        public void costosGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1, int idMedicmentoSeleccionado, int idTicketSeleccionado)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idCosto = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroCosto(idCosto))
                {
                    string consulta = "UPDATE COSTO SET proveedor = @Proveedor, costopedido = @CostoPedido, costoventa = @CostoVenta, idmedicmento = @IdMedicmento, idticket = @IdTicket WHERE idcosto = @IdCosto";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCosto", idCosto);
                        comando.Parameters.AddWithValue("@Proveedor", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@CostoPedido", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CostoVenta", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdMedicmento", idMedicmentoSeleccionado);
                        comando.Parameters.AddWithValue("@IdTicket", idTicketSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosCosto(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO COSTO (idcosto, proveedor, costopedido, costoventa, idmedicmento, idticket) VALUES (@IdCosto, @Proveedor, @CostoPedido, @CostoVenta, @IdMedicmento, @idTicket)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCosto", idCosto);
                        comando.Parameters.AddWithValue("@Proveedor", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@CostoPedido", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CostoVenta", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdMedicmento", idMedicmentoSeleccionado);
                        comando.Parameters.AddWithValue("@IdTicket", idTicketSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaCostos.LimpiarCampos(textBoxes);
                CargarDatosCosto(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroCosto(int idCosto)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM COSTO WHERE idcosto = @IdCosto";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdCosto", idCosto);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDCosto(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM COSTO WHERE idcosto = @Valor";
                    SqlCommand comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@Valor", valorABuscar);

                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    dataGridView1.DataSource = tabla;
                }
            }
            else
            {
                MessageBox.Show("Por favor ingrese un valor para buscar.");
            }
        }
        public void EliminarEnBDCosto(TextBox textBox5, TextBox textBox1, TextBox textBox2, TextBox textBox3, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idCosto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idcosto"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM COSTO WHERE idcosto = @IdCosto";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdCosto", idCosto);
                        comando.ExecuteNonQuery();
                        CargarDatosCosto(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }
        private void CostosForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable ticket = instanciaFunciones.ObtenerTickets();
            DataTable medicamento = instanciaFunciones.ObtenerMedicamentos();

            comboBoxTicket.DisplayMember = "idticket"; // Cambiado a "idticket"
            comboBoxTicket.ValueMember = "idticket";
            comboBoxTicket.DataSource = ticket;

            comboBoxMedicamento.DisplayMember = "idmedicmento"; // Cambiado a "idmedicmento"
            comboBoxMedicamento.ValueMember = "idmedicmento";
            comboBoxMedicamento.DataSource = medicamento;
        }

        public void CargarDatosCosto(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM COSTO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CargarDatosCosto(dataGridView1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int idMedicmentoSeleccionado = (int)comboBoxMedicamento.SelectedValue;
            int idTicketSeleccionado = (int)comboBoxTicket.SelectedValue;
            costosGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4 }, dataGridView1, idMedicmentoSeleccionado, idTicketSeleccionado);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idCosto = Convert.ToInt32(filaSeleccionada.Cells["idcosto"].Value);
                string proveedor = Convert.ToString(filaSeleccionada.Cells["proveedor"].Value);
                string costoPedido = Convert.ToString(filaSeleccionada.Cells["costopedido"].Value);
                string costoVenta = Convert.ToString(filaSeleccionada.Cells["costoventa"].Value);

                textBox1.Text = idCosto.ToString();
                textBox2.Text = proveedor;
                textBox3.Text = costoPedido;
                textBox4.Text = costoVenta;

                btn_Guardar.Visible = true;
                label11.Visible = true;
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para modificar.");
            }
        }

        private void btn_Guardar_Click_1(object sender, EventArgs e)
        {
            int idMedicmentoSeleccionado = (int)comboBoxMedicamento.SelectedValue;
            int idTicketSeleccionado = (int)comboBoxTicket.SelectedValue;
            costosGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4 }, dataGridView1, idMedicmentoSeleccionado, idTicketSeleccionado);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click_1(object sender, EventArgs e)
        {
            BuscarEnBDCosto(txtBuscar, dataGridView1);
        }

        private void btn_Eliminar_Click_1(object sender, EventArgs e)
        {
            EliminarEnBDCosto(textBox1, textBox2, textBox3, textBox4, dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
        }
    }
}
