using GitarUberProject;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace EditChordsWindow
{
    public class NoteDetails
    {
        public string Name { get; set; }
        public int Octave { get; set; }

        public NoteDetails(string name, int octave)
        {
            Name = name;
            Octave = octave;
        }

        public override string ToString()
        {
            return $"{Name}{Octave} EditChordsWindow.NoteDetails";
        }
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19

        // ...
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class KolorowanieChordWindow : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public static string[] AllNotes { get; set; } = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public NoteDetails[] DefaultNotesNames { get; set; }
        public NoteDetails[] NotesNames { get; set; }

        private Brush myBackground;
        private Brush hoverBackground;
        private string chordName;
        private int fingerIdx;

        public NotesViewModel NotesVM { get; set; } = new NotesViewModel();
        public LinearGradientBrush[] FingerBrushes { get; set; }
        public string[] FingerRbNames { get; set; }
        public bool ResultDialog { get; set; }

        public KolorowanieChordWindow(NotesViewModelLiteVersion notesViewModelLiteVersion)
        {
            InitDicts();

            int counter = 0;
            foreach (var item in notesViewModelLiteVersion.Notes)
            {
                NoteModel noteModel = new NoteModel(item.Struna, item.Prog);
                noteModel.CheckedFinger = item.CheckedFinger;

                if (noteModel.CheckedFinger == CheckedFinger.None) noteModel.IsEnabled = false;
                NotesVM.Notes[counter++] = noteModel;
            }

            ChordName = notesViewModelLiteVersion.ChordFullName;

            InitializeComponent();
            this.DataContext = this;

            itemsControlNotes.DataContext = NotesVM;

            for (int i = 0; i < notesViewModelLiteVersion.NoteOctaves.Count; i++)
            {
                var currentNoteOctave = notesViewModelLiteVersion.NoteOctaves[i];
                int.TryParse(currentNoteOctave.Octave, out var octave);
                UpdateNoteName(i + 1, currentNoteOctave.Name, octave);
            }

            TextBlock[] NotesO_Textboxes = new TextBlock[]
            {
                tbNotesO_1, tbNotesO_2, tbNotesO_3, tbNotesO_4, tbNotesO_5, tbNotesO_6,
            };

            int counterO = 0;
            foreach (var item in NotesO_Textboxes)
            {
                item.Text = notesViewModelLiteVersion.NotesO[counterO];
                counterO++;
            }

            tbFr.Text = notesViewModelLiteVersion.FrText;
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

        private void SetWindowCompositionAttribute(object handle, ref WindowCompositionAttributeData data)
        {
            throw new NotImplementedException();
        }

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

        public string ChordName
        {
            get
            {
                return chordName;
            }

            set
            {
                chordName = value;
                OnPropertyChanged("ChordName");
            }
        }

        public int FingerIdx
        {
            get => fingerIdx;
            set
            {
                fingerIdx = value;
                SetFingerIdx(FingerIdx);
            }
        }

        private void InitDicts()
        {
            FingerBrushes = new LinearGradientBrush[]
            {
                new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#E29A05"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFEBA619"), 1) }), new Point(0.5, 0), new Point(0.5, 1)),
                new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#B71DE9"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFA50CD6"), 1) }), new Point(0.5, 0), new Point(0.5, 1)),
                new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#00A0E8"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF0784BD"), 1) }), new Point(0.5, 0), new Point(0.5, 1)),
                new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#E16449"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FFDB5639"), 1) }), new Point(0.5, 0), new Point(0.5, 1)),
                new LinearGradientBrush(new GradientStopCollection(new List<GradientStop>() { new GradientStop((Color)ColorConverter.ConvertFromString("#FF036C50"), 0),  new GradientStop((Color)ColorConverter.ConvertFromString("#FF07946E"), 1) }), new Point(0.5, 0), new Point(0.5, 1))
            };

            FingerRbNames = new string[]
            {
                "rbFirstFinger",
                "rbSecondFinger",
                "rbThirdFinger",
                "rbFourthFinger",
                "rbOtherFinger"
            };

            DefaultNotesNames = new NoteDetails[6]
            {
                new NoteDetails("E", 5),
                new NoteDetails("B", 4),
                new NoteDetails("G", 4),
                new NoteDetails("D", 4),
                new NoteDetails("A", 3),
                new NoteDetails("E", 3)
            };

            NotesNames = new NoteDetails[6]
            {
                new NoteDetails("E", 5),
                new NoteDetails("B", 4),
                new NoteDetails("G", 4),
                new NoteDetails("D", 4),
                new NoteDetails("A", 3),
                new NoteDetails("E", 3)
            };
        }

        private void UpdateNoteName(int struna, string name, int octave)
        {
            switch (struna)
            {
                case 1:
                    tb1Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb1Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }

                    tb1Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                case 2:
                    tb2Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb2Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }
                    tb2Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                case 3:
                    tb3Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb3Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }
                    tb3Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                case 4:
                    tb4Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb4Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }
                    tb4Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                case 5:
                    tb5Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb5Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }
                    tb5Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                case 6:
                    tb6Note.Text = name;
                    if (!string.IsNullOrEmpty(name))
                    {
                        tb6Note.Foreground = NotesHelper.ChordColorNoOpacity[name];
                    }
                    tb6Octave.Text = octave != 0 ? octave.ToString() : "";
                    break;

                default:
                    throw new Exception("Nie ma takiej struny");
            }

            NotesNames[struna - 1].Name = name;
            NotesNames[struna - 1].Octave = octave;
        }

        private void rbFinger_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;

            if (rb == null) return;

            FingerIdx = Array.IndexOf(FingerRbNames, rb.Name);
        }

        private void SetFingerIdx(int idx)
        {
            MyBackground = FingerBrushes[idx];

            HoverBackground = FingerBrushes[idx];

            NotesVM.HoverFinger = (CheckedFinger)idx + 1;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            NotesVM.ClearButtons(FingerIdx);
        }

        private void HeaderNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int progToSet = int.Parse(button.Tag.ToString());

            var notesOnProg = NotesVM.Notes.Where(a => a.Prog == progToSet && a.IsEnabled == true).ToList();
            notesOnProg.ForEach(a => a.CheckedFinger = (CheckedFinger)FingerIdx + 1);
        }

        private void HeaderNoteButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;

            int progToSet = int.Parse(button.Tag.ToString());

            var notesOnProg = NotesVM.Notes.Where(a => a.Prog == progToSet && a.IsEnabled == true).ToList();
            notesOnProg.ForEach(a => a.CheckedFinger = CheckedFinger.None);
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            ResultDialog = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResultDialog = false;
            Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            ResultDialog = false;
            Close();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.D1)
            {
                rbFirstFinger.IsChecked = true;
            }
            else if (e.Key == Key.D2)
            {
                rbSecondFinger.IsChecked = true;
            }
            else if (e.Key == Key.D3)
            {
                rbThirdFinger.IsChecked = true;
            }
            else if (e.Key == Key.D4)
            {
                rbFourthFinger.IsChecked = true;
            }
            else if (e.Key == Key.D5)
            {
                rbOtherFinger.IsChecked = true;
            }
            else if (e.Key == Key.Enter && btnApply.IsEnabled)
            {
                ResultDialog = true;
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

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool changed = false;

            if (e.Delta <= 0 && FingerIdx < FingerRbNames.Length - 1)
            {
                FingerIdx++;
                changed = true;
            }
            else if (FingerIdx > 0)
            {
                FingerIdx--;
                changed = true;
            }

            if (changed)
            {
                var rb = (RadioButton)this.FindName(FingerRbNames[FingerIdx]);
                rb.IsChecked = true;
            }
        }
    }
}