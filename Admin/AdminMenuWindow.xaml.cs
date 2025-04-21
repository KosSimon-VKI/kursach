using System.Windows;
using Агеенков_курсач.Admin.Договоры;
using Агеенков_курсач.Admin.Заказчики;
using Агеенков_курсач.Admin.Измерения;
using Агеенков_курсач.Admin.Координаты;
using Агеенков_курсач.Admin.Оборудование;
using Агеенков_курсач.Admin.Операторы;
using Агеенков_курсач.Admin.Отчеты;
using Агеенков_курсач.Admin.Пикеты;
using Агеенков_курсач.Admin.Площади;
using Агеенков_курсач.Admin.Пользователи;
using Агеенков_курсач.Admin.Проекты;
using Агеенков_курсач.Admin.Профили;
using Агеенков_курсач.Admin.ТипыПользователей;


namespace Агеенков_курсач.Admin
{
    public partial class AdminMenuWindow : Window
    {
        public AdminMenuWindow()
        {
            InitializeComponent();
        }

        private void Contracts_Click(object sender, RoutedEventArgs e)
        {
            new ManageContractsWindow().Show();
            this.Close();
        }
        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            new ManageCustomersWindow().Show();
            this.Close();
        }
        private void Measurements_Click(object sender, RoutedEventArgs e) 
        {
            new ManageMeasurementsWindow().Show();
            this.Close();
        }
        private void Coordinates_Click(object sender, RoutedEventArgs e)
        {
            new ManageCoordinatesWindow().Show();
            this.Close();
        }
        private void Equipment_Click(object sender, RoutedEventArgs e)
        {
            new ManageEquipmentWindow().Show();
            this.Close();
        }
        private void Operators_Click(object sender, RoutedEventArgs e)
        {
            new ManageOperatorsWindow().Show();
            this.Close();
        }
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            new ManageReportsWindow().Show();
            this.Close();
        }
        private void Pickets_Click(object sender, RoutedEventArgs e)
        {
            new ManagePicketsWindow().Show();
            this.Close();
        }
        private void Areas_Click(object sender, RoutedEventArgs e)
        { 
            new ManageAreasWindow().Show(); 
            this.Close(); 
        }
        private void Users_Click(object sender, RoutedEventArgs e)
        {
            new ManageUsersWindow().Show();
            this.Close();
        }
        private void Projects_Click(object sender, RoutedEventArgs e)
        {
            new ManageProjectsWindow().Show();
            this.Close();
        }
        private void Profiles_Click(object sender, RoutedEventArgs e)
        {
            new ManageProfilesWindow().Show();
            this.Close();
        }
        private void UserTypes_Click(object sender, RoutedEventArgs e)
        {
            new ManageUserTypesWindow().Show();
            this.Close();
        }

        private void ViewAreas_Click(object sender, RoutedEventArgs e)
        {
            new AreaViewerWindow().Show();
            this.Close();
        }

        private void ViewProfiles_Click(object sender, RoutedEventArgs e)
        {
            new ProfileViewerWindow().Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }
}