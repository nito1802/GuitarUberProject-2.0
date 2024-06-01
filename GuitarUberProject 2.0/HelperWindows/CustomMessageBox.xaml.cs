using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GitarUberProject.HelperWindows
{
    public class CustomMessageBoxValidation
    {
        public Func<string, bool> ErrorCondition { get; set; }
        public string ErrorText { get; set; }
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
    /// Interaction logic for CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window, INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public List<CustomMessageBoxValidation> ErrorsValidationList { get; set; }
        private string textPrefix;
        private string text;
        private string errorMessage;

        //Jesteś pewien, że chcesz wyłączyć komputer?
        public CustomMessageBox(string _TextPrefix, string _Text, List<CustomMessageBoxValidation> errorsValidationList)
        {
            InitializeComponent();

            this.ErrorsValidationList = errorsValidationList;
            this.TextPrefix = _TextPrefix;
            this.Text = _Text;
            this.DataContext = this;

            tbText.Focus();
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

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
                OnPropertyChanged("Text");

                ErrorMessage = "";
                btnYes.Opacity = 1;
                btnYes.IsEnabled = true;
                ValidateErrors();
            }
        }

        public string TextPrefix
        {
            get
            {
                return textPrefix;
            }

            set
            {
                textPrefix = value;
                OnPropertyChanged("TextPrefix");
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public void ValidateErrors()
        {
            foreach (var item in ErrorsValidationList)
            {
                if (item.ErrorCondition(Text))
                {
                    ErrorMessage = item.ErrorText;
                    btnYes.Opacity = 0.5;
                    btnYes.IsEnabled = false;
                    break;
                }
            }
        }

        public void Apply()
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) return;

            DialogResult = true;
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

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Apply();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Apply();
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
            tbText.SelectAll();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}