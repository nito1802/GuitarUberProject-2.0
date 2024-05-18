using EditChordsWindow;
using Newtonsoft.Json;
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
    public class ToViewEditStrumNoteDetailsModel : INotifyPropertyChanged, ICloneable
    {
        private Brush myBackground;
        private CheckedFinger checkedNote;

        public ToViewEditStrumNoteDetailsModel()
        {
            MyBackground = NotesHelper.EditStrumBrushes[CheckedFinger.None];
        }

        public int MyIdx { get; set; }
        [JsonIgnore]
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

        public object Clone()
        {
            var clone = new ToViewEditStrumNoteDetailsModel();
            clone.CheckedNote = this.CheckedNote;
            clone.MyBackground = NotesHelper.EditStrumBrushes[clone.CheckedNote];
            clone.MyIdx = this.MyIdx;

            return clone;
        }
    }
}
