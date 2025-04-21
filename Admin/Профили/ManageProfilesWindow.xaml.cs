using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Профили
{
    public partial class ManageProfilesWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageProfilesWindow()
        {
            InitializeComponent();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            п.id, 
                            п.площадь_id,
                            п.название, 
                            п.тип,
                            CASE WHEN пл.id IS NULL THEN 'Не назначена' 
                                 ELSE CONCAT(пл.id, ' - ', пл.название) 
                            END AS Площадь
                        FROM Профили п
                        LEFT JOIN Площади пл ON п.площадь_id = пл.id
                        ORDER BY п.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ProfilesGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профилей: {ex.Message}");
            }
        }

        private void AddProfile_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditProfileWindow();
            if (editWindow.ShowDialog() == true)
                LoadProfiles();
            LoadProfiles();
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditProfileWindow(
                    (int)row["id"],
                    row["площадь_id"] != DBNull.Value ? (int)row["площадь_id"] : (int?)null,
                    row["название"].ToString(),
                    row["тип"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadProfiles();
                LoadProfiles();
            }
        }

        private void DeleteProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfilesGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить профиль?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Профили WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadProfiles();
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