using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;

namespace Агеенков_курсач.Researcher
{
    public partial class ResearcherProjectDetailsWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int currentProjectId;

        public ResearcherProjectDetailsWindow(int projectId)
        {
            InitializeComponent();
            currentProjectId = projectId;
            LoadProjectData();
            LoadEquipment();
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

                    string query = @"SELECT p.название, p.описание,
                                   z.название AS заказчик
                                   FROM Проекты p
                                   JOIN Договоры d ON p.договор_id = d.id
                                   JOIN Заказчики z ON d.заказчик_id = z.id
                                   WHERE p.id = @ProjectId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", ProjectManager.Instance.CurrentProject);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ProjectNameText.Text = reader["название"].ToString();
                                ProjectDescriptionText.Text = reader["описание"].ToString();
                                CustomerText.Text = reader["заказчик"].ToString();
                                
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

        private void LoadEquipment()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT id, название, тип, серийный_номер, дата_добавления
                                   FROM Оборудование
                                   WHERE проект_id = @ProjectId
                                   ORDER BY дата_добавления DESC";

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

        private void LoadMeasurements()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT m.id, m.тип_измерения, m.результат, m.дата_время, 
                                   u.фио AS оператор
                                   FROM Измерения m
                                   JOIN Операторы u ON m.оператор_id = u.id
                                   JOIN Пикеты pk ON m.пикет_id = pk.id
                                   JOIN Профили pr ON pk.профиль_id = pr.id
                                   JOIN Площади pl ON pr.площадь_id = pl.id
                                   WHERE pl.проект_id = @ProjectId";

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
                                   WHERE проект_id = @ProjectId 
                                   ORDER BY дата_создания DESC";

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResearcherMenuWindow researcherMenu = new ResearcherMenuWindow();
            researcherMenu.Show();
            this.Close();
        }
    }
}