using System.ComponentModel;

namespace GuitarUberProject_2._0.ViewModels
{
    public class ChordNameViewModel : INotifyPropertyChanged
    {
        private string chordName;

        public string ChordName
        {
            get { return chordName; }
            set
            {
                chordName = value;
                OnPropertyChanged(nameof(ChordName));
            }
        }

        private string chordType;

        public string ChordType
        {
            get { return chordType; }
            set
            {
                chordType = value;
                OnPropertyChanged(nameof(ChordType));
            }
        }

        public void SetData(string chordName, string chordType)
        {
            ChordName = chordName;
            ChordType = chordType;
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