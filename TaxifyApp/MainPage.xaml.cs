using Microsoft.Maui.Controls;

namespace TaxifyApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        // เมื่อกดปุ่ม "รายการ" จะนำทางไปยังหน้า ItemPage
        private async void OnItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ItemPage());
        }

        // เมื่อกดปุ่ม "คำนวณ" จะนำทางไปยังหน้า CalculatePage
        private async void OnCalculateClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CalculatePage());
        }
    }
}