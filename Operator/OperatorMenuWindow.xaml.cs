using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Агеенков_курсач.Operator
{
    public partial class OperatorMenuWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int currentProjectId;

        public OperatorMenuWindow()
        {
            InitializeComponent();
            currentProjectId = ProjectManager.Instance.CurrentProject;
            MeasurementTypeComboBox.ItemsSource = new string[] { "Высота", "Напряжение", "Температура", "Влажность" };
            GraphMeasurementTypeComboBox.ItemsSource = new string[] { "Высота", "Напряжение", "Температура", "Влажность" };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProjectData();
            LoadEquipmentData();
            LoadPickets();
            LoadMeasurements();
            LoadReports();
        }

        private void LoadProjectData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT p.название AS ProjectName, p.описание AS ProjectDescription, 
                                    z.название AS CustomerName FROM Проекты p
                                    JOIN Договоры d ON p.договор_id = d.id
                                    JOIN Заказчики z ON d.заказчик_id = z.id
                                    WHERE p.id = @ProjectId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", currentProjectId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ProjectNameText.Text = reader["ProjectName"].ToString();
                                ProjectDescriptionText.Text = reader["ProjectDescription"].ToString();
                                CustomerText.Text = reader["CustomerName"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных проекта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadEquipmentData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, название, тип, серийный_номер, дата_добавления, характеристики 
                                   FROM Оборудование WHERE проект_id = @ProjectId ORDER BY дата_добавления DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ProjectId", currentProjectId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    EquipmentDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPickets()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT pk.id, pk.номер FROM Пикеты pk
                                   JOIN Профили pr ON pk.профиль_id = pr.id
                                   JOIN Площади pl ON pr.площадь_id = pl.id
                                   WHERE pl.проект_id = @ProjectId ORDER BY pk.номер";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ProjectId", currentProjectId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    PicketComboBox.ItemsSource = dt.DefaultView;
                    GraphPicketComboBox.ItemsSource = dt.DefaultView;
                    if (dt.Rows.Count > 0) PicketComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пикетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMeasurements()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT i.id, pk.номер, i.тип_измерения, i.результат, i.дата_время 
                                   FROM Измерения i
                                   JOIN Пикеты pk ON i.пикет_id = pk.id
                                   JOIN Профили pr ON pk.профиль_id = pr.id
                                   JOIN Площади pl ON pr.площадь_id = pl.id
                                   WHERE pl.проект_id = @ProjectId ORDER BY i.дата_время DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ProjectId", currentProjectId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    MeasurementsDataGrid.ItemsSource = dt.DefaultView;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке измерений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadReports()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, дата_создания, описание, файл_отчета 
                                   FROM Отчёт_об_измерениях 
                                   WHERE проект_id = @ProjectId ORDER BY дата_создания DESC";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.SelectCommand.Parameters.AddWithValue("@ProjectId", currentProjectId);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ReportsDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddMeasurement_Click(object sender, RoutedEventArgs e)
        {
            if (PicketComboBox.SelectedItem == null || MeasurementTypeComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(ResultTextBox.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(ResultTextBox.Text, out double result))
            {
                MessageBox.Show("Результат должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Измерения (пикет_id, оператор_id, дата_время, 
                                   тип_измерения, результат, примечания)
                                   VALUES (@PicketId, @OperatorId, @DateTime, 
                                   @MeasurementType, @Result, @Notes)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PicketId", (PicketComboBox.SelectedItem as DataRowView)["id"]);
                        command.Parameters.AddWithValue("@OperatorId", ProjectManager.Instance.CurrentOperator);
                        command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        command.Parameters.AddWithValue("@MeasurementType", MeasurementTypeComboBox.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@Result", result);
                        command.Parameters.AddWithValue("@Notes", NotesTextBox.Text ?? string.Empty);

                        command.ExecuteNonQuery();
                        MessageBox.Show("Измерение успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                        LoadMeasurements();
                        ResultTextBox.Clear();
                        NotesTextBox.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении измерения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EquipmentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно добавить дополнительную логику при выборе оборудования
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEquipmentWindow(currentProjectId);
            if (dialog.ShowDialog() == true)
            {
                LoadEquipmentData();
            }
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)EquipmentDataGrid.SelectedItem;
            int equipmentId = (int)selected["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Оборудование WHERE id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", equipmentId);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Оборудование удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEquipmentData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowEquipmentDetails_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите оборудование", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)EquipmentDataGrid.SelectedItem;
            string details = $"Название: {selected["название"]}\n" +
                           $"Тип: {selected["тип"]}\n" +
                           $"Серийный номер: {selected["серийный_номер"]}\n" +
                           $"Дата добавления: {((DateTime)selected["дата_добавления"]).ToString("dd.MM.yyyy")}\n" +
                           $"Характеристики:\n{selected["характеристики"]}";

            MessageBox.Show(details, "Характеристики оборудования", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteMeasurement_Click(object sender, RoutedEventArgs e)
        {
            if (MeasurementsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите измерение для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)MeasurementsDataGrid.SelectedItem;
            int measurementId = (int)selected["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Измерения WHERE id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", measurementId);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Измерение удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadMeasurements();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении измерения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddReport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddReportWindow(currentProjectId);
            if (dialog.ShowDialog() == true)
            {
                LoadReports();
            }
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)ReportsDataGrid.SelectedItem;
            int reportId = (int)selected["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Отчёт_об_измерениях WHERE id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", reportId);

                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Отчет удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadReports();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите отчет", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)ReportsDataGrid.SelectedItem;
            string filePath = selected["файл_отчета"].ToString();

            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть отчет: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildGraph_Click(object sender, RoutedEventArgs e)
        {
            if (GraphMeasurementTypeComboBox.SelectedItem == null || GraphPicketComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип измерения и пикет", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT i.результат, i.дата_время 
                                   FROM Измерения i
                                   JOIN Пикеты pk ON i.пикет_id = pk.id
                                   JOIN Профили pr ON pk.профиль_id = pr.id
                                   JOIN Площади pl ON pr.площадь_id = pl.id
                                   WHERE pl.проект_id = @ProjectId 
                                   AND i.тип_измерения = @MeasurementType
                                   AND i.пикет_id = @PicketId
                                   AND i.оператор_id = @OperatorId
                                   ORDER BY i.дата_время";

                    var measurements = new List<MeasurementPoint>();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", currentProjectId);
                        command.Parameters.AddWithValue("@MeasurementType", GraphMeasurementTypeComboBox.SelectedItem.ToString());
                        command.Parameters.AddWithValue("@PicketId", (GraphPicketComboBox.SelectedItem as DataRowView)["id"]);
                        command.Parameters.AddWithValue("@OperatorId", ProjectManager.Instance.CurrentOperator);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                measurements.Add(new MeasurementPoint
                                {
                                    Value = Convert.ToDouble(reader["результат"]),
                                    DateTime = (DateTime)reader["дата_время"]
                                });
                            }
                        }
                    }

                    if (measurements.Count == 0)
                    {
                        MessageBox.Show("Нет данных для построения графика", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    DrawGraph(measurements);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при построении графика: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void DrawGraph(List<MeasurementPoint> measurements)
        {
            GraphCanvas.Children.Clear();

            double canvasWidth = GraphCanvas.ActualWidth - 80; // Оставляем место для осей
            double canvasHeight = GraphCanvas.ActualHeight - 40;

            if (canvasWidth <= 0 || canvasHeight <= 0) return;

            // Находим минимальные и максимальные значения
            double minValue = measurements.Min(m => m.Value);
            double maxValue = measurements.Max(m => m.Value);
            DateTime minDate = measurements.Min(m => m.DateTime);
            DateTime maxDate = measurements.Max(m => m.DateTime);
            double valueRange = maxValue - minValue;
            double dateRange = (maxDate - minDate).TotalDays;

            // Рисуем оси
            XAxis.X1 = 40;
            XAxis.Y1 = canvasHeight;
            XAxis.X2 = canvasWidth + 40;
            XAxis.Y2 = canvasHeight;

            YAxis.X1 = 40;
            YAxis.Y1 = canvasHeight;
            YAxis.X2 = 40;
            YAxis.Y2 = 0;

            // Подписи осей
            XAxisLabel.Text = "Дата";
            System.Windows.Controls.Canvas.SetLeft(XAxisLabel, canvasWidth / 2 + 20);
            System.Windows.Controls.Canvas.SetTop(XAxisLabel, canvasHeight + 5);

            YAxisLabel.Text = "Значение";
            System.Windows.Controls.Canvas.SetLeft(YAxisLabel, 5);
            System.Windows.Controls.Canvas.SetTop(YAxisLabel, canvasHeight / 2);

            // Рисуем график
            Polyline polyline = new Polyline
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2
            };

            for (int i = 0; i < measurements.Count; i++)
            {
                double x = 40 + (i * canvasWidth / (measurements.Count - 1));
                double y = canvasHeight - ((measurements[i].Value - minValue) * canvasHeight / valueRange);

                polyline.Points.Add(new Point(x, y));

                if (ShowValuesCheckBox.IsChecked ?? false)
                {
                    TextBlock valueLabel = new TextBlock
                    {
                        Text = measurements[i].Value.ToString("N2"),
                        FontSize = 10,
                        Foreground = Brushes.Black,
                        Background = Brushes.White
                    };

                    System.Windows.Controls.Canvas.SetLeft(valueLabel, x - 15);
                    System.Windows.Controls.Canvas.SetTop(valueLabel, y - 15);
                    GraphCanvas.Children.Add(valueLabel);
                }

                // Подписи дат (каждую 5-ю или если мало точек)
                if (i % Math.Max(1, measurements.Count / 5) == 0 || measurements.Count < 10)
                {
                    TextBlock dateLabel = new TextBlock
                    {
                        Text = measurements[i].DateTime.ToString("dd.MM"),
                        FontSize = 10,
                        Foreground = Brushes.Black
                    };

                    System.Windows.Controls.Canvas.SetLeft(dateLabel, x - 15);
                    System.Windows.Controls.Canvas.SetTop(dateLabel, canvasHeight + 5);
                    GraphCanvas.Children.Add(dateLabel);
                }
            }

            GraphCanvas.Children.Add(polyline);

            // Информация о графике
            GraphInfoText.Text = $"{GraphMeasurementTypeComboBox.SelectedItem} (пикет {(GraphPicketComboBox.SelectedItem as DataRowView)["номер"]}) " +
                                $"с {minDate:dd.MM.yyyy} по {maxDate:dd.MM.yyyy}. Всего точек: {measurements.Count}";
        }

        private class MeasurementPoint
        {
            public double Value { get; set; }
            public DateTime DateTime { get; set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}