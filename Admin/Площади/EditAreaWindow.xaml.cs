using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Площади
{
    public partial class EditAreaWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? areaId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => areaId.HasValue ? "Редактирование площади" : "Добавление площади";

        public EditAreaWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProjects();
        }

        public EditAreaWindow(int id, int? projectId, string name) : this()
        {
            areaId = id;
            if (projectId.HasValue) ProjectComboBox.SelectedValue = projectId.Value;
            NameTextBox.Text = name;
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
                    ProjectComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}");
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

                    if (areaId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Площади SET проект_id = @projectId, название = @name WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", areaId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Площади (проект_id, название) VALUES (@projectId, @name)", connection);
                    }

                    cmd.Parameters.AddWithValue("@projectId", ProjectComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);

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