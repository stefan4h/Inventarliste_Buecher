using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Inventarliste_Buecher.Annotations;
using Microsoft.Win32;

namespace Inventarliste_Buecher
{
    public class Book:INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _author;
        public string Author
        {
            get => _author;
            set {
                _author = value;
                OnPropertyChanged();
            }
        }

        public static void Save(List<Book> books)
        {
            StreamWriter writer=new StreamWriter("books.csv");
            foreach (Book book in books)
            {
                writer.WriteLine($"{book.Title},{book.Author}");
            }
            writer.Close();
        }

        public static List<Book> Read()
        {
            try
            {
                List<Book> books = new List<Book>();
                StreamReader reader = new StreamReader("books.csv");
                while (!reader.EndOfStream) {
                    string[] parts = reader.ReadLine().Split(',');
                    books.Add(new Book() { Title = parts[0], Author = parts[1] });
                }

                return books;
            }
            catch (Exception e)
            {
                return new List<Book>();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
