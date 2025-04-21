using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Профили
{
    public partial class EditProfileWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? profileId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => profileId.HasValue ? "Редактирование профиля" : "Добавление профиля";

        public EditProfileWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadAreas();
        }

        public EditProfileWindow(int id, int? areaId, string name, string type) : this()
        {
            profileId = id;
            if (areaId.HasValue) AreaComboBox.SelectedValue = areaId.Value;
            NameTextBox.Text = name;
            TypeTextBox.Text = type;
        }

        private void LoadAreas()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название FROM Площади ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    AreaComboBox.ItemsSource = dt.DefaultView;
                    AreaComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки площадей: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Название не может быть пустым!");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (profileId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Профили SET площадь_id = @areaId, название = @name, тип = @type WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", profileId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Профили (площадь_id, название, тип) VALUES (@areaId, @name, @type)", connection);
                    }

                    cmd.Parameters.AddWithValue("@areaId", AreaComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                    cmd.Parameters.AddWithValue("@type", TypeTextBox.Text ?? "");

                    cmd.ExecuteNonQuery();
                }

                IsSaved = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}