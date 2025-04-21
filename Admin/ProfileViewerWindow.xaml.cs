using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;
using Агеенков_курсач.Admin;

namespace Агеенков_курсач
{
    public partial class ProfileViewerWindow : Window
    {
        private string connectionString = @"Data Source=DESKTOP-HVQ1BQC\SQLEXPRESS;Initial Catalog=БД_Агеенков;Integrated Security=True";
        private List<Point> profilePoints = new List<Point>();
        private List<Point> picketPoints = new List<Point>();

        public ProfileViewerWindow()
        {
            InitializeComponent();
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT id, название, тип FROM Профили ORDER BY название";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    ProfileComboBox.ItemsSource = dt.DefaultView;
                    ProfileComboBox.SelectedValuePath = "id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки профилей: {ex.Message}");
            }
        }

        private void ShowProfile_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите профиль!");
                return;
            }

            int profileId = (int)ProfileComboBox.SelectedValue;
            LoadAndDrawProfile(profileId);
        }

        private void LoadAndDrawProfile(int profileId)
        {
            try
            {
                DrawingCanvas.Children.Clear();
                profilePoints.Clear();
                picketPoints.Clear();

                // Получаем координаты профиля
                profilePoints = GetProfileCoordinates(profileId);
                if (profilePoints == null || profilePoints.Count < 2)
                {
                    NoDataText.Visibility = Visibility.Visible;
                    StatusText.Text = "Недостаточно точек для построения профиля (минимум 2)";
                    return;
                }

                NoDataText.Visibility = Visibility.Collapsed;

                // Получаем пикеты профиля
                if (ShowPicketsCheckBox.IsChecked == true)
                {
                    picketPoints = GetPicketCoordinates(profileId);
                }

                // Масштабируем точки
                List<Point> scaledProfilePoints = ScalePoints(profilePoints, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);
                List<Point> scaledPicketPoints = ScalePoints(picketPoints, DrawingCanvas.ActualWidth, DrawingCanvas.ActualHeight);

                // Рисуем линию профиля
                Polyline profileLine = new Polyline
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    Points = new PointCollection(scaledProfilePoints)
                };
                DrawingCanvas.Children.Add(profileLine);

                // Рисуем точки профиля
                for (int i = 0; i < scaledProfilePoints.Count; i++)
                {
                    var point = scaledProfilePoints[i];

                    // Точка
                    Ellipse ellipse = new Ellipse
                    {
                        Width = 6,
                        Height = 6,
                        Fill = Brushes.Red,
                        Stroke = Brushes.DarkRed,
                        StrokeThickness = 1
                    };
                    Canvas.SetLeft(ellipse, point.X - 3);
                    Canvas.SetTop(ellipse, point.Y - 3);
                    DrawingCanvas.Children.Add(ellipse);

                    // Номер точки
                    TextBlock text = new TextBlock
                    {
                        Text = (i + 1).ToString(),
                        Foreground = Brushes.DarkBlue,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(point.X + 5, point.Y - 10, 0, 0)
                    };
                    DrawingCanvas.Children.Add(text);
                }

                // Рисуем пикеты
                if (ShowPicketsCheckBox.IsChecked == true && picketPoints.Count > 0)
                {
                    for (int i = 0; i < scaledPicketPoints.Count; i++)
                    {
                        var point = scaledPicketPoints[i];

                        // Пикет
                        Ellipse ellipse = new Ellipse
                        {
                            Width = 8,
                            Height = 8,
                            Fill = Brushes.Green,
                            Stroke = Brushes.DarkGreen,
                            StrokeThickness = 1
                        };
                        Canvas.SetLeft(ellipse, point.X - 4);
                        Canvas.SetTop(ellipse, point.Y - 4);
                        DrawingCanvas.Children.Add(ellipse);

                        // Номер пикета
                        TextBlock text = new TextBlock
                        {
                            Text = $"ПК-{i + 1}",
                            Foreground = Brushes.DarkGreen,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(point.X + 5, point.Y - 10, 0, 0)
                        };
                        DrawingCanvas.Children.Add(text);
                    }
                }

                StatusText.Text = $"Отображен профиль: {ProfileComboBox.Text}. Точек: {profilePoints.Count}" +
                    (picketPoints.Count > 0 ? $", Пикетов: {picketPoints.Count}" : "");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка отображения профиля: {ex.Message}");
            }
        }

        private List<Point> GetProfileCoordinates(int profileId)
        {
            List<Point> points = new List<Point>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT к.широта, к.долгота 
                    FROM Профили_Координаты пк
                    JOIN Координаты к ON пк.координата_id = к.id
                    WHERE пк.профиль_id = @profileId
                    ORDER BY пк.порядковый_номер";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@profileId", profileId);

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

        private List<Point> GetPicketCoordinates(int profileId)
        {
            List<Point> points = new List<Point>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT к.широта, к.долгота, п.номер
                    FROM Пикеты п
                    JOIN Координаты к ON п.координата_id = к.id
                    WHERE п.профиль_id = @profileId
                    ORDER BY п.номер";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@profileId", profileId);

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

            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);

            double scaleX = canvasWidth / (maxX - minX) * 0.8;
            double scaleY = canvasHeight / (maxY - minY) * 0.8;
            double scale = Math.Min(scaleX, scaleY);

            double offsetX = (canvasWidth - (maxX - minX) * scale) / 2 - minX * scale;
            double offsetY = (canvasHeight - (maxY - minY) * scale) / 2 - minY * scale;

            return points.Select(p => new Point(
                p.X * scale + offsetX,
                canvasHeight - (p.Y * scale + offsetY)
            )).ToList();
        }

        private void ExportToPng_Click(object sender, RoutedEventArgs e)
        {
            if (profilePoints.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта!");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                Title = "Сохранить изображение профиля",
                FileName = $"{ProfileComboBox.Text}.png"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var renderBitmap = new RenderTargetBitmap(
                        (int)DrawingCanvas.ActualWidth,
                        (int)DrawingCanvas.ActualHeight,
                        96d, 96d, PixelFormats.Pbgra32);

                    renderBitmap.Render(DrawingCanvas);

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
            if (ProfileComboBox.SelectedValue != null)
            {
                int profileId = (int)ProfileComboBox.SelectedValue;
                LoadAndDrawProfile(profileId);
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