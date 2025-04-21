using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Пользователи
{
    public partial class EditUserWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? _userId;
        public bool IsSaved { get; private set; } = false;

        public string Title => _userId.HasValue ? "Редактирование пользователя" : "Добавление пользователя";

        public EditUserWindow()
        {
            InitializeComponent();
            LoadUserTypes();
            LoadProjects();
        }

        public EditUserWindow(int id, string login, string fullName, string email, int userTypeId, int? projectId) : this()
        {
            _userId = id;
            LoginTextBox.Text = login;
            FullNameTextBox.Text = fullName;
            EmailTextBox.Text = email;
            UserTypeComboBox.SelectedValue = userTypeId;
            ProjectComboBox.SelectedValue = projectId;
        }

        private void LoadUserTypes()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название FROM ТипыПользователей ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    UserTypeComboBox.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов пользователей: {ex.Message}");
            }
        }

        private void LoadProjects()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название FROM Проекты ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ProjectComboBox.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (UserTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип пользователя");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (_userId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Пользователи SET логин = @login, " +
                            (PasswordBox.Password.Length > 0 ? "пароль = @password, " : "") +
                            "тип_id = @userTypeId, проект_id = @projectId, фио = @fullName, email = @email " +
                            "WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", _userId.Value);
                    }
                    else
                    {
                        if (PasswordBox.Password.Length == 0)
                        {
                            MessageBox.Show("Введите пароль");
                            return;
                        }

                        cmd = new SqlCommand(
                            "INSERT INTO Пользователи (логин, пароль, тип_id, проект_id, фио, email) " +
                            "VALUES (@login, @password, @userTypeId, @projectId, @fullName, @email)", connection);
                        cmd.Parameters.AddWithValue("@password", PasswordBox.Password);
                    }

                    cmd.Parameters.AddWithValue("@login", LoginTextBox.Text);
                    if (_userId.HasValue && PasswordBox.Password.Length > 0)
                    {
                        cmd.Parameters.AddWithValue("@password", PasswordBox.Password);
                    }
                    cmd.Parameters.AddWithValue("@userTypeId", UserTypeComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@projectId", ProjectComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@fullName", FullNameTextBox.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@email", EmailTextBox.Text ?? string.Empty);

                    cmd.ExecuteNonQuery();
                }

                IsSaved = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}