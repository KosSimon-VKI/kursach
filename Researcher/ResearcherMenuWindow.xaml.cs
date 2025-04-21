using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Researcher
{
    public partial class ResearcherMenuWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ResearcherMenuWindow()
        {
            InitializeComponent();
            LoadProjects();
        }

        private void LoadProjects()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT p.id, p.договор_id, p.название, p.описание
                                   FROM Проекты p";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ProjectsDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке проектов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewProjectDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите проект для просмотра", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = (DataRowView)ProjectsDataGrid.SelectedItem;
            int projectId = (int)selected["id"];
            ProjectManager.Instance.CurrentProject = projectId;

            var projectDetailsWindow = new ResearcherProjectDetailsWindow(projectId);
            projectDetailsWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}