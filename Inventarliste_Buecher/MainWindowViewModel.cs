using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using BespokeFusion;
using Inventarliste_Buecher.Annotations;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Inventarliste_Buecher
{
    public class MainWindowViewModel:INotifyPropertyChanged
    {
        private string _searchToken;
        private bool _enableButtons;
        public string SearchToken
        {
            get => _searchToken;
            set
            {
                _searchToken = value;
                OnPropertyChanged();
                if (string.IsNullOrWhiteSpace(_searchToken))
                {
                    Books=new ObservableCollection<Book>(Book.Read());
                    OnPropertyChanged("Books");
                    _enableButtons = true;
                }
                else
                {
                    Books=new ObservableCollection<Book>(Book.Read());
                    Books=new ObservableCollection<Book>(Books.Where(b => b.Title.ToLower().Contains(_searchToken.ToLower()) || b.Author.ToLower().Contains(_searchToken.ToLower())));
                    OnPropertyChanged("Books");
                    _enableButtons = false;
                }
            }
        }
        public ObservableCollection<Book> Books { get; set; }
        private Book _selectedBook;
        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        public ICommand ImportFile=>new RelayCommand(o =>
        {
            OpenFileDialog fileDialog=new OpenFileDialog();
            fileDialog.Filter = "CSV Files (*.csv)|*.csv";
            fileDialog.InitialDirectory = @"c:\desktop\";
            bool? result=fileDialog.ShowDialog();
            if (result==true)
            {
                string[] lines = File.ReadAllLines(fileDialog.FileName);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    if(parts.Length==2)
                        Books.Insert(FindIndex(parts[0]), new Book() { Title = parts[0], Author = parts[1] });
                    else
                        Books.Insert(FindIndex(parts[0]), new Book() { Title = parts[0] });
                    UpdateStorage();
                }
            }
        },o=>_enableButtons);

        public ICommand ExportFile=>new RelayCommand(o =>
        {
            SaveFileDialog fileDialog=new SaveFileDialog();
            fileDialog.OverwritePrompt = true;
            fileDialog.InitialDirectory = @"c:\desktop\";
            fileDialog.FileName = $"buecher{DateTime.Now.ToString("dd-MM-yyyy")}.csv";
            var result = fileDialog.ShowDialog();
            if (result==DialogResult.OK)
            {
                File.Copy("books.csv",fileDialog.FileName,true);
            }
        },o=>_enableButtons&&Books.Count>0);

        public ICommand AddCommand=>new RelayCommand(o =>
        {
            Books.Insert(FindIndex(),new Book(){Title = SelectedBook.Title,Author = SelectedBook.Author});
            SelectedBook=new Book();
            UpdateStorage();
        },o=>
            SelectedBook!=null&&!string.IsNullOrWhiteSpace(SelectedBook.Title)&&!Books.Contains(SelectedBook)&&_enableButtons);

        public ICommand ClearCommand=>new RelayCommand(o=>SelectedBook=new Book(),
            o=>(!string.IsNullOrWhiteSpace(SelectedBook.Title)||!string.IsNullOrWhiteSpace(SelectedBook.Author))&&_enableButtons);

        public ICommand RemoveCommand=>new RelayCommand(o =>
        {
            var messageBox = new CustomMaterialMessageBox
            {
                TxtMessage = {Text = $"Sind Sie sicher, dass Sie das Buch {SelectedBook.Title} löschen wollen?"},
                TxtTitle = {Text = "Bestätigungsfenster"},
                BtnOk = { Content = "Ja" },
                BtnCancel = { Content = "Nein" },
                BtnCopyMessage = {Visibility = Visibility.Collapsed}
            };
            messageBox.ShowDialog();
            if (messageBox.Result==MessageBoxResult.OK)
            {
                Books.Remove(SelectedBook);
                SelectedBook = new Book();
                UpdateStorage();
            }
        },o=>Books.Contains(SelectedBook)&&_enableButtons);

        private int FindIndex()
        {
            int index = 0;
            foreach (Book book in Books)
            {
                if (SelectedBook.Title.CompareTo(book.Title) < 0)
                    return index;
                index++;
            }
            return index;
        }
        private int FindIndex(string title)
        {
            int index = 0;
            foreach (Book book in Books) {
                if (title.CompareTo(book.Title) < 0)
                    return index;
                index++;
            }
            return index;
        }
        private void UpdateStorage()
        {
            Book.Save(Books.ToList());
        }

        public MainWindowViewModel()
        {
            Books=new ObservableCollection<Book>(Book.Read());
            _enableButtons = true;
            SelectedBook=new Book();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
