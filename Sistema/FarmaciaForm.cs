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
    public partial class FarmaciaForm : Form
    {
        FuncionesBasicas instanciaFarmacia = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";
        public FarmaciaForm()
        {
            InitializeComponent();
            CargarDatosFarmacia(dataGridView1);
            this.Load += FarmaciaForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        public void farmaciaGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1, int idCiudadSeleccionada, int idPropietarioSeleccionado)
        {
            conexionbd conexion = new conexionbd();
                    // Obtener el estado de la ciudad seleccionada
       // string estadoCiudad = instanciaFarmacia.ObtenerEstadoDeCiudad((int)comboBoxCiudades.SelectedValue);

        // Asignar el estado al TextBox correspondiente
       // textBox7.Text = estadoCiudad;
            try
            {
                conexion.abrir();

                int idFarmacia = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroFarmacia(idFarmacia))
                {
                    string consulta = "UPDATE FARMACIA SET nombre = @Nombre, telefono = @Telefono, calle = @Calle, colonia = @Colonia, cp = @CP, estado = @Estado, idciudad = @IdCiudad, idpropietario = @IdPropietario WHERE idfarmacia = @Idfarmacia";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmacia);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Telefono", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Calle", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Colonia", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@CP", textBoxes[5].Text);
                        comando.Parameters.AddWithValue("@Estado", textBoxes[6].Text);
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudadSeleccionada);
                        comando.Parameters.AddWithValue("@IdPropietario", idPropietarioSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosFarmacia(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO FARMACIA (idfarmacia, nombre, telefono, calle, colonia, cp, estado, idciudad, idpropietario) VALUES (@IdFarmacia, @Nombre, @Telefono, @Calle, @Colonia, @CP, @Estado, @idciudad, @idpropietario)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdFarmacia", textBoxes[0].Text);
                        comando.Parameters.AddWithValue("@Nombre", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Telefono", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Calle", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Colonia", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@CP", textBoxes[5].Text);
                        comando.Parameters.AddWithValue("@Estado", textBoxes[6].Text);
                        comando.Parameters.AddWithValue("@IdCiudad", idCiudadSeleccionada);
                        comando.Parameters.AddWithValue("@IdPropietario", idPropietarioSeleccionado);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaFarmacia.LimpiarCampos(textBoxes);
                CargarDatosFarmacia(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroFarmacia(int idFarmacia)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM FARMACIA WHERE idfarmacia = @IdFarmacia";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdFarmacia", idFarmacia);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        //private void comboBoxCiudades_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    // Obtener el estado de la ciudad seleccionada
        //    string estadoCiudad = instanciaFarmacia.ObtenerEstadoDeCiudad((int)comboBoxCiudades.SelectedValue);

        //    // Asignar el estado al TextBox correspondiente
        //    textBox7.Text = estadoCiudad;
        //}

        public void BuscarEnBDFarmacia(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM FARMACIA WHERE idfarmacia = @Valor";
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
        //private void comboBoxCiudades_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //// Obtener el estado de la ciudad seleccionada
        //string estadoCiudad = instanciaFarmacia.ObtenerEstadoDeCiudad((int)comboBoxCiudades.SelectedValue);

        //// Asignar el estado al TextBox correspondiente
        //textBox7.Text = estadoCiudad;
        //}
        public void EliminarEnBDFarmacia(TextBox textBox1, TextBox textBox2, TextBox textBox3, TextBox textBox4, TextBox textBox5, TextBox textBox6, TextBox textBox7, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idFarmacia = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idfarmacia"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM FARMACIA WHERE idfarmacia = @IdFarmacia";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmacia);
                        comando.ExecuteNonQuery();
                        CargarDatosFarmacia(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }


        private void FarmaciaForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable ciudades = instanciaFunciones.ObtenerCiudades();
            DataTable propietarios = instanciaFunciones.ObtenerPropietarios();

            // Crear una nueva fila para representar "Ninguno"
            DataRow newRow = propietarios.NewRow();
            newRow["idpropietario"] = DBNull.Value;
            newRow["nombre"] = "Ninguno";
            propietarios.Rows.InsertAt(newRow, 0); // Insertar al inicio de la tabla
                                                   // Crear una nueva fila para representar "Ninguno"
            DataRow newRow1 = ciudades.NewRow();
            newRow1["idciudad"] = DBNull.Value;
            newRow1["nombre"] = "Ninguna";
            ciudades.Rows.InsertAt(newRow1, 0); // Insertar al inicio de la tabla

            comboBoxCiudades.DisplayMember = "nombre";
            comboBoxCiudades.ValueMember = "idciudad";
            comboBoxCiudades.DataSource = ciudades;

            comboBoxPropietarios.DisplayMember = "nombre";
            comboBoxPropietarios.ValueMember = "idpropietario";
            comboBoxPropietarios.DataSource = propietarios;
        }

        public void CargarDatosFarmacia(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM FARMACIA";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
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
            CargarDatosFarmacia(dataGridView1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            

            // Verificar si se seleccionó "Ninguno" en el ComboBox de propietarios
            int idPropietarioSeleccionado;
            int idCiudadSeleccionada;
            if (comboBoxPropietarios.SelectedIndex == 0) // El índice 0 corresponde a "Ninguno"
            {
                idPropietarioSeleccionado = 0;// Asignar un valor especial para representar "Ninguno"
                idCiudadSeleccionada = 0;
            }
            else
            {
                idPropietarioSeleccionado = (int)comboBoxPropietarios.SelectedValue;
                idCiudadSeleccionada = (int)comboBoxCiudades.SelectedValue;
            }

            farmaciaGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7 }, dataGridView1, idCiudadSeleccionada, idPropietarioSeleccionado);
          
        }

        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            EliminarEnBDFarmacia(textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, dataGridView1);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idFarmacia = Convert.ToInt32(filaSeleccionada.Cells["idfarmacia"].Value);
                string nombre = Convert.ToString(filaSeleccionada.Cells["nombre"].Value);
                string telefono = Convert.ToString(filaSeleccionada.Cells["telefono"].Value);
                string calle = Convert.ToString(filaSeleccionada.Cells["calle"].Value);
                string colonia = Convert.ToString(filaSeleccionada.Cells["colonia"].Value);
                string cp = Convert.ToString(filaSeleccionada.Cells["cp"].Value);
                string estado = Convert.ToString(filaSeleccionada.Cells["estado"].Value);


                textBox1.Text = idFarmacia.ToString();
                textBox2.Text = nombre;
                textBox3.Text = telefono;
                textBox4.Text = calle;
                textBox5.Text = colonia;
                textBox6.Text = cp;
                textBox7.Text = estado;

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
            int idPropietarioSeleccionado = (int)comboBoxPropietarios.SelectedValue;
            farmaciaGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7 }, dataGridView1, idCiudadSeleccionada, idPropietarioSeleccionado);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click(object sender, EventArgs e)
        {
            BuscarEnBDFarmacia(txtBuscar, dataGridView1);
        }
    }

}
