using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Проекты
{
    public partial class EditProjectWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? _projectId;
        public bool IsSaved { get; private set; } = false;

        public string Title => _projectId.HasValue ? "Редактирование проекта" : "Добавление проекта";

        public EditProjectWindow()
        {
            InitializeComponent();
            LoadContracts();
        }

        public EditProjectWindow(int id, string name, string description, int? contractId) : this()
        {
            _projectId = id;
            NameTextBox.Text = name;
            DescriptionTextBox.Text = description;
            ContractComboBox.SelectedValue = contractId;
        }

        private void LoadContracts()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, номер_договора FROM Договоры ORDER BY номер_договора";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    ContractComboBox.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки договоров: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название проекта");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (_projectId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Проекты SET договор_id = @contractId, название = @name, " +
                            "описание = @description WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", _projectId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Проекты (договор_id, название, описание) " +
                            "VALUES (@contractId, @name, @description)", connection);
                    }

                    cmd.Parameters.AddWithValue("@contractId", ContractComboBox.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                    cmd.Parameters.AddWithValue("@description", DescriptionTextBox.Text ?? string.Empty);

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