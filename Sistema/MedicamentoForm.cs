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
    public partial class MedicamentoForm : Form
    {
        FuncionesBasicas instanciaMedicamento = new FuncionesBasicas();
        string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";
        public MedicamentoForm()
        {
            InitializeComponent();
            CargarDatosMedicamento(dataGridView1);
            this.Load += MedicamentoForm_Load; // Suscribe el evento Load al método Formulario_Load
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }
        public void medicamentoGuardarDatosDB(List<TextBox> textBoxes, DataGridView dataGridView1,int idFarmaciaSeleccionada)
        {
            conexionbd conexion = new conexionbd();
            try
            {
                conexion.abrir();

                int idMedicmento = int.Parse(textBoxes[0].Text);

                if (ExisteRegistroMedicamento(idMedicmento))
                {
                    string consulta = "UPDATE MEDICAMENTO SET costo = @Costo, stock = @Stock, decripcion = @Decripcion, contenido = @Contenido, elementoActivo = @ElementoActivo, idfarmacia = @IdFarmacia WHERE idmedicmento = @IdMedicmento";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdMedicmento", idMedicmento);
                        comando.Parameters.AddWithValue("@Costo", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Stock", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Decripcion", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Contenido", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@ElementoActivo", textBoxes[5].Text);
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmaciaSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos actualizados correctamente.");
                    CargarDatosMedicamento(dataGridView1);
                }
                else
                {
                    string consulta = "INSERT INTO MEDICAMENTO (idmedicmento, costo, stock, decripcion, contenido, elementoActivo, idfarmacia) VALUES (@IdMedicmento, @Costo, @Stock, @Decripcion, @Contenido, @ElementoActivo, @IdFarmacia)";
                    using (SqlCommand comando = new SqlCommand(consulta, conexion.obtenerconexion()))
                    {
                        comando.Parameters.AddWithValue("@IdMedicmento", textBoxes[0].Text);
                        comando.Parameters.AddWithValue("@Costo", textBoxes[1].Text);
                        comando.Parameters.AddWithValue("@Stock", textBoxes[2].Text);
                        comando.Parameters.AddWithValue("@Decripcion", textBoxes[3].Text);
                        comando.Parameters.AddWithValue("@Contenido", textBoxes[4].Text);
                        comando.Parameters.AddWithValue("@ElementoActivo", textBoxes[5].Text);
                        comando.Parameters.AddWithValue("@IdFarmacia", idFarmaciaSeleccionada);

                        comando.ExecuteNonQuery();
                    }
                    MessageBox.Show("Datos insertados correctamente.");
                }
                instanciaMedicamento.LimpiarCampos(textBoxes);
                CargarDatosMedicamento(dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar datos en la base de datos: " + ex.Message);
            }
        }
        private bool ExisteRegistroMedicamento(int idMedicmento)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM MEDICAMENTO WHERE idmedicmento = @IdMedicmento";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdMedicmento", idMedicmento);
                    int count = (int)comando.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public void BuscarEnBDMedicamento(TextBox txtBuscar, DataGridView dataGridView1)
        {
            if (int.TryParse(txtBuscar.Text, out int valorABuscar))
            {
                using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                {
                    string consulta = "SELECT * FROM MEDICAMENTO WHERE idmedicmento = @Valor";
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
        public void EliminarEnBDMedicamento(TextBox textBox1, TextBox textBox2, TextBox textBox3, TextBox textBox4, TextBox textBox5, TextBox textBox6, DataGridView dataGridView1)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idMedicmento = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idmedicmento"].Value);

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
                    {
                        conexion.Open();
                        string consulta = "DELETE FROM MEDICAMENTO WHERE idmedicmento = @IdMedicmento";
                        SqlCommand comando = new SqlCommand(consulta, conexion);
                        comando.Parameters.AddWithValue("@IdMedicmento", idMedicmento);
                        comando.ExecuteNonQuery();
                        CargarDatosMedicamento(dataGridView1);
                        MessageBox.Show("Registro eliminado correctamente.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor seleccione una fila para eliminar.");
            }
        }
        public void CargarDatosMedicamento(DataGridView dataGridView1)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT * FROM MEDICAMENTO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dataGridView1.DataSource = tabla;
            }
        }
        private void MedicamentoForm_Load(object sender, EventArgs e)
        {
            FuncionesBasicas instanciaFunciones = new FuncionesBasicas();
            DataTable farmacia = instanciaFunciones.ObtenerFarmacias();

            comboBoxFarmacias.DisplayMember = "nombre"; // Define qué campo se mostrará en el ComboBox
            comboBoxFarmacias.ValueMember = "idfarmacia"; // Define qué campo se usará como valor seleccionado
            comboBoxFarmacias.DataSource = farmacia; // Asigna el DataTable al ComboBox
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            CargarDatosMedicamento(dataGridView1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int idFarmaciaSeleccionada = (int)comboBoxFarmacias.SelectedValue;
            medicamentoGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 }, dataGridView1, idFarmaciaSeleccionada);
        }

        private void btn_Eliminar_Click_1(object sender, EventArgs e)
        {
            EliminarEnBDMedicamento(textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, dataGridView1);
        }

        private void btn_Modificar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow filaSeleccionada = dataGridView1.SelectedRows[0];
                int idMedicmento = Convert.ToInt32(filaSeleccionada.Cells["idmedicmento"].Value);
                string costo = Convert.ToString(filaSeleccionada.Cells["costo"].Value);
                string stock = Convert.ToString(filaSeleccionada.Cells["stock"].Value);
                string descripcion = Convert.ToString(filaSeleccionada.Cells["decripcion"].Value);
                string contenido = Convert.ToString(filaSeleccionada.Cells["contenido"].Value);
                string elementoActivo = Convert.ToString(filaSeleccionada.Cells["elementoActivo"].Value);


                textBox1.Text = idMedicmento.ToString();
                textBox2.Text = costo;
                textBox3.Text = stock;
                textBox4.Text = descripcion;
                textBox5.Text = contenido;
                textBox6.Text = elementoActivo;

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
            medicamentoGuardarDatosDB(new System.Collections.Generic.List<TextBox> { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6 }, dataGridView1, idFarmaciaSeleccionada);
            btn_Guardar.Visible = false;
            label11.Visible = false;
        }

        private void btn_Buscar_Click_1(object sender, EventArgs e)
        {
            BuscarEnBDMedicamento(txtBuscar, dataGridView1);
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
    }
}
