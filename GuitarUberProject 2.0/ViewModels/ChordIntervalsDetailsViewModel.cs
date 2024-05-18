using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GitarUberProject.ViewModels
{
    public class ChordIntervalsDetailsViewModel
    {
        private string rootNoteName;
        private string chordType;

        public ObservableCollection<NotesGroup> NotesGroup { get; set; } = new ObservableCollection<NotesGroup>();
        public string RootNoteName
        {
            get => rootNoteName;
            set
            {
                rootNoteName = value;
                OnPropertyChanged("RootNoteName");
            } 
        }

        public string ChordType
        {
            get => chordType;
            set
            {
                chordType = value;
                OnPropertyChanged("ChordType");
            }
        }

        public override string ToString()
        {
            return $"Name: {RootNoteName}{ChordType} Groups: {NotesGroup.Count} ChordIntervalsDetails";
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


    

    public class NotesGroup
    {
        public NotesGroup(List<NoteOctaveIntervalDetails> notes)
        {
            Notes = notes;
        }

        public List<NoteOctaveIntervalDetails> Notes { get; set; }

        public override string ToString()
        {
            return string.Join(", ", Notes.Select(a => $"{a.Note}{a.Octave}({a.Interval})"));
        }
    }

    public class NoteOctaveIntervalDetails
    {
        public string Note { get; set; }
        public int Octave { get; set; }
        public Brush ForegroundNoteBrush { get; set; }
        public int FontSizeNote { get; set; }
        public string Interval { get; set; }


        public NoteOctaveIntervalDetails(string note, int octave)
        {
            Note = note;
            Octave = octave;

            ForegroundNoteBrush = NotesHelper.ChordColorNoOpacity[Note];
            FontSizeNote = NotesHelper.OctaveToFontSizeDict[Octave];
            //FontSizeNote = 25;
        }

        public string FullName
        {
            get
            {
                return $"{Note}{Octave}";
            }
        }

        public override string ToString()
        {
            return $"{Note}{Octave}({Interval})";
        }
    }
}
