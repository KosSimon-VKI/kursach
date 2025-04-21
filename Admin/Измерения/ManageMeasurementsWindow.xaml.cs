using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Измерения
{
    public partial class ManageMeasurementsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageMeasurementsWindow()
        {
            InitializeComponent();
            LoadMeasurements();
        }

        private void LoadMeasurements()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            и.id, 
                            и.пикет_id,
                            и.оператор_id,
                            и.дата_время, 
                            и.тип_измерения,
                            и.результат,
                            и.примечания,
                            CONCAT(п.id, ' - ', п.номер) AS Пикет,
                            о.фио AS Оператор
                        FROM Измерения и
                        LEFT JOIN Пикеты п ON и.пикет_id = п.id
                        LEFT JOIN Операторы о ON и.оператор_id = о.id
                        ORDER BY и.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    MeasurementsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки измерений: {ex.Message}");
            }
        }

        private void AddMeasurement_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditMeasurementWindow();
            if (editWindow.ShowDialog() == true)
                LoadMeasurements();
            LoadMeasurements();
        }

        private void EditMeasurement_Click(object sender, RoutedEventArgs e)
        {
            if (MeasurementsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditMeasurementWindow(
                    (int)row["id"],
                    row["пикет_id"] != DBNull.Value ? (int)row["пикет_id"] : (int?)null,
                    row["оператор_id"] != DBNull.Value ? (int)row["оператор_id"] : (int?)null,
                    row["дата_время"] != DBNull.Value ? (DateTime)row["дата_время"] : DateTime.Now,
                    row["тип_измерения"].ToString(),
                    row["результат"] != DBNull.Value ? (double?)Convert.ToDouble(row["результат"]) : null,
                    row["примечания"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadMeasurements();
                LoadMeasurements();
            }
        }

        private void DeleteMeasurement_Click(object sender, RoutedEventArgs e)
        {
            if (MeasurementsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить измерение?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Измерения WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadMeasurements();
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