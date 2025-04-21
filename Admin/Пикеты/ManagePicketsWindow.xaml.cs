using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Пикеты
{
    public partial class ManagePicketsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManagePicketsWindow()
        {
            InitializeComponent();
            LoadPickets();
        }

        private void LoadPickets()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            п.id, 
                            п.профиль_id,
                            п.номер, 
                            п.координата_id,
                            п.высота,
                            CONCAT(пр.id, ' - ', пр.название) AS Профиль,
                            CONCAT(к.id, ' - ', к.широта, ', ', к.долгота) AS Координата
                        FROM Пикеты п
                        LEFT JOIN Профили пр ON п.профиль_id = пр.id
                        LEFT JOIN Координаты к ON п.координата_id = к.id
                        ORDER BY п.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    PicketsGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пикетов: {ex.Message}");
            }
        }

        private void AddPicket_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditPicketWindow();
            if (editWindow.ShowDialog() == true)
                LoadPickets();
            LoadPickets();
        }

        private void EditPicket_Click(object sender, RoutedEventArgs e)
        {
            if (PicketsGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditPicketWindow(
                    (int)row["id"],
                    row["профиль_id"] != DBNull.Value ? (int)row["профиль_id"] : (int?)null,
                    (int)row["номер"],
                    (int)row["координата_id"],
                    row["высота"] != DBNull.Value ? (double?)Convert.ToDouble(row["высота"]) : null);

                if (editWindow.ShowDialog() == true)
                    LoadPickets();
                LoadPickets();
            }
        }

        private void DeletePicket_Click(object sender, RoutedEventArgs e)
        {
            if (PicketsGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить пикет?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Пикеты WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadPickets();
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