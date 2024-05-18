using EditChordsWindow;
using GitarUberProject.EditStrumWindow;
using GitarUberProject.Helperes;
using GitarUberProject.Helpers;
using GitarUberProject.HelperWindows;
using GitarUberProject.Models;
using GitarUberProject.Services;
using GitarUberProject.ViewModels;
using Microsoft.Win32;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

//using NoteMy = Melanchall.DryWetMidi.Smf.Interaction.Note;

namespace GitarUberProject
{
    public enum ChordFocus
    {
        Gitar,
        Chord,
        OtherChord
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static double ReadChordWidth { get; set; } = 145;
        public static double ReadChordHeight { get; set; } = 30;

        public static double NormalChordWidth { get; set; } = 130;
        public static double NormalChordHeight { get; set; } = 70;

        public double ScreenWidth { get; set; }
        public double ScreenHeight { get; set; }

        public string SeparatedLine { get; } = "-s-p-a-f-g-h-h--tht-h-trrtgrtgteea-";
        public string DefaultChordsPath { get; } = "DefaultChords.gup";
        public Brush PianoPressedBrush { get; set; }
        public List<Button> PianoKeys { get; set; }
        public Dictionary<string, Button> PianoKeysDict { get; set; }
        public Dictionary<string, Brush> PianoNormalBackground { get; set; }
        public static Dictionary<string, NotesViewModelLiteVersion> GlobalNotesChordDict { get; set; } = new Dictionary<string, NotesViewModelLiteVersion>();
        public static Dictionary<string, List<CheckedFinger>> CheckedFingerDict { get; set; } = new Dictionary<string, List<CheckedFinger>>();

        public NotesViewModel NotesViewModel { get; set; } = new NotesViewModel();
        public ChordsViewModel ChordsViewModel { get; set; } = new ChordsViewModel();
        public CustomChordsViewModel CustomChordsViewModel { get; set; } = new CustomChordsViewModel();
        public ScaleNotesViewModel ScaleNotesViewModel { get; set; } = new ScaleNotesViewModel();
        public ChordIntervalsDetailsViewModel ChordIntervalsDetailsViewModel { get; set; }
        public PianoNotesViewModel PianoNotesViewModel { get; set; } = new PianoNotesViewModel();
        public GlobalGitarButtonViewModel GlobalGitarViewModel { get; set; } = new GlobalGitarButtonViewModel();
        public ChordBoxViewModel ChordBoxViewModel { get; set; } = new ChordBoxViewModel();
        public ToViewStrumViewModels ToViewStrumViewModels { get; set; } = new ToViewStrumViewModels();
        public MixerViewModel MixerViewModel { get; set; } = new MixerViewModel();

        //public NotesViewModelLiteVersion ChordMain { get; set; }
        public List<string> AllNotes { get; set; } = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        //Focused purpose:
        public NotesViewModelLiteVersion SelectedChord { get; set; }

        public NotesViewModelLiteVersion SelectedOtherChord { get; set; }
        public NotesViewModelLiteVersion SelectedGitarChord { get; set; }
        public NotesViewModelLiteVersion SelectedViewModel { get; set; }
        public EditStrumViewModel EditStrumViewModel { get; set; } = new EditStrumViewModel();
        public ChordFocus GlobalChordFocus { get; set; } = ChordFocus.Gitar;
        public bool NeedToRefreshImages { get; set; } //jesli trzeba odswiezyc obrazki chordow (bo np ktos usunal)
        public bool FirstChecked { get; set; } = true;
        public List<int> GridsBeatsBarsCount { get; set; } = new List<int>();
        private string profileName;

        public static NotesViewModelLiteVersion PrevFocusedChord { get; set; }

        private List<Point> coordinateEllipse { get; set; } = new List<Point>();

        //public bool IsMouseClicked { get; set; }
        private List<DependencyObject> hitResultsList { get; set; } = new List<DependencyObject>();

        public List<string> Bords { get; set; } = new List<string>();
        public StrumDirection PrevStrumDir { get; set; } = StrumDirection.None;
        public List<StrumDetails> Strums { get; set; } = new List<StrumDetails>();
        public Stopwatch SwCurrentStrumGlobal { get; set; } = new Stopwatch();
        public Stopwatch SwSingleNote { get; set; } = new Stopwatch();
        public List<NotesInStrum> NotesInStrum { get; set; } = new List<NotesInStrum>();
        public long DelayBeforeStrum { get; set; }
        public Dictionary<string, Border> StrunaGlobalDict { get; set; }
        public SolidColorBrush[] StrunaGlobalBrushes { get; set; } = new SolidColorBrush[6];
        public WaveOut[] GlobalStruny { get; set; } = new WaveOut[6];
        public string[] NotsyPath { get; set; } = new string[6];
        public WaveOut[] GlobalStrunyNew { get; set; } = new WaveOut[6];
        public WaveFileReader[] WaveFileReaderNew { get; set; } = new WaveFileReader[6];
        public Task GlobalStrunyTask { get; set; }
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static CancellationToken token = cts.Token;

        public static CancellationTokenSource ctsUpdateChordContent = new CancellationTokenSource();
        public static CancellationToken tokenctzUpdateChordContent = ctsUpdateChordContent.Token;

        public bool IsStrumming { get; set; }
        public bool AlignToGrid { get; } = false;
        public double ItemHeight { get; set; } = 25;
        public KlocekChordViewModel KlocekViewModel { get; set; } = new KlocekChordViewModel();
        public bool IsDragging { get; set; }
        public bool IsSelection { get; set; }
        public bool IsScrollingPlayListCanvas { get; set; }
        public Point StartCanvasScrollingPos { get; set; }
        public Point StartSelectionPos { get; set; }
        public DispatcherTimer PlaylistTimer { get; set; } = new DispatcherTimer();
        public DispatcherTimer StrumTimer { get; set; } = new DispatcherTimer();
        public double[] LastScrollPlayListMousePos { get; set; } = new double[2];
        private double tempX = 0;
        public double BorderPlaylistOffset { get; set; }
        private Stack<List<KlocekChordModel>> PlaylistStack = new Stack<List<KlocekChordModel>>();
        private Stack<List<KlocekChordModel>> PlaylistRedoStack = new Stack<List<KlocekChordModel>>();
        public Stopwatch SwStrumPatternKeys { get; set; } = new Stopwatch();
        public List<StrumKeysModel> StrumKeys { get; set; } = new List<StrumKeysModel>();
        public MidiKeyboardService MidiKeyboardService { get; set; }

