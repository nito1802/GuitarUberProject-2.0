using EditChordsWindow;
using GitarUberProject.Helperes;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using GuitarUberProject;
using GuitarUberProject_2._0.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject
{
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

    public class NotesViewModel : INotifyPropertyChanged
    {
        public int ChordDelayMsDefault { get; } = 20;
        public static double Bpm { get; set; } = 100;
        public static double BeatWidth { get; } = 100;
        public List<StrumModel> GlobalStrumPattern { get; set; } = new List<StrumModel>();
        public List<EditStrumModel> GlobalEditStrumModels { get; set; } = new List<EditStrumModel>();

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

        public List<NoteModel> Notes { get; set; }
        public static Dictionary<string, NoteModel> NotesDict { get; set; }
        public static Dictionary<string, NoteModel> NotesNameDict { get; set; } = new Dictionary<string, NoteModel>();

        /*
         PlaySoundService _playSoundService;
        private readonly PlaySoundMapper _playSoundMapper;
         */

        public NotesViewModel()
        {
            //LiteNote[] startNotes = new LiteNote[] {"E", "A", "D", "G", "B", "E" };
            InitDicts();
            InitNotes();
            InitPlayNotes();
            StrumDelayMs = ChordDelayMsDefault;

            if (NotesViewModelLiteVersion.PlayDownAction == null)
            {
                NotesViewModelLiteVersion.PlayDownAction = (a) => PlayDownMethod(a);
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
                        if (GlobalStrumPattern.Any())
                        {
                            List<string> playedNotesPaths = PrepareToPlayForChord();
                            GlobalStrumPattern.ForEach(a => a.AssignPaths(playedNotesPaths));
                            StrumViewModel strumManager = new StrumViewModel(GlobalStrumPattern);
                            strumManager.CalculateDelays();

                            var mappedStrumViewModel = DependencyInjection.PlaySoundMapper.MapStrumViewModel(strumManager);
                            DependencyInjection.PlaySoundService.MyExtraPlayChordWithStrumPattern(mappedStrumViewModel);
                        }
                        else
                        {
                            var playedNotes = PrepareToPlay();
                            playedNotes.Reverse();
                            DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
                        }
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

        public static Func<List<ToggleButton>> GetPianoButtonsFunc { get; set; }

        public ICommand PlayDownGuitarControl
        {
            get
            {
                if (playDown == null)
                {
                    playDown = new RelayCommand(param =>
                    {
                        //string pathToJson = @"C:\Users\dante\Desktop\Istotne\MojeDane\2023\grudzień\18_12_2023\Inne\serializedChords.json";

                        //TODO: przenalizować kod i dostosować piano
                        //string pathToJson = @"C:\Users\dante\Desktop\Istotne\MojeDane\2023\grudzień\26_12_2023\Inne\serializedChordsMy.json";
                        //var jsonContent = File.ReadAllText(pathToJson);
                        //var chordsVariants = JsonConvert.DeserializeObject<List<MyChord>>(jsonContent);
                        //Stopwatch sw = Stopwatch.StartNew();
                        //for (int i = 0; i < chordsVariants.Count; i++)
                        //{
                        //    var item = chordsVariants[i];
                        //    PlayChordPiano(item, strumDelayMs, i);
                        //}
                        //sw.Stop();
                        //return;

                        var pianoKeys = GetPianoButtonsFunc?.Invoke();
                        var pianoCheckedKeys = pianoKeys.Where(a => a.IsChecked == true).Select(b => b.Content.ToString()).ToList();
                        if (pianoCheckedKeys.Any())
                        {
                            DependencyInjection.PlaySoundService.PlayChordPiano(pianoCheckedKeys, strumDelayMs, PathService.GetBasePathToRecords());
                        }
                        else
                        {
                            var playedNotes = PrepareToPlay();
                            playedNotes.Reverse();
                            DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
                        }
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
                        var pianoKeys = GetPianoButtonsFunc?.Invoke();
                        foreach (var item in pianoKeys)
                        {
                            item.IsChecked = false;
                        }

                        NoteModel.UpdateGitarMainChord();
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

            NoteModel.RefreshNotesOnStrunaAction = RefreshNotesOnStrunaAction;
        }

        private void InitNotes()
        {
            Notes = new List<NoteModel>();
            NotesDict = new Dictionary<string, NoteModel>();

            int notesColumnCount = 15;

            int rowCounter = 1;

            LiteNote[] startNotes = new LiteNote[]
            {
                new LiteNote("E", 4),
                new LiteNote("B", 3),
                new LiteNote("G", 3),
                new LiteNote("D", 3),
                new LiteNote("A", 2),
                new LiteNote("E", 2),
            };

            foreach (var item in startNotes)
            {
                LiteNote currentNote = new LiteNote(item.Name, item.Octave);
                for (int i = 0; i < notesColumnCount; i++)
                {
                    NoteModel noteModel = new NoteModel(currentNote.Name, currentNote.Octave, i, rowCounter);
                    noteModel.NoteBrush = (Brush)(new BrushConverter().ConvertFrom(BrushNotesDict[currentNote.Name]));
                    noteModel.OctaveBrush = (Brush)(new BrushConverter().ConvertFrom(BrushOctavesDict[currentNote.Octave]));
                    noteModel.NoteBackground = NotesHelper.ChordColor[currentNote.Name];

                    Notes.Add(noteModel);
                    NotesDict.Add($"s{rowCounter}p{i}", noteModel);

                    string notesNameDictKey = $"{noteModel.Name}{noteModel.Octave}";
                    if (!NotesNameDict.ContainsKey(notesNameDictKey))
                    {
                        NotesNameDict.Add(notesNameDictKey, noteModel);
                    }
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
                { 2, "#E8BD00"},
                { 3, "#1D47C2"},
                { 4, "#008730"},
                { 5, "#B50461"},
            };
        }

        public void PlayDownMethod()
        {
            if (GlobalStrumPattern.Any())
            {
                List<string> playedNotesPaths = PrepareToPlayForChord();
                GlobalStrumPattern.ForEach(a => a.AssignPaths(playedNotesPaths));
                StrumViewModel strumManager = new StrumViewModel(GlobalStrumPattern);
                strumManager.CalculateDelays();
                var mappedStrumViewModel = DependencyInjection.PlaySoundMapper.MapStrumViewModel(strumManager);

                DependencyInjection.PlaySoundService.MyExtraPlayChordWithStrumPattern(mappedStrumViewModel);
            }
            else
            {
                var playedNotes = PrepareToPlay();
                playedNotes.Reverse();
                DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
            }
        }

        public void PlayDownMethod(NotesViewModelLiteVersion notesViewModelLite)
        {
            if (GlobalStrumPattern.Any())
            {
                List<string> playedNotesPaths = PrepareToPlayForChord(notesViewModelLite);
                GlobalStrumPattern.ForEach(a => a.AssignPaths(playedNotesPaths));
                StrumViewModel strumManager = new StrumViewModel(GlobalStrumPattern);
                strumManager.CalculateDelays();
                var mappedStrumViewModel = DependencyInjection.PlaySoundMapper.MapStrumViewModel(strumManager);

                DependencyInjection.PlaySoundService.MyExtraPlayChordWithStrumPattern(mappedStrumViewModel);
            }
            else
            {
                var playedNotes = PrepareToPlay(notesViewModelLite);
                playedNotes.Reverse();
                DependencyInjection.PlaySoundService.PlayChord(playedNotes, strumDelayMs);
            }
        }

        public StrumViewModel GetStrumModels(List<string> playedNotes)
        {
            List<StrumModel> myModel = null;

            if (GlobalStrumPattern != null && GlobalStrumPattern.Any())
            {
                myModel = GlobalStrumPattern.Select(a => (StrumModel)a.Clone()).ToList();
                myModel.ForEach(a => a.AssignPaths(playedNotes));
            }
            else
            {
                myModel = new List<StrumModel>()
                {
                    new StrumModel(playedNotes, StrumDirection.Downward, 20, 0, 6, 0),
                };
            }

            Stopwatch myU = new Stopwatch();
            myU.Start();
            //StrumViewModel strumManager = new StrumViewModel(strumPattern);
            StrumViewModel strumManager = new StrumViewModel(myModel);
            strumManager.CalculateDelays();

            return strumManager;
        }

        public List<StrumModel> ConvertEditStrumModelsToStrumPattern(List<EditStrumModel> editStrumModels)
        {
            List<StrumModel> res = new List<StrumModel>();

            foreach (var item in editStrumModels)
            {
                var playedNotes = item.Notes.Where(a => a.CheckedNote != CheckedFinger.None).ToList();
                var firstNote = playedNotes.First();
                var lastNote = playedNotes.Last();

                StrumDirection strumDir = (firstNote.CheckedNote == CheckedFinger.secondFinger) ? StrumDirection.Downward : StrumDirection.Upward;
                long delayMs = item.DelayBetweenStrunaMs;

                int skipNotes = 0;
                if (strumDir == StrumDirection.Downward)
                {
                    skipNotes = 5 - lastNote.MyIdx;
                }
                else
                {
                    skipNotes = firstNote.MyIdx;
                }

                StrumModel strumModel = new StrumModel(strumDir, delayMs, item.DelayBeforeMs, playedNotes.Count, skipNotes);
                res.Add(strumModel);
            }

            return res;
        }

        public List<StrumModel> ConvertToViewStrumModelsToStrumPattern(List<ToViewStrumModels> editStrumModels)
        {
            List<StrumModel> res = new List<StrumModel>();

            foreach (var item in editStrumModels)
            {
                var playedNotes = item.Notes.Where(a => a.CheckedNote != CheckedFinger.None).ToList();
                var firstNote = playedNotes.First();
                var lastNote = playedNotes.Last();

                StrumDirection strumDir = (firstNote.CheckedNote == CheckedFinger.secondFinger) ? StrumDirection.Downward : StrumDirection.Upward;
                long delayMs = item.DelayBetweenStrunaMs;

                int skipNotes = 0;
                if (strumDir == StrumDirection.Downward)
                {
                    skipNotes = 5 - lastNote.MyIdx;
                }
                else
                {
                    skipNotes = firstNote.MyIdx;
                }

                StrumModel strumModel = new StrumModel(strumDir, delayMs, item.DelayBeforeMs, playedNotes.Count, skipNotes);
                res.Add(strumModel);
            }

            return res;
        }

        public void PlayAtOnceMethod()
        {
            var playedNotes = PrepareToPlay();
            playedNotes.Reverse();

            DependencyInjection.PlaySoundService.PlayChord(playedNotes, 0);
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

        public List<NoteModel> GetCheckedNotes()
        {
            List<NoteModel> res = Notes.Where(a => a.IsSelected).ToList();
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

        public static List<string> PrepareToPlay(NotesViewModelLiteVersion notesViewModelLite)
        {
            var groupByStruny = notesViewModelLite.Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            int currStruna = 1;
            int offsetFr = notesViewModelLite.Fr > 1 ? notesViewModelLite.Fr - 1 : 0; //nie do konca rozumiem czemu tak....

            foreach (var strunaNotsy in groupByStruny)
            {
                bool foundNote = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.CheckedFinger != CheckedFinger.None)
                    {
                        playedNotes.Add($"s{note.Struna}p{note.Prog + 1 + offsetFr}");
                        foundNote = true;
                        break;
                    }
                }

                if (!foundNote)
                {
                    if (notesViewModelLite.NotesO[currStruna - 1] == "O")
                    {
                        playedNotes.Add($"s{currStruna}p0");
                    }
                }

                currStruna++;
            }

            return playedNotes;
        }

        public List<string> PrepareToPlayForChord()
        {
            var groupByStruny = Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            foreach (var strunaNotsy in groupByStruny)
            {
                bool found = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.IsSelected && note.IsNoteEnabled)
                    {
                        playedNotes.Add(note.Mp3Name);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    playedNotes.Add("");
                }
            }

            return playedNotes;
        }

        public List<string> PrepareToPlayForChord(NotesViewModelLiteVersion notesViewModelLite)
        {
            var groupByStruny = notesViewModelLite.Notes.GroupBy(a => a.Struna).ToList();

            List<string> playedNotes = new List<string>();

            int currStruna = 1;
            int offsetFr = notesViewModelLite.Fr > 1 ? notesViewModelLite.Fr - 1 : 0; //nie do konca rozumiem czemu tak....
            foreach (var strunaNotsy in groupByStruny)
            {
                bool found = false;
                foreach (var note in strunaNotsy.Reverse())
                {
                    if (note.CheckedFinger != CheckedFinger.None)
                    {
                        playedNotes.Add($"s{note.Struna}p{note.Prog + 1 + offsetFr}");
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    if (notesViewModelLite.NotesO[currStruna - 1] == "O")
                    {
                        playedNotes.Add($"s{currStruna}p0");
                        found = true;
                    }
                }

                if (!found)
                {
                    playedNotes.Add("");
                }

                currStruna++;
            }

            return playedNotes;
        }

        public void MoveToChooseProg(int prog)
        {
            var selectedNotes = Notes.Where(a => a.IsSelected).ToList();
            var minProg = selectedNotes.Min(a => a.Prog);

            int treshold = prog - minProg;

            bool changed = false;
            var usedNotes = new List<NoteModel>();

            for (int i = 0; i < selectedNotes.Count; i++)
            {
                var myNote = selectedNotes[i];

                var nextNote = Notes.FirstOrDefault(a => a.Struna == myNote.Struna && a.Prog == (myNote.Prog + treshold));

                if (nextNote != null)
                {
                    nextNote.IsSelecteOnlySet = true;
                    usedNotes.Add(nextNote);

                    if (!usedNotes.Contains(myNote)) myNote.IsSelecteOnlySet = false;

                    changed = true;
                }
            }

            if (changed)
            {
                NoteModel.UpdateGitarMainChordAction?.Invoke();
                NoteModel.SetGitarFocusAction.Invoke();
            }
        }

        private void MoveNotesHorizontally(int treshold)
        {
            var selectedNotes = Notes.Where(a => a.IsSelected).ToList();

            bool changed = false;
            var usedNotes = new List<NoteModel>();

            for (int i = 0; i < selectedNotes.Count; i++)
            {
                var myNote = selectedNotes[i];

                var nextNote = Notes.FirstOrDefault(a => a.Struna == myNote.Struna && a.Prog == (myNote.Prog + treshold));

                if (nextNote != null)
                {
                    nextNote.IsSelecteOnlySet = true;
                    usedNotes.Add(nextNote);

                    if (!usedNotes.Contains(myNote)) myNote.IsSelecteOnlySet = false;

                    changed = true;
                }
            }

            if (changed)
            {
                NoteModel.UpdateGitarMainChordAction?.Invoke();
                NoteModel.SetGitarFocusAction.Invoke();
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