using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Пользователи
{
    public partial class ManageUsersWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageUsersWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            п.id, 
                            п.логин,
                            п.фио, 
                            п.email,
                            п.тип_id,
                            п.проект_id,
                            т.название AS Тип,
                            CASE WHEN пр.id IS NULL THEN 'Не назначен' 
                                 ELSE CONCAT(пр.id, ' - ', пр.название) 
                            END AS Проект
                        FROM Пользователи п
                        LEFT JOIN ТипыПользователей т ON п.тип_id = т.id
                        LEFT JOIN Проекты пр ON п.проект_id = пр.id
                        ORDER BY п.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    UsersGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditUserWindow();
            if (editWindow.ShowDialog() == true)
                LoadUsers();
            LoadUsers();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditUserWindow(
                    (int)row["id"],
                    row["логин"].ToString(),
                    row["фио"].ToString(),
                    row["email"].ToString(),
                    (int)row["тип_id"],
                    row["проект_id"] != DBNull.Value ? (int)row["проект_id"] : (int?)null);

                if (editWindow.ShowDialog() == true)
                    LoadUsers();
                LoadUsers();
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Пользователи WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadUsers();
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