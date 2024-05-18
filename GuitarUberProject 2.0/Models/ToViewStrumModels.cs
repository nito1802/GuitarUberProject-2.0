using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitarUberProject.Models
{
    public class ToViewStrumModels : INotifyPropertyChanged, ICloneable
    {
        private long delayBeforeMs;
        private long delayBetweenStrunaMs;

        public List<ToViewEditStrumNoteDetailsModel> Notes { get; set; } = new List<ToViewEditStrumNoteDetailsModel>();
        [JsonIgnore]
        public static Action<EditStrumModel> InsertSingleStrumAction { get; set; }
        [JsonIgnore]
        public static Action<EditStrumModel> RemoveSingleStrumAction { get; set; }

        public ToViewStrumModels()
        {
            
        }

        public long DelayBeforeMs
        {
            get => delayBeforeMs;
            set
            {
                delayBeforeMs = value;
                OnPropertyChanged("DelayBeforeMs");
            }
        }
        public long DelayBetweenStrunaMs
        {
            get => delayBetweenStrunaMs;
            set
            {
                delayBetweenStrunaMs = value;
                OnPropertyChanged("DelayBetweenStrunaMs");
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
            var clone = new ToViewStrumModels();
            clone.DelayBeforeMs = this.DelayBeforeMs;
            clone.DelayBetweenStrunaMs = this.DelayBetweenStrunaMs;
            clone.Notes = this.Notes.Select(a => a.Clone()).Cast<ToViewEditStrumNoteDetailsModel>().ToList();

            return clone;
        }
    }
}
