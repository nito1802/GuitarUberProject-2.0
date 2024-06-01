using AudioMaker.NAudiox.Services;
using GitarUberProject;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace EditChordsWindow
{
    public enum CheckedFinger
    {
        None,
        firstFinger,
        secondFinger,
        thirdFinger,
        fourthFinger,
        Other
    }

    public class NoteModel : INotifyPropertyChanged, ICloneable
    {
        private readonly PlaySoundService _playSoundService;

        //public static Action<int> UpdateNotesNamesAction { get; set; }
        public static SolidColorBrush DefaultBtnBackground { get; set; }

        public static SolidColorBrush DefaultBtnHover { get; set; }
        public static Dictionary<CheckedFinger, SolidColorBrush> FingerDict { get; set; }

        private SolidColorBrush myBackground;
        private SolidColorBrush hoverBackground;
        private RelayCommand clickNote;
        private RelayCommand clearNote;
        private CheckedFinger checkedFinger;
        private CheckedFinger hoverFinger;
        private bool isEnabled = true;

        public NoteModel(int struna, int prog)
        {
            this.Struna = struna;
            this.Prog = prog;
            this.MyBackground = DefaultBtnBackground;
            this.HoverBackground = DefaultBtnHover;
        }

        public int Struna { get; set; } //liczymy od 1
        public int Prog { get; set; } //od 0

        public SolidColorBrush MyBackground
        {
            get
            {
                return myBackground;
            }

            set
            {
                myBackground = value;
                OnPropertyChanged("MyBackground");
            }
        }

        public CheckedFinger CheckedFinger
        {
            get
            {
                return checkedFinger;
            }

            set
            {
                checkedFinger = value;
                MyBackground = FingerDict[value];
            }
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public SolidColorBrush HoverBackground
        {
            get
            {
                return hoverBackground;
            }

            set
            {
                hoverBackground = value;
                OnPropertyChanged("HoverBackground");
            }
        }

        public CheckedFinger HoverFinger
        {
            get
            {
                return hoverFinger;
            }

            set
            {
                hoverFinger = value;
                HoverBackground = FingerDict[value];
            }
        }

        public ICommand ClickNote
        {
            get
            {
                if (clickNote == null)
                {
                    clickNote = new RelayCommand(param =>
                    {
                        CheckedFinger = HoverFinger;
                    }
                     , param => true);
                }
                return clickNote;
            }
        }

        public ICommand ClearNote
        {
            get
            {
                if (clearNote == null)
                {
                    clearNote = new RelayCommand(param =>
                    {
                        CheckedFinger = CheckedFinger.None;
                    }
                     , param => true);
                }
                return clearNote;
            }
        }

        public override string ToString()
        {
            return $"{CheckedFinger} Struna: {Struna} Prog: {Prog}";
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

        public object Clone()
        {
            NoteModel copy = new NoteModel(this.Struna, this.Prog);
            copy.CheckedFinger = this.CheckedFinger;

            return copy;
        }
    }
}