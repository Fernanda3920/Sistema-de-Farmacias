using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Sistema
{
    internal class FuncionesBasicas
    {
        private string cadenaDeCarga = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";


        public void LimpiarCampos(List<TextBox> textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Text = "";
            }
        }
        public string ObtenerEstadoDeCiudad(int idCiudad)
        {
            string estado = "";
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                conexion.Open();
                string consulta = "SELECT estado FROM CIUDADES WHERE idciudad = @IdCiudad";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdCiudad", idCiudad);
                    estado = (string)comando.ExecuteScalar();
                }
            }
            return estado;
        }

        public DataTable ObtenerCiudades()
        {
            DataTable ciudades = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idciudad, nombre FROM CIUDAD";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(ciudades);
            }
            return ciudades;
        }
        public DataTable ObtenerPropietarios()
        {
            DataTable propietarios = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idpropietario, nombre FROM PROPIETARIO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(propietarios);
            }
            return propietarios;
        }
        public DataTable ObtenerFarmacias()
        {
            DataTable farmacia = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idfarmacia, nombre FROM FARMACIA";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(farmacia);
            }
            return farmacia;
        }
        public DataTable ObtenerClientes()
        {
            DataTable clientes = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idcliente, nombre FROM CLIENTE";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(clientes);
            }
            return clientes;
        }
        public DataTable ObtenerMedicamentos()
        {
            DataTable medicamentos= new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idmedicmento FROM MEDICAMENTO";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(medicamentos);
            }
            return medicamentos;
        }
        public DataTable ObtenerTickets()
        {
            DataTable ticket = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaDeCarga))
            {
                string consulta = "SELECT idticket FROM TICKET";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                adaptador.Fill(ticket);
            }
            return ticket;
        }
    }
}


