using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Измерения
{
    public partial class EditMeasurementWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? _measurementId;
        public bool IsSaved { get; private set; } = false;

        public string Title => _measurementId.HasValue ? "Редактирование измерения" : "Добавление измерения";

        public EditMeasurementWindow()
        {
            InitializeComponent();
            LoadPickets();
            LoadOperators();
            DatePicker.SelectedDate = DateTime.Now;
        }

        public EditMeasurementWindow(int id, int? picketId, int? operatorId, DateTime dateTime,
                                  string type, double? result, string notes) : this()
        {
            _measurementId = id;
            PicketComboBox.SelectedValue = picketId;
            OperatorComboBox.SelectedValue = operatorId;
            DatePicker.SelectedDate = dateTime;
            TimeTextBox.Text = dateTime.ToString("HH:mm");
            TypeTextBox.Text = type;
            ResultTextBox.Text = result?.ToString();
            NotesTextBox.Text = notes;
        }

        private void LoadPickets()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, номер FROM Пикеты ORDER BY номер";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    PicketComboBox.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пикетов: {ex.Message}");
            }
        }

        private void LoadOperators()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, фио FROM Операторы ORDER BY фио";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    OperatorComboBox.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки операторов: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TypeTextBox.Text))
            {
                MessageBox.Show("Введите тип измерения");
                return;
            }

            if (!DateTime.TryParse(TimeTextBox.Text, out var time))
            {
                MessageBox.Show("Введите корректное время в формате HH:mm");
                return;
            }

            DateTime measurementDate = DatePicker.SelectedDate ?? DateTime.Now;
            measurementDate = measurementDate.Date + time.TimeOfDay;

            object resultValue = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(ResultTextBox.Text))
            {
                if (double.TryParse(ResultTextBox.Text, out double res))
                {
                    resultValue = res;
                }
                else
                {
                    MessageBox.Show("Введите корректное числовое значение результата");
                    return;
                }
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (_measurementId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Измерения SET пикет_id = @picketId, оператор_id = @operatorId, " +
                            "дата_время = @dateTime, тип_измерения = @type, результат = @result, " +
                            "примечания = @notes WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", _measurementId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Измерения (пикет_id, оператор_id, дата_время, тип_измерения, " +
                            "результат, примечания) VALUES (@picketId, @operatorId, @dateTime, @type, " +
                            "@result, @notes)", connection);
                    }

                    cmd.Parameters.AddWithValue("@picketId", PicketComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@operatorId", OperatorComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@dateTime", measurementDate);
                    cmd.Parameters.AddWithValue("@type", TypeTextBox.Text);
                    cmd.Parameters.AddWithValue("@result", resultValue);
                    cmd.Parameters.AddWithValue("@notes", NotesTextBox.Text ?? string.Empty);

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