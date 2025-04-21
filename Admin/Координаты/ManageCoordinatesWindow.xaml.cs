using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Координаты
{
    public partial class ManageCoordinatesWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageCoordinatesWindow()
        {
            InitializeComponent();
            LoadCoordinates();
        }

        private void LoadCoordinates()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Координаты ORDER BY id";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    CoordinatesGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки координат: {ex.Message}");
            }
        }

        private void AddCoordinate_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditCoordinateWindow();
            if (editWindow.ShowDialog() == true)
                LoadCoordinates();
            LoadCoordinates();
        }

        private void EditCoordinate_Click(object sender, RoutedEventArgs e)
        {
            if (CoordinatesGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditCoordinateWindow(
                    (int)row["id"],
                    (decimal)row["широта"],
                    (decimal)row["долгота"]);

                if (editWindow.ShowDialog() == true)
                    LoadCoordinates();
                LoadCoordinates();
            }
        }

        private void DeleteCoordinate_Click(object sender, RoutedEventArgs e)
        {
            if (CoordinatesGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить координату?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Координаты WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCoordinates();
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