        public MainWindow(bool needToRefreshImages)
        {
            Stopwatch swMainWindow = new Stopwatch();
            swMainWindow.Start();
            NeedToRefreshImages = needToRefreshImages;

            InitializeComponent();

            this.DataContext = this;
            this.ProfileName = $"Profile: {App.ProfileName}";
            GridGuitar.DataContext = NotesViewModel;
            CustomControlsItemsControl.DataContext = CustomChordsViewModel;
            ChordsDg.DataContext = ChordsViewModel;
            GridProgression.DataContext = ScaleNotesViewModel;
            pianoCanvas.DataContext = PianoNotesViewModel;
            icGlobalGitarButtons.DataContext = GlobalGitarViewModel;
            icKlocki.DataContext = KlocekViewModel;
            gridStrumPatterns.DataContext = ToViewStrumViewModels;
            MixerViewModel.InitMixerModels();
            gridMixer.DataContext = MixerViewModel;

            for (int i = 0; i < 20; i++)
            {
                GridsBeatsBarsCount.Add(i);
            }

            PianoPressedBrush = new LinearGradientBrush((Color)ColorConverter.ConvertFromString("#FF09787A"), (Color)ColorConverter.ConvertFromString("#FF1BAFB2"), new Point(0.5, 0), new Point(0.5, 1));

            List<NoteOctaveIntervalDetails> notesMy = new List<NoteOctaveIntervalDetails>
            {
                new NoteOctaveIntervalDetails("E", 5),
                new NoteOctaveIntervalDetails("E", 4),
                new NoteOctaveIntervalDetails("E", 3),
                new NoteOctaveIntervalDetails("G", 3),
                new NoteOctaveIntervalDetails("D", 2),
                new NoteOctaveIntervalDetails("D", 3),
                new NoteOctaveIntervalDetails("D", 4),
                new NoteOctaveIntervalDetails("A", 2),
            };

            StrunaGlobalDict = new Dictionary<string, Border>
            {
                { "bordStr7", bordStr7},
                { "bordStr6", bordStr6},
                { "bordStr5", bordStr5},
                { "bordStr4", bordStr4},
                { "bordStr3", bordStr3},
                { "bordStr2", bordStr2},
                { "bordStr1", bordStr1}
            };

            ChordsViewModel.RefreshItems = () => ChordsDg.Items.Refresh();
            NoteModel.SetGitarFocusAction = () => SetChordFocus(ChordFocus.Gitar);
            NoteModel.UpdateGitarMainChordAction = () => UpdateMainGitarChord();
            NoteModel.MoveNoteToPlaylistAction = (note) =>
            {
                PlaylistService.MoveNoteToPlaylistAction(
                    note,
                    KlocekViewModel,
                    cbCurrentChannel,
                    MixerViewModel,
                    PlaylistRedoStack,
                    PlaylistStack
                    );
            };

            NotesViewModelLiteVersion.AddChordToChordsBoxAction = (chord) =>
            {
                PlaylistService.AddChordToPlaylist(
                    chord,
                    KlocekViewModel,
                    NotesViewModel,
                    MixerViewModel,
                    cbCurrentChannel,
                    PlaylistRedoStack,
                    PlaylistStack
                    );
            };

            NotesViewModelLiteVersion.RemoveChordsBoxAction = (chord) =>
            {
                ChordBoxViewModel.ChordsInBox.Remove(chord);
            };

            NotesViewModelLiteVersion.SetFocusOtherChord = (chord) =>
            {
                if (SelectedChord == chord && GlobalChordFocus == ChordFocus.OtherChord) return;
                SelectedOtherChord = chord;
                SetChordFocus(ChordFocus.OtherChord);
            };

            NotesViewModelLiteVersion.SetFocusChord = (chord) =>
            {
                if (SelectedChord == chord && GlobalChordFocus == ChordFocus.Chord) return;
                SelectedChord = chord;
                SetChordFocus(ChordFocus.Chord);
            };

            NotesViewModelLiteVersion.DeselectPrevChord = () =>
            {
                if (SelectedChord != null)
                {
                    SelectedChord.IsSelected = false;
                    SelectedChord.FocusedChord = false;
                }
            };

            NotesViewModelLiteVersion.UpdateIntervalsAction = (type, intervals) =>
            {
                var chordsRow = ChordsViewModel.Chords.Single(a => a.Type == type);

                chordsRow.Intervals = intervals;

                chordsRow.ActionForAllChords
                (
                    (a) =>
                    {
                        a.UpdateIntervals(intervals);

                        if (!a.ChordIntervalsNumbers.SequenceEqual(intervals))
                        {
                            throw new Exception($"Error while generating Intervals! {a.ChordFullName}");
                        }
                    }
                );
            };

            NotesViewModelLiteVersion.RenderChordAction = (viewModel, path, readChordPath) =>
            {
                mainContentControl.Content = viewModel;

                viewModel.ChordReadModeBackground = NotesHelper.ChordColor[viewModel.ChordName];
                viewModel.UpdateChordBorders();
                mainContentControlReadChord.Content = viewModel;

                mainContentControl.UpdateLayout();
                RenderChordService.WriteToPng(mainContentControl, path, NormalChordWidth, NormalChordHeight);

                mainContentControlReadChord.Visibility = Visibility.Visible;
                mainContentControlReadChord.UpdateLayout();
                RenderChordService.WriteToPngReadChord(mainContentControlReadChord, readChordPath, ReadChordWidth, ReadChordHeight);
                mainContentControlReadChord.Visibility = Visibility.Hidden;
            };

            NotesViewModelLiteVersion.MainWindowBlackAction = (makeBlack) =>
            {
                gridBlack.Visibility = makeBlack ? Visibility.Visible : Visibility.Hidden;
            };

            NotesViewModelLiteVersion.GetAllChordsIntervalsAction = () =>
            {
                var res = ChordsViewModel.Chords
                                         .Where(a => a.Intervals?.Count > 0)
                                         .ToDictionary(b => b.Type, c => c.Intervals);
                return res;
            };

            NotesViewModelLiteVersion.SetHeightForAllChords = (height) =>
            {
                foreach (var item in ChordsViewModel.Chords)
                {
                    item.ActionForAllChords((a => a.ChordHeight = height));
                }
            };

            NotesViewModelLiteVersion.ScrollDataGridToItemAction = (obj) =>
            {
                var foundChord = ChordsViewModel.Chords.FirstOrDefault(a => a.Type == "m7");

                if (foundChord != null)
                {
                    ChordsDg.ScrollIntoView(foundChord);
                }
            };

            NotesViewModelLiteVersion.RemoveOtherChord = (obj) =>
            {
                CustomChordsViewModel.Chords.Remove(obj);
            };

            NotesViewModelLiteVersion.MoveToOtherAction = (obj) =>
            {
                CustomChordsViewModel.Chords.Insert(0, obj);
            };

            NotesViewModelLiteVersion.RefreshKolorowanieChordsAction = (chordCodeNormalized, ukladFingers) =>
            {
                foreach (var item in ChordsViewModel.Chords)
                {
                    item.ActionForAllChords
                    (
                        (a) =>
                        {
                            if (a.ChordCodeNormalized == chordCodeNormalized)
                            {
                                for (int i = 0; i < ukladFingers.Count; i++)
                                {
                                    a.Notes[i].CheckedFinger = ukladFingers[i];
                                }

                                a.UpdateChordImageFile();
                                a.UpdateChordImage();
                            }
                        }
                    );
                }

                foreach (var item in CustomChordsViewModel.Chords)
                {
                    if (item.ChordCodeNormalized == chordCodeNormalized)
                    {
                        for (int i = 0; i < ukladFingers.Count; i++)
                        {
                            item.Notes[i].CheckedFinger = ukladFingers[i];
                        }
                    }
                }
            };

            KlocekChordModel.RemoveKlocekFromPlaylist = (klocek) =>
            {
                PlaylistService.AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
                KlocekViewModel.Klocki.Remove(klocek);
            };

            EditStrumViewModel.TryPlayChordWithPattern = (editPattern) =>
            {
                var pattern = NotesViewModel.ConvertEditStrumModelsToStrumPattern(editPattern);
                NotesViewModel.MyExtraPlayChordWithStrumPattern(pattern);
            };

            ToViewSingleStrumViewModels.EditStrumPatternAction = (strumPattern) =>
            {
                EditStrumViewModel.AddEditStrumModel(strumPattern.ToViewStrumModels.ToList());
                EditStrumView editStrumView = new EditStrumView();
                editStrumView.DataContext = EditStrumViewModel;
                editStrumView.Owner = System.Windows.Application.Current.MainWindow;
                editStrumView.Left = 1150 * App.CustomScaleX;
                editStrumView.Top = 200 * App.CustomScaleY;
                NotesViewModelLiteVersion.MainWindowBlackAction?.Invoke(true);

                editStrumView.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                editStrumView.Width *= App.CustomScaleX;
                editStrumView.Height *= App.CustomScaleY;

                editStrumView.ShowDialog();
                NotesViewModelLiteVersion.MainWindowBlackAction?.Invoke(false);

                if (editStrumView.ResultDialog == true)
                {
                    NotesViewModel.GlobalEditStrumModels = EditStrumViewModel.EditStrumModels.ToList();
                    NotesViewModel.GlobalStrumPattern = NotesViewModel.ConvertEditStrumModelsToStrumPattern(NotesViewModel.GlobalEditStrumModels);

                    ToViewStrumViewModels.EditStrumModel(strumPattern, NotesViewModel.GlobalEditStrumModels);
                }
            };

            ToViewSingleStrumViewModels.RemoveStrumPatternAction = (strumPattern) =>
            {
                ToViewStrumViewModels.ToViewSingleStrumModels.Remove(strumPattern);
            };

            ToViewSingleStrumViewModels.RemoveStrumPatternAction = (strumPattern) =>
            {
                var clone = strumPattern.Clone() as ToViewSingleStrumViewModels;
                var idxToInsert = ToViewStrumViewModels.ToViewSingleStrumModels.IndexOf(strumPattern);

                ToViewStrumViewModels.ToViewSingleStrumModels.Insert(idxToInsert, clone);
            };

            KlocekChordModel.PlayChordFromPlaylist = (strumViewModel) =>
            {
                NotesViewModel.TryPlayChordFromPlaylist(strumViewModel);
            };

            MixerModel.UpdateKlocekVisibilityAction = (name, checkboxValue) =>
            {
                int idx = -1;
                for (int i = 0; i < MixerViewModel.MixerModels.Count; i++)
                {
                    if (MixerViewModel.MixerModels[i].Name == name)
                    {
                        idx = i;
                        break;
                    }
                }

                var klockiOnChannel = KlocekViewModel.Klocki.Where(a => a.ChannelNr == idx).ToList();

                foreach (var item in klockiOnChannel)
                {
                    if (checkboxValue)
                    {
                        item.KlocekVisibility = Visibility.Visible;
                    }
                    else
                    {
                        item.KlocekVisibility = Visibility.Hidden;
                    }
                }
            };

            swMainWindow.Stop();
            Debug.WriteLine($"swMainWindow: {swMainWindow.Elapsed}");

            PlaylistTimer.Tick += (send, er) =>
            {
                if (NotesViewModel.MainWaveOut.PlaybackState == PlaybackState.Stopped)
                {
                    PlaylistTimer.Stop();
                }

                var pos = NotesViewModel.MainWaveOut.GetPosition();

                double ms = NotesViewModel.MainWaveOut.GetPosition() * 1000.0 / NotesViewModel.MainWaveOut.OutputWaveFormat.BitsPerSample / NotesViewModel.MainWaveOut.OutputWaveFormat.Channels * 8 / NotesViewModel.MainWaveOut.OutputWaveFormat.SampleRate;
                Debug.WriteLine($"Playlistms: {ms}");
                double delayBpm = 60_000 / NotesViewModel.Bpm;
                double beatPos = ms / delayBpm * NotesViewModel.BeatWidth;

                Canvas.SetLeft(borderPositionPlaylist, beatPos + BorderPlaylistOffset);
            };

            PlaylistTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }

