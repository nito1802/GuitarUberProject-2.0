using System.ComponentModel;

namespace GitarUberProject.Models
{
    public class MixerModel : INotifyPropertyChanged
    {
        private string name;
        private double vol;
        private bool isTurn;

        public static Action<string, bool> UpdateKlocekVisibilityAction { get; set; }

        public MixerModel()
        {
        }

        public MixerModel(string name, double vol, bool isTurn)
        {
            Name = name;
            Vol = vol;
            IsTurn = isTurn;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public double Vol
        {
            get => vol;
            set
            {
                vol = value;
                OnPropertyChanged("Vol");
            }
        }

        public bool IsTurn
        {
            get => isTurn;
            set
            {
                isTurn = value;
                OnPropertyChanged("IsTurn");
                UpdateKlocekVisibilityAction?.Invoke(Name, value);
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