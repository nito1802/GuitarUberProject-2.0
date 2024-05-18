using EditChordsWindow;
using GitarUberProject.Helperes;
using GitarUberProject.EditChord;
using GitarUberProject.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GitarUberProject.Games_And_Fun;
using GitarUberProject.Games_and_Fun;
using System.Collections.ObjectModel;
using GitarUberProject.Games_and_Fun.HighScoreStats;
using NAudio.Midi;
using System.Diagnostics;

namespace GitarUberProject
{
    /// <summary>
    /// Interaction logic for ChordEdit.xaml
    /// </summary>
    public partial class RecognizeNote : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public string HighScoreFileName { get; set; } = "RecognizeNoteEasy.json";
        public string HighScoreStatsFileName { get; set; } = "RecognizeNoteEasy.json";

        public int MaxStruna { get; set; } = 6;//obszar w jakim losuje
        public int MaxProg { get; set; } = 5;
        public int ErrorTolerance { get; set; } = 2;

        public int ErrorToleranceMaxTries { get; set; } = 1;
        public int BullseyePoints { get; set; } = 3;
        public SolidColorBrush BullseyeTextColor { get; } = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF007C9C"));


        public int ErrorToleranceTries { get; set; } = 1;
        public string[] PossibleErrorMessages { get; set; }

        public HighScoreViewModel ScoreViewModel { get; set; } = new HighScoreViewModel();
        public GamesViewModelEdit NotesViewModel { get; set; } = new GamesViewModelEdit();
        public NotesViewModelLiteVersion SelectedGitarChord { get; set; }
        public ChordIntervalsDetailsViewModel ChordIntervalsDetailsViewModel { get; set; }
        public List<string> IntervalsNotes { get; set; }
        public List<string> notesRemainToAcceptChord { get; set; }
        public Dictionary<string, List<int>> AllIntervals { get; set; }
        public List<int> IntervalNumbers { get; set; } = new List<int>();
        public Dictionary<string, int> AllNotesFromGuitar { get; set; }
        public GameModelEdit QuestionModel { get; set; }
        public Random Rand { get; set; } = new Random();
        public MidiIn MidiKeyboard { get; set; }

        string notesRemainText;
        string chordsAlreadyWithInterval;
        string chordName;
        private int streak;
        private int possibleErrors;
        private Brush messageBrush;
        private string messageText;

