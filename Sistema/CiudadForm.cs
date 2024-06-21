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
    public partial class CiudadForm : Form
    {
        FuncionesBasicas instanciaCiudad = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";
        public CiudadForm()
        {
            InitializeComponent();
            CargarDatosCiudad(dataGridView1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        public void ciudadGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idCiudad = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroCiudad(idCiudad))
                {
                    string consulta = "UPDATE CIUDAD SET nombre = @Nombre, estado = @Estado, habitantes = @Habitantes, superficie = @Superficie WHERE idciudad = @IdCiudad";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudad);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Estado", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Habitantes", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Superficie", textBoxes[4].Text);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                }
                else
                {
                    string consulta = "INSERT INTO CIUDAD (idciudad, nombre, estado, habitantes, superficie) VALUES (@IdCiudad, @Nombre, @Estado, @Habitantes, @Superficie)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdCiudad", textBoxes[0].Text);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Estado", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Habitantes", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Superficie", textBoxes[4].Text);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaCiudad.LimpiarCampos(textBoxes);
                CargarDatosCiudad(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }


        private bool ExisteRegistroCiudad(int idCiudad)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM CIUDAD WHERE idciudad = @IdCiudad";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdCiudad", idCiudad);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDCiudad(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM CIUDAD WHERE idciudad = @Valor";
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
        private void btn_Modificar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idCiudad = Convert.ToInt32(filaSeleccionada.Cells["idciudad"].Value);
                string nombre = Convert.ToString(filaSeleccionada.Cells["nombre"].Value);
                string estado = Convert.ToString(filaSeleccionada.Cells["estado"].Value);
                string habitantes = Convert.ToString(filaSeleccionada.Cells["habitantes"].Value);
                string superficie = Convert.ToString(filaSeleccionada.Cells["superficie"].Value);

                textBox5.Text = idCiudad.ToString();
                textBox1.Text = nombre;
                textBox2.Text = estado;
                textBox3.Text = habitantes.ToString();
                textBox4.Text = superficie.ToString();

                btn_Guardar.Visible = true;
                label11.Visible = true;
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para modificar.");
            }
        }
        public void EliminarEnBDCiudad(TextBox textBox5, TextBox textBox1, TextBox textBox2, TextBox textBox3, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idCiudad = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idciudad"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM CIUDAD WHERE idciudad = @IdCiudad";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudad);
                        comando.ExecuteNonQuery();
                        CargarDatosCiudad(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ciudadGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox5, textBox1, textBox2, textBox3, textBox4 }, dataGridView1);
        }
        private void btn_Buscar_Click(object sender, EventArgs e)
        {
            BuscarEnBDCiudad(txtBuscar, dataGridView1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CargarDatosCiudad(dataGridView1);
        }

        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            EliminarEnBDCiudad(textBox5, textBox1, textBox2, textBox3, dataGridView1);
        }

        
        public void CargarDatosCiudad(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM CIUDAD";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }

        private void btn_Guardar_Click(object sender, EventArgs e)
        {
            ciudadGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox5, textBox1, textBox2, textBox3, textBox4 }, dataGridView1);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 forma = new Form1();
            this.Hide();
            forma.ShowDialog();
        }

        private void CiudadForm_Load(object sender, EventArgs e)
        {

        }

        
    }

}