        public string ProfileName
        {
            get
            {
                return profileName;
            }

            set
            {
                profileName = value;
                OnPropertyChanged("ProfileName");
            }
        }

        #region Methods

        private int GetFr(List<NoteModel> checkedNotes)
        {
            if (checkedNotes == null || !checkedNotes.Any()) return 0;

            int fr = checkedNotes.Min(a => a.Prog);

            return fr;
        }

        private InputViewModelFacade GuitarControlVMToInputViewModelFacade(List<NoteModel> checkedNotes, int treshhold = 0)
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

        private void SaveChords(string path)
        {
            StringBuilder sb = new StringBuilder();

            string serializedText = JsonConvert.SerializeObject(CustomChordsViewModel, Formatting.Indented);
            sb.AppendLine("CustomChordsState.json");
            sb.AppendLine(serializedText);
            sb.AppendLine(SeparatedLine);

            serializedText = JsonConvert.SerializeObject(ChordsViewModel, Formatting.Indented);
            sb.AppendLine("DefaultChordsState.json");
            sb.AppendLine(serializedText);
            sb.AppendLine(SeparatedLine);

            serializedText = JsonConvert.SerializeObject(CheckedFingerDict, Formatting.Indented);
            sb.AppendLine("CheckedFingerDict");
            sb.AppendLine(serializedText);
            sb.AppendLine(SeparatedLine);

            serializedText = JsonConvert.SerializeObject(ToViewStrumViewModels, Formatting.Indented);
            sb.AppendLine("ToViewStrumViewModels");
            sb.AppendLine(serializedText);

            string content = sb.ToString();

            File.WriteAllText(path, content);
            AppOptions.Options.LastUsedFile = path;
            AppOptions.Save();

            App.ProfileName = Path.GetFileNameWithoutExtension(path);
            this.ProfileName = $"Profile: {App.ProfileName}";

            string profilePath = Path.Combine(App.ChordImagesProfiles, App.ProfileName);

            if (!Directory.Exists(profilePath))
            {
                Directory.CreateDirectory(profilePath);
            }

            string readProfilePath = Path.Combine(App.ReadChordImagesProfiles, App.ProfileName);

            if (!Directory.Exists(readProfilePath))
            {
                Directory.CreateDirectory(readProfilePath);
            }

            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(App.ChordImagesWorkingPath, profilePath, true);
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(App.ReadChordImagesWorkingPath, readProfilePath, true);
            }
            catch (Exception ex)
            {
            }
        }

