using SoftApplication = Microsoft.Maui.Controls.Application;
namespace Softhand.Infrastructure.Converters;

public class SipStatusColorConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 1 || values[0] is not bool registered)
            return Colors.OrangeRed;

        var theme = Microsoft.Maui.Controls.Application.Current.RequestedTheme;
        var resources = Microsoft.Maui.Controls.Application.Current.Resources;

        string key = (registered, theme) switch
        {
            (true, AppTheme.Dark) => "OnSipOnDark",
            (true, AppTheme.Light) => "OnSipOn",
            (false, AppTheme.Dark) => "OnSipOffDark",
            (false, AppTheme.Light) => "OnSipOff",
            _ => string.Empty
        };

        return resources.TryGetValue(key, out var color) ? color : Colors.OrangeRed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

}
