using EditChordsWindow;
using GitarUberProject.EditChord;
using GitarUberProject.Games_and_Fun;
using GitarUberProject.Games_And_Fun;
using GitarUberProject.Helperes;
using GitarUberProject.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace GitarUberProject
{
    /// <summary>
    /// Interaction logic for ChordEdit.xaml
    /// </summary>
    public partial class GameWindow : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public int MaxStruna { get; set; } = 3;//obszar w jakim losuje
        public int MaxProg { get; set; } = 5;

        public HighScoreViewModel ScoreViewModel { get; set; } = new HighScoreViewModel();
        public GamesViewModelEdit NotesViewModel { get; set; } = new GamesViewModelEdit();
        public NotesViewModelLiteVersion SelectedGitarChord { get; set; }
        public ChordIntervalsDetailsViewModel ChordIntervalsDetailsViewModel { get; set; }
        public List<string> IntervalsNotes { get; set; }
        public List<string> notesRemainToAcceptChord { get; set; }
        public Dictionary<string, List<int>> AllIntervals { get; set; }
        public List<int> IntervalNumbers { get; set; } = new List<int>();
        public Dictionary<string, int> AllNotesFromGuitar { get; set; }

        public GameModelEdit BtnA { get; set; }
        public GameModelEdit BtnB { get; set; }
        public GameModelEdit BtnC { get; set; }
        public GameModelEdit BtnD { get; set; }
        public GameModelEdit QuestionModel { get; set; }
        public Random Rand { get; set; } = new Random();
        public int Streak { get; set; }

        private string notesRemainText;
        private string chordsAlreadyWithInterval;
        private string chordName;

        public GameWindow()
        {
            InitializeComponent();

            //ChordName = notesViewModelLiteVersion.ChordFullName;
            //SelectedGitarChord = notesViewModelLiteVersion;

            //IntervalsNotes = intervalsNotes;
            //AllIntervals = allIntervals;

            //startNote e3

            ScoreViewModel.FileName = "RecognizeNoteEasy.json";
            var loadedScore = ScoreViewModel.Load();

            if (loadedScore != null && loadedScore.BestScores.Any())
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
            icBestScores.DataContext = ScoreViewModel;
            icNetworkBestScores.DataContext = ScoreViewModel;

            GameModelEdit.UpdateGitarMainChordAction = () => UpdateMainGitarChordEdit();
            if (IntervalsNotes != null && IntervalsNotes.Any())
            {
                DisableNotesOutsideChord();
            }

            AllNotesFromGuitar = NotesHelper.GetAllNotesFromGuitar();

            var rrr = GetLengthBetweenNotes("F4", "E4");

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

            var disabledNotes = NotesViewModel.Notes.Where(a => a.Prog > MaxProg || a.Struna > MaxStruna).ToList();
            disabledNotes.ForEach(a => a.IsNoteEnabled = false);
            RandNote();
        }

        private void RandNote()
        {
            List<GameModelEdit> btnsRand = new List<GameModelEdit>();

            for (int i = 0; i < 5; i++)
            {
                int strunaRnd = 0;
                int progRnd = 0;

                GameModelEdit tempNoteRnd = null;
                do
                {
                    strunaRnd = Rand.Next(1, MaxStruna + 1);
                    progRnd = Rand.Next(0, MaxProg + 1);

                    tempNoteRnd = NotesViewModel.Notes.First(a => a.Struna == strunaRnd && a.Prog == progRnd);
                } while (btnsRand.Any(a => a.Struna == strunaRnd && a.Prog == progRnd || (a.Name == tempNoteRnd.Name && a.Octave == tempNoteRnd.Octave)));

                var noteRnd = NotesViewModel.Notes.First(a => a.Struna == strunaRnd && a.Prog == progRnd);
                var noteRndClone = (GameModelEdit)noteRnd.Clone();
                btnsRand.Add(noteRndClone);
            }

            QuestionModel = btnsRand[0];

            BtnA = btnsRand[1];
            BtnB = btnsRand[2];
            BtnC = btnsRand[3];
            BtnD = btnsRand[4];

            var goodAnswer = Rand.Next(1, 4);

            switch (goodAnswer)
            {
                case 1:
                    BtnA = QuestionModel;
                    break;

                case 2:
                    BtnB = QuestionModel;
                    break;

                case 3:
                    BtnC = QuestionModel;
                    break;

                case 4:
                    BtnD = QuestionModel;
                    break;
            }

            btnWindowA.DataContext = BtnA;
            btnWindowB.DataContext = BtnB;
            btnWindowC.DataContext = BtnC;
            btnWindowD.DataContext = BtnD;
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

        //private bool IsChordCorrect()
        //{
        //    var chordNotes = SelectedGitarChord.NoteOctaves
        //                                       .Where(a => !string.IsNullOrEmpty(a.Name))
        //                                       .Select(b => b.Name)
        //                                       .ToList();

        //    NotesRemainToAcceptChord = IntervalsNotes.Except(chordNotes).ToList();

        //    IntervalNumbers = ChordIntervalsDetailsViewModel.NotesGroup.Select(a =>
        //    {
        //        string intervalText = a.Notes?.First().Interval;

        //        if (string.IsNullOrEmpty(intervalText)) return 0;

        //        int parsedIntervalNumber = int.Parse(intervalText);
        //        return parsedIntervalNumber;
        //    }).Where(b => b != 0).ToList();
        //    //IntervalNumbers.Insert(0, 1);

        //    var inters = AllIntervals.Where(a => a.Key != SelectedGitarChord.ChordType)
        //                             .Where(b => b.Value.SequenceEqual(IntervalNumbers))
        //                             .ToList();

        //    if(inters.Any())
        //    {
        //        ChordsAlreadyWithInterval = $"{string.Join(", ", inters.Select(a => a.Key))} Intervals";
        //    }
        //    else
        //    {
        //        ChordsAlreadyWithInterval = string.Empty;
        //    }

        //    bool res = !NotesRemainToAcceptChord.Any() && !inters.Any();

        //    return res;
        //}

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

        private InputViewModelFacade GuitarControlVMToInputViewModelFacade(List<GameModelEdit> checkedNotes, int treshhold = 0)
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

        private int GetFr(List<GameModelEdit> checkedNotes)
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
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.Enter && btnApply.IsEnabled)
            {
                DialogResult = true;

                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1 || e.ChangedButton == MouseButton.XButton2)
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
        }

        private void BtnWindow_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var clickedDataContext = ((GameModelEdit)btn.DataContext);

            //clickedDataContext.PlaySingleNote();

            if (clickedDataContext.Name == QuestionModel.Name && clickedDataContext.Octave == QuestionModel.Octave)
            {
                Streak++;
            }
            else
            {
                if (Streak > ScoreViewModel.BestScores.Min(a => a.Score))
                {
                    ScoreViewModel.AddBestScore(Streak);
                    ScoreViewModel.Save();
                }

                Streak = 0;
            }

            RandNote();
            NotesViewModel.PlayChordWithStrumPattern(new List<string> { clickedDataContext.Mp3Name, QuestionModel.Mp3Name }, 1000);

            tbCounter.Text = Streak.ToString();
        }
    }
}