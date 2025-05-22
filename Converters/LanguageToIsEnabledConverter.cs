using System.Globalization;

namespace Story_Teller.Converters;

public class LanguageToIsEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string currentLanguage && parameter is string buttonLanguage)
        {
            return currentLanguage != buttonLanguage;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}