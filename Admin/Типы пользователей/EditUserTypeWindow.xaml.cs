using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.ТипыПользователей
{
    public partial class EditUserTypeWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? userTypeId;

        public bool IsSaved { get; private set; } = false;
        public string WindowTitle => userTypeId.HasValue ? "Редактирование типа пользователя" : "Добавление типа пользователя";

        public EditUserTypeWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public EditUserTypeWindow(int id, string name, string description) : this()
        {
            userTypeId = id;
            NameTextBox.Text = name;
            DescriptionTextBox.Text = description;
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

                    if (userTypeId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE ТипыПользователей SET название = @name, описание = @description WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", userTypeId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO ТипыПользователей (название, описание) VALUES (@name, @description)", connection);
                    }

                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                    cmd.Parameters.AddWithValue("@description", DescriptionTextBox.Text ?? "");

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