using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Агеенков_курсач.Customer
{
    public partial class CustomerMenuWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public CustomerMenuWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProjectData();
            LoadEquipmentData();
            LoadReportsData();
            LoadMeasurementsData();
        }

        private void LoadProjectData()
        {
            int currentProjectId = ProjectManager.Instance.CurrentProject;

            if (currentProjectId <= 0)
            {
                MessageBox.Show("Проект не выбран или не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            p.название AS ProjectName,
                            p.описание AS ProjectDescription,
                            d.номер_договора AS ContractNumber,
                            z.название AS CustomerName
                        FROM 
                            Проекты p
                            JOIN Договоры d ON p.договор_id = d.id
                            JOIN Заказчики z ON d.заказчик_id = z.id
                        WHERE 
                            p.id = @ProjectId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", currentProjectId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ProjectNameText.Text = reader["ProjectName"].ToString();
                                ProjectDescriptionText.Text = reader["ProjectDescription"].ToString();
                                ContractText.Text = reader["ContractNumber"].ToString();
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
            int currentProjectId = ProjectManager.Instance.CurrentProject;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            название,
                            тип,
                            серийный_номер,
                            дата_добавления
                        FROM 
                            Оборудование
                        WHERE 
                            проект_id = @ProjectId
                        ORDER BY 
                            дата_добавления DESC";

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

        private void LoadReportsData()
        {
            int currentProjectId = ProjectManager.Instance.CurrentProject;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            дата_создания,
                            описание
                        FROM 
                            Отчёт_об_измерениях
                        WHERE 
                            проект_id = @ProjectId
                        ORDER BY 
                            дата_создания DESC";

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

        private void LoadMeasurementsData()
        {
            int currentProjectId = ProjectManager.Instance.CurrentProject;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            i.дата_время,
                            i.тип_измерения,
                            i.результат,
                            o.фио AS Оператор
                        FROM 
                            Измерения i
                            JOIN Операторы o ON i.оператор_id = o.id
                            JOIN Пикеты p ON i.пикет_id = p.id
                            JOIN Профили pr ON p.профиль_id = pr.id
                            JOIN Площади pl ON pr.площадь_id = pl.id
                        WHERE 
                            pl.проект_id = @ProjectId
                        ORDER BY 
                            i.дата_время DESC";

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
           MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}