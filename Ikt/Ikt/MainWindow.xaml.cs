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
            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            string connectionString = "Server=localhost;Database=ikt_sq;User=root;Password=;";
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                cmd = new MySqlCommand("SELECT * FROM contacts", connection);
                adapter = new MySqlDataAdapter(cmd);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
                dgContacts.ItemsSource = dataTable.DefaultView;

                dgContacts.MouseDoubleClick += DgContacts_MouseDoubleClick;
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

        private void DgContacts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgContacts.SelectedItem is DataRowView selectedRow)
            {
                string firstName = selectedRow["first_name"].ToString();
                string lastName = selectedRow["last_name"].ToString();
                string email = selectedRow["email"].ToString();
                string phone = selectedRow["phone"].ToString();

                var editContactWindow = new EditContactWindow(
                    firstName,
                    lastName,
                    email,
                    phone);

                if (editContactWindow.ShowDialog() == true)
                {
                    string firstName2 = editContactWindow.FirstName;
                    string lastName2 = editContactWindow.LastName;
                    string email2 = editContactWindow.Email;
                    string phone2 = editContactWindow.Phone;

                    string updateQuery = "UPDATE contacts SET first_name = @FirstName, last_name = @LastName, " +
                                         "email = @Email, phone = @Phone WHERE id = @Id";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);

                    updateCmd.Parameters.AddWithValue("@FirstName", firstName2);
                    updateCmd.Parameters.AddWithValue("@LastName", lastName2);
                    updateCmd.Parameters.AddWithValue("@Email", email2);
                    updateCmd.Parameters.AddWithValue("@Phone", phone2);
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
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addContactWindow = new AddContactWindow();
            addContactWindow.ShowDialog();

            if (addContactWindow.IsSaved)
            {
                string firstName = addContactWindow.FirstName;
                string lastName = addContactWindow.LastName;
                string email = addContactWindow.Email;
                string phone = addContactWindow.Phone;

                SaveNewContact(firstName, lastName, email, phone);
            }
        }

        private void SaveNewContact(string firstName, string lastName, string email, string phone)
        {
            
            string insertQuery = "INSERT INTO contacts (first_name, last_name, email, phone) " +
                                 "VALUES (@FirstName, @LastName, @Email, @Phone)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);

            insertCmd.Parameters.AddWithValue("@FirstName", firstName);
            insertCmd.Parameters.AddWithValue("@LastName", lastName);
            insertCmd.Parameters.AddWithValue("@Email", email);
            insertCmd.Parameters.AddWithValue("@Phone", phone);

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
                string firstName = selectedRow["first_name"].ToString();
                string lastName = selectedRow["last_name"].ToString(); ;
                string email = selectedRow["email"].ToString(); ;
                string phone = selectedRow["phone"].ToString(); ;

                var editContactWindow = new EditContactWindow(
                    firstName,
                    lastName,
                    email,
                    phone);


                if (editContactWindow.ShowDialog() == true)
                {
                    string firstName2 = editContactWindow.FirstName;
                    string lastName2 = editContactWindow.LastName;
                    string email2 = editContactWindow.Email;
                    string phone2 = editContactWindow.Phone;

                    string updateQuery = "UPDATE contacts SET first_name = @FirstName, last_name = @LastName, " +
                                         "email = @Email, phone = @Phone WHERE id = @Id";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);

                    updateCmd.Parameters.AddWithValue("@FirstName", firstName2);
                    updateCmd.Parameters.AddWithValue("@LastName", lastName2);
                    updateCmd.Parameters.AddWithValue("@Email", email2);
                    updateCmd.Parameters.AddWithValue("@Phone", phone2);
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
        }





        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgContacts.SelectedItem is DataRowView selectedRow)
            {
               
                var confirmWindow = new ConfirmDeleteWindow();
                confirmWindow.ShowDialog();

               
                if (confirmWindow.IsConfirmed)
                {
                    DeleteContact(Convert.ToInt32(selectedRow["id"]));
                }
            }
        }

        private void DeleteContact(int contactId)
        {
           
            string deleteQuery = "DELETE FROM contacts WHERE id = @Id";
            MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, connection);
            deleteCmd.Parameters.AddWithValue("@Id", contactId);

            try
            {
                connection.Open();
                deleteCmd.ExecuteNonQuery();
                dataTable.Clear();
                adapter.Fill(dataTable);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba az adatok törlésekor: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


    }
}
