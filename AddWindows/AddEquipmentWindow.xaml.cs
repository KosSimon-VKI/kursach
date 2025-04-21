using System;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Operator
{
    public partial class AddEquipmentWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int projectId;

        public AddEquipmentWindow(int projectId)
        {
            InitializeComponent();
            this.projectId = projectId;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TypeTextBox.Text) ||
                string.IsNullOrWhiteSpace(SerialNumberTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Оборудование (проект_id, название, тип, серийный_номер, характеристики, дата_добавления)
                                   VALUES (@ProjectId, @Name, @Type, @SerialNumber, @Specs, @Date)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProjectId", projectId);
                        command.Parameters.AddWithValue("@Name", NameTextBox.Text);
                        command.Parameters.AddWithValue("@Type", TypeTextBox.Text);
                        command.Parameters.AddWithValue("@SerialNumber", SerialNumberTextBox.Text);
                        command.Parameters.AddWithValue("@Specs", SpecsTextBox.Text ?? string.Empty);
                        command.Parameters.AddWithValue("@Date", DateTime.Now);

                        command.ExecuteNonQuery();
                        DialogResult = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}