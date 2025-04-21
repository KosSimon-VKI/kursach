using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;

namespace Агеенков_курсач.Admin.Координаты
{
    public partial class EditCoordinateWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? _coordinateId;
        public bool IsSaved { get; private set; } = false;

        public string Title => _coordinateId.HasValue ? "Редактирование координаты" : "Добавление координаты";

        public EditCoordinateWindow()
        {
            InitializeComponent();
        }

        public EditCoordinateWindow(int id, decimal latitude, decimal longitude) : this()
        {
            _coordinateId = id;
            LatitudeTextBox.Text = latitude.ToString(CultureInfo.InvariantCulture);
            LongitudeTextBox.Text = longitude.ToString(CultureInfo.InvariantCulture);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(LatitudeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal latitude) ||
                !decimal.TryParse(LongitudeTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal longitude))
            {
                MessageBox.Show("Введите корректные числовые значения для координат");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (_coordinateId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Координаты SET широта = @latitude, долгота = @longitude WHERE id = @id",
                            connection);
                        cmd.Parameters.AddWithValue("@id", _coordinateId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Координаты (широта, долгота) VALUES (@latitude, @longitude)",
                            connection);
                    }

                    cmd.Parameters.AddWithValue("@latitude", latitude);
                    cmd.Parameters.AddWithValue("@longitude", longitude);

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