using GuitarUberProject;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject
{
    public class NoteModel : INotifyPropertyChanged
    {
        private double DisabledNoteOpacity = 0.4;

        private Brush octaveBrush;
        private Brush noteBrush;
        private RelayCommand playNote;
        private RelayCommand deselectNote;
        private RelayCommand moveNoteToPlaylist;
        private bool isSelected;
        private double noteOpacity;
        private bool playedOrBefore; //nuta grana lub bedaca za grana na tej samej strunie (opacity: 1, inne na tej strunie opacity mniejsze)
        private bool isNoteEnabled = true;
        public static Action<int, string> PlayNoteAction { get; set; } //idx struny, pathToMp3 - statyczny action, zeby dalo sie dostac do gitary z viewmodelu
        public static Action<int> RefreshNotesOnStrunaAction { get; set; } //jesli klikniety checkbox, wtedy robie refresh  (args: struna i prog, isSelected)
        public static Action SetGitarFocusAction { get; set; }
        public static Action UpdateGitarMainChordAction { get; set; }
        public static Action<NoteModel> MoveNoteToPlaylistAction { get; set; }

        public NoteModel(string name, int octave, int prog, int struna)
        {
            Name = name;
            Octave = octave;
            Prog = prog;
            Struna = struna;
            PlayedOrBefore = false;

            Mp3Name = $"s{Struna}p{Prog}";
        }

        public string Name { get; set; }
        public int Octave { get; set; }
        public int Prog { get; set; }
        public int Struna { get; set; }
        public string Mp3Name { get; set; }
        public Brush NoteBackground { get; set; } = Brushes.Red;

        public ICommand PlayNote
        {
            get
            {
                if (playNote == null)
                {
                    playNote = new RelayCommand(param =>
                    {
                        DependencyInjection.PlaySoundService.PlayNote(Struna, Mp3Name);

                        //if (PlayNoteAction != null)
                        //{
                        //    PlayNoteAction(Struna, Mp3Name);
                        //}
                        //else
                        //{
                        //    throw new NotImplementedException("Brakuje PlayNoteAction!");
                        //}

                        SetGitarFocusAction?.Invoke();
                    }
                     , param => true);
                }
                return playNote;
            }
        }

        public ICommand DeselectNote
        {
            get
            {
                if (deselectNote == null)
                {
                    deselectNote = new RelayCommand(param =>
                    {
                        IsSelected = false;
                    }
                     , param => true);
                }
                return deselectNote;
            }
        }

        public ICommand MoveNoteToPlaylist
        {
            get
            {
                if (moveNoteToPlaylist == null)
                {
                    moveNoteToPlaylist = new RelayCommand(param =>
                    {
                        MoveNoteToPlaylistAction?.Invoke(this);
                    }
                     , param => true);
                }
                return moveNoteToPlaylist;
            }
        }

        public Brush OctaveBrush
        {
            get
            {
                return octaveBrush;
            }

            set
            {
                octaveBrush = value;
                OnPropertyChanged("OctaveBrush");
            }
        }

        public Brush NoteBrush
        {
            get
            {
                return noteBrush;
            }

            set
            {
                noteBrush = value;
                OnPropertyChanged("NoteBrush");
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");

                RefreshNotesOnStrunaAction?.Invoke(Struna);
                UpdateGitarMainChordAction?.Invoke();
                SetGitarFocusAction?.Invoke();
            }
        }

        public bool IsSelectedWithoutFocus
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");

                RefreshNotesOnStrunaAction?.Invoke(Struna);
                UpdateGitarMainChordAction?.Invoke();
            }
        }

        public bool IsSelecteOnlySet
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");

                RefreshNotesOnStrunaAction?.Invoke(Struna);
                //UpdateGitarMainChordAction?.Invoke();
            }
        }

        public static void UpdateGitarMainChord()
        {
            UpdateGitarMainChordAction?.Invoke();
        }

        public double NoteOpacity
        {
            get
            {
                return noteOpacity;
            }

            set
            {
                noteOpacity = value;
                OnPropertyChanged("NoteOpacity");
            }
        }

        public bool PlayedOrBefore //dotyczacy opacity
        {
            get
            {
                return playedOrBefore;
            }

            set
            {
                playedOrBefore = value;

                if (playedOrBefore) NoteOpacity = 1;
                else NoteOpacity = DisabledNoteOpacity;
            }
        }

        public bool IsNoteEnabled
        {
            get
            {
                return isNoteEnabled;
            }

            set
            {
                isNoteEnabled = value;
                OnPropertyChanged("IsNoteEnabled");
            }
        }

        public void ClearIsSelected()
        {
            isSelected = false;
            OnPropertyChanged("IsSelected");
        }

        public void ClearNote()
        {
            isSelected = false;
            OnPropertyChanged("IsSelected");
            PlayedOrBefore = false;
        }

        public override string ToString()
        {
            return $"{Name}{Octave} S{Struna}_P{Prog} IsSel: {IsSelected} playedOrBef: {PlayedOrBefore}";
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