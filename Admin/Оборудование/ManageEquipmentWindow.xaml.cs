using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Агеенков_курсач.Admin.Оборудование
{
    public partial class ManageEquipmentWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";

        public ManageEquipmentWindow()
        {
            InitializeComponent();
            LoadEquipment();
        }

        private void LoadEquipment()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            о.id, 
                            о.проект_id,
                            о.название, 
                            о.тип,
                            о.серийный_номер,
                            о.характеристики,
                            о.дата_добавления,
                            CONCAT(п.id, ' - ', п.название) AS Проект
                        FROM Оборудование о
                        LEFT JOIN Проекты п ON о.проект_id = п.id
                        ORDER BY о.id";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    EquipmentGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}");
            }
        }

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditEquipmentWindow();
            if (editWindow.ShowDialog() == true)
                LoadEquipment();
            LoadEquipment();
        }

        private void EditEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem is DataRowView row)
            {
                var editWindow = new EditEquipmentWindow(
                    (int)row["id"],
                    (int)row["проект_id"],
                    row["название"].ToString(),
                    row["тип"].ToString(),
                    row["серийный_номер"].ToString(),
                    row["характеристики"].ToString(),
                    (DateTime)row["дата_добавления"]);

                if (editWindow.ShowDialog() == true)
                    LoadEquipment();
                LoadEquipment();
            }
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentGrid.SelectedItem is DataRowView row &&
                MessageBox.Show("Удалить оборудование?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand("DELETE FROM Оборудование WHERE id = @id", connection);
                        cmd.Parameters.AddWithValue("@id", row["id"]);
                        cmd.ExecuteNonQuery();
                    }
                    LoadEquipment();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new AdminMenuWindow().Show();
            this.Close();
        }
    }
}