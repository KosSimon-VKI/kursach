using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Операторы
{
    public partial class EditOperatorWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? operatorId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => operatorId.HasValue ? "Редактирование оператора" : "Добавление оператора";

        public EditOperatorWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public EditOperatorWindow(int id, string fullName, string position, string phone) : this()
        {
            operatorId = id;
            FullNameTextBox.Text = fullName;
            PositionTextBox.Text = position;
            PhoneTextBox.Text = phone;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show("ФИО не может быть пустым!");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (operatorId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Операторы SET фио = @fullName, должность = @position, " +
                            "контактный_телефон = @phone WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", operatorId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Операторы (фио, должность, контактный_телефон) " +
                            "VALUES (@fullName, @position, @phone)", connection);
                    }

                    cmd.Parameters.AddWithValue("@fullName", FullNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@position", PositionTextBox.Text ?? "");
                    cmd.Parameters.AddWithValue("@phone", PhoneTextBox.Text ?? "");

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