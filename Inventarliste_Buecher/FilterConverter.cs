using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Inventarliste_Buecher
{
    public class FilterConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Book> books = value as ObservableCollection<Book>;
            string searchToken = parameter as string;
            if (string.IsNullOrWhiteSpace((string) parameter))
                return books;
            return books.Where(b =>
                    b.Title.ToLower().Contains(searchToken.ToLower()) ||
                    b.Author.ToLower().Contains(searchToken.ToLower()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Book> books = value as ObservableCollection<Book>;
            string searchToken = parameter as string;
            if (string.IsNullOrWhiteSpace((string)parameter))
                return books;
            return books.Where(b =>
                b.Title.ToLower().Contains(searchToken.ToLower()) ||
                b.Author.ToLower().Contains(searchToken.ToLower()));
        }
    }
}
