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
    public partial class PropietarioForm : Form
    {
        FuncionesBasicas instanciaPropietario = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";
        public PropietarioForm()
        {
            InitializeComponent();
            CargarDatosPropietario(dataGridView1);
            this.Load += PropietarioForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        public void propietarioGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1, int idCiudadSeleccionada)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idPropietario = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroPropietario(idPropietario))
                {
                    string consulta = "UPDATE PROPIETARIO SET nombre = @Nombre, calle = @Calle, cp = @CP, idciudad = @IdCiudad WHERE idpropietario = @IdPropietario";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdPropietario", idPropietario);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Calle", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CP", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudadSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosPropietario(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO PROPIETARIO (idpropietario, nombre, calle, cp, idciudad) VALUES (@IdPropietario, @Nombre, @Calle, @CP, @IdCiudad)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdPropietario", textBoxes[0].Text);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Calle", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@CP", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudadSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaPropietario.LimpiarCampos(textBoxes);
               CargarDatosPropietario(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroPropietario(int idPropietario)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM PROPIETARIO WHERE idpropietario = @IdPropietario";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdPropietario", idPropietario);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDPropietario(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM PROPIETARIO WHERE idpropietario = @Valor";
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

        public void EliminarEnBDPropietario(TextBox textBox5, TextBox textBox1, TextBox textBox2, TextBox textBox3, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idPropietario = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idpropietario"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM PROPIETARIO WHERE idpropietario = @IdPropietario";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdPropietario", idPropietario);
                        comando.ExecuteNonQuery();
                        CargarDatosPropietario(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }


        private void PropietarioForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable ciudades = instanciaFunciones.ObtenerCiudades();

            comboBoxCiudades.DisplayMember = "nombre"; // Define qué campo se mostrará en el ComboBox
            comboBoxCiudades.ValueMember = "idciudad"; // Define qué campo se usará como valor seleccionado
            comboBoxCiudades.DataSource = ciudades; // Asigna el DataTable al ComboBox
        }

        public void CargarDatosPropietario(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM PROPIETARIO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int idCiudadSeleccionada = (int)comboBoxCiudades.SelectedValue;
            propietarioGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox5, textBox1, textBox2, textBox3 }, dataGridView1, idCiudadSeleccionada);
        }

        private void btn_Eliminar_Click_1(object sender, EventArgs e)
        {
            EliminarEnBDPropietario(textBox5, textBox1, textBox2, textBox3, dataGridView1);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idPropietario = Convert.ToInt32(filaSeleccionada.Cells["idpropietario"].Value);
                string nombre = Convert.ToString(filaSeleccionada.Cells["nombre"].Value);
                string calle = Convert.ToString(filaSeleccionada.Cells["calle"].Value);
                string cp = Convert.ToString(filaSeleccionada.Cells["cp"].Value);

                textBox5.Text = idPropietario.ToString();
                textBox1.Text = nombre;
                textBox2.Text = calle;
                textBox3.Text = cp;

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
            int idCiudadSeleccionada = (int)comboBoxCiudades.SelectedValue;
            propietarioGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox5, textBox1, textBox2, textBox3 }, dataGridView1, idCiudadSeleccionada);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click_1(object sender, EventArgs e)
        {
            BuscarEnBDPropietario(txtBuscar, dataGridView1);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            CargarDatosPropietario(dataGridView1);
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

