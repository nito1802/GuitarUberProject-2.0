using EditChordsWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject.Models
{
    public class EditStrumNoteDetailsModel : INotifyPropertyChanged
    {
        private Brush myBackground;
        private Brush hoverBackground;
        private CheckedFinger checkedNote;
        private RelayCommand clickStruna;
        private RelayCommand clickStrunaStop;

        public static Action<EditStrumNoteDetailsModel, CheckedFinger> SetStruna { get; set; }

        public EditStrumNoteDetailsModel()
        {
            MyBackground = NotesHelper.EditStrumBrushes[CheckedFinger.None];
            HoverBackground = Brushes.Aqua;
        }

        public int MyIdx { get; set; }

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

        public CheckedFinger CheckedNote
        {
            get
            {
                return checkedNote;
            }

            set
            {
                checkedNote = value;
                MyBackground = NotesHelper.EditStrumBrushes[value];
            }
        }

        public ICommand ClickStruna
        {
            get
            {
                if (clickStruna == null)
                {
                    clickStruna = new RelayCommand(param =>
                    {
                        SetStruna?.Invoke(this, CheckedFinger.firstFinger);
                    }
                     , param => true);
                }
                return clickStruna;
            }
        }

        public ICommand ClickStrunaStop
        {
            get
            {
                if (clickStrunaStop == null)
                {
                    clickStrunaStop = new RelayCommand(param =>
                    {
                        SetStruna?.Invoke(this, CheckedFinger.secondFinger);
                    }
                     , param => true);
                }
                return clickStrunaStop;
            }
        }

        public override string ToString()
        {
            return $"{CheckedNote}";
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
