using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema
{
    internal class conexionbd
    {
        //Cadena de Conexion
        string cadena = "data source = DESKTOP-C4J7T14; Initial Catalog = FARMACIAS; Integrated Security = True";

        public SqlConnection Conectarbd = new SqlConnection();

        //Constructor
        public conexionbd()
        {
            Conectarbd.ConnectionString = cadena;
        }

        //Metodo para abrir la conexion
        public void abrir()
        {
            try
            {
                Conectarbd.Open();
                
            }
            catch (Exception ex)
            {
               MessageBox.Show("error al abrir BD " + ex.Message);
            }
        }

        //Metodo para cerrar la conexion
        public void cerrar()
        {
            Conectarbd.Close();
        }
        public void EjecutarConsulta(string consulta)
        {
            SqlCommand comando = new SqlCommand(consulta, Conectarbd);
            comando.ExecuteNonQuery();
        }
        public SqlConnection obtenerconexion()
        {
            SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            return conexion;
        }

    }
}
