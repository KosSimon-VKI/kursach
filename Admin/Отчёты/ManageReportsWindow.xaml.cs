using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Отчеты
{
    public partial class ManageReportsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageReportsWindow()
        {
            InitializeComponent();
            LoadReports();
        }

        private void LoadReports()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            о.id, 
                            о.измерения_id,
                            о.проект_id,
                            о.дата_создания, 
                            о.описание,
                            о.графики,
                            о.файл_отчета,
                            CONCAT(и.id, ' - ', и.тип_измерения) AS Измерение,
                            CONCAT(п.id, ' - ', п.название) AS Проект
                        FROM Отчёт_об_измерениях о
                        LEFT JOIN Измерения и ON о.измерения_id = и.id
                        LEFT JOIN Проекты п ON о.проект_id = п.id
                        ORDER BY о.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ReportsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки отчетов: {ex.Message}");
            }
        }

        private void AddReport_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditReportWindow();
            if (editWindow.ShowDialog() == true)
                LoadReports();
            LoadReports();
        }

        private void EditReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditReportWindow(
                    (int)row["id"],
                    (int)row["измерения_id"],
                    (int)row["проект_id"],
                    row["дата_создания"] != DBNull.Value ? (DateTime)row["дата_создания"] : DateTime.Now,
                    row["описание"].ToString(),
                    row["графики"].ToString(),
                    row["файл_отчета"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadReports();
                LoadReports();
            }
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить отчет?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Отчёт_об_измерениях WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadReports();
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