        private void LoadChords(string path, bool refreshImages = false, bool firstRun = false)
        {
            if (!File.Exists(path)) return;

            string chordsContent = File.ReadAllText(path);

            var jsons = chordsContent.Split(new string[] { SeparatedLine }, StringSplitOptions.None);
            var customChordsJsonContent = "{" + jsons[0].Substring(jsons[0].IndexOf("\"Chords\""));
            var chordsJsonContent = "{" + jsons[1].Substring(jsons[0].IndexOf("\"Chords\""));
            var checkedFingerDictContent = jsons[2].Replace("CheckedFingerDict", "");
            var toViewStrumViewModelsContent = jsons[3].Replace("ToViewStrumViewModels", "");

            var customChordsViewModelTemp = JsonConvert.DeserializeObject<CustomChordsViewModel>(customChordsJsonContent);
            var chordsViewModelTemp = JsonConvert.DeserializeObject<ChordsViewModel>(chordsJsonContent);
            CheckedFingerDict = JsonConvert.DeserializeObject<Dictionary<string, List<CheckedFinger>>>(checkedFingerDictContent);
            var toViewStrumViewModelsTemp = JsonConvert.DeserializeObject<ToViewStrumViewModels>(toViewStrumViewModelsContent);

            if (toViewStrumViewModelsTemp.ToViewSingleStrumModels != null && toViewStrumViewModelsTemp.ToViewSingleStrumModels.Any())
            {
                ToViewStrumViewModels.ToViewSingleStrumModels.Clear();

                foreach (var item in toViewStrumViewModelsTemp.ToViewSingleStrumModels)
                {
                    ToViewStrumViewModels.ToViewSingleStrumModels.Add(item);
                }
            }

            CustomChordsViewModel.Chords.Clear();
            foreach (var item in customChordsViewModelTemp.Chords)
            {
                CustomChordsViewModel.Chords.Add(item);
            }
            //--------------------------------
            ChordsViewModel.Chords.Clear();

            foreach (var item in chordsViewModelTemp.Chords.Where(a => DisplayChordsFilter.GetListOfDisplayedChords().Contains(a.Type)))
            {
                ChordsViewModel.Chords.Add(item);
            }

            GlobalNotesChordDict.Clear();

            int fontSizeIntervalText;
            ChordMode chordMode;
            if (AppOptions.Options != null && AppOptions.Options.ChordReadMode)
            {
                chordMode = ChordMode.Read;
                fontSizeIntervalText = 10;
                cbReadChordMode.IsChecked = true;
            }
            else
            {
                chordMode = ChordMode.Normal;
                fontSizeIntervalText = 13;
                cbReadChordMode.IsChecked = false;
            }
            FirstChecked = false;

            List<NotesViewModelLiteVersion> totallyAllChords = new List<NotesViewModelLiteVersion>();

            foreach (var item in ChordsViewModel.Chords)
            {
                totallyAllChords.AddRange(item.GetAllChords());
            }

            var tttt = totallyAllChords.AsParallel().Select(a => a.UpdateChordImage(chordMode)).ToList();

            foreach (var item in ChordsViewModel.Chords)
            {
                item.FontSizeIntervalText = fontSizeIntervalText;
                item.ActionForAllChords
                (
                    (chord) =>
                    {
                        if (!string.IsNullOrEmpty(chord.ChordCode))
                        {
                            if (GlobalNotesChordDict.ContainsKey(chord.ChordCode))
                            {
                                var debugChord = GlobalNotesChordDict[chord.ChordCode];
                            }
                            else
                            {
                                GlobalNotesChordDict.Add(chord.ChordCode, chord);
                            }
                        }
                    }
                );
            }

            foreach (var item in CustomChordsViewModel.Chords)
            {
                if (!string.IsNullOrEmpty(item.ChordCode))
                {
                    if (GlobalNotesChordDict.ContainsKey(item.ChordCode))
                    {
                        string code = GetCodeToDuplicateChord(item.ChordCode);
                    }
                    else
                    {
                        GlobalNotesChordDict.Add(item.ChordCode, item);
                    }
                }
            }

            foreach (var item in ChordsViewModel.Chords)
            {
                item.InitIntervalsActions();
            }

            AppOptions.Options.LastUsedFile = path;
            AppOptions.Save();

            if (AppOptions.Options.RenderImage)
            {
                Stopwatch swLocally = new Stopwatch();
                swLocally.Start();
                RefreshAllChordImages();
                swLocally.Stop();
            }
            else if (!firstRun)
            {
                App.ProfileName = Path.GetFileNameWithoutExtension(path);
                this.ProfileName = $"Profile: {App.ProfileName}";

                string profilePath = Path.Combine(App.ChordImagesProfiles, App.ProfileName);

                if (!Directory.Exists(profilePath))
                {
                    Directory.CreateDirectory(profilePath);
                    RefreshAllChordImages(profilePath);
                }

                string readProfilePath = Path.Combine(App.ReadChordImagesProfiles, App.ProfileName);

                if (!Directory.Exists(readProfilePath))
                {
                    Directory.CreateDirectory(readProfilePath);
                    RefreshAllChordImages(readProfilePath);
                }

                try
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(profilePath, App.ChordImagesWorkingPath, true);
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(readProfilePath, App.ReadChordImagesWorkingPath, true);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private string GetCodeToDuplicateChord(string code)
        {
            for (int i = 2; ; i++)
            {
                string tempCode = $"{code} {i}";

                if (!GlobalNotesChordDict.ContainsKey(tempCode)) return tempCode;
            }
        }

        private void RefreshAllChordImages(string path = null)
        {
            foreach (var item in ChordsViewModel.Chords)
            {
                item.ActionForAllChords
                (
                    (a) =>
                    {
                        a.UpdateChordImageFile(path);
                    }
                );
            }
        }

        private void LoadDefaultChords()
        {
            string appSettingsFolder = App.FolderSettingsPath;
            string fullFilePath = Path.Combine(appSettingsFolder, DefaultChordsPath);
            LoadChords(fullFilePath);
        }

        public static ScrollViewer GetScrollViewer(UIElement element)
        {
            if (element == null) return null;

            ScrollViewer retour = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element) && retour == null; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is ScrollViewer)
                {
                    retour = (ScrollViewer)(VisualTreeHelper.GetChild(element, i));
                }
                else
                {
                    retour = GetScrollViewer(VisualTreeHelper.GetChild(element, i) as UIElement);
                }
            }
            return retour;
        }

        #endregion Methods

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineLeft();
            }
            else
            {
                scrollViewer.LineRight();
            }
            e.Handled = true;
        }

        private void BarButtonGuitar_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            int prog = int.Parse(btn.Tag.ToString());

            var notesOnProg = NotesViewModel.Notes.Where(a => a.Prog == prog).ToList();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            notesOnProg.ForEach(a => a.IsSelectedWithoutFocus = true);
            sw.Stop();
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();

            Stopwatch swPArt1 = new Stopwatch();
            swPArt1.Start();
            UpdateMainGitarChord();
            swPArt1.Stop();

            Stopwatch swPArt2 = new Stopwatch();
            swPArt2.Start();
            SetChordFocus(ChordFocus.Gitar);
            swPArt2.Stop();

            sw2.Stop();
        }

        private void BarButtonGuitar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;

            int prog = int.Parse(btn.Tag.ToString());

            var notesOnProg = NotesViewModel.Notes.Where(a => a.Prog == prog).ToList();

            notesOnProg.ForEach(a => a.IsSelected = false);

            SetChordFocus(ChordFocus.Gitar);
        }

        private void btnSaveChords_Click(object sender, RoutedEventArgs e)
        {
            SaveProfileChords();
        }

        private void btnLoadChordsDefault_Click(object sender, RoutedEventArgs e)
        {
            LoadDefaultChords();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            ScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;

            App.CustomScaleX = ScreenWidth / 1920;
            App.CustomScaleY = ScreenHeight / 1080;
            MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            Width *= App.CustomScaleX;
            Height *= App.CustomScaleY;

            Top = (ScreenHeight - Height) / 2;
            Left = (ScreenWidth - Width) / 2;

            Stopwatch swLoaded = new Stopwatch();
            swLoaded.Start();
            BlurService.EnableBlur(this);
            string fullJsonPath = Path.Combine(App.FolderSettingsPath, $"{App.ProfileName}.gup");
            Stopwatch swLoad = new Stopwatch();
            swLoad.Start();
            LoadChords(fullJsonPath, NeedToRefreshImages, firstRun: true);
            swLoad.Stop();
            NeedToRefreshImages = false; //jakby kto usunal obrazki chordow, to generuje przy starcie na nowo
            PianoKeys = pianoCanvas.Children.OfType<Button>().ToList();
            PianoKeysDict = PianoKeys.ToDictionary(a => a.Content.ToString(), b => b);
            PianoNormalBackground = PianoKeys.ToDictionary(a => a.Content.ToString(), b => b.Background);
            foreach (var item in PianoKeys)
            {
                string content = item.Content.ToString();
                string note = content.Substring(0, content.Length - 1);
                item.Background = Brushes.Red;
                //item.Background = NotesHelper.NotesColor[note];
            }

            foreach (var item in PianoKeys)
            {
                item.Background = PianoNormalBackground[item.Content.ToString()];
            }

            if (ChordsViewModel.Chords != null && ChordsViewModel.Chords.Any())
            {
                var firstCChord = ChordsViewModel.Chords.First().ChordC;
                SelectedChord = firstCChord;
                firstCChord.IsSelected = true;
                SetChordFocus(ChordFocus.Chord);
                swLoaded.Stop();
            }

            StrumTimer.Interval = TimeSpan.FromMilliseconds(1);
            StrumTimer.Tick += Timer_Tick;
            Debug.WriteLine($"swLoaded: {swLoaded.Elapsed} swLoad: {swLoad.Elapsed}");
            SetDefaultGlobalStrunaBrushes();

            string playlistSongPath = Path.Combine(App.FolderSettingsPath, $"{App.ProfileName}_Playlist.pgup");

            if (File.Exists(playlistSongPath))
            {
                PlaylistService.LoadPlaylist(
                    playlistSongPath,
                    KlocekViewModel,
                    tbBpm,
                    tbPlaylistWidth,
                    MixerViewModel
                    );
            }

            var rectBesideScroll = svCanvasInput.FindVisualChildren<System.Windows.Shapes.Rectangle>();

            if (rectBesideScroll.Any())
            {
                rectBesideScroll.First().Visibility = Visibility.Hidden;
            }

            //MidiKeyboardService = new MidiKeyboardService(this.Dispatcher);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            KlocekViewModel.Klocki.Clear();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && IsStrumming)
            {
                if (!SwStrumPatternKeys.IsRunning) SwStrumPatternKeys.Start();

                var strumKey = new StrumKeysModel(EditChord.StrumDirection.Downward, SwStrumPatternKeys.ElapsedMilliseconds);
                StrumKeys.Add(strumKey);
            }
            else if (e.Key == Key.Up && IsStrumming)
            {
                if (!SwStrumPatternKeys.IsRunning) SwStrumPatternKeys.Start();

                var strumKey = new StrumKeysModel(EditChord.StrumDirection.Upward, SwStrumPatternKeys.ElapsedMilliseconds);
                StrumKeys.Add(strumKey);
            }
            else if (e.Key == Key.Escape)
            {
                this.WindowState = WindowState.Minimized;
            }
            else if (e.Key == Key.D && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (KlocekViewModel.SelectedItems.Any())
                {
                    foreach (var item in KlocekViewModel.SelectedItems)
                    {
                        var cloneItem = (KlocekChordModel)item.Clone();
                        cloneItem.KlocekOpacity = 1;
                        KlocekViewModel.Klocki.Add(cloneItem);
                    }
                }
            }
            else if (e.Key == Key.Delete)
            {
                if (KlocekViewModel.SelectedItems.Any())
                {
                    PlaylistService.AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
                    foreach (var item in KlocekViewModel.SelectedItems)
                    {
                        KlocekViewModel.Klocki.Remove(item);
                    }
                }
            }
            else if (e.Key == Key.Z && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                //undo
                PlaylistService.GetPlaylistActionFromStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel, MixerViewModel);
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                PlaylistService.GetPlaylistActionFromRedo(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
            }
            else if (e.Key == Key.S && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                string appSettingsFolder = App.FolderSettingsPath;
                string fullJsonPath = Path.Combine(appSettingsFolder, $"{App.ProfileName}_Playlist.pgup");
                PlaylistService.SavePlaylistSong(fullJsonPath, tbBpm, KlocekViewModel, tbPlaylistWidth);
            }
            else
            {
                e.Handled = false;
            }
        }

        private void btnNewChordsRow_Click(object sender, RoutedEventArgs e)
        {
            string textPrefix = "Nowy akord: ";
            string text = "";

            List<CustomMessageBoxValidation> errorsValidation = new List<CustomMessageBoxValidation>()
            {
                new CustomMessageBoxValidation()
                {
                    ErrorText = "Taka nazwa już istnieje",
                    ErrorCondition = (arg) =>
                    {
                        foreach (var item in ChordsViewModel.Chords)
                        {
                            if (item.Type == arg) return true;
                        }

                        return false;
                    }
                },

                new CustomMessageBoxValidation()
                {
                    ErrorText = "Nazwa nie może być pusta",
                    ErrorCondition = (arg) =>
                    {
                        return string.IsNullOrEmpty(arg);
                    }
                }
            };

            CustomMessageBox customMessageBox = new CustomMessageBox(textPrefix, text, errorsValidation);

            customMessageBox.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            customMessageBox.Width *= App.CustomScaleX;
            customMessageBox.Height *= App.CustomScaleY;

            customMessageBox.Owner = System.Windows.Application.Current.MainWindow;
            customMessageBox.ShowDialog();

            if (customMessageBox.DialogResult == true)
            {
                var newChord = new ChordRow(customMessageBox.Text);

                ChordsViewModel.Chords.Add(newChord);

                ChordsDg.Items.Refresh();
            }
        }

        private void btnSaveChordsAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = App.FolderSettingsPath;
            saveFileDialog.Filter = "Guitar Uber Project file (*.gup)|*.gup";

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveChords(saveFileDialog.FileName);
            }
        }

        private void btnLoadChords_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = App.FolderSettingsPath;
            openFileDialog.Filter = "Guitar Uber Project file (*.gup)|*.gup";

            if (openFileDialog.ShowDialog() == true)
            {
                LoadChords(openFileDialog.FileName);
            }
        }

        private void GitarBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChordFocus(ChordFocus.Gitar);
        }

        private void SaveProfileChords()
        {
            string appSettingsFolder = App.FolderSettingsPath;
            string fullJsonPath = Path.Combine(appSettingsFolder, $"{App.ProfileName}.gup");

            SaveChords(fullJsonPath);
        }

        private void UpdateMainGitarChord()
        {
            var checkedNotes = NotesViewModel.GetCheckedNotes();
            InputViewModelFacade inputViewModelFacade = GuitarControlVMToInputViewModelFacade(checkedNotes, treshhold: -1);

            var debugData = inputViewModelFacade.GetNotesShape();

            SelectedGitarChord = NotesHelper.InputFacadeToViewModelLite(inputViewModelFacade);

            if (GlobalNotesChordDict.ContainsKey(SelectedGitarChord.ChordCode))//typ akordu ten sam, ale inny układ palcow
            {
                var chordFound = GlobalNotesChordDict[SelectedGitarChord.ChordCode];
                SelectedGitarChord.ChordName = chordFound.ChordName;
                SelectedGitarChord.ChordType = chordFound.ChordType;

                if (string.IsNullOrWhiteSpace(SelectedViewModel.ChordFullName))
                {
                    SelectedViewModel.ChordName = chordFound.ChordName;
                    SelectedViewModel.ChordType = chordFound.ChordType;
                }
            }

            if (CheckedFingerDict.ContainsKey(SelectedGitarChord.ChordCodeNormalized))
            {
                for (int i = 0; i < SelectedGitarChord.Notes.Count; i++)
                {
                    SelectedGitarChord.Notes[i].CheckedFinger = CheckedFingerDict[SelectedGitarChord.ChordCodeNormalized][i];
                }
            }

            List<int> inactiveStruny = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                var prog = SelectedGitarChord.GetCheckedNoteOnProg(i + 1);

                if (prog == null) inactiveStruny.Add(i + 1);
            }
        }

        private void SetChordFocus(ChordFocus chordFocus)
        {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            switch (chordFocus)
            {
                case ChordFocus.Gitar:
                    if (SelectedChord != null) SelectedChord.FocusedChord = false;
                    if (SelectedOtherChord != null) SelectedOtherChord.FocusedChord = false;
                    gitarBorder.Visibility = Visibility.Visible;

                    GlobalChordFocus = ChordFocus.Gitar;
                    SelectedViewModel = SelectedGitarChord;
                    break;

                case ChordFocus.Chord:
                    if (SelectedChord != null) SelectedChord.FocusedChord = true;
                    if (SelectedOtherChord != null) SelectedOtherChord.FocusedChord = false;
                    gitarBorder.Visibility = Visibility.Hidden;

                    GlobalChordFocus = ChordFocus.Chord;
                    SelectedViewModel = SelectedChord;
                    if (cbSynchroChordsOnGuitar.IsChecked == true)
                    {
                        if (GlobalStrunyTask?.Status == TaskStatus.Running)
                        {
                            cts.Cancel();
                        }
                        GlobalStrunyTask = Task.Factory.StartNew(() => SynchroToGuitar(), token);
                    }
                    break;

                case ChordFocus.OtherChord:
                    if (SelectedChord != null) SelectedChord.FocusedChord = false;
                    if (SelectedOtherChord != null) SelectedOtherChord.FocusedChord = true;
                    gitarBorder.Visibility = Visibility.Hidden;

                    GlobalChordFocus = ChordFocus.OtherChord;
                    SelectedViewModel = SelectedOtherChord;
                    if (cbSynchroChordsOnGuitar.IsChecked == true) SynchroToGuitar();
                    break;

                default:
                    break;
            }
            sw1.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            mainContentControl.Content = SelectedViewModel;
            sw2.Stop();

            Stopwatch sw3 = new Stopwatch();
            sw3.Start();
            if (SelectedViewModel != null && SelectedViewModel.NoteOctaves != null && SelectedViewModel.NoteOctaves.Any(a => !string.IsNullOrEmpty(a.Name)))
            {
                var filteredNoteOctaves = SelectedViewModel.NoteOctaves
                                                        .Where(a => !string.IsNullOrEmpty(a.Name))
                                                        .ToList();

                var notesIntervals = filteredNoteOctaves
                                                .Select(b => new NoteOctaveIntervalDetails(b.Name, int.Parse(b.Octave)))
                                                .Reverse()
                                                .ToList();

                string chordName = SelectedViewModel.ChordName ?? notesIntervals.First().Note;
                string chordType = SelectedViewModel.ChordType ?? "None";

                try
                {
                    ChordIntervalsDetailsViewModel = ChordIntervalHelper.ConvertIntoChordIntervalDetails(chordName, chordType, notesIntervals);
                }
                catch (Exception)
                {
                }
                chordIntervalsDetailsPanel.DataContext = ChordIntervalsDetailsViewModel;

                SetPianoKeys(filteredNoteOctaves);
            }
            else
            {
                ChordIntervalsDetailsViewModel = new ChordIntervalsDetailsViewModel();
                chordIntervalsDetailsPanel.DataContext = ChordIntervalsDetailsViewModel;

                ResetPianoKeys();
            }
            sw3.Stop();
        }

        private void SynchroToGuitar()
        {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            if (SelectedViewModel == null) return;

            var checkedNotes = SelectedViewModel.Notes.Where(a => a.CheckedFinger != CheckedFinger.None).ToList();

            NotesViewModel.Notes.ForEach(a => a.ClearNote());

            //Fr: 2
            int myFr = SelectedViewModel.Fr;

            if (myFr == 0) myFr = 1;

            foreach (var item in checkedNotes)
            {
                string key = $"s{item.Struna}p{item.Prog + myFr}";
                NotesViewModel.NotesDict[key].IsSelecteOnlySet = true;
            }

            for (int i = 0; i < SelectedViewModel.NotesO.Count; i++)
            {
                if (SelectedViewModel.NotesO[i] == "O")
                {
                    string key = $"s{i + 1}p0";
                    NotesViewModel.NotesDict[key].IsSelecteOnlySet = true;
                }
            }
            sw1.Stop();

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            NoteModel.UpdateGitarMainChord();
            sw2.Stop();
        }

        private void SetPianoKeys(List<NoteOctaveDetails> notes)
        {
            ResetPianoKeys();

            foreach (var item in notes)
            {
                var pianoBrush = NotesHelper.ChordColor[item.Name];
                string key = $"{item.Name}{item.Octave}";

                PianoKeysDict[key].Background = pianoBrush;
            }
        }

        private void ResetPianoKeys()
        {
            foreach (var item in PianoKeys)
            {
                item.Background = PianoNormalBackground[item.Content.ToString()];
            }
        }

        private void Border_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetChordFocus(ChordFocus.Gitar);
        }

        private void BtnSynchroToGitar_Click(object sender, RoutedEventArgs e)
        {
            SynchroToGuitar();
        }

        private void borderMinimize_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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

        private void DisableNotesOutsideScale()
        {
            //var printedScale = GetPrintedScale("Eolska");

            var scaleNotes = ScaleNotesViewModel.ScaleNotes;

            NotesViewModel.Notes.ForEach(a => a.IsNoteEnabled = true);

            foreach (var item in NotesViewModel.Notes)
            {
                if (!scaleNotes.Any(a => a.Note == item.Name))
                {
                    item.IsNoteEnabled = false;
                }
            }

            foreach (var item in ChordsViewModel.Chords)
            {
                item.ActionForAllChords
                (
                    (a) =>
                    {
                        a.ChordOpacity = 1;
                    }
                );
            }

            //czesciowe ukrycie akordow nie pasujacych do skali
            foreach (var item in ChordsViewModel.Chords)
            {
                item.ActionForAllChords
                (
                    (a) =>
                    {
                        var chordNotes = a.NoteOctaves.Select(c => c.Name).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();

                        if (!chordNotes.Any())
                        {
                            a.ChordOpacity = 0.2;
                            return;
                        }

                        var scaleNotesName = scaleNotes.Select(b => b.Note).ToList();
                        var notesOutsideScale = chordNotes.Except(scaleNotesName).ToList();

                        if (notesOutsideScale.Any()) a.ChordOpacity = 0.2;
                    }
                );
            }

            foreach (var item in CustomChordsViewModel.Chords)
            {
                item.ChordOpacity = 1;
            }

            foreach (var item in CustomChordsViewModel.Chords)
            {
                var chordNotes = item.NoteOctaves.Select(c => c.Name).Where(d => !string.IsNullOrEmpty(d)).Distinct().ToList();

                if (!chordNotes.Any())
                {
                    item.ChordOpacity = 0.2;
                    return;
                }

                var scaleNotesName = scaleNotes.Select(b => b.Note).ToList();
                var notesOutsideScale = chordNotes.Except(scaleNotesName).ToList();

                if (notesOutsideScale.Any()) item.ChordOpacity = 0.2;
            }
        }

        private string GetPrintedScale(string partScaleName)
        {
            ScaleNotesViewModel.GetAllScaleNotes();
            var allScales = ScaleNotesViewModel.GetMyDict();

            var durowa = allScales.Where(a => a.Key.Contains(partScaleName)).ToList();

            StringBuilder globalSb = new StringBuilder();
            var fitChords = new List<string>();
            foreach (var item in durowa)
            {
                var scaleNotesMy = item.Value.Select(a => a.Note).ToList();
                fitChords.Clear();
                foreach (var chords in ChordsViewModel.Chords)
                {
                    chords.ActionForAllChords
                    (
                        (a) =>
                        {
                            var except = a.ChordIntervalsNotes.Except(scaleNotesMy).ToList();
                            if (!except.Any())
                            {
                                var text = $"{a.ChordFullName} - {string.Join(" ", a.ChordIntervalsNotes)}";
                                fitChords.Add(text);
                            }
                        }
                    );
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(item.Key);
                sb.AppendLine(string.Join("  ", scaleNotesMy));
                sb.AppendLine(string.Join(Environment.NewLine, fitChords));
                globalSb.AppendLine(sb.ToString());
            }

            var wholeText = globalSb.ToString();
            return wholeText;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DisableNotesOutsideScale();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDisableNotesOutsideScale.IsChecked.Value == true)
            {
                DisableNotesOutsideScale();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            NotesViewModel.Notes.ForEach(a => a.IsNoteEnabled = true);

            foreach (var item in ChordsViewModel.Chords)
            {
                item.ActionForAllChords
                (
                    (a) =>
                    {
                        a.ChordOpacity = 1;
                    }
                );
            }

            foreach (var item in CustomChordsViewModel.Chords)
            {
                item.ChordOpacity = 1;
            }
        }

        private void ChordsDg_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        private void CbReadChordMode_Checked(object sender, RoutedEventArgs e)
        {
            if (ChordsDg == null || FirstChecked) return;

            AppOptions.Options.ChordReadMode = true;
            AppOptions.Save();

            ChordMode chordMode = ChordMode.Read;

            List<NotesViewModelLiteVersion> totallyAllChords = new List<NotesViewModelLiteVersion>();

            foreach (var item in ChordsViewModel.Chords)
            {
                totallyAllChords.AddRange(item.GetAllChords());
                item.FontSizeIntervalText = 10;
            }

            var tttt = totallyAllChords.AsParallel().Select(a => a.UpdateChordImage(chordMode)).ToList();
        }

        private void CbReadChordMode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ChordsDg == null || FirstChecked) return;

            AppOptions.Options.ChordReadMode = false;
            AppOptions.Save();

            ChordMode chordMode = ChordMode.Normal;

            List<NotesViewModelLiteVersion> totallyAllChords = new List<NotesViewModelLiteVersion>();

            foreach (var item in ChordsViewModel.Chords)
            {
                totallyAllChords.AddRange(item.GetAllChords());
                item.FontSizeIntervalText = 13;
            }

            var tttt = totallyAllChords.AsParallel().Select(a => a.UpdateChordImage(chordMode)).ToList();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed) return;

            var mousePos = Mouse.GetPosition(mainCanvas);
            Point p = Mouse.GetPosition(mainCanvas);
            HitTestResult result = VisualTreeHelper.HitTest(mainCanvas, p);

            hitResultsList.Clear();

            // Set up a callback to receive the hit test result enumeration.
            VisualTreeHelper.HitTest(mainCanvas, null,
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(p));

            var resHitTest = hitResultsList.FirstOrDefault(a => a.GetType().Name == "Border");
            if (resHitTest != null)
            {
                string borderName = ((Border)resHitTest).Name;
                Strum(borderName);
            }
        }

        private void Struny_MouseEnter(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed) return;

            Border border = sender as Border;

            Strum(border.Name);
        }

        private void Struny_MouseLeave(object sender, MouseEventArgs e)
        {
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed) return;

            Border border = sender as Border;

            Strum(border.Name);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsStrumming) return;

            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed) return;

            Point p = Mouse.GetPosition(mainCanvas);
            HitTestResult result = VisualTreeHelper.HitTest(mainCanvas, p);

            hitResultsList.Clear();

            // Set up a callback to receive the hit test result enumeration.
            VisualTreeHelper.HitTest(mainCanvas, null,
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(p));

            var resHitTest = hitResultsList.FirstOrDefault(a => a.GetType().Name == "Border");
            if (resHitTest != null)
            {
                string borderName = ((Border)resHitTest).Name;

                Strum(borderName);
            }

            int ss = 2;
        }

        public HitTestResultBehavior MyHitTestResult(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add(result.VisualHit);

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        private void Strum(string borderName)
        {
            if (Bords.Count == 0)
            {
                Bords.Add(borderName);

                DelayBeforeStrum = SwCurrentStrumGlobal.ElapsedMilliseconds;
                SwCurrentStrumGlobal.Restart();
            }
            else
            {
                string prevBorderName = Bords[Bords.Count - 1];

                if (borderName == prevBorderName) return;

                var currentDir = AnalyzeStrumDirection(prevBorderName, borderName);
                if (currentDir != PrevStrumDir && PrevStrumDir != StrumDirection.None)
                {
                    EndOfStrumSerie();
                }

                int currentStruna = int.Parse(borderName.Replace("bordStr", ""));

                if (currentDir == StrumDirection.Downward)
                {
                    currentStruna--;
                }
                else if (currentDir == StrumDirection.Upward)
                {
                    //currentStruna++;
                }
                else
                {
                }

                Bords.Add(borderName);

                long delayBeforeNote = SwSingleNote.ElapsedMilliseconds;
                if (!NotesInStrum.Any())
                {
                    delayBeforeNote = 0;
                    DelayBeforeStrum = SwSingleNote.ElapsedMilliseconds;
                }

                if (GlobalStrunyNew[6 - currentStruna] != null)
                {
                    //GlobalStrunyNew[6 - currentStruna].Stop();
                    WaveFileReaderNew[6 - currentStruna].Position = 0;
                    GlobalStrunyNew[6 - currentStruna].Play();
                }

                StrunaGlobalDict[$"bordStr{currentStruna + 1}"].BorderBrush = Brushes.Green;

                if (IsStrumming)
                {
                    NotesInStrum.Add(new NotesInStrum(currentStruna, delayBeforeNote));
                }
                SwSingleNote.Restart();
                tbStrumCurrent.Text = string.Join(", ", NotesInStrum.Select(a => a.StrunaNr));
                PrevStrumDir = currentDir;
            }
        }

        private void SetDefaultGlobalStrunaBrushes()
        {
            for (int i = 0; i < StrunaGlobalBrushes.Length; i++)
            {
                StrunaGlobalBrushes[i] = NotesHelper.GlobalStrunaBrush;
            }
        }

        private void SetGlobalStrunaBrushesInactive(List<int> struny)
        {
            if (!struny.Any()) return;

            for (int i = 0; i < StrunaGlobalBrushes.Length; i++)
            {
                StrunaGlobalBrushes[i] = NotesHelper.GlobalStrunaBrush;
            }

            foreach (var item in struny)
            {
                int borderIdx = 6 - item;
                //StrunaGlobalDict[$"bordStr{borderIdx + 2}"].BorderBrush = NotesHelper.GlobalStrunaBrushInactive;
                StrunaGlobalBrushes[borderIdx] = NotesHelper.GlobalStrunaBrushInactive;
            }

            RepaintStruny();
        }

        private void RepaintStruny()
        {
            int idx = 0;
            foreach (var item in StrunaGlobalDict.Values.Reverse())
            {
                if (idx - 1 < 0)
                {
                    item.BorderBrush = NotesHelper.GlobalStrunaBrush;
                }
                else
                {
                    item.BorderBrush = StrunaGlobalBrushes[idx - 1];
                }

                idx++;
            }
        }

        private void EndOfStrumSerie()
        {
            var notes = NotesInStrum.ToList();

            if (!notes.Any())
            {
                NotesInStrum.Clear();
                Bords.Clear();
                PrevStrumDir = StrumDirection.None;
                SwCurrentStrumGlobal.Restart();
                return;
            }

            foreach (var item in notes)
            {
                item.StrunaNr = 7 - item.StrunaNr;
            }

            var myTook = notes.Sum(a => a.DelayBeforeMs) / notes.Count;

            StrumDetails strumDetails = new StrumDetails(myTook, DelayBeforeStrum, PrevStrumDir, notes);

            Strums.Add(strumDetails);
            NotesInStrum.Clear();
            Bords.Clear();
            PrevStrumDir = StrumDirection.None;
            SwCurrentStrumGlobal.Restart();

            tbStrum.Text = string.Join(Environment.NewLine, Strums);
        }

        private StrumDirection AnalyzeStrumDirection(string borderNamePrev, string borderNameCurrent)
        {
            int prevStruna = int.Parse(borderNamePrev.Replace("bordStr", ""));
            int currentStruna = int.Parse(borderNameCurrent.Replace("bordStr", ""));

            StrumDirection res = (currentStruna > prevStruna) ? StrumDirection.Downward : StrumDirection.Upward;
            return res;
        }

        private void TitleWindow_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsStrumming)
            {
                EndOfStrumSerie();
            }
            RepaintStruny();

            if (IsDragging)
            {
                KlocekViewModel.LeaveKlocekDragged();
                IsDragging = false;
            }

            if (IsSelection)
            {
                borderSelection.Visibility = Visibility.Hidden;
                Canvas.SetZIndex(borderSelection, 0);
                IsSelection = false;
            }
        }

        private void BtnLoadSong_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = App.FolderSettingsPath;
            openFileDialog.Filter = "Playlist Guitar Uber Project file (*.pgup)|*.pgup";

            if (openFileDialog.ShowDialog() == true)
            {
                PlaylistService.LoadPlaylist(
                    openFileDialog.FileName,
                    KlocekViewModel,
                    tbBpm,
                    tbPlaylistWidth,
                    MixerViewModel
                    );
            }
        }

        private void BarButtonGuitar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                Button btn = (Button)sender;
                int prog = int.Parse(btn.Tag.ToString());

                NotesViewModel.MoveToChooseProg(prog);
                e.Handled = true;
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                NotesViewModel.PlayDownMethod();
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                NotesViewModel.PlayAtOnceMethod();
            }
        }

        private void Border_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                NotesViewModel.StrumDelayMs -= 10;

                if (NotesViewModel.StrumDelayMs < 0) NotesViewModel.StrumDelayMs = 0;
            }
            else
            {
                NotesViewModel.StrumDelayMs += 10;
            }
            e.Handled = true;
        }

        private void CanvasInput_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = Mouse.GetPosition(canvasInput);
            HitTestResult result = VisualTreeHelper.HitTest(canvasInput, mousePos);

            hitResultsList.Clear();

            // Set up a callback to receive the hit test result enumeration.
            VisualTreeHelper.HitTest(canvasInput, null,
                new HitTestResultCallback(MyHitTestResult),
                new PointHitTestParameters(mousePos));

            var klocekToDrag = hitResultsList.Where(a => a.GetType().Name == "Border" && (((Border)a).Name == "borderInput" || ((Border)a).Name == "chordBorder"))
                                            .Select(b => ((KlocekChordModel)((Border)b).DataContext))
                                            .Where(c => c.ChannelNr == cbCurrentChannel.SelectedIndex)
                                            .FirstOrDefault();

            if (klocekToDrag != null && !klocekToDrag.IsChord)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    KlocekViewModel.CtrlSelection.Add(klocekToDrag);
                }

                IsDragging = true;

                var pos = e.GetPosition(canvasInput);
                KlocekViewModel.SetKlocekDragged(klocekToDrag, pos);

                //UIElement container = VisualTreeHelper.GetParent(mainCanvas) as UIElement;
                //Point relativeLocation = argKlocek.TranslatePoint(new Point(0, 0), container);
                //DeltaMousePos = new Point(pos.X - relativeLocation.X, pos.Y - relativeLocation.Y);
                PlaylistService.AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
            }
            else if (klocekToDrag != null && klocekToDrag.IsChord)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    KlocekViewModel.CtrlSelection.Add(klocekToDrag);
                }

                IsDragging = true;

                var pos = e.GetPosition(canvasInput);
                KlocekViewModel.SetKlocekDragged(klocekToDrag, pos);

                //UIElement container = VisualTreeHelper.GetParent(mainCanvas) as UIElement;
                //Point relativeLocation = argKlocek.TranslatePoint(new Point(0, 0), container);
                //DeltaMousePos = new Point(pos.X - relativeLocation.X, pos.Y - relativeLocation.Y);
                PlaylistService.AddPlaylistActionToStack(PlaylistRedoStack, PlaylistStack, KlocekViewModel);
            }
            else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                Canvas.SetLeft(borderSelection, mousePos.X);
                Canvas.SetTop(borderSelection, mousePos.Y);
                IsSelection = true;

                StartSelectionPos = mousePos;
                borderSelection.Visibility = Visibility.Visible;
                Canvas.SetZIndex(borderSelection, 30);
            }
            else
            {
                KlocekViewModel.LeaveSelectedItems();

                if (IsStrumming) return;
                if (KlocekViewModel.Klocki == null || !KlocekViewModel.Klocki.Any()) return;

                double delayBpm = 60_000 / NotesViewModel.Bpm;
                double fullDelay = mousePos.X / NotesViewModel.BeatWidth * delayBpm;

                Canvas.SetLeft(borderPositionPlaylist, mousePos.X);

                BorderPlaylistOffset = mousePos.X;
                PlaylistTimer.Stop();
                PlaylistTimer.Start();
                NotesViewModel.PlayPlaylist(KlocekViewModel.Klocki, MixerViewModel.MixerModels.ToList(), fullDelay);

                //NotesViewModel.PlayPlaylist(KlocekViewModel.Klocki, fullDelay);
            }
        }

        private void TitleWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDragging && !IsSelection && !IsScrollingPlayListCanvas) return;

            Point mousePos = e.GetPosition(canvasInput);

            if (IsDragging)
            {
                bool alignToGrid = !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
                KlocekViewModel.KlocekDragging(mousePos, NotesViewModel.BeatWidth / 4, alignToGrid);
            }
            else if (IsSelection)
            {   //For X
                if (mousePos.X < StartSelectionPos.X)
                {
                    Canvas.SetLeft(borderSelection, mousePos.X);
                    borderSelection.Width = StartSelectionPos.X - mousePos.X;
                }
                else
                {
                    Canvas.SetLeft(borderSelection, StartSelectionPos.X);
                    borderSelection.Width = mousePos.X - StartSelectionPos.X;
                }

                //For Y
                if (mousePos.Y < StartSelectionPos.Y)
                {
                    Canvas.SetTop(borderSelection, mousePos.Y);
                    borderSelection.Height = StartSelectionPos.Y - mousePos.Y;
                }
                else
                {
                    Canvas.SetTop(borderSelection, StartSelectionPos.Y);
                    borderSelection.Height = mousePos.Y - StartSelectionPos.Y;
                }

                Rect selection = new Rect(Canvas.GetLeft(borderSelection), Canvas.GetTop(borderSelection), borderSelection.Width, borderSelection.Height);

                KlocekViewModel.CalculateSelection(selection, cbCurrentChannel.SelectedIndex);
            }
            else if (IsScrollingPlayListCanvas)
            {
                if (LastScrollPlayListMousePos.Any(a => a == mousePos.X)) return;

                var deltaScroll = StartCanvasScrollingPos.X - mousePos.X;
                var rr = svCanvasInput.HorizontalOffset;
                svCanvasInput.ScrollToHorizontalOffset(rr + deltaScroll);

                LastScrollPlayListMousePos[0] = StartCanvasScrollingPos.X;
                LastScrollPlayListMousePos[1] = mousePos.X;

                StartCanvasScrollingPos = new Point(StartCanvasScrollingPos.X - deltaScroll, 0);
            }

            //var tempRect = new Rect(Canvas.GetLeft(DraggedItemControl), Canvas.GetTop(DraggedItemControl), DraggedItemControl.ActualWidth, DraggedItemControl.ActualHeight);
            //KlocekViewModel.CalculatePosition(tempRect);
        }

        private void TitleWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            KlocekViewModel.LeaveSelectedItems();
        }

        private void BtnPlayPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (NotesViewModel.MainWaveOut.PlaybackState == PlaybackState.Paused)
            {
                NotesViewModel.MainWaveOut.Resume();
            }
            else
            {
                if (KlocekViewModel.Klocki == null || !KlocekViewModel.Klocki.Any()) return;

                IsStrumming = false;
                BorderPlaylistOffset = 0;
                PlaylistTimer.Stop();
                PlaylistTimer.Start();

                NotesViewModel.PlayPlaylist(KlocekViewModel.Klocki, MixerViewModel.MixerModels.ToList());

                //NotesViewModel.PlayPlaylist(KlocekViewModel.Klocki);
            }
        }

        private void BtnPausePlaylist_Click(object sender, RoutedEventArgs e)
        {
            NotesViewModel.MainWaveOut.Pause();
        }

        private void TbBpm_TextChanged(object sender, TextChangedEventArgs e)
        {
            var myText = tbBpm.Text.Replace(',', '.');
            bool parseResult = double.TryParse(myText, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed);

            if (parseResult)
            {
                NotesViewModel.Bpm = parsed;
                PlaylistService.UpdateChordsInPlaylist(
                    KlocekViewModel,
                    NotesViewModel,
                    PlaylistRedoStack,
                    PlaylistStack,
                    selectedItemsOnly: false,
                    changeStrumPattern: false
                    );
            }
        }

        private void BtnStopPlaylist_Click(object sender, RoutedEventArgs e)
        {
            PlaylistService.StopPlaylist(BorderPlaylistOffset, NotesViewModel, PlaylistTimer, borderPositionPlaylist);
        }

        private void BtnSaveSongAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = App.FolderSettingsPath;
            saveFileDialog.Filter = "Playlist Guitar Uber Project file (*.pgup)|*.pgup";

            if (saveFileDialog.ShowDialog() == true)
            {
                PlaylistService.SavePlaylistSong(saveFileDialog.FileName, tbBpm, KlocekViewModel, tbPlaylistWidth);
            }
        }

        private void CanvasInput_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                IsScrollingPlayListCanvas = true;
                StartCanvasScrollingPos = Mouse.GetPosition(canvasInput);
            }
        }

        private void CanvasInput_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                IsScrollingPlayListCanvas = false;
            }
        }

        private void TbPlaylistWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (canvasInput == null) return;

            bool parseResult = int.TryParse(tbPlaylistWidth.Text, out var parsed);

            if (parseResult)
            {
                canvasInput.Width = parsed;
            }
        }

        private void BtnCreateStrum_Click(object sender, RoutedEventArgs e)
        {
            StrumPatternService.CreateStrumPattern(
                IsStrumming,
                SwStrumPatternKeys,
                StrumKeys,
                NotesViewModel,
                EditStrumViewModel,
                ToViewStrumViewModels,
                StrumTimer,
                SwCurrentStrumGlobal,
                SwSingleNote,
                Bords,
                Strums,
                NotesInStrum
                );
        }

        private void BtnNewStrum_Click(object sender, RoutedEventArgs e)
        {
            Strums.Clear();
            NotesInStrum.Clear();
            Bords.Clear();
            SwCurrentStrumGlobal.Restart();
            SwSingleNote.Restart();
            tbStrum.Text = "";
            tbStrumCurrent.Text = "";
            StrumTimer.Stop();
            StrumTimer.Start();
            IsStrumming = true;
            StrumKeys.Clear();
            SwStrumPatternKeys.Stop();
        }

        private void MainCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            StrumPatternService.StrumStop(
                StrumTimer,
                SwCurrentStrumGlobal,
                SwSingleNote,
                Bords,
                IsStrumming,
                NotesViewModel,
                Strums,
                NotesInStrum
                );

            NotesViewModel.MyExtraPlayChordWithStrumPattern(null);
        }

        private void LbStrumPatterns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbStrumPatterns.SelectedItem == null) return;

            var inputStrumModel = ToViewStrumViewModels.ToViewSingleStrumModels[lbStrumPatterns.SelectedIndex].ToViewStrumModels.ToList();
            EditStrumViewModel.AddEditStrumModel(inputStrumModel);
            NotesViewModel.GlobalEditStrumModels = EditStrumViewModel.EditStrumModels.ToList();
            NotesViewModel.GlobalStrumPattern = NotesViewModel.ConvertEditStrumModelsToStrumPattern(NotesViewModel.GlobalEditStrumModels);

            PlaylistService.UpdateChordsInPlaylist(
                KlocekViewModel,
                NotesViewModel,
                PlaylistRedoStack,
                PlaylistStack,
                selectedItemsOnly: true,
                changeStrumPattern: true);
        }

        private void LbStrumPatterns_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                var myBorder = e.OriginalSource as Border;
                if (myBorder != null)
                {
                    var myStrumPattern = myBorder.DataContext as ToViewSingleStrumViewModels;
                    if (myStrumPattern != null)
                    {
                        var myConvertedPattern = NotesViewModel.ConvertToViewStrumModelsToStrumPattern(myStrumPattern.ToViewStrumModels.ToList());
                        NotesViewModel.MyExtraPlayChordWithStrumPattern(myConvertedPattern);
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
        }

        private void BtnRemoveStrumSelection_Click(object sender, RoutedEventArgs e)
        {
            lbStrumPatterns.SelectedItem = null;
            NotesViewModel.GlobalEditStrumModels.Clear();
            NotesViewModel.GlobalStrumPattern.Clear();
            SwStrumPatternKeys.Stop();
            StrumKeys.Clear();
        }

        private void SvCanvasInput_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineLeft();
            }
            else
            {
                scrollViewer.LineRight();
            }
            e.Handled = true;
        }

        private void BtnAddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            var checkedNotes = NotesViewModel.GetCheckedNotes();
            InputViewModelFacade inputViewModelFacade = GuitarControlVMToInputViewModelFacade(checkedNotes, treshhold: -1);

            //var debugData = inputViewModelFacade.GetNotesShape();

            inputViewModelFacade.Name = "GDM 5.0";
            NotesViewModelLiteVersion res = NotesHelper.InputFacadeToViewModelLite(inputViewModelFacade);
            res.ChordName = SelectedGitarChord.ChordName;
            res.ChordType = SelectedGitarChord.ChordType;

            PlaylistService.AddChordToPlaylist(
                res,
                KlocekViewModel,
                NotesViewModel,
                MixerViewModel,
                cbCurrentChannel,
                PlaylistRedoStack,
                PlaylistStack
                );
        }

        private void BtnExportToMp3_Click(object sender, RoutedEventArgs e)
        {
            if (KlocekViewModel.Klocki == null || !KlocekViewModel.Klocki.Any()) return;

            var offset = 1000;
            for (int i = 1; i < KlocekViewModel.Klocki.Count; i++)
            {
                var item = KlocekViewModel.Klocki[i];
                item.XPos = offset;
                offset += 1000;
            }

            IsStrumming = false;
            BorderPlaylistOffset = 0;
            PlaylistTimer.Stop();
            PlaylistTimer.Start();

            NotesViewModel.PlayPlaylist(KlocekViewModel.Klocki, MixerViewModel.MixerModels.ToList(), exportToWavMp3: true);
        }

        private void BtnGame_Click(object sender, RoutedEventArgs e)
        {
            //GameWindow gameWindow = new GameWindow();
            //RecognizeNote gameWindow = new RecognizeNote();
            //RecognizeChord gameWindow = new RecognizeChord();
            //FindChordsOnGuitar gameWindow = new FindChordsOnGuitar();
            GameChoose gameWindow = new GameChoose();

            gameWindow.Owner = System.Windows.Application.Current.MainWindow;

            gameWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
            gameWindow.Width *= App.CustomScaleX;
            gameWindow.Height *= App.CustomScaleY;

            gridBlack.Visibility = Visibility.Visible;
            gameWindow.ShowDialog();

            gridBlack.Visibility = Visibility.Hidden;
        }

        private void BtnExportToMidi_Click(object sender, RoutedEventArgs e)
        {
            var pathMidi = Path.Combine(App.FolderSettingsPath, App.PathMidi);

            if (!Directory.Exists(pathMidi))
                Directory.CreateDirectory(pathMidi);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = pathMidi;
            saveFileDialog.Filter = "MIDI (*.mid)|*.mid";

            if (saveFileDialog.ShowDialog() == true)
            {
                MidiService.ExportToMidiFile(saveFileDialog.FileName, KlocekViewModel);
            }
        }

        private string GetPossibleScalesForNotes()
        {
            string result = string.Empty;

            ScaleNotesViewModel.GetAllScaleNotes();
            var notesInPlaylist = KlocekViewModel.GetAllNoteNamesFromKlocki();

            if (!notesInPlaylist.Any())
            {
                result = "No notes in Playlist";
                return result;
            }

            var possibleScales = ScaleNotesViewModel.GetPossibleScales(notesInPlaylist);
            result = possibleScales.Any() ? string.Join(Environment.NewLine, possibleScales) : "No results";

            return result;
        }

        private void BtnFindScale_Click(object sender, RoutedEventArgs e)
        {
            string scaleResult = GetPossibleScalesForNotes();
            MessageBox.Show(scaleResult);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbDisableNotesOutsideScale.IsChecked == true)
            {
                DisableNotesOutsideScale();
            }
        }
    }
}