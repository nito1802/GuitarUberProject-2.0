using EditChordsWindow;
using GitarUberProject.EditChord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GitarUberProject.Models
{
    public class EditStrumModel : INotifyPropertyChanged
    {
        private RelayCommand insertSingleStrum;
        private RelayCommand removeSingleStrum;

        private long delayBeforeMs;
        private long delayBetweenStrunaMs;

        public List<EditStrumNoteDetailsModel> Notes { get; set; } = new List<EditStrumNoteDetailsModel>();
        public static Action<EditStrumModel> InsertSingleStrumAction { get; set; }
        public static Action<EditStrumModel> RemoveSingleStrumAction { get; set; }

        public EditStrumModel()
        {
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 0});
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 1});
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 2});
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 3});
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 4});
            Notes.Add(new EditStrumNoteDetailsModel() { MyIdx = 5});
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
        public ICommand InsertSingleStrum
        {
            get
            {
                if (insertSingleStrum == null)
                {
                    insertSingleStrum = new RelayCommand(param =>
                    {
                        InsertSingleStrumAction?.Invoke(this);
                    }
                     , param => true);
                }
                return insertSingleStrum;
            }
        }

        public ICommand RemoveSingleStrum
        {
            get
            {
                if (removeSingleStrum == null)
                {
                    removeSingleStrum = new RelayCommand(param =>
                    {
                        RemoveSingleStrumAction?.Invoke(this);
                    }
                     , param => true);
                }
                return removeSingleStrum;
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
