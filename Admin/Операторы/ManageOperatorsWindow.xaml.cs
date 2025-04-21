using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Операторы
{
    public partial class ManageOperatorsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageOperatorsWindow()
        {
            InitializeComponent();
            LoadOperators();
        }

        private void LoadOperators()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Операторы ORDER BY id";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    OperatorsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки операторов: {ex.Message}");
            }
        }

        private void AddOperator_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditOperatorWindow();
            if (editWindow.ShowDialog() == true)
                LoadOperators();
            LoadOperators();
        }

        private void EditOperator_Click(object sender, RoutedEventArgs e)
        {
            if (OperatorsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditOperatorWindow(
                    (int)row["id"],
                    row["фио"].ToString(),
                    row["должность"].ToString(),
                    row["контактный_телефон"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadOperators();
                LoadOperators();
            }
        }

        private void DeleteOperator_Click(object sender, RoutedEventArgs e)
        {
            if (OperatorsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить оператора?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Операторы WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadOperators();
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