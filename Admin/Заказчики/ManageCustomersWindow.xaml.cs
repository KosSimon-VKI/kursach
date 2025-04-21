using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Заказчики
{
    public partial class ManageCustomersWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageCustomersWindow()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Заказчики ORDER BY id";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    CustomersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказчиков: {ex.Message}");
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditCustomerWindow();
            if (editWindow.ShowDialog() == true)
                LoadCustomers();
            LoadCustomers();
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditCustomerWindow(
                    (int)row["id"],
                    row["название"].ToString(),
                    row["адрес"].ToString(),
                    row["контактное_лицо"].ToString(),
                    row["телефон"].ToString(),
                    row["email"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadCustomers();
            }
            LoadCustomers();
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить заказчика?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Заказчики WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new AdminMenuWindow().Show();
            this.Close();
        }
    }
}