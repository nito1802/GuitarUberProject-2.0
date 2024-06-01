using GitarUberProject;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace EditChordsWindow
{
    public class NoteModelLiteVersion : INotifyPropertyChanged, ICloneable
    {
        public static Action<int> UpdateNotesNamesAction { get; set; }
        public static Brush DefaultBtnBackground { get; set; }
        public static Brush DefaultBtnHover { get; set; }

        private Brush myBackground;
        private Brush hoverBackground;
        private RelayCommand clickNote;
        private RelayCommand clearNote;
        private CheckedFinger checkedFinger;
        private CheckedFinger hoverFinger;
        private int prog;
        private int struna;  //liczymy od 1

        public NoteModelLiteVersion(int struna, int prog)
        {
            this.Struna = struna;
            this.Prog = prog;
            this.MyBackground = DefaultBtnBackground;
            this.HoverBackground = DefaultBtnHover;
        }

        [JsonIgnore]
        public Brush MyBackground
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

        [JsonIgnore]
        public ICommand ClickNote
        {
            get
            {
                if (clickNote == null)
                {
                    clickNote = new RelayCommand(param =>
                    {
                        CheckedFinger = HoverFinger;

                        if (UpdateNotesNamesAction != null) UpdateNotesNamesAction(Struna);
                    }
                     , param => true);
                }
                return clickNote;
            }
        }

        [JsonIgnore]
        public ICommand ClearNote
        {
            get
            {
                if (clearNote == null)
                {
                    clearNote = new RelayCommand(param =>
                    {
                        CheckedFinger = CheckedFinger.None;

                        if (UpdateNotesNamesAction != null) UpdateNotesNamesAction(Struna);
                    }
                     , param => true);
                }
                return clearNote;
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
                MyBackground = NotesHelper.FingerBrushes[value];
            }
        }

        [JsonIgnore]
        public Brush HoverBackground
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

        [JsonIgnore]
        public CheckedFinger HoverFinger
        {
            get
            {
                return hoverFinger;
            }

            set
            {
                hoverFinger = value;
                HoverBackground = NotesHelper.FingerBrushes[value];
            }
        }

        public int Prog //od 0
        {
            get
            {
                return prog;
            }

            set
            {
                prog = value;
            }
        }

        public int Struna
        {
            get
            {
                return struna;
            }

            set
            {
                struna = value;
            }
        }

        public object Clone()
        {
            NoteModelLiteVersion clone = new NoteModelLiteVersion(this.Struna, this.Prog);
            clone.CheckedFinger = this.CheckedFinger;
            clone.MyBackground = this.MyBackground;
            clone.HoverFinger = this.HoverFinger;

            return clone;
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
    }
}