using GitarUberProject;
using GitarUberProject.Helperes;
using GitarUberProject.Helpers;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EditChordsWindow
{
    public enum ChordMode
    {
        Read,
        Normal
    }

    public class NotesViewModelLiteVersion : INotifyPropertyChanged, ICloneable
    {
        private RelayCommand kolorujChord;
        private RelayCommand usunOtherChord;
        private RelayCommand editChord;
        private RelayCommand grajCommand;
        private RelayCommand isSelectCommand;
        private RelayCommand isSelectChordCommand;
        private RelayCommand playDownCommand;
        private RelayCommand playDownOtherCommand;
        private RelayCommand moveToChordBox;
        private RelayCommand removeFromChordBox;
        private RelayCommand moveToOther;

        private int fr;
        private string frText;
        private bool focusedChord;
        private bool isSelected;
        private string chordName;
        private string chordType;
        private string chordId;
        private string chordImagePath;
        private BitmapImage chordImage;
        private double chordOpacity = 1;
        private double chordWidth = 145; // read: 145, normal 140
        private double chordHeight = 30; // read: 30, normal 70
        private Thickness chordMargin { get; set; } = new Thickness(10, 5, 5, 5); // read: 10 5 5 5, normal 0
        private LinearGradientBrush chordReadModeBackground;

        public string ChordCode { get; set; }
        public string ChordCodeNormalized{ get; set; }

        public static int RowSize { get; } = 6;
        public static int ColumnSize { get; } = 6;
        private CheckedFinger hoverFinger;
        public static Action DeselectAllOtherChords { get; set; }
        public static Action<NotesViewModelLiteVersion> SetFocusOtherChord { get; set; }
        public static Action DeselectPrevChord { get; set; }
        public static Action<NotesViewModelLiteVersion> SetFocusChord { get; set; }
        public static Func<Dictionary<string, List<int>>> GetAllChordsIntervalsAction { get; set; }
        public static Action<string, List<int>> UpdateIntervalsAction { get; set; }
        public static Action<double> SetHeightForAllChords { get; set; }
        public static Action<NotesViewModelLiteVersion> ScrollDataGridToItemAction { get; set; }
        public static Action<NotesViewModelLiteVersion, string, string> RenderChordAction { get; set; }
        public static Action<NotesViewModelLiteVersion> PlayDownAction { get; set; }
        public static Action<string, List<CheckedFinger>> RefreshKolorowanieChordsAction { get; set; }
        public static Action<bool> MainWindowBlackAction { get; set; }
        public static Action<NotesViewModelLiteVersion> AddChordToChordsBoxAction  { get; set; }
        public static Action<NotesViewModelLiteVersion> RemoveChordsBoxAction { get; set; }
        public static Action<NotesViewModelLiteVersion> RemoveOtherChord{ get; set; }
        public static Action<NotesViewModelLiteVersion> MoveToOtherAction { get; set; }


        [JsonIgnore]
        public Func<List<int>> GetIntervalsFromParent { get; set; }
        [JsonIgnore]
        public Action<List<int>> SetIntervalInParent { get; set; }


        public ObservableCollection<NoteModelLiteVersion> Notes { get; set; } = new ObservableCollection<NoteModelLiteVersion>();
        public ObservableCollection<string> NotesO { get; set; } = new ObservableCollection<string>()
        {
        
        };
        public ObservableCollection<NoteOctaveDetails> NoteOctaves { get; set; } = new ObservableCollection<NoteOctaveDetails>()
        {
        
        };

        //tylko do wyswietlenia na dataGrid
        public ObservableCollection<NoteOctaveDetails> NoteOctavesDataGrid { get; set; } = new ObservableCollection<NoteOctaveDetails>()
        {

        };

        [JsonIgnore]
        public ObservableCollection<ChordBorderModel> ChordBorders { get; set; } = new ObservableCollection<ChordBorderModel>()
        {

        };

        public static string[] AllNotes { get; set; } = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        private NoteDetails[] DefaultNotesNames = new NoteDetails[6]
        {
            new NoteDetails("E", 5),
            new NoteDetails("B", 4),
            new NoteDetails("G", 4),
            new NoteDetails("D", 4),
            new NoteDetails("A", 3),
            new NoteDetails("E", 3)
        };

        public NotesViewModelLiteVersion()
        {
            NoteModelLiteVersion.DefaultBtnBackground = Brushes.Transparent;

            NoteModelLiteVersion.DefaultBtnHover = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBEE6FD")); ;

            HoverFinger = CheckedFinger.firstFinger;
        }

        public NotesViewModelLiteVersion(string chordName, string chordType)
        {
            this.ChordName = chordName;
            this.ChordType = chordType;

            NoteModelLiteVersion.DefaultBtnBackground = Brushes.Transparent;
            NoteModelLiteVersion.DefaultBtnHover = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBEE6FD")); ;

            HoverFinger = CheckedFinger.firstFinger;
        }

        public void InitNotes()
        {
            NotesO = new ObservableCollection<string>()
            {
                "X", "X", "X","X", "X", "X"
            };

            NoteOctaves = new ObservableCollection<NoteOctaveDetails>()
            {
                new NoteOctaveDetails(), new NoteOctaveDetails(),new NoteOctaveDetails(),new NoteOctaveDetails(), new NoteOctaveDetails(),new NoteOctaveDetails()
            };

            for (int i = 0; i < RowSize; i++)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    NoteModelLiteVersion note = new NoteModelLiteVersion(i + 1, j);
                    Notes.Add(note);
                }
            }
        }

        private void AssignNotesAfterChordEdit(NotesViewModelLiteVersion afterConvert)
        {
            for (int i = 0; i < afterConvert.Notes.Count; i++)
            {
                this.Notes[i] = afterConvert.Notes[i];
            }

            for (int i = 0; i < afterConvert.NotesO.Count; i++)
            {
                this.NotesO[i] = afterConvert.NotesO[i];
            }

            for (int i = 0; i < afterConvert.NoteOctaves.Count; i++)
            {
                this.NoteOctaves[i] = afterConvert.NoteOctaves[i];
            }

            this.ChordCode = afterConvert.ChordCode;
            this.ChordCodeNormalized = afterConvert.ChordCodeNormalized;
            this.Fr = afterConvert.Fr;
            this.ChordIntervalsNotes = afterConvert.ChordIntervalsNotes;
            this.ChordIntervalsNumbers = afterConvert.ChordIntervalsNumbers;
        }

        [JsonIgnore]
        public ICommand KolorujChord
        {
            get
            {
                if (kolorujChord == null)
                {
                    kolorujChord = new RelayCommand(param =>
                    {
                        KolorowanieChordWindow kolorowanieChordWindow = new KolorowanieChordWindow(this);
                        kolorowanieChordWindow.Owner = System.Windows.Application.Current.MainWindow;

                        kolorowanieChordWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        kolorowanieChordWindow.Width *= App.CustomScaleX;
                        kolorowanieChordWindow.Height *= App.CustomScaleY;

                        MainWindowBlackAction?.Invoke(true);
                        kolorowanieChordWindow.ShowDialog();
                        MainWindowBlackAction?.Invoke(false);

                        if (kolorowanieChordWindow.ResultDialog == true)
                        {
                            for (int i = 0; i < kolorowanieChordWindow.NotesVM.Notes.Count; i++)
                            {
                                Notes[i].CheckedFinger = kolorowanieChordWindow.NotesVM.Notes[i].CheckedFinger;
                            }

                            var checkedNotesDebug = Notes.Where(a => a.CheckedFinger != CheckedFinger.None).ToList();


                            var ukladFingers = Notes.Select(a => a.CheckedFinger).ToList();
                            RefreshKolorowanieChordsAction?.Invoke(ChordCodeNormalized, ukladFingers);

                            if (!MainWindow.CheckedFingerDict.ContainsKey(ChordCodeNormalized))
                            {
                                MainWindow.CheckedFingerDict.Add(ChordCodeNormalized, ukladFingers);
                            }
                            else
                            {
                                MainWindow.CheckedFingerDict[ChordCodeNormalized] = ukladFingers;
                            }
                        }
                    }
                     , param => true);
                }
                return kolorujChord;
            }
        }

        [JsonIgnore]
        public ICommand UsunOtherChord
        {
            get
            {
                if (usunOtherChord == null)
                {
                    usunOtherChord = new RelayCommand(param =>
                    {
                        RemoveOtherChord?.Invoke(this);
                    }
                     , param => true);
                }
                return usunOtherChord;
            }
        }

        [JsonIgnore]
        public ICommand EditChord
        {
            get
            {
                if (editChord == null)
                {
                    editChord = new RelayCommand(param =>
                    {
                        NotesViewModelLiteVersion clone = (NotesViewModelLiteVersion)this.Clone();

                        var allChordsIntervals = GetAllChordsIntervalsAction();

                        var err = allChordsIntervals.GroupBy(a => a).Where(b => b.Count() > 1).ToList();


                        EditChordWindow editChordWindow = new EditChordWindow(clone, ChordIntervalsNotes, allChordsIntervals);
                        editChordWindow.Owner = System.Windows.Application.Current.MainWindow;

                        editChordWindow.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        editChordWindow.Width *= App.CustomScaleX;
                        editChordWindow.Height *= App.CustomScaleY;

                        MainWindowBlackAction?.Invoke(true);
                        editChordWindow.ShowDialog();
                        if(editChordWindow.DialogResult.Value == true)
                        {
                            var checkedNotesDebug = Notes.Where(a => a.CheckedFinger != CheckedFinger.None).ToList();

                            AssignNotesAfterChordEdit(editChordWindow.SelectedGitarChord);

                            var intervals = editChordWindow.ChordIntervalsDetailsViewModel.NotesGroup.Select(a =>
                            {
                                string intervalText = a.Notes?.First().Interval;

                                if (string.IsNullOrEmpty(intervalText)) return 0;

                                int res = int.Parse(intervalText);
                                return res;
                            }).Where(b => b != 0).ToList();

                            UpdateIntervals(intervals);
                            UpdateChordBorders();
                            UpdateChordImageFile();

                            ChordMode chordMode = AppOptions.Options.ChordReadMode ? ChordMode.Read : ChordMode.Normal;
                            UpdateChordImage(chordMode);

                            var intervalsFromParent = GetIntervalsFromParent?.Invoke();

                            if(intervalsFromParent == null || !intervalsFromParent.Any())
                            {
                                SetIntervalInParent?.Invoke(ChordIntervalsNumbers);
                                UpdateIntervalsAction?.Invoke(ChordType, ChordIntervalsNumbers);
                            }

                            SetFocusChord?.Invoke(this);
                        }

                        MainWindowBlackAction?.Invoke(false);
                    }
                     , param => true);
                }
                return editChord;
            }
        }

        [JsonIgnore]
        public ICommand GrajCommand
        {
            get
            {
                if (grajCommand == null)
                {
                    grajCommand = new RelayCommand(param =>
                    {
                        ScrollDataGridToItemAction(this);
                        //SetHeightForAllChords(ChordHeight - 10);

                    }
                     , param => true);
                }
                return grajCommand;
            }
        }

        [JsonIgnore]
        public ICommand IsSelectChordCommand
        {
            get
            {
                if (isSelectChordCommand == null)
                {
                    isSelectChordCommand = new RelayCommand(param =>
                    {
                        DeselectPrevChord();
                        SetFocusChord?.Invoke(this);
                        IsSelected = true;
                        FocusedChord = true;
                    }
                     , param => true);
                }
                return isSelectChordCommand;
            }
        }

        [JsonIgnore]
        public ICommand IsSelectCommand
        {
            get
            {
                if (isSelectCommand == null)
                {
                    isSelectCommand = new RelayCommand(param =>
                    {
                        DeselectAllOtherChords();
                        SetFocusOtherChord?.Invoke(this);
                        IsSelected = true;
                        FocusedChord = true;
                    }
                     , param => true);
                }
                return isSelectCommand;
            }
        }

        [JsonIgnore]
        public ICommand PlayDownCommand
        {
            get
            {
                if (playDownCommand == null)
                {
                    playDownCommand = new RelayCommand(param =>
                    {
                        PlayDownAction?.Invoke(this);
                    }
                     , param => true);
                }
                return playDownCommand;
            }
        }

        [JsonIgnore]
        public ICommand PlayDownOtherCommand
        {
            get
            {
                if (playDownOtherCommand == null)
                {
                    playDownOtherCommand = new RelayCommand(param =>
                    {
                        DeselectAllOtherChords();
                        SetFocusOtherChord?.Invoke(this);
                        IsSelected = true;
                        FocusedChord = true;

                        PlayDownAction?.Invoke(this);
                    }
                     , param => true);
                }
                return playDownOtherCommand;
            }
        }

        [JsonIgnore]
        public ICommand MoveToChordBox
        {
            get
            {
                if (moveToChordBox == null)
                {
                    moveToChordBox = new RelayCommand(param =>
                    {
                        AddChordToChordsBoxAction?.Invoke(this);
                    }
                     , param => true);
                }
                return moveToChordBox;
            }
        }

        [JsonIgnore]
        public ICommand RemoveFromChordBox
        {
            get
            {
                if (removeFromChordBox == null)
                {
                    removeFromChordBox = new RelayCommand(param =>
                    {
                        RemoveChordsBoxAction?.Invoke(this);
                    }
                     , param => true);
                }
                return removeFromChordBox;
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

                foreach (var item in Notes)
                {
                    item.HoverFinger = value;
                }
            }
        }

        [JsonIgnore]
        public ICommand MoveToOther
        {
            get
            {
                if (moveToOther == null)
                {
                    moveToOther = new RelayCommand(param =>
                    {
                        MoveToOtherAction?.Invoke(this);
                    }
                     , param => true);
                }
                return moveToOther;
            }
        }

        public int Fr
        {
            get
            {
                return fr;
            }

            set
            {
                fr = value;
                FrText = fr > 0 ? $"Fr: {fr}" : "";
                OnPropertyChanged("Fr");
            }
        }

        public string FrText
        {
            get
            {
                return frText;
            }

            set
            {
                frText = value;
                OnPropertyChanged("FrText");
            }
        }

        [JsonIgnore]
        public bool FocusedChord
        {
            get => focusedChord;

            set
            {
                focusedChord = value;
                OnPropertyChanged("FocusedChord");
            }
        }

        [JsonIgnore]
        public bool IsSelected
        {
            get => isSelected;

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public string ChordName
        {
            get => chordName;

            set
            {
                chordName = value;
                OnPropertyChanged("ChordFullName");
            }
        }

        public string ChordType
        {
            get => chordType;

            set
            {
                chordType = value;
                OnPropertyChanged("ChordFullName");
            }
        }

        public string ChordFullName
        {
            get
            {
                return $"{ChordName} {ChordType}";
            }
        }

        public string ChordId
        {
            get => chordId;

            set
            {
                chordId = value;
                OnPropertyChanged("ChordId");
            }
        }
        public List<int> ChordIntervalsNumbers { get; set; }
        public List<string> ChordIntervalsNotes{ get; set; }

        [JsonIgnore]
        public LinearGradientBrush ChordReadModeBackground
        {
            get => chordReadModeBackground;
            set
            {
                chordReadModeBackground = value;
                OnPropertyChanged("ChordReadModeBackground");
            }
        }

        [JsonIgnore]
        public double ChordOpacity
        {
            get => chordOpacity;
            set
            {
                chordOpacity = value;
                OnPropertyChanged("ChordOpacity");
            }
        }

        [JsonIgnore]
        public double ChordHeight
        {
            get => chordHeight;
            set
            {
                chordHeight = value;
                OnPropertyChanged("ChordHeight");
            }
        }

        [JsonIgnore]
        public double ChordWidth
        {
            get => chordWidth;
            set
            {
                chordWidth = value;
                OnPropertyChanged("ChordWidth");
            }
        }

        [JsonIgnore]
        public Thickness ChordMargin
        {
            get => chordMargin;
            set
            {
                chordMargin = value;
                OnPropertyChanged("ChordMargin");
            }
        }

        [JsonIgnore]
        public BitmapImage ChordImage
        {
            get => chordImage;
            set
            {
                chordImage = value;
                OnPropertyChanged("ChordImage");
            }
        }

        public void UpdateAllStrunsNames()
        {
            for (int i = 1; i < 7; i++)
            {
                UpdateStrunaNoteName(i, Fr);
            }
        }

        private void UpdateStrunaNoteName(int struna, int fr)
        {
            int? progIdx = GetCheckedNoteOnProg(struna);
            int realProg = 0;
            NoteDetails newNote = null;

            if (progIdx == null)
            {
                newNote = new NoteDetails("", 0);
            }
            else
            {
                int offset = progIdx.Value + fr;
                realProg = offset+1;

                if (fr == 0)
                {
                    offset++;
                }

                var defaultNoteOnStruna = DefaultNotesNames[struna - 1];

                newNote = GetOffsetNoteDetails(defaultNoteOnStruna.Name, defaultNoteOnStruna.Octave, offset);
            }

            NoteOctaves[struna - 1].Name = newNote.Name;
            NoteOctaves[struna - 1].Octave = newNote.Octave != 0 ? newNote.Octave.ToString() : "";
            if(!string.IsNullOrEmpty(newNote.Name))
            {
                NoteOctaves[struna - 1].NoteColor = NotesHelper.ChordColorNoOpacity[newNote.Name];
            }

            NoteOctaves[struna - 1].Struna = struna;
            NoteOctaves[struna - 1].Prog = realProg;

            var newNoteDetails = NoteOctaves[struna - 1]; //Debug
        }

        public List<string> GetCheckedNotesO()
        {
            List<string> checkedNotesO = new List<string>();

            for (int i = 0; i < NotesO.Count; i++)
            {
                if(NotesO[i] == "O")
                {
                    string note = $"s{i+1}p{0}";
                    checkedNotesO.Add(note);
                }
            }

            return checkedNotesO;
        }

        public int? GetCheckedNoteOnProg(int struna)
        {
            var sameStruna = Notes.Where(b => b.Struna == struna).ToList();

            int? progIdx = null;
            for (int i = sameStruna.Count - 1; i >= 0; i--)
            {
                if (sameStruna[i].CheckedFinger != CheckedFinger.None)
                {
                    progIdx = sameStruna[i].Prog;
                    break;
                }
            }

            if(progIdx == null && NotesO[struna-1] == "O")
            {
                progIdx = -1;
            }

            return progIdx;
        }

        private NoteDetails GetOffsetNoteDetails(string name, int octave, int offset)
        {
            int normalizedOffset = offset; //moze byc wiekszy niz 12, wtedy robimy modulo 12
            int startNoteIdx = Array.IndexOf(AllNotes, name);
            int resultIdx = -1;
            int resultOctave = octave;

            if (offset >= 12)
            {
                normalizedOffset = offset % 12;
                int octaveOffset = offset / 12;
                resultOctave += octaveOffset;
            }

            if (startNoteIdx + normalizedOffset >= AllNotes.Length)
            {
                int firstPart = AllNotes.Length - startNoteIdx;
                resultIdx = normalizedOffset - firstPart;
                resultOctave++;
            }
            else
            {
                resultIdx = startNoteIdx + normalizedOffset;
            }

            string resultName = AllNotes[resultIdx];
            NoteDetails resultNote = new NoteDetails(resultName, resultOctave);

            return resultNote;
        }
        
        public void ClearButtons(int fingerIdx)
        {
            foreach (var item in Notes)
            {
                item.CheckedFinger = CheckedFinger.None;
                item.HoverFinger = (CheckedFinger)fingerIdx + 1; ;
            }
        }
        
        public void UpdateIntervals(List<int> intervals)
        {
            string rootNote = ChordName;

            ChordIntervalsNotes = ChordIntervalHelper.GetNotesFromIntervals(rootNote, intervals);
            ChordIntervalsNumbers = ChordIntervalHelper.GetIntervalsFromNotes(rootNote, ChordIntervalsNotes);
            ChordCode = string.Join("-", ChordIntervalsNotes.OrderBy(b => b).Distinct().ToList());
        }

        public void UpdateIntervalsFromCurrent()
        {
            string rootNote = ChordName;

            ChordIntervalsNotes = ChordIntervalHelper.GetNotesFromIntervals(rootNote, ChordIntervalsNumbers);
            ChordIntervalsNumbers = ChordIntervalHelper.GetIntervalsFromNotes(rootNote, ChordIntervalsNotes);
            ChordCode = string.Join("-", ChordIntervalsNotes.OrderBy(b => b).Distinct().ToList());
        }

        public void UpdateChordImageFile(string path = null)
        {
            string normalizeChordFullname = ChordFullName.Replace('/', ';');

            string fullPath = string.IsNullOrEmpty(path) ? Path.Combine(App.ChordImagesWorkingPath, normalizeChordFullname + ".png") : Path.Combine(path, normalizeChordFullname + ".png");
            string fullReadChordPath = string.IsNullOrEmpty(path) ? Path.Combine(App.ReadChordImagesWorkingPath, normalizeChordFullname + ".png") : Path.Combine(path, normalizeChordFullname + ".png");

            RenderChordAction?.Invoke(this, fullPath, fullReadChordPath);
        }

        private void SetChordMode(ChordMode mode)
        {
            switch (mode)
            {
                case ChordMode.Read:
                    ChordWidth = MainWindow.ReadChordWidth;
                    ChordHeight = MainWindow.ReadChordHeight;
                    ChordMargin = new Thickness(0);
                    break;
                case ChordMode.Normal:
                    ChordWidth = MainWindow.NormalChordWidth;
                    ChordHeight = MainWindow.NormalChordHeight;
                    ChordMargin = new Thickness(10, 5, 5, 5);
                    break;
                default:
                    break;
            }
        }

        public BitmapImage UpdateChordImage(ChordMode mode)
        {
                SetChordMode(mode);

                if (string.IsNullOrEmpty(ChordFullName)) return null;

                string chordImagesDirectory = (AppOptions.Options != null && AppOptions.Options.ChordReadMode) ? App.ReadChordImagesWorkingPath : App.ChordImagesWorkingPath;
                string chordNameNormalized = $"{ChordFullName.Replace('/', ';')}.png";

                string fullChordImagePath = Path.Combine(chordImagesDirectory, chordNameNormalized);

                if (File.Exists(fullChordImagePath))
                {
                    BitmapImage bitmapImage = null;
                    //Application.Current.Dispatcher.Invoke(new Action(() =>
                    //{

                    bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(fullChordImagePath);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    //}));
                    ChordImage = bitmapImage;
                    return bitmapImage;
                }

                return null;
        }

        public void UpdateChordImage()
        {
            if (string.IsNullOrEmpty(ChordFullName)) return;

            string chordImagesDirectory = AppOptions.Options.ChordReadMode ? App.ReadChordImagesWorkingPath : App.ChordImagesWorkingPath;
            string chordNameNormalized = $"{ChordFullName.Replace('/', ';')}.png";

            string fullChordImagePath = Path.Combine(chordImagesDirectory, chordNameNormalized);

            if (File.Exists(fullChordImagePath))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fullChordImagePath);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmapImage.EndInit();
                ChordImage = bitmapImage;
            }
        }

        //?
        public void UpdateChordBorders()
        {
            if (ChordIntervalsNotes == null || !ChordIntervalsNotes.Any() || ChordIntervalsNumbers == null || !ChordIntervalsNumbers.Any()) return;

            List<int> chordBorderHeigths = new List<int>();
            int chordHeightStep = 5;

            foreach (var item in ChordIntervalsNotes)
            {
                chordBorderHeigths.Add(0);
            }

            foreach (var noteOctav in NoteOctaves)
            {
                string noteName = noteOctav.Name;

                if (string.IsNullOrEmpty(noteName)) continue;

                int idx = ChordIntervalsNotes.IndexOf(noteName);
                chordBorderHeigths[idx] += chordHeightStep;

                if (chordBorderHeigths[idx] > 15) chordBorderHeigths[idx] = 15;
            }

            if (ChordBorders == null) ChordBorders = new ObservableCollection<ChordBorderModel>();
            else ChordBorders.Clear();

            for (int i = 0; i < ChordIntervalsNotes.Count; i++)
            {
                ChordBorderModel model = new ChordBorderModel(NotesHelper.ChordColorNoOpacity[ChordIntervalsNotes[i]], chordBorderHeigths[i]);
                ChordBorders.Add(model);
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

        public object Clone()
        {
            NotesViewModelLiteVersion clone = new NotesViewModelLiteVersion(this.ChordName, this.ChordType);
            clone.InitNotes();

            for (int i = 0; i < this.Notes.Count; i++)
            {
                clone.Notes[i] = (NoteModelLiteVersion)this.Notes[i].Clone();
            }

            for (int i = 0; i < this.NotesO.Count; i++)
            {
                clone.NotesO[i] = this.NotesO[i];
            }

            for (int i = 0; i < this.NoteOctaves.Count; i++)
            {
                clone.NoteOctaves[i] = (NoteOctaveDetails)this.NoteOctaves[i].Clone();
            }

            clone.Fr = this.Fr;
            clone.IsSelected = this.IsSelected;
            clone.ChordCode = this.ChordCode;
            clone.ChordCodeNormalized = this.ChordCodeNormalized;
            clone.ChordId = this.ChordId;

            clone.ChordIntervalsNotes = new List<string>();
            if (this.ChordIntervalsNotes == null) this.ChordIntervalsNotes = new List<string>();
            for (int i = 0; i < this.ChordIntervalsNotes.Count; i++)
            {
                clone.ChordIntervalsNotes.Add(this.ChordIntervalsNotes[i]);
            }

            clone.ChordIntervalsNumbers = new List<int>();
            if (this.ChordIntervalsNumbers == null) this.ChordIntervalsNumbers = new List<int>();
            for (int i = 0; i < this.ChordIntervalsNumbers.Count; i++)
            {
                clone.ChordIntervalsNumbers.Add(this.ChordIntervalsNumbers[i]);
            }

            clone.FocusedChord = this.FocusedChord;
            clone.HoverFinger = this.HoverFinger;

            return clone;
        }
    }
}
