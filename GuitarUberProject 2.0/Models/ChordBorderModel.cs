using System.ComponentModel;
using System.Windows.Media;

namespace GitarUberProject.Models
{
    public class ChordBorderModel : INotifyPropertyChanged
    {
        private int height;

        public ChordBorderModel(Brush backgroundBrush, int height)
        {
            BackgroundBrush = backgroundBrush;
            Height = height;
        }

        public Brush BackgroundBrush { get; set; }

        public int Height
        {
            get => height;

            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}