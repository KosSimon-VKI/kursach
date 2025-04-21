using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Заказчики
{
    public partial class EditCustomerWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? _customerId;
        public bool IsSaved { get; private set; } = false;

        public string Title => _customerId.HasValue ? "Редактирование заказчика" : "Добавление заказчика";

        public EditCustomerWindow()
        {
            InitializeComponent();
        }



        public EditCustomerWindow(int id, string name, string address, string contactPerson, string phone, string email) : this()
        {
            _customerId = id;
            NameTextBox.Text = name;
            AddressTextBox.Text = address;
            ContactPersonTextBox.Text = contactPerson;
            PhoneTextBox.Text = phone;
            EmailTextBox.Text = email;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите название заказчика");
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (_customerId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Заказчики SET название = @name, адрес = @address, " +
                            "контактное_лицо = @contactPerson, телефон = @phone, email = @email WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", _customerId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Заказчики (название, адрес, контактное_лицо, телефон, email) " +
                            "VALUES (@name, @address, @contactPerson, @phone, @email)", connection);
                    }

                    cmd.Parameters.AddWithValue("@name", NameTextBox.Text);
                    cmd.Parameters.AddWithValue("@address", AddressTextBox.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@contactPerson", ContactPersonTextBox.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@phone", PhoneTextBox.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@email", EmailTextBox.Text ?? string.Empty);

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