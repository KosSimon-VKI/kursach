using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.ТипыПользователей
{
    public partial class ManageUserTypesWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageUserTypesWindow()
        {
            InitializeComponent();
            LoadUserTypes();
        }

        private void LoadUserTypes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM ТипыПользователей ORDER BY id";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    UserTypesGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов пользователей: {ex.Message}");
            }
        }

        private void AddUserType_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditUserTypeWindow();
            if (editWindow.ShowDialog() == true)
                LoadUserTypes();
            LoadUserTypes();
        }

        private void EditUserType_Click(object sender, RoutedEventArgs e)
        {
            if (UserTypesGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditUserTypeWindow(
                    (int)row["id"],
                    row["название"].ToString(),
                    row["описание"].ToString());

                if (editWindow.ShowDialog() == true)
                    LoadUserTypes();
                LoadUserTypes();
            }
        }

        private void DeleteUserType_Click(object sender, RoutedEventArgs e)
        {
            if (UserTypesGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить тип пользователя?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM ТипыПользователей WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadUserTypes();
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