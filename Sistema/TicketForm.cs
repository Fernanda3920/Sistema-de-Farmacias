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
    public partial class TicketForm : Form
    {
        FuncionesBasicas instanciaTicket = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";

        public TicketForm()
        {
            InitializeComponent();
            CargarDatosTicket(dataGridView1);
            this.Load += TicketForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        public void ticketGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1, int idClienteSeleccionado)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idTicket = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroTicket(idTicket))
                {
                    string consulta = "UPDATE TICKET SET fecha = @Fecha, hora = @Hora, costounitario = @CostoUnitario, costototal = @CostoTotal, idcliente = @IdCliente WHERE idticket = @IdTicket";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdTicket", idTicket);
                        comando.Parameters.AddWithValue("@Fecha", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Hora", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CostoUnitario", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@CostoTotal", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@IdCliente", idClienteSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosTicket(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO TICKET (idticket, fecha, hora, costounitario, costototal, idcliente) VALUES (@IdTicket, @Fecha, @Hora, @CostoUnitario, @CostoTotal, @idCliente)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdTicket", idTicket);
                        comando.Parameters.AddWithValue("@Fecha", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Hora", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CostoUnitario", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@CostoTotal", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@IdCliente", idClienteSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaTicket.LimpiarCampos(textBoxes);
                CargarDatosTicket(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroTicket(int idTicket)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM TICKET WHERE idticket = @IdTicket";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdTicket", idTicket);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDTicket(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM TICKET WHERE idticket = @Valor";
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
        public void CargarDatosTicket(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM TICKET";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }
        public void EliminarEnBDTicket(DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idTicket = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idticket"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM TICKET WHERE idticket = @IdTicket";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdTicket", idTicket);
                        comando.ExecuteNonQuery();
                        CargarDatosTicket(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }

        private void TicketForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable clientes = instanciaFunciones.ObtenerClientes();

            comboBoxCliente.DisplayMember = "nombre"; // Define qué campo se mostrará en el ComboBox
            comboBoxCliente.ValueMember = "idcliente"; // Define qué campo se usará como valor seleccionado
            comboBoxCliente.DataSource = clientes; // Asigna el DataTable al ComboBox
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CargarDatosTicket(dataGridView1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int idClienteSeleccionado = (int)comboBoxCliente.SelectedValue;
            ticketGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5 }, dataGridView1, idClienteSeleccionado);
        }

        private void btn_Eliminar_Click_1(object sender, EventArgs e)
        {
            EliminarEnBDTicket(dataGridView1);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idTicket = Convert.ToInt32(filaSeleccionada.Cells["idticket"].Value);
                string fecha = Convert.ToString(filaSeleccionada.Cells["fecha"].Value);
                string hora = Convert.ToString(filaSeleccionada.Cells["hora"].Value);
                string costoUnitario = Convert.ToString(filaSeleccionada.Cells["costoUnitario"].Value);
                string costoTotal = Convert.ToString(filaSeleccionada.Cells["costoTotal"].Value);

                textBox1.Text = idTicket.ToString();
                textBox2.Text = fecha;
                textBox3.Text = hora;
                textBox4.Text = costoUnitario;
                textBox5.Text = costoTotal;


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
            int idClienteSeleccionado = (int)comboBoxCliente.SelectedValue;
            ticketGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5 }, dataGridView1, idClienteSeleccionado);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click_1(object sender, EventArgs e)
        {
            BuscarEnBDTicket(txtBuscar, dataGridView1);
        }
    }
}
