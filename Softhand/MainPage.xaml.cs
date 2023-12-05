namespace Softhand;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
        {
            CounterBtn.Text = $"Clicked {count} time";
        }
        else
        {
            Sample.RunSample();
            CounterBtn.Text = $"Clicked {count} times and sample started";
        }

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}


