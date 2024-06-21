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
    public partial class ClienteForm : Form
    {
        FuncionesBasicas instanciaCliente = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";

        public ClienteForm()
        {
            InitializeComponent();
            CargarDatosCliente(dataGridView1);
            this.Load += ClienteForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
       
        public void clienteGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1, int idFarmaciaSeleccionada)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idCliente = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroCliente(idCliente))
                {
                    string consulta = "UPDATE CLIENTE SET nombre = @Nombre, telefono = @Telefono, RFC = @RFC, idfarmacia = @IdFarmacia WHERE idcliente = @IdCliente";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCliente", idCliente);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Telefono", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@RFC", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmaciaSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosCliente(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO CLIENTE (idcliente, nombre, telefono, RFC, idfarmacia) VALUES (@IdCliente, @Nombre, @Telefono, @RFC, @IdFarmacia)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCliente", textBoxes[0].Text);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Telefono", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@RFC", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmaciaSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaCliente.LimpiarCampos(textBoxes);
                CargarDatosCliente(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroCliente(int idCliente)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM CLIENTE WHERE idcliente = @IdCliente";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdCliente", idCliente);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDCliente(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM CLIENTE WHERE idcliente = @Valor";
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

        private void ClienteForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable farmacias = instanciaFunciones.ObtenerFarmacias();

            comboBoxFarmacias.DisplayMember = "nombre"; // Define qué campo se mostrará en el ComboBox
            comboBoxFarmacias.ValueMember = "idfarmacia"; // Define qué campo se usará como valor seleccionado
            comboBoxFarmacias.DataSource = farmacias; // Asigna el DataTable al ComboBox
        }
        public void EliminarEnBDCliente(TextBox textBox5, TextBox textBox1, TextBox textBox2, TextBox textBox3, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idCliente = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idcliente"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM CLIENTE WHERE idcliente = @IdCliente";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdCliente", idCliente);
                        comando.ExecuteNonQuery();
                        CargarDatosCliente(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }
        public void CargarDatosCliente(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM CLIENTE";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }
      
        private void button1_Click_1(object sender, EventArgs e)
        {
            int idFarmaciaSeleccionada = (int)comboBoxFarmacias.SelectedValue;
            clienteGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4 }, dataGridView1, idFarmaciaSeleccionada);
        }

        private void btn_Eliminar_Click_1(object sender, EventArgs e)
        {
            EliminarEnBDCliente(textBox1, textBox2, textBox3, textBox4, dataGridView1);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idCliente = Convert.ToInt32(filaSeleccionada.Cells["idcliente"].Value);
                string nombre = Convert.ToString(filaSeleccionada.Cells["nombre"].Value);
                string telefono = Convert.ToString(filaSeleccionada.Cells["telefono"].Value);
                string RFC = Convert.ToString(filaSeleccionada.Cells["RFC"].Value);

                textBox1.Text = idCliente.ToString();
                textBox2.Text = nombre;
                textBox3.Text = telefono;
                textBox4.Text = RFC;

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
            int idFarmaciaSeleccionada = (int)comboBoxFarmacias.SelectedValue;
            clienteGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4 }, dataGridView1, idFarmaciaSeleccionada);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click_1(object sender, EventArgs e)
        {
            BuscarEnBDCliente(txtBuscar, dataGridView1);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CargarDatosCliente(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
