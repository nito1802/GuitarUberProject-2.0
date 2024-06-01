using EditChordsWindow;
using GitarUberProject.ViewModels;
using GuitarUberProject;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject.Models
{
    public class KlocekNoteDetails
    {
        private double xPos;
        private double yPos;
        private double noteWidth;
        private double zIndex;
        private double klocekOpacity = 1;

        public KlocekNoteDetails()
        {
        }

        public void AssignBackground()
        {
            NoteBackground = NotesHelper.ChordColor[Name];
        }

        public string Name { get; set; }
        public int Octave { get; set; }
        public int Prog { get; set; }
        public int Struna { get; set; }
        public string Mp3Name { get; set; }
        public bool IsChord { get; set; }

        [JsonIgnore]
        public Brush NoteBackground { get; set; } = Brushes.Red;

        public double XPos
        {
            get
            {
                return xPos;
            }

            set
            {
                xPos = value;
                OnPropertyChanged("XPos");
            }
        }

        public double YPos
        {
            get
            {
                return yPos;
            }

            set
            {
                yPos = value;
                OnPropertyChanged("YPos");
            }
        }

        public double NoteWidth
        {
            get
            {
                return noteWidth;
            }

            set
            {
                noteWidth = value;
                OnPropertyChanged("NoteWidth");
            }
        }

        public double ZIndex
        {
            get
            {
                return zIndex;
            }

            set
            {
                zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }

        [JsonIgnore]
        public double KlocekOpacity
        {
            get => klocekOpacity;
            set
            {
                klocekOpacity = value;
                OnPropertyChanged("KlocekOpacity");
            }
        }

        public override string ToString()
        {
            return $"X: {XPos} Y: {YPos} Path: {Mp3Name}";
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

    public class KlocekChordModel : INotifyPropertyChanged, ICloneable
    {
        //private Brush backgroundBrush;
        private RelayCommand removeFromPlaylist;

        private RelayCommand playNote;
        private double xPos;
        private double yPos;
        private double zIndex;
        private double klocekOpacity = 1;
        private double chordWidth;
        private double chordHeight;
        private Brush chordBackground;
        private Brush chordBorder;
        private string chordName;
        private Visibility klocekVisibility;
        private int channelNr;

        [JsonIgnore]
        public Point DeltaMouse { get; set; }

        public static Action<KlocekChordModel> RemoveKlocekFromPlaylist { get; set; }
        public static Action<StrumViewModel> PlayChordFromPlaylist { get; set; }

        public KlocekChordModel()
        {
        }

        //singleNote
        public KlocekChordModel(string name, int octave, int prog, int struna)
        {
            Name = name;
            Octave = octave;
            Prog = prog;
            Struna = struna;

            NoteBackground = NotesHelper.ChordColor[Name];
        }

        //private RelayCommand mouseEnterCommand;
        public NotesViewModelLiteVersion ChordModel { get; set; }

        public ObservableCollection<KlocekNoteDetails> NotesInChord { get; set; } = new ObservableCollection<KlocekNoteDetails>();
        public string Name { get; set; }
        public int Octave { get; set; }
        public int Prog { get; set; }
        public int Struna { get; set; }
        public string Mp3Name { get; set; }
        public bool IsChord { get; set; }
        public StrumViewModel StrumViewModel { get; set; }
        public List<string> NotesToPlay { get; set; }

        [JsonIgnore]
        public Brush NoteBackground { get; set; } = Brushes.Red;

        public double XPos
        {
            get
            {
                return xPos;
            }

            set
            {
                xPos = value;
                OnPropertyChanged("XPos");
            }
        }

        public double YPos
        {
            get
            {
                return yPos;
            }

            set
            {
                yPos = value;
                OnPropertyChanged("YPos");
            }
        }

        public double ZIndex
        {
            get
            {
                return zIndex;
            }

            set
            {
                zIndex = value;
                OnPropertyChanged("ZIndex");
            }
        }

        public double ChordWidth
        {
            get
            {
                return chordWidth;
            }

            set
            {
                chordWidth = value;
                OnPropertyChanged("ChordWidth");
            }
        }

        public double ChordHeight
        {
            get
            {
                return chordHeight;
            }

            set
            {
                chordHeight = value;
                OnPropertyChanged("ChordHeight");
            }
        }

        [JsonIgnore]
        public double KlocekOpacity
        {
            get => klocekOpacity;
            set
            {
                klocekOpacity = value;
                OnPropertyChanged("KlocekOpacity");
            }
        }

        [JsonIgnore]
        public Brush ChordBackground
        {
            get => chordBackground;
            set
            {
                chordBackground = value;
                OnPropertyChanged("ChordBackground");
            }
        }

        [JsonIgnore]
        public Brush ChordBorder
        {
            get => chordBorder;
            set
            {
                chordBorder = value;
                OnPropertyChanged("ChordBorder");
            }
        }

        public string ChordName
        {
            get => chordName;
            set
            {
                chordName = value;
                OnPropertyChanged("ChordName");
            }
        }

        public Visibility KlocekVisibility
        {
            get => klocekVisibility;
            set
            {
                klocekVisibility = value;
                OnPropertyChanged("KlocekVisibility");
            }
        }

        public int ChannelNr
        {
            get => channelNr;
            set
            {
                channelNr = value;
                OnPropertyChanged("ChannelNr");
            }
        }

        //channelNr
        [JsonIgnore]
        public ICommand RemoveFromPlaylist
        {
            get
            {
                if (removeFromPlaylist == null)
                {
                    removeFromPlaylist = new RelayCommand(param =>
                    {
                        RemoveKlocekFromPlaylist?.Invoke(this);
                    }
                     , param => true);
                }
                return removeFromPlaylist;
            }
        }

        [JsonIgnore]
        public ICommand PlayNote
        {
            get
            {
                if (playNote == null)
                {
                    playNote = new RelayCommand(param =>
                    {
                        if (!IsChord)
                        {
                            DependencyInjection.PlaySoundService.PlayNote(Struna, Mp3Name);
                        }
                        else
                        {
                            PlayChordFromPlaylist?.Invoke(StrumViewModel);
                        }
                    }
                     , param => true);
                }
                return playNote;
            }
        }

        public object Clone()
        {
            KlocekChordModel klocekClone = new KlocekChordModel();

            if (!this.IsChord)
            {
                klocekClone.KlocekOpacity = 1;
                klocekClone.Mp3Name = this.Mp3Name;
                klocekClone.Name = this.Name;
                klocekClone.NoteBackground = this.NoteBackground;
                klocekClone.Octave = this.Octave;
                klocekClone.Prog = this.Prog;
                klocekClone.Struna = this.Struna;
                klocekClone.XPos = this.XPos;
                klocekClone.YPos = this.YPos;
                klocekClone.ZIndex = this.ZIndex;
            }
            else
            {
                NotesViewModelLiteVersion clone = (NotesViewModelLiteVersion)this.ChordModel.Clone();

                klocekClone.XPos = this.XPos;
                klocekClone.YPos = this.YPos;
                klocekClone.KlocekOpacity = 1;
                klocekClone.ChordModel = clone;
                klocekClone.ChordWidth = this.ChordWidth;
                klocekClone.ChordHeight = this.ChordHeight;
                klocekClone.ChordBackground = NotesHelper.ChordColor[clone.ChordName];
                klocekClone.ChordBorder = NotesHelper.ChordColorNoOpacity[clone.ChordName];
                klocekClone.ChordName = clone.ChordFullName;

                foreach (var noteInChord in this.NotesInChord)
                {
                    var myNote = new KlocekNoteDetails();// item.Name, item.Octave, item.Prog, item.Struna) { Mp3Name = item.Mp3Name, XPos = item.XPos, YPos = item.YPos };
                    myNote.Name = noteInChord.Name;
                    myNote.Octave = noteInChord.Octave;
                    myNote.Prog = noteInChord.Prog;
                    myNote.Struna = noteInChord.Struna;
                    myNote.Mp3Name = noteInChord.Mp3Name;
                    myNote.XPos = noteInChord.XPos;
                    myNote.YPos = noteInChord.YPos;
                    myNote.KlocekOpacity = 1;
                    myNote.AssignBackground();

                    klocekClone.NotesInChord.Add(myNote);
                }

                klocekClone.NotesToPlay = new List<string>();
                foreach (var noteToPlay in this.NotesToPlay)
                {
                    klocekClone.NotesToPlay.Add(noteToPlay);
                }

                klocekClone.StrumViewModel = this.StrumViewModel;
                klocekClone.IsChord = true;
            }
            klocekClone.channelNr = this.ChannelNr;

            return klocekClone;
        }

        //public ICommand TrimEnterCommand
        //{
        //    get
        //    {
        //        if (mouseEnterCommand == null)
        //        {
        //            mouseEnterCommand = new RelayCommand(param =>
        //            {
        //                Border kloc = ((MouseEventArgs)param).Source as Border;
        //                var x = Canvas.GetLeft(kloc);
        //                var y = Canvas.GetTop(kloc);

        //                Canvas.SetLeft(TempKlocekUi, x);
        //                Canvas.SetTop(TempKlocekUi, y);
        //            }
        //             , param => true);
        //        }
        //        return mouseEnterCommand;
        //    }
        //}

        public override string ToString()
        {
            return $"X: {XPos} Y: {YPos} Name: {Name} isChord: {IsChord}";
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