using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TakeHome.Services
{
    class AmountToCurrencyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String currencyAmount = null;
            if (value != null)
            {
                currencyAmount = String.Format("{0:C2}", value);
            }
            return currencyAmount;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
