using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Договоры
{
    public partial class EditContractWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private int? contractId;

        public class CustomerItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DisplayName => $"{Id} - {Name}";
        }

        public EditContractWindow()
        {
            InitializeComponent();
            Title = "Добавление договора";
            DatePicker.SelectedDate = DateTime.Today;
            LoadCustomers();
        }

        public EditContractWindow(int id, int customerId, string contractNumber, DateTime conclusionDate, string description) : this()
        {
            contractId = id;
            Title = "Редактирование договора";
            ContractNumberBox.Text = contractNumber;
            DatePicker.SelectedDate = conclusionDate;
            DescriptionBox.Text = description;

            // Выбираем заказчика в ComboBox
            foreach (CustomerItem item in CustomerComboBox.Items)
            {
                if (item.Id == customerId)
                {
                    CustomerComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadCustomers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id, название FROM Заказчики ORDER BY id";
                    SqlCommand cmd = new SqlCommand(query, connection);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerComboBox.Items.Add(new CustomerItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказчиков: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(ContractNumberBox.Text) ||
                DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Заполните обязательные поля: Заказчик, Номер договора и Дата заключения");
                return;
            }

            try
            {
                int customerId = ((CustomerItem)CustomerComboBox.SelectedItem).Id;
                string description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand cmd;

                    if (contractId.HasValue)
                    {
                        cmd = new SqlCommand(
                            "UPDATE Договоры SET заказчик_id=@cid, номер_договора=@num, дата_заключения=@date, описание=@desc WHERE id=@id",
                            connection);
                        cmd.Parameters.AddWithValue("@id", contractId.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand(
                            "INSERT INTO Договоры (заказчик_id, номер_договора, дата_заключения, описание) VALUES (@cid, @num, @date, @desc)",
                            connection);
                    }

                    cmd.Parameters.AddWithValue("@cid", customerId);
                    cmd.Parameters.AddWithValue("@num", ContractNumberBox.Text);
                    cmd.Parameters.AddWithValue("@date", DatePicker.SelectedDate);
                    cmd.Parameters.AddWithValue("@desc", (object)description ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}