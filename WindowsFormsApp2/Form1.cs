using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        private List<Button> list = new List<Button>();
        private string conStr = ConfigurationManager.ConnectionStrings["restoApp"].ToString();


        public Form1()
        {
            InitializeComponent();
            CenterToScreen();
            llenarLista();
            marcarMesas();
        }

        public void llenarLista()
        {
            list.Add(mesa1);
            list.Add(mesa2);
            list.Add(mesa3);
            list.Add(mesa4);
            list.Add(mesa5);
            list.Add(mesa6);
            list.Add(mesa7);
            list.Add(mesa8);
            list.Add(mesa9);
            list.Add(mesa10);
            list.Add(mesa11);
            list.Add(mesa12);
            
        }

        public void marcarMesas()
        {
            foreach(Button btn in list)
            {
                checkEstado(btn);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String meseroNombre = txtMesero.Text;
            if(meseroNombre.Equals("No tiene"))
            {
                MessageBox.Show("No tiene mesero asignado!","Error");
                return;
            }
            int id;
            double valor;
            try
            {
                id= Convert.ToInt16(txtMesa.Text);
                //CultureInfo culture = new CultureInfo("ar");
                valor = double.Parse(Interaction.InputBox("Ingrese valor a sumar"), NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
            }
            catch(Exception a)
            {
                MessageBox.Show( "El valor ingresado no es valido", "Error!");
                return;
            }
            sumarSubtotal(id,valor);
        }

        public void sumarSubtotal(int id,double valor)
        {
            string query = "Select subtotal From mesa where id=" + id;
            double aux=0;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        aux =(double)(reader["subtotal"]);
                        
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            
            aux =aux+valor;
            String aux4 = "Update mesa set subtotal=" + aux + " where id=" + id;
            String query2 = aux4.Replace(",",".");
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query2, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        MessageBox.Show("El valor fue sumado","Exito!" );                       
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
           
            list[id-1].PerformClick();
            
        }

        public void mostrarData(Button b)
        {
            String[] aux = b.Text.Split(' ');
            String choice = aux[1];
            cargarData(choice);
        }

        private void checkEstado(Button b)
        {
            String[] aux = b.Text.Split(' ');
            String choice = aux[1];
            int id;
            try
            {
                id = Convert.ToInt16(choice);
            }
            catch (Exception e)
            {
                return;
            }
            string query = "Select mesero From mesa where id=" + id;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String cont = (String)(reader["mesero"]);
                        if(cont.Equals("No tiene"))
                        {
                             b.BackColor = Color.FromArgb(0, 255, 0);
                        }
                        else
                        {
                            b.BackColor = Color.FromArgb(255, 0, 0);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        private void cargarData(String choice)
        {
            
            int id;
            try
            {
                id = Convert.ToInt16(choice);
            }
            catch (Exception e)
            {
                MessageBox.Show( "El valor ingresado no es valido", "Error!");
                return;
            }
            string query = "Select mesero,subtotal From mesa where id=" + id;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String cont = (String)(reader["mesero"]+" "+(reader["subtotal"]));
                        if (cont == null || cont.Equals("") || cont.Equals(" "))
                        {
                            MessageBox.Show("No existe la entrada!");
                        }
                        else
                        {
                            txtMesero.Text = (String)(reader["mesero"]);
                            txtMesa.Text = (String)choice;
                            txtSubtotal.Text =Convert.ToString(reader["subtotal"]);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

        }

        public void cerrarMesa()
        {
            int idMesa;
            double subtotal = 0;
            string mesero = "";
            try
            {
                idMesa = Convert.ToInt16(txtMesa.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show("Primero selecciona una mesa!", "Error!");
                return;
            }
            Boolean disponible = false;
            String query = "Select * From mesa where id=" + idMesa;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String cont = (String)(reader["mesero"]);
                        if (!cont.Equals("No tiene"))
                        {
                            disponible = true;
                            subtotal = (double)(reader["subtotal"]);
                            mesero = (string)(reader["mesero"]);
                        }
                        else
                        {
                            disponible = false;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (!disponible)
            {
                MessageBox.Show("Esta mesa no esta ocupada!", "Error!");
                return;
            }

            MessageBox.Show("El total de la mesa es: " + subtotal+"\nEl mesero fue :"+mesero+"\nLa propina esperada es: "+subtotal*0.1,"Cuenta");
            vaciarMesa(idMesa);
            marcarMesas();
            list[idMesa - 1].PerformClick();
        }

        public void vaciarMesa(int id)
        {
            String query = "update mesa set mesero='No tiene',subtotal=0 where id=" + id;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {

                }
                finally
                {
                    reader.Close();
                }
            }
        }

        public void tomarMesa()
        {
            int idMesa;
            try
            {
                idMesa = Convert.ToInt16(txtMesa.Text);
            }catch(Exception e)
            {
                MessageBox.Show( "Primero selecciona una mesa!", "Error!");
                return;
            }
            Boolean disponible = false ;
            String query = "Select mesero From mesa where id=" + idMesa;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        String cont = (String)(reader["mesero"]);
                        if (cont.Equals("No tiene"))
                        {
                            disponible = true;
                        }
                        else
                        {
                            disponible = false;
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (!disponible)
            {
                MessageBox.Show( "Esta mesa esta ocupada!", "Error!");
            }
            else
            {
                String choice = Interaction.InputBox("Ingrese el nombre del mesero");
                if(choice != null && choice !="" && choice !=" ")
                {
                    String query2 = "UPDATE mesa SET mesero = '"+choice +"' WHERE id=" + idMesa;
                    using (SqlConnection connection = new SqlConnection(conStr))
                    {
                        SqlCommand command = new SqlCommand(query2, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        try
                        {
                            while (reader.Read())
                            {
                                String cont = (String)(reader["mesero"]);
                                if (cont.Equals(choice))
                                {
                                    MessageBox.Show( "Mesa Actualizada","Exito!");
                                    
                                }
                                else
                                {
                                    MessageBox.Show("Ha ocurrido un error!", "Error!");
                                }
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }
            marcarMesas();
            list[idMesa - 1].PerformClick();
        }

        private void mesa1_Click(object sender, EventArgs e)
        {
            mostrarData(mesa1);
        }

        private void mesa2_Click(object sender, EventArgs e)
        {
            mostrarData(mesa2);
        }

        private void mesa3_Click(object sender, EventArgs e)
        {
            mostrarData(mesa3);
        }

        private void mesa4_Click(object sender, EventArgs e)
        {
            mostrarData(mesa4);
        }

        private void mesa5_Click(object sender, EventArgs e)
        {
            mostrarData(mesa5);
        }

        private void mesa6_Click(object sender, EventArgs e)
        {
            mostrarData(mesa6);
        }

        private void mesa7_Click(object sender, EventArgs e)
        {
            mostrarData(mesa7);
        }

        private void mesa8_Click(object sender, EventArgs e)
        {
            mostrarData(mesa8);
        }

        private void mesa9_Click(object sender, EventArgs e)
        {
            mostrarData(mesa9);
        }

        private void mesa10_Click(object sender, EventArgs e)
        {
            mostrarData(mesa10);
        }

        private void mesa11_Click(object sender, EventArgs e)
        {
            mostrarData(mesa11);
        }

        private void mesa12_Click(object sender, EventArgs e)
        {
            mostrarData(mesa12);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tomarMesa();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cerrarMesa();
        }
    }
}

