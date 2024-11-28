using System.Collections.ObjectModel;
using TaxifyApp.Models;
using TaxifyApp.Services;

namespace TaxifyApp;

public partial class ItemPage : ContentPage
{
    public ObservableCollection<CalculateModel> Calculations { get; set; } = new ObservableCollection<CalculateModel>();

    public ItemPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadCalculations();
    }

    private async void LoadCalculations()
    {
        FirestoreService service = new FirestoreService();
        try
        {
            var calculations = await service.GetAllCalculations();

            // ล้างรายการเก่าก่อนเพิ่มรายการใหม่
            Calculations.Clear();
            foreach (var calc in calculations)
            {
                Calculations.Add(calc);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("ข้อผิดพลาด", $"ไม่สามารถโหลดข้อมูลได้: {ex.Message}", "ตกลง");
        }
    }
}
