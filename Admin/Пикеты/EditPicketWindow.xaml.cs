using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Пикеты
{
    public partial class EditPicketWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? picketId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => picketId.HasValue ? "Редактирование пикета" : "Добавление пикета";

        public EditPicketWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProfiles();
            LoadCoordinates();
        }

        public EditPicketWindow(int id, int? profileId, int number, int coordinateId, double? height) : this()
        {
            picketId = id;
            if (profileId.HasValue) ProfileComboBox.SelectedValue = profileId.Value;
            NumberTextBox.Text = number.ToString();
            CoordinateComboBox.SelectedValue = coordinateId;
            HeightTextBox.Text = height?.ToString();
        }

        private void LoadProfiles()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название FROM Профили ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ProfileComboBox.ItemsSource = dt.DefaultView;
                    ProfileComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профилей: {ex.Message}");
            }
        }

        private void LoadCoordinates()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, широта, долгота FROM Координаты ORDER BY id";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Формируем строку для отображения
                    foreach (DataRow row in dt.Rows)
                    {
                        row["широта"] = $"{row["широта"]}, {row["долгота"]}";
                    }

                    CoordinateComboBox.ItemsSource = dt.DefaultView;
                    CoordinateComboBox.SelectedValuePath = "id";
                    CoordinateComboBox.DisplayMemberPath = "широта";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки координат: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(NumberTextBox.Text, out int number))
            {
                MessageBox.Show("Номер должен быть целым числом!");
                return;
            }

            if (!double.TryParse(HeightTextBox.Text, out double height) && !string.IsNullOrEmpty(HeightTextBox.Text))
            {
                MessageBox.Show("Высота должна быть числом!");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (picketId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Пикеты SET профиль_id = @profileId, номер = @number, " +
                            "координата_id = @coordinateId, высота = @height WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", picketId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Пикеты (профиль_id, номер, координата_id, высота) " +
                            "VALUES (@profileId, @number, @coordinateId, @height)", connection);
                    }

                    cmd.Parameters.AddWithValue("@profileId", ProfileComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@number", number);
                    cmd.Parameters.AddWithValue("@coordinateId", CoordinateComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@height", string.IsNullOrEmpty(HeightTextBox.Text) ? DBNull.Value : (object)height);

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