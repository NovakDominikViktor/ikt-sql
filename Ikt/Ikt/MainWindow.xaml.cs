using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Ikt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private MySqlDataAdapter adapter;
        private DataTable dataTable;

        public MainWindow()
        {
            InitializeComponent();

            // Kapcsolódás az adatbázishoz
            string connectionString = "Server=192.168.50.98;Database=ikt_sq;User=root;Password=password;";
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                cmd = new MySqlCommand("SELECT * FROM contacts", connection);
                adapter = new MySqlDataAdapter(cmd);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgContacts.ItemsSource = dataTable.DefaultView;

                // Eseménykezelő hozzáadása a DataGrid-hez a kattintásra
                dgContacts.MouseLeftButtonUp += DgContacts_MouseLeftButtonUp;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba a kapcsolódás során: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void DgContacts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dgContacts.SelectedItem is DataRowView selectedRow)
            {
                // A kiválasztott sor adatainak beállítása a TextBox-okban
                txtFirstName.Text = selectedRow["first_name"].ToString();
                txtLastName.Text = selectedRow["last_name"].ToString();
                txtEmail.Text = selectedRow["email"].ToString();
                txtPhone.Text = selectedRow["phone"].ToString();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string insertQuery = "INSERT INTO contacts (first_name, last_name, email, phone) " +
                                 "VALUES (@FirstName, @LastName, @Email, @Phone)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);

            insertCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
            insertCmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
            insertCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
            insertCmd.Parameters.AddWithValue("@Phone", txtPhone.Text);

            try
            {
                connection.Open();
                insertCmd.ExecuteNonQuery();
                dataTable.Clear();
                adapter.Fill(dataTable);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba az adatok hozzáadásakor: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }



        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgContacts.SelectedItem is DataRowView selectedRow)
            {
                string updateQuery = "UPDATE contacts SET first_name = @FirstName, last_name = @LastName, " +
                                     "email = @Email, phone = @Phone WHERE id = @Id";
                MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);

                updateCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                updateCmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                updateCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                updateCmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                updateCmd.Parameters.AddWithValue("@Id", selectedRow["id"]);

                try
                {
                    connection.Open();
                    updateCmd.ExecuteNonQuery();
                    dataTable.Clear();
                    adapter.Fill(dataTable);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Hiba az adatok frissítésekor: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

    }

}