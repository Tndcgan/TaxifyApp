namespace TaxifyApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// ใช้ NavigationPage เพื่อให้สามารถนำทางระหว่างหน้าได้
            MainPage = new NavigationPage(new MainPage());

	}
}