        public RecognizeNote()
        {
            InitializeComponent();

            //ChordName = notesViewModelLiteVersion.ChordFullName;
            //SelectedGitarChord = notesViewModelLiteVersion;

            //IntervalsNotes = intervalsNotes;
            //AllIntervals = allIntervals;


            //startNote e3

            ScoreViewModel.FileName = "RecognizeNoteEasy.json";
            var loadedScore = ScoreViewModel.Load();

            if(loadedScore != null && loadedScore.BestScores.Any())
            {
                ScoreViewModel = loadedScore;
                ScoreViewModel.FileName = "RecognizeNoteEasy.json";
            }
            else
            {
                ScoreViewModel.BestScores = new ObservableCollection<HighScoreModel>
                {

                };

                ScoreViewModel.NetworkBestScores = new ObservableCollection<HighScoreModel>
                {

                };
            }

            SynchroToGuitar();
            this.DataContext = this;
            GridGuitar.DataContext = NotesViewModel;
            DataG.DataContext = ScoreViewModel;
            icNetworkBestScores.DataContext = ScoreViewModel;
            gridStatsAllTime.DataContext = ScoreViewModel.AllTimeStats;
            myRadialGaugeChartAllTime.DataContext = ScoreViewModel.AllTimeStats.RadialViewModel;
            gridStatsToday.DataContext = ScoreViewModel.TodayStats;
            myRadialGaugeChart.DataContext = ScoreViewModel.TodayStats.RadialViewModel;

            GameModelEdit.UpdateGitarMainChordAction = () => UpdateMainGitarChordEdit();
            GameModelEdit.OnPlayNoteCustomAction = (clickedNote) =>
            {
                string qNote = $"{QuestionModel.Name}{QuestionModel.Octave}";
                string cNote = $"{clickedNote.Name}{clickedNote.Octave}";

                var intervalBetweenCorrectNote = GetLengthBetweenNotes(qNote, cNote);

                ScoreViewModel.AllTimeStats.AddResult(intervalBetweenCorrectNote);
                ScoreViewModel.TodayStats.AddResult(intervalBetweenCorrectNote);

                bool res = false;
                if (intervalBetweenCorrectNote == 0)//correct note
                {
                    bool bullseye = ErrorToleranceTries == ErrorToleranceMaxTries;
                    var awardPoints = bullseye ? BullseyePoints : 1;

                    Streak += awardPoints;
                    
                    ErrorToleranceTries = ErrorToleranceMaxTries;

                    bool addPossibleErrors = false;
                    if (Streak % 5 == 0)
                    {
                        PossibleErrors++;
                        addPossibleErrors = true;
                    }

                    if(!bullseye)
                    {
                        MessageText = addPossibleErrors ? $"Dobrze! {cNote}  (+1)" : $"Dobrze! {cNote}";
                        MessageBrush = Brushes.Green;
                    }
                    else
                    {
                        MessageText = addPossibleErrors ? $"Doskonale! {cNote}  (+{BullseyePoints})" : $"Doskonale! {cNote}";
                        MessageBrush = BullseyeTextColor;
                    }

                    RandNote();
                    NotesViewModel.PlayChordWithStrumPattern(new List<string> { clickedNote.Mp3Name, QuestionModel.Mp3Name }, 1000);

                    res = true;
                }
                else if(intervalBetweenCorrectNote <= ErrorTolerance)
                {
                    if (ErrorToleranceTries > 0)
                    {
                        ErrorToleranceTries--;
                        MessageText = $"Max 2 półstopnie dalej!";
                        MessageBrush = Brushes.Orange;
                    }
                    else
                    {
                        if (PossibleErrors > 0) PossibleErrors--;
                        else
                        {
                            if (Streak > 0)
                            {
                                ScoreViewModel.AddBestScore(Streak);
                            }

                            Streak = 0;
                            PossibleErrors = 0;
                        }

                        var randIdx = Rand.Next(0, PossibleErrorMessages.Length);
                        MessageText = PossibleErrorMessages[randIdx];
                        MessageBrush = Brushes.HotPink;
                    }
                    res = false;
                }
                else
                {
                    if (PossibleErrors > 0) PossibleErrors--;
                    else
                    {
                        if (Streak > 0)
                        {
                            ScoreViewModel.AddBestScore(Streak);
                        }

                        Streak = 0;
                        PossibleErrors = 0;
                    }

                    var randIdx = Rand.Next(0, PossibleErrorMessages.Length);
                    MessageText = PossibleErrorMessages[randIdx];
                    MessageBrush = Brushes.HotPink;
                    MessageText += $" {res}";
                    res = false;
                }

                ScoreViewModel.Save();
                return res;
            };
            if (IntervalsNotes != null && IntervalsNotes.Any())
            {
                DisableNotesOutsideChord();
            }

            AllNotesFromGuitar = NotesHelper.GetAllNotesFromGuitar();

            PossibleErrorMessages = new string[]
            {
                "Źle",
                "Nie tym razem",
                "Niekoniecznie",
                "Błąd",
                "Cóż, źle",
                "Pomyłka",
            };


            //bool isChordCorrect = IsChordCorrect();
            //btnApply.IsEnabled = isChordCorrect;
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

        public List<string> NotesRemainToAcceptChord
        {
            get => notesRemainToAcceptChord;
            set
            {
                notesRemainToAcceptChord = value;
                NotesRemainText = string.Join(" ", notesRemainToAcceptChord);
            }
        }

        public string NotesRemainText
        {
            get => notesRemainText;
            set
            {
                notesRemainText = value;
                OnPropertyChanged("NotesRemainText");
            }
        }

        public string ChordsAlreadyWithInterval
        {
            get => chordsAlreadyWithInterval;
            set
            {
                chordsAlreadyWithInterval = value;
                OnPropertyChanged("ChordsAlreadyWithInterval");
            }
        }

        public int Streak
        {
            get => streak;
            set
            {
                streak = value;
                OnPropertyChanged("Streak");
            }
        }

        public int PossibleErrors
        {
            get => possibleErrors;
            set
            {
                possibleErrors = value;
                OnPropertyChanged("PossibleErrors");
            }
        }

        public Brush MessageBrush
        {
            get => messageBrush;
            set
            {
                messageBrush = value;
                OnPropertyChanged("MessageBrush");
            }
        }

        public string MessageText
        {
            get => messageText;
            set
            {
                messageText = value;
                OnPropertyChanged("MessageText");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();

            double screenWidth = System.Windows.SystemParameters.WorkArea.Width;
            double screenHeight = System.Windows.SystemParameters.WorkArea.Height;

            double windowRight = this.Left + this.ActualWidth;

            if (windowRight > screenWidth)
            {
                double tresholdX = windowRight - screenWidth;
                this.Left -= tresholdX;
            }


            NotesViewModel.Notes.ForEach(a => a.NoteOpacity = 1);
            var disabledNotes = NotesViewModel.Notes.Where(a => a.Prog > MaxProg || a.Struna > MaxStruna).ToList();
            disabledNotes.ForEach(a =>
            {
                a.IsNoteEnabled = false;
                a.NoteOpacity = GameModelEdit.DisabledNoteOpacity;
            });


            var dis = NotesViewModel.Notes.Where(a => a.NoteOpacity == 1).ToList();
            RandNote();
            var dis2 = NotesViewModel.Notes.Where(a => a.NoteOpacity == 1).ToList();

            var midiCounter = MidiIn.NumberOfDevices;

            if(midiCounter > 0)
            {
                MidiKeyboard = new MidiIn(0);
                MidiKeyboard.MessageReceived += MidiKeyboard_MessageReceived;
                MidiKeyboard.Start();
            }

            
        }

        private void MidiKeyboard_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                NoteEvent noteEvent = (NoteEvent)e.MidiEvent;
                
                if(noteEvent.NoteName == "C4")
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        QuestionModel.PlaySingleNote();
                        MessageText = string.Empty;
                    });
                }
                else
                {
                    string midiNoteName = noteEvent.NoteName.Substring(0, noteEvent.NoteName.Length - 1);
                    string midiNoteOctave = noteEvent.NoteName.Substring(noteEvent.NoteName.Length - 1, 1);

                    var myNote = NotesViewModel.Notes.First(a => a.Name == midiNoteName && a.Octave.ToString() == midiNoteOctave);

                    this.Dispatcher.Invoke(() =>
                    {
                        myNote.PlayNoteMethod();
                    });
                }
            }

            //e.MidiEvent.Note

            string text = e.MidiEvent.ToString();
            Debug.WriteLine(text);
        }

        private void RandNote()
        {
            List<GameModelEdit> btnsRand = new List<GameModelEdit>();

            for (int i = 0; i < 1; i++)
            {
                int strunaRnd = 0;
                int progRnd = 0;


                GameModelEdit tempNoteRnd = null;
                do
                {
                    strunaRnd = Rand.Next(1, MaxStruna+1);
                    progRnd = Rand.Next(0, MaxProg+1);

                    tempNoteRnd = NotesViewModel.Notes.First(a => a.Struna == strunaRnd && a.Prog == progRnd);

                } while (btnsRand.Any(a => a.Struna == strunaRnd && a.Prog == progRnd || (a.Name == tempNoteRnd.Name && a.Octave == tempNoteRnd.Octave)));

                var noteRnd = NotesViewModel.Notes.First(a => a.Struna == strunaRnd && a.Prog == progRnd);
                var noteRndClone = (GameModelEdit)noteRnd.CloneBieda();
                btnsRand.Add(noteRndClone);
            }

            QuestionModel = btnsRand[0];
        }

        private int GetLengthBetweenNotes(string noteA, string noteB)
        {
            int noteAIndex = AllNotesFromGuitar[noteA];
            int noteBIndex = AllNotesFromGuitar[noteB];

            int res = Math.Abs(noteAIndex - noteBIndex);
            return res;
        }

        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        private void AssignNotesAfterConvert(NotesViewModelLiteVersion afterConvert)
        {
            for (int i = 0; i < afterConvert.Notes.Count; i++)
            {
                SelectedGitarChord.Notes[i] = afterConvert.Notes[i];
            }

            for (int i = 0; i < afterConvert.NotesO.Count; i++)
            {
                SelectedGitarChord.NotesO[i] = afterConvert.NotesO[i];
            }

            for (int i = 0; i < afterConvert.NoteOctaves.Count; i++)
            {
                SelectedGitarChord.NoteOctaves[i] = afterConvert.NoteOctaves[i];
            }

            SelectedGitarChord.ChordCode = afterConvert.ChordCode;
            SelectedGitarChord.ChordCodeNormalized = afterConvert.ChordCodeNormalized;
            SelectedGitarChord.Fr = afterConvert.Fr;
        }

        private void UpdateMainGitarChordEdit()
        {
            if (SelectedGitarChord == null) return;

            var checkedNotes = NotesViewModel.GetCheckedNotes();
            InputViewModelFacade inputViewModelFacade = GuitarControlVMToInputViewModelFacade(checkedNotes, treshhold: -1);

            var debugData = inputViewModelFacade.GetNotesShape();

            var convertedViewModelLite = NotesHelper.InputFacadeToViewModelLite(inputViewModelFacade);
            AssignNotesAfterConvert(convertedViewModelLite);

            //ChordDetails
            if (SelectedGitarChord.NoteOctaves != null && SelectedGitarChord.NoteOctaves.Any(a => !string.IsNullOrEmpty(a.Name)))
            {
                var filteredNoteOctaves = SelectedGitarChord.NoteOctaves
                                                        .Where(a => !string.IsNullOrEmpty(a.Name))
                                                        .ToList();

                var notesIntervals = filteredNoteOctaves
                                                .Select(b => new NoteOctaveIntervalDetails(b.Name, int.Parse(b.Octave)))
                                                .Reverse()
                                                .ToList();

                string chordName = SelectedGitarChord.ChordName ?? notesIntervals.First().Note;
                string chordType = SelectedGitarChord.ChordType ?? "None";

                try
                {
                    ChordIntervalsDetailsViewModel = ChordIntervalHelper.ConvertIntoChordIntervalDetails(chordName, chordType, notesIntervals);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                ChordIntervalsDetailsViewModel = new ChordIntervalsDetailsViewModel();
            }

            if (MainWindow.CheckedFingerDict.ContainsKey(SelectedGitarChord.ChordCodeNormalized))
            {
                for (int i = 0; i < SelectedGitarChord.Notes.Count; i++)
                {
                    SelectedGitarChord.Notes[i].CheckedFinger = MainWindow.CheckedFingerDict[SelectedGitarChord.ChordCodeNormalized][i];
                }
            }

            //bool isChordCorrect = IsChordCorrect();
            //btnApply.IsEnabled = isChordCorrect;

        }

        InputViewModelFacade GuitarControlVMToInputViewModelFacade(List<GameModelEdit> checkedNotes, int treshhold = 0)
        {
            InputViewModelFacade res = new InputViewModelFacade();

            int fr = GetFr(checkedNotes);
            res.Fr = fr;

            for (int i = 1; i <= 6; i++)
            {
                var notesOnStruna = checkedNotes.Where(a => a.Struna == i).ToList();

                if (notesOnStruna.Any())
                {
                    foreach (var item in notesOnStruna)
                    {
                        int struna = item.Struna;
                        int prog = item.Prog - fr + treshhold;

                        if (fr > 0) prog++; //bo jak bary oznaczamy, a strun na 0 progu nie

                        if (prog == -1)
                        {
                            res.NotesO[i - 1] = NotesOStates.O;
                        }
                        else
                        {
                            if (prog < res.InputNotes.GetLength(0))
                                res.InputNotes[struna - 1, prog].CheckedFingerProp = CheckedFinger.Other;
                        }
                    }
                }
                else
                {
                    res.NotesO[i - 1] = NotesOStates.X;
                }
            }

            return res;
        }

        int GetFr(List<GameModelEdit> checkedNotes)
        {
            if (checkedNotes == null || !checkedNotes.Any()) return 0;

            int fr = checkedNotes.Min(a => a.Prog);

            return fr;
        }

        private void SynchroToGuitar()
        {
            if (SelectedGitarChord == null) return;

            var checkedNotes = SelectedGitarChord.Notes.Where(a => a.CheckedFinger != CheckedFinger.None).ToList();

            NotesViewModel.Notes.ForEach(a => a.ClearNote());

            //Fr: 2
            int myFr = SelectedGitarChord.Fr;

            if (myFr == 0) myFr = 1;

            foreach (var item in checkedNotes)
            {
                string key = $"s{item.Struna}p{item.Prog + myFr}";
                NotesViewModelEdit.NotesDict[key].IsSelecteOnlySet = true;
            }

            for (int i = 0; i < SelectedGitarChord.NotesO.Count; i++)
            {
                if (SelectedGitarChord.NotesO[i] == "O")
                {
                    string key = $"s{i + 1}p0";
                    NotesViewModelEdit.NotesDict[key].IsSelecteOnlySet = true;
                }
            }

            UpdateMainGitarChordEdit();
        }

        private void DisableNotesOutsideChord()
        {
            NotesViewModel.Notes.ForEach(a => a.IsNoteEnabled = true);

            foreach (var item in NotesViewModel.Notes)
            {
                if (!IntervalsNotes.Any(a => a == item.Name))
                {
                    item.IsNoteEnabled = false;
                }
            }
        }


        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
            else if(e.Key == Key.Enter && btnApply.IsEnabled)
            {
                DialogResult = true;

                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
            {
                Close();
            }
        }

        private void BarButtonGuitar_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;

            int prog = int.Parse(btn.Tag.ToString());

            var notesOnProg = NotesViewModel.Notes.Where(a => a.Prog == prog).ToList();

            notesOnProg.ForEach(a => a.IsSelectedWithoutFocus = true);

            UpdateMainGitarChordEdit();
        }

        private void BarButtonGuitar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Button btn = (System.Windows.Controls.Button)sender;

            int prog = int.Parse(btn.Tag.ToString());

            var notesOnProg = NotesViewModel.Notes.Where(a => a.Prog == prog).ToList();

            notesOnProg.ForEach(a => a.IsSelected = false);
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

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

            Close();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            QuestionModel.PlaySingleNote();
            MessageText = string.Empty;
            //RadialGaugeViewModel.Data.First().Count++;
        }

        private void DataG_MouseLeave(object sender, MouseEventArgs e)
        {
            DataG.UnselectAll();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(MidiKeyboard != null)
            {
                MidiKeyboard.Stop();
                MidiKeyboard.Dispose();
            }

            if (Streak > 0)
            {
                ScoreViewModel.AddBestScore(Streak);
                ScoreViewModel.Save();
            }
        }
    }
}
