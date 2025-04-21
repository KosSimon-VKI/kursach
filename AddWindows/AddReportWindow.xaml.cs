using System;
using System.Data.SqlClient;
using System.Windows;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Агеенков_курсач.Operator
{
    public partial class AddReportWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int projectId;

        public class MeasurementItem
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Result { get; set; }
            public string DisplayText => $"{Id} - {Type} - {Result}";
        }

        public AddReportWindow(int projectId)
        {
            InitializeComponent();
            this.projectId = projectId;
            LoadMeasurements();
        }

        private void LoadMeasurements()
        {
            try
            {
                List<MeasurementItem> measurements = new List<MeasurementItem>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, тип_измерения, результат 
                           FROM Измерения 
                           WHERE оператор_id = @OperatorId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@OperatorId", ProjectManager.Instance.CurrentOperator);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                measurements.Add(new MeasurementItem
                                {
                                    Id = reader.GetInt32(0),
                                    Type = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                    Result = reader.IsDBNull(2) ? string.Empty : reader[2].ToString() // Преобразуем любой тип в строку
                                });
                            }
                        }
                    }
                }

                MeasurementComboBox.ItemsSource = measurements;
                if (measurements.Count > 0)
                    MeasurementComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке измерений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (MeasurementComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(FilePathTextBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var selectedMeasurement = (MeasurementItem)MeasurementComboBox.SelectedItem;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Отчёт_об_измерениях (измерения_id, проект_id, дата_создания, описание, файл_отчета)
                                   VALUES (@MeasurementId, @ProjectId, @Date, @Description, @FilePath)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MeasurementId", selectedMeasurement.Id);
                        command.Parameters.AddWithValue("@ProjectId", projectId);
                        command.Parameters.AddWithValue("@Date", DateTime.Now);
                        command.Parameters.AddWithValue("@Description", DescriptionTextBox.Text);
                        command.Parameters.AddWithValue("@FilePath", FilePathTextBox.Text);

                        command.ExecuteNonQuery();
                        DialogResult = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}