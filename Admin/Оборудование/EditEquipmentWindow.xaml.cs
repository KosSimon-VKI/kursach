using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Оборудование
{
    public partial class EditEquipmentWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? equipmentId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => equipmentId.HasValue ? "Редактирование оборудования" : "Добавление оборудования";

        public EditEquipmentWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProjects();
            DatePicker.SelectedDate = DateTime.Now;
        }

        public EditEquipmentWindow(int id, int projectId, string name, string type,
                                 string serialNumber, string specs, DateTime addDate) : this()
        {
            equipmentId = id;
            ProjectComboBox.SelectedValue = projectId;
            NameTextBox.Text = name;
            TypeTextBox.Text = type;
            SerialNumberTextBox.Text = serialNumber;
            SpecsTextBox.Text = specs;
            DatePicker.SelectedDate = addDate;
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

                    if (equipmentId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Оборудование SET проект_id = @projectId, название = @name, " +
                            "тип = @type, серийный_номер = @serialNumber, характеристики = @specs, " +
"дата_добавления = @addDate WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", equipmentId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                        "INSERT INTO Оборудование (проект_id, название, тип, серийный_номер, характеристики, дата_добавления) " +
                        "VALUES (@projectId, @name, @type, @serialNumber, @specs, @addDate)", connection);
                    }
                    cmd.Parameters.AddWithValue("@projectId", ProjectComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                    cmd.Parameters.AddWithValue("@type", TypeTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@serialNumber", SerialNumberTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@specs", SpecsTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@addDate", DatePicker.SelectedDate ?? DateTime.Now);

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

