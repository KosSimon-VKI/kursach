using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Договоры
{
    public partial class ManageContractsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageContractsWindow()
        {
            InitializeComponent();
            LoadContracts();
        }

        private void LoadContracts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            д.id, 
                            д.заказчик_id,
                            д.номер_договора, 
                            д.дата_заключения, 
                            д.описание,
                            CONCAT(з.id, ' - ', з.название) AS Заказчик
                        FROM Договоры д
                        LEFT JOIN Заказчики з ON д.заказчик_id = з.id
                        ORDER BY д.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ContractsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки договоров: {ex.Message}");
            }
        }

        private void AddContract_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditContractWindow();
            if (editWindow.ShowDialog() == true)
                LoadContracts();
        }

        private void EditContract_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditContractWindow(
                    (int)row["id"],
                    (int)row["заказчик_id"],
                    row["номер_договора"].ToString(),
                    (DateTime)row["дата_заключения"],
                    row["описание"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadContracts();
            }
        }

        private void DeleteContract_Click(object sender, RoutedEventArgs e)
        {
            if (ContractsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить договор?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Договоры WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadContracts();
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