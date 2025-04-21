using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Проекты
{
    public partial class ManageProjectsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageProjectsWindow()
        {
            InitializeComponent();
            LoadProjects();
        }

        private void LoadProjects()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            п.id, 
                            п.договор_id,
                            п.название, 
                            п.описание,
                            CONCAT(д.id, ' - ', д.номер_договора) AS Договор
                        FROM Проекты п
                        LEFT JOIN Договоры д ON п.договор_id = д.id
                        ORDER BY п.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ProjectsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}");
            }
        }

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProjectWindow();
            if (editWindow.ShowDialog() == true)
                LoadProjects();
            LoadProjects();
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditProjectWindow(
                    (int)row["id"],
                    row["название"].ToString(),
                    row["описание"].ToString(),
                    row["договор_id"] != DBNull.Value ? (int)row["договор_id"] : (int?)null);

                if (editWindow.ShowDialog() == true)
                    LoadProjects();
                LoadProjects();
            }
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить проект?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Проекты WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadProjects();
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