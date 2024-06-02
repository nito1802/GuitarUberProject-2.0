using EditChordsWindow;
using GuitarUberProject;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject.Games_And_Fun
{
    public enum StrumDirection
    {
        Upward,
        Downward
    }

    public class NoteDescription
    {
        public string NoteName { get; set; }
        public string NotePath { get; set; }

        public NoteDescription(string noteName, string notePath)
        {
            NoteName = noteName;
            NotePath = notePath;
        }
    }

    public class StrumNoteDetails
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public int DelayMs { get; set; }
        public int PlayTime { get; set; } //jak dlugo ma grac
        public int Struna { get; set; } //aby dzwieki z tej samej struny nie nakladaly sie na siebie

        public StrumNoteDetails(string path, string name, int struna)
        {
            this.Path = path;
            this.Name = name;
            this.Struna = struna;
        }

        public override string ToString()
        {
            return $"{Name} {Path} Delay: {DelayMs} Struna: {Struna} Time: {PlayTime}";
        }
    }

    public class StrumManager
    {
        public List<SingleStrum> StrumPattern { get; set; }

        public StrumManager(List<SingleStrum> strumPattern)
        {
            StrumPattern = strumPattern;
        }

        public void CalculateDelays()
        {
            //ustawienie delayow
            SetDelays();
            var allPlayedNotes = StrumPattern.SelectMany(a => a.PlayedNotes).ToList();

            //foreach (var item in allPlayedNotes)
            for (int i = 0; i < allPlayedNotes.Count; i++)
            {
                var nextNoteOnStruna = allPlayedNotes.Skip(i + 1).FirstOrDefault(a => a.Struna == allPlayedNotes[i].Struna);

                if (nextNoteOnStruna != null)
                {
                    allPlayedNotes[i].PlayTime = nextNoteOnStruna.DelayMs - allPlayedNotes[i].DelayMs;
                }
                else
                {
                    allPlayedNotes[i].PlayTime = -1;
                }
            }

            //foreach (var item in StrumPattern)
            //{
            //    if(item.StrumDir == StrumDirection.Downward)
            //    {
            //        for (int i = 0; i < item.PlayedNotes.Count; i++)
            //        {
            //            item.PlayedNotes[i].Struna = i + 1;
            //        }
            //    }
            //}
        }

        private void SetDelays()
        {
            int offsetMs = 0;

            foreach (var item in StrumPattern)
            {
                int counter = 0;

                foreach (var strumItem in item.PlayedNotes)
                {
                    int delayMs = item.DelayMs + offsetMs;

                    if (counter == 0)
                    {
                        delayMs = item.DelayBeforeMs + offsetMs;
                    }

                    strumItem.DelayMs = delayMs;
                    offsetMs = delayMs;

                    counter++;
                }
            }
        }
    }

    public class SingleStrum
    {
        public StrumDirection StrumDir { get; set; }
        public int DelayMs { get; set; } //pomiedzy strunami
        public int DelayBeforeMs { get; set; } //jesli jest to kolejny strum z kolei
        public List<StrumNoteDetails> PlayedNotes { get; set; }
        public int TakeNotes { get; set; }
        public int SkipNotes { get; set; }

        public SingleStrum(List<string> notesPaths, StrumDirection strumDir, int delayMs, int delayBeforeMs, int takeNotes = -1, int skipNotes = 0)
        {
            this.StrumDir = strumDir;
            this.DelayMs = delayMs;
            this.DelayBeforeMs = delayBeforeMs;
            this.TakeNotes = takeNotes;
            this.SkipNotes = skipNotes;

            if (StrumDir == StrumDirection.Downward)
            {
                PlayedNotes = notesPaths.Skip(SkipNotes)
                                        .Take(TakeNotes == -1 ? notesPaths.Count : TakeNotes)
                                        .Select(a => new StrumNoteDetails(a, $"{GamesViewModelEdit.NotesDict[a].Name}{GamesViewModelEdit.NotesDict[a].Octave}", GamesViewModelEdit.NotesDict[a].Struna))
                                        .ToList();
            }
            else
            {
                PlayedNotes = notesPaths.AsEnumerable()
                                        .Reverse()
                                        .Skip(SkipNotes)
                                        .Take(TakeNotes == -1 ? notesPaths.Count : TakeNotes)
                                        .Select(a => new StrumNoteDetails(a, $"{GamesViewModelEdit.NotesDict[a].Name}{GamesViewModelEdit.NotesDict[a].Octave}", GamesViewModelEdit.NotesDict[a].Struna))
                                        .ToList();
            }
        }
    }

    public class GamesViewModelEdit : INotifyPropertyChanged
    {
        public int ChordDelayMsDefault { get; } = 20;

        private RelayCommand moveChordLeft;
        private RelayCommand moveChordRight;
        private RelayCommand playAtOnce;
        private RelayCommand playUp;
        private RelayCommand playDown;
        private RelayCommand clearChords;
        private int strumDelayMs;

        public Dictionary<string, string> BrushNotesDict { get; set; }
        public Dictionary<int, string> BrushOctavesDict { get; set; }

        public string[] AllNotes { get; set; } = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

        public List<GameModelEdit> Notes { get; set; }
        public static Dictionary<string, GameModelEdit> NotesDict { get; set; }

        public GamesViewModelEdit()
        {
            //LiteNote[] startNotes = new LiteNote[] {"E", "A", "D", "G", "B", "E" };
            InitDicts();
            InitNotes();
            InitPlayNotes();
            StrumDelayMs = ChordDelayMsDefault;

            if (NotesViewModelLiteVersion.PlayDownAction == null)
            {
                NotesViewModelLiteVersion.PlayDownAction = (a) => PlayDownMethod();
            }
        }

        public ICommand MoveChordLeft
        {
            get
            {
                if (moveChordLeft == null)
                {
                    moveChordLeft = new RelayCommand(param =>
                    {
                        MoveNotesHorizontally(-1);
                    }
                     , param => true);
                }
                return moveChordLeft;
            }
        }

        public ICommand MoveChordRight
        {
            get
            {
                if (moveChordRight == null)
                {
                    moveChordRight = new RelayCommand(param =>
                    {
                        MoveNotesHorizontally(1);
                    }
                     , param => true);
                }
                return moveChordRight;
            }
        }

        public ICommand PlayAtOnce
        {
            get
            {
                if (playAtOnce == null)
                {
                    playAtOnce = new RelayCommand(param =>
                    {
                        var playedNotes = PrepareToPlay();
                        playedNotes.Reverse();

                        DependencyInjection.PlaySoundService.PlayChord(playedNotes, 0);
                    }
                     , param => true);
                }
                return playAtOnce;
            }
        }

        public ICommand PlayDown
        {
            get
            {
                if (playDown == null)
                {
                    playDown = new RelayCommand(param =>
                    {
                        PlayDownMethod();
                    }
                     , param => true);
                }
                return playDown;
            }
        }

        public ICommand PlayUp
        {
            get
            {
                if (playUp == null)
                {
                    playUp = new RelayCommand(param =>
                    {
                        var playedNotes = PrepareToPlay();
                        DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
                    }
                     , param => true);
                }
                return playUp;
            }
        }

        public ICommand ClearChords
        {
            get
            {
                if (clearChords == null)
                {
                    clearChords = new RelayCommand(param =>
                    {
                        Notes.ForEach(a => a.ClearNote());
                        GameModelEdit.UpdateGitarMainChordAction?.Invoke();
                    }
                     , param => true);
                }
                return clearChords;
            }
        }

        public int StrumDelayMs
        {
            get
            {
                return strumDelayMs;
            }

            set
            {
                strumDelayMs = value;
                OnPropertyChanged("StrumDelayMs");
            }
        }

        private void InitPlayNotes()
        {
            Action<int> RefreshNotesOnStrunaAction = (nrStruny) =>
            {
                RefreshStrunaGuitar(nrStruny);
            };

            GameModelEdit.RefreshNotesOnStrunaAction = RefreshNotesOnStrunaAction;
        }

        private void InitNotes()
        {
            Notes = new List<GameModelEdit>();
            NotesDict = new Dictionary<string, GameModelEdit>();

            int notesColumnCount = 15;

            int rowCounter = 1;

            LiteNote[] startNotes = new LiteNote[]
            {
                new LiteNote("E", 5),
                new LiteNote("B", 4),
                new LiteNote("G", 4),
                new LiteNote("D", 4),
                new LiteNote("A", 3),
                new LiteNote("E", 3),
            };

            foreach (var item in startNotes)
            {
                LiteNote currentNote = new LiteNote(item.Name, item.Octave);
                for (int i = 0; i < notesColumnCount; i++)
                {
                    GameModelEdit noteModel = new GameModelEdit(currentNote.Name, currentNote.Octave, i, rowCounter);
                    noteModel.NoteBrush = (Brush)(new BrushConverter().ConvertFrom(BrushNotesDict[currentNote.Name]));
                    noteModel.OctaveBrush = (Brush)(new BrushConverter().ConvertFrom(BrushOctavesDict[currentNote.Octave]));
                    noteModel.NoteBackground = NotesHelper.ChordColor[currentNote.Name];

                    Notes.Add(noteModel);
                    NotesDict.Add($"s{rowCounter}p{i}", noteModel);
                    //S1_P0
                    currentNote = NotesHelper.NextNote(currentNote);
                }

                rowCounter++;
            }
        }

        private void InitDicts()
        {
            BrushNotesDict = new Dictionary<string, string>
            {
                { "A", "#C9006A"},
                { "A#", "#000000"},
                { "B", "#BA7706"},
                { "C", "#129107"},
                { "C#", "#000000"},
                { "D", "#0649AD"},
                { "D#", "#000000"},
                { "E", "#6A0A82"},
                { "F", "#005E4A"},
                { "F#", "#000000"},
                { "G", "#D63E00"},
                { "G#", "#000000"},
            };

            BrushOctavesDict = new Dictionary<int, string>
            {
                { 3, "#E8BD00"},
                { 4, "#1D47C2"},
                { 5, "#008730"},
                { 6, "#B50461"},
            };
        }

        private void PlayDownMethod()
        {
            var playedNotes = PrepareToPlay();
            playedNotes.Reverse();

            DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
        }

        public void RefreshStrunaGuitar(int nrStruny)
        {
            var notesOnStruna = Notes.Where(a => a.Struna == nrStruny).ToList();

            int checkedIdx = -1;
            for (int i = notesOnStruna.Count - 1; i >= 0; i--)
            {
                if (notesOnStruna[i].IsSelected)
                {
                    checkedIdx = i;
                    break;
                }
            }

            for (int i = 0; i < notesOnStruna.Count; i++)
            {
                var note = notesOnStruna[i];

                if (i <= checkedIdx)
                {
                    note.PlayedOrBefore = true;

                    if (note.IsSelected) continue;
                    if (i != checkedIdx) note.ClearIsSelected();
                }
                else
                {
                    if (note.IsSelected) continue;
                    else note.PlayedOrBefore = false;

                    if (i != checkedIdx) note.ClearIsSelected();
                }
            }
        }

        public void RefreshAllStrunyOnGuitar()
        {
            for (int i = 1; i < 7; i++)
            {
                RefreshStrunaGuitar(i);
            }
        }

        public List<GameModelEdit> GetCheckedNotes()
        {
            List<GameModelEdit> res = Notes.Where(a => a.IsSelected).ToList();
            return res;
        }

        public List<string> PrepareToPlay()
        {
            var groupByStruny = Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            foreach (var strunaNotsy in groupByStruny)
            {
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.IsSelected && note.IsNoteEnabled)
                    {
                        playedNotes.Add(note.Mp3Name);
                        break;
                    }
                }
            }

            return playedNotes;
        }

        private void MoveNotesHorizontally(int treshold)
        {
            var selectedNotes = Notes.Where(a => a.IsSelected).ToList();

            bool changed = false;
            var usedNotes = new List<GameModelEdit>();

            for (int i = 0; i < selectedNotes.Count; i++)
            {
                var myNote = selectedNotes[i];

                var nextNote = Notes.FirstOrDefault(a => a.Struna == myNote.Struna && a.Prog == (myNote.Prog + treshold));

                if (nextNote != null)
                {
                    nextNote.IsSelected = true;
                    usedNotes.Add(nextNote);

                    if (!usedNotes.Contains(myNote)) myNote.IsSelected = false;

                    changed = true;
                }
            }

            if (changed)
            {
                RefreshAllStrunyOnGuitar();
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