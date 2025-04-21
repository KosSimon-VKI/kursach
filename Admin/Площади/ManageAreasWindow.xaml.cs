using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Площади
{
    public partial class ManageAreasWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageAreasWindow()
        {
            InitializeComponent();
            LoadAreas();
        }

        private void LoadAreas()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            п.id, 
                            п.проект_id,
                            п.название,
                            CONCAT(пр.id, ' - ', пр.название) AS Проект
                        FROM Площади п
                        LEFT JOIN Проекты пр ON п.проект_id = пр.id
                        ORDER BY п.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    AreasGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки площадей: {ex.Message}");
            }
        }

        private void AddArea_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditAreaWindow();
            if (editWindow.ShowDialog() == true)
                LoadAreas();
            LoadAreas();
        }
        private void EditArea_Click(object sender, RoutedEventArgs e)
        {
            if (AreasGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditAreaWindow(
                    (int)row["id"],
                    row["проект_id"] != DBNull.Value ? (int)row["проект_id"] : (int?)null,
                    row["название"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadAreas();
                LoadAreas();
            }
        }

        private void DeleteArea_Click(object sender, RoutedEventArgs e)
        {
            if (AreasGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить площадь?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Площади WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadAreas();
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
