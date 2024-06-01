using EditChordsWindow;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject.EditChord
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
                                        .Select(a => new StrumNoteDetails(a, $"{NotesViewModelEdit.NotesDict[a].Name}{NotesViewModelEdit.NotesDict[a].Octave}", NotesViewModelEdit.NotesDict[a].Struna))
                                        .ToList();
            }
            else
            {
                PlayedNotes = notesPaths.AsEnumerable()
                                        .Reverse()
                                        .Skip(SkipNotes)
                                        .Take(TakeNotes == -1 ? notesPaths.Count : TakeNotes)
                                        .Select(a => new StrumNoteDetails(a, $"{NotesViewModelEdit.NotesDict[a].Name}{NotesViewModelEdit.NotesDict[a].Octave}", NotesViewModelEdit.NotesDict[a].Struna))
                                        .ToList();
            }
        }
    }

    public class NotesViewModelEdit : INotifyPropertyChanged
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

        public List<NoteModelEdit> Notes { get; set; }
        public static Dictionary<string, NoteModelEdit> NotesDict { get; set; }
        public WaveOut[] StrunyWaves { get; set; } = new WaveOut[6];
        public WaveOut MainWaveOut { get; set; } = new WaveOut();

        public NotesViewModelEdit()
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

                        PlayChord(playedNotes, 0);
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
                        PlayChord(playedNotes, strumDelayMs);
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
                        NoteModelEdit.UpdateGitarMainChordAction?.Invoke();
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

            NoteModelEdit.RefreshNotesOnStrunaAction = RefreshNotesOnStrunaAction;
        }

        private void InitNotes()
        {
            Notes = new List<NoteModelEdit>();
            NotesDict = new Dictionary<string, NoteModelEdit>();

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
                    NoteModelEdit noteModel = new NoteModelEdit(currentNote.Name, currentNote.Octave, i, rowCounter);
                    noteModel.NoteBrush = (Brush)(new BrushConverter().ConvertFrom(BrushNotesDict[currentNote.Name]));
                    noteModel.OctaveBrush = (Brush)(new BrushConverter().ConvertFrom(BrushOctavesDict[currentNote.Octave]));
                    noteModel.NoteBackground = NotesHelper.ChordColorNoOpacity[currentNote.Name];

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

            PlayChord(playedNotes, strumDelayMs);
            //PlayChordWithStrumPattern(playedNotes);
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

        public List<NoteModelEdit> GetCheckedNotes()
        {
            List<NoteModelEdit> res = Notes.Where(a => a.IsSelected).ToList();
            return res;
        }

        private void PlayChordWithStrumPattern(List<string> paths)
        {
            Debug.WriteLine(string.Join(", ", paths.Select(a => $"\"{a}\"")));

            List<string> GmChord = new List<string> { "s6p3", "s5p5", "s4p5", "s3p3", "s2p3", "s1p3" };
            List<string> EbChord = new List<string> { "s5p6", "s4p5", "s3p3", "s2p4", "s1p3" };
            List<string> BbChord = new List<string> { "s6p6", "s5p8", "s4p8", "s3p7", "s2p6", "s1p6" };

            int offsetMs = 0;

            List<ISampleProvider> samples = new List<ISampleProvider>();

            //List<SingleStrum> strumPattern = new List<SingleStrum>()
            //{
            //    new SingleStrum(StrumDirection.Downward, 30, 0, 6, paths),
            //    new SingleStrum(StrumDirection.Downward, 20, 100, 2, paths),
            //    new SingleStrum(StrumDirection.Downward, 20, 20, 2, paths),

            //    new SingleStrum(StrumDirection.Downward, 30, 400, 6, paths),
            //    new SingleStrum(StrumDirection.Downward, 30, 400, 6, paths),
            //    new SingleStrum(StrumDirection.Downward, 30, 400, 6, paths)
            //};

            List<SingleStrum> strumPattern = new List<SingleStrum>()
            {
                new SingleStrum(GmChord, StrumDirection.Downward, 40, 0, 3, 0),
                new SingleStrum(GmChord, StrumDirection.Downward, 40, 800, 4, 0),

                new SingleStrum(GmChord, StrumDirection.Upward, 15, 400, 6, 1),
                new SingleStrum(GmChord, StrumDirection.Downward, 10, 100, 5, 0),

                new SingleStrum(EbChord, StrumDirection.Upward, 15, 160, 6, 1),
                new SingleStrum(EbChord, StrumDirection.Downward, 15, 160, EbChord.Count-1, 0),
                new SingleStrum(EbChord, StrumDirection.Upward, 15, 160, 6, 1),
                new SingleStrum(EbChord, StrumDirection.Downward, 20, 350, EbChord.Count-1, 0),

                new SingleStrum(EbChord, StrumDirection.Upward, 15, 360, 6, 1),
                new SingleStrum(EbChord, StrumDirection.Downward, 25, 100, EbChord.Count-1, 0),

                new SingleStrum(BbChord, StrumDirection.Downward, 40, 160, BbChord.Count-1, 0),
                new SingleStrum(BbChord, StrumDirection.Downward, 40, 700, BbChord.Count-1, 0),

                new SingleStrum(BbChord, StrumDirection.Downward, 40, 400, BbChord.Count-1, 0),
                new SingleStrum(BbChord, StrumDirection.Downward, 25, 300, BbChord.Count-1, 0),
                new SingleStrum(BbChord, StrumDirection.Upward, 25, 100, 6, 1),

                new SingleStrum(BbChord, StrumDirection.Downward, 25, 400, BbChord.Count-1, 0),
                new SingleStrum(BbChord, StrumDirection.Upward, 25, 100, 6, 1),
                new SingleStrum(BbChord, StrumDirection.Downward, 25, 100, BbChord.Count-1, 0),
                new SingleStrum(BbChord, StrumDirection.Upward, 40, 100, 6, 1),

                //new SingleStrum(EbChord, StrumDirection.Downward, 20, 450, EbChord.Count-1, 0),

                //new SingleStrum(StrumDirection.Downward, 20, 200, 6, EbChord)
            };

            StrumManager strumManager = new StrumManager(strumPattern);
            strumManager.CalculateDelays();

            //Gm     "s6p3", "s5p5", "s4p5", "s3p3", "s2p3", "s1p3"
            //Eb     "s5p6", "s4p5", "s3p3", "s2p4", "s1p3"
            //Bb     "s6p6", "s5p8", "s4p8", "s3p7", "s2p6", "s1p6"
            int strumCounter = 0;
            foreach (var item in strumManager.StrumPattern)
            {
                int counter = 0;
                foreach (var strumItem in item.PlayedNotes)
                {
                    OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{strumItem.Path}.wav"));

                    offsetSample.DelayBy = TimeSpan.FromMilliseconds(strumItem.DelayMs);
                    var takeSample = strumItem.PlayTime == -1 ? offsetSample : offsetSample.Take(TimeSpan.FromMilliseconds(strumItem.PlayTime + strumItem.DelayMs));
                    samples.Add(takeSample);
                    counter++;
                }
                strumCounter++;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);

            //NAudioHelper.ConvertToFileWavMp3(mixSample, "mixedBest.wav", "mixedBest.mp3");

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(mixSample);
            MainWaveOut.Play();
        }

        private void PlayChord(List<string> paths, int delayMs)
        {
            int offsetMs = 0;

            List<ISampleProvider> samples = new List<ISampleProvider>();

            foreach (var path in paths)
            {
                OffsetSampleProvider offsetSample = new OffsetSampleProvider(new AudioFileReader($@"NotesMp3\GibsonSj200 New\{path}.wav"));
                offsetSample.DelayBy = TimeSpan.FromMilliseconds(offsetMs);
                samples.Add(offsetSample);
                offsetMs += delayMs;
            }

            if (!samples.Any()) return;

            MixingSampleProvider mixSample = new MixingSampleProvider(samples);

            //WaveFileWriter.CreateWaveFile16("mixed22.wav", mixSample);

            MainWaveOut.Dispose();
            MainWaveOut = new WaveOut();
            MainWaveOut.Init(mixSample);
            MainWaveOut.Play();
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
            var usedNotes = new List<NoteModelEdit>();

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