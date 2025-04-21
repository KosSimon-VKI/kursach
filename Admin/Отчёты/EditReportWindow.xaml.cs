using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Отчеты
{
    public partial class EditReportWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? reportId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => reportId.HasValue ? "Редактирование отчета" : "Добавление отчета";

        public EditReportWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadMeasurements();
            LoadProjects();
            CreateDatePicker.SelectedDate = DateTime.Now;
        }

        public EditReportWindow(int id, int measurementId, int projectId, DateTime createDate,
                              string description, string graphs, string reportFile) : this()
        {
            reportId = id;
            MeasurementComboBox.SelectedValue = measurementId;
            ProjectComboBox.SelectedValue = projectId;
            CreateDatePicker.SelectedDate = createDate;
            DescriptionTextBox.Text = description;
            GraphsTextBox.Text = graphs;
            ReportFileTextBox.Text = reportFile;
        }

        private void LoadMeasurements()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, тип_измерения FROM Измерения ORDER BY тип_измерения";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    MeasurementComboBox.ItemsSource = dt.DefaultView;
                    MeasurementComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки измерений: {ex.Message}");
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
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (reportId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Отчёт_об_измерениях SET измерения_id = @measurementId, проект_id = @projectId, " +
                            "дата_создания = @createDate, описание = @description, графики = @graphs, " +
                            "файл_отчета = @reportFile WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", reportId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Отчёт_об_измерениях (измерения_id, проект_id, дата_создания, описание, графики, файл_отчета) " +
                            "VALUES (@measurementId, @projectId, @createDate, @description, @graphs, @reportFile)", connection);
                    }

                    cmd.Parameters.AddWithValue("@measurementId", MeasurementComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@projectId", ProjectComboBox.SelectedValue);
                    cmd.Parameters.AddWithValue("@createDate", CreateDatePicker.SelectedDate ?? DateTime.Now);
                    cmd.Parameters.AddWithValue("@description", DescriptionTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@graphs", GraphsTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@reportFile", ReportFileTextBox.Text ?? "");

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