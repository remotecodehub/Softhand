using Application = Microsoft.Maui.Controls.Application;
using Window = Microsoft.Maui.Controls.Window;
namespace Softhand;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
    }
}

