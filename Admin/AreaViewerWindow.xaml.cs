using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Агеенков_курсач.Admin;

namespace Агеенков_курсач
{
    public partial class AreaViewerWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private List<Point> currentPoints = new List<Point>();

        public AreaViewerWindow()
        {
            InitializeComponent();
            LoadAreas();
        }

        private void LoadAreas()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название FROM Площади ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    AreaComboBox.ItemsSource = dt.DefaultView;
                    AreaComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки площадей: {ex.Message}");
            }
        }

        private void ShowArea_Click(object sender, RoutedEventArgs e)
        {
            if (AreaComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите площадь!");
                return;
            }

            int areaId = (int)AreaComboBox.SelectedValue;
            LoadAndDrawArea(areaId);
        }

        private void LoadAndDrawArea(int areaId)
        {
            try
            {
                // Очищаем канвас
                DrawingCanvas.Children.Clear();
                currentPoints.Clear();

                // Получаем координаты площади
                List<Point> points = GetAreaCoordinates(areaId);
                if (points == null || points.Count < 3)
                {
                    NoDataText.Visibility = Visibility.Visible;
                    StatusText.Text = "Недостаточно точек для построения площади (минимум 3)";
                    return;
                }

                NoDataText.Visibility = Visibility.Collapsed;
                currentPoints = points;

                // Масштабируем точки для отображения на канвасе
                List<Point> scaledPoints = ScalePoints(points, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);

                // Рисуем полигон
                Polygon polygon = new Polygon
                {
                    Stroke = Brushes.Blue,
                    Fill = Brushes.LightBlue,
                    StrokeThickness = 2,
                    Points = new PointCollection(scaledPoints)
                };

                DrawingCanvas.Children.Add(polygon);

                // Добавляем номера точек
                for (int i = 0; i < scaledPoints.Count; i++)
                {
                    var point = scaledPoints[i];
                    var text = new TextBlock
                    {
                        Text = (i + 1).ToString(),
                        Foreground = Brushes.Red,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(point.X - 10, point.Y - 10, 0, 0)
                    };
                    DrawingCanvas.Children.Add(text);
                }

                StatusText.Text = $"Отображена площадь: {AreaComboBox.Text}. Точек: {points.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения площади: {ex.Message}");
            }
        }

        private List<Point> GetAreaCoordinates(int areaId)
        {
            List<Point> points = new List<Point>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT к.широта, к.долгота 
                    FROM Площади_Координаты пк
                    JOIN Координаты к ON пк.координата_id = к.id
                    WHERE пк.площадь_id = @areaId
                    ORDER BY пк.порядковый_номер";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@areaId", areaId);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    decimal latitude = (decimal)reader["широта"];
                    decimal longitude = (decimal)reader["долгота"];
                    points.Add(new Point((double)longitude, (double)latitude));
                }
            }

            return points;
        }

        private List<Point> ScalePoints(List<Point> points, double canvasWidth, double canvasHeight)
        {
            if (points == null || points.Count == 0)
                return new List<Point>();

            // Находим минимальные и максимальные значения координат
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            // Вычисляем масштабные коэффициенты
            double scaleX = canvasWidth / (maxX - minX) * 0.8;
            double scaleY = canvasHeight / (maxY - minY) * 0.8;
            double scale = Math.Min(scaleX, scaleY);

            // Центрируем изображение
            double offsetX = (canvasWidth - (maxX - minX) * scale) / 2 - minX * scale;
            double offsetY = (canvasHeight - (maxY - minY) * scale) / 2 - minY * scale;

            // Масштабируем и смещаем точки
            return points.Select(p => new Point(
                p.X * scale + offsetX,
                canvasHeight - (p.Y * scale + offsetY) // Инвертируем Y для правильной ориентации
            )).ToList();
        }

        private void ExportToPng_Click(object sender, RoutedEventArgs e)
        {
            if (currentPoints.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Сохранить изображение площади",
                FileName = $"{AreaComboBox.Text}.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Создаем RenderTargetBitmap для рендеринга канваса
                    var renderBitmap = new RenderTargetBitmap(
                        (int)DrawingCanvas.ActualWidth,
                        (int)DrawingCanvas.ActualHeight,
                        96d, 96d, PixelFormats.Pbgra32);

                    renderBitmap.Render(DrawingCanvas);

                    // Кодируем в PNG и сохраняем
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }

                    MessageBox.Show("Изображение успешно сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}");
                }
            }
        }

        private void DrawingCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // При изменении размера канваса перерисовываем площадь
            if (AreaComboBox.SelectedValue != null)
            {
                int areaId = (int)AreaComboBox.SelectedValue;
                LoadAndDrawArea(areaId);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AdminMenuWindow window = new AdminMenuWindow();
            window.Show();
            this.Close();
        }
    }
}