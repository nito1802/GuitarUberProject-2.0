using EditChordsWindow;
using GitarUberProject;
using GitarUberProject.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;

namespace GuitarUberProject_2._0.ViewModels
{
    public class MyChordSuper
    {
        public string ChordName { get; set; }
        public string NotesContent { get; set; }

        public override string ToString()
        {
            return $"{ChordName}\t{NotesContent}";
        }
    }

    public class ScaleNoteModel
    {
        private string note;

        public ScaleNoteModel(string note)
        {
            Note = note;
        }

        public string Note
        {
            get { return note; }
            set
            {
                note = value;
                ForegroundNoteBrush = NotesHelper.ChordColorNoOpacity[Note];
            }
        }

        public Brush ForegroundNoteBrush { get; set; }
    }

    public class ScalesInChordsViewModel
    {
        private string scaleName;

        public ObservableCollection<ScaleNoteModel> Scales { get; set; } = new ObservableCollection<ScaleNoteModel>();

        public void SetScalesForChord(ScaleNotesViewModel scaleNotesViewModel, List<NoteOctaveIntervalDetails> notes)
        {
            Scales.Clear();
            var dinstictNotes = notes.Select(a => a.Note).Distinct().ToList();
            scaleNotesViewModel.GetAllScaleNotes();
            var allScales = scaleNotesViewModel.GetMyDict();

            var durowa = allScales.Where(a => a.Key.Contains("durowa")/* || a.Key.Contains("molowa")*/).ToList();
            var scalesContainingChordNotes = durowa
                .Where(a => a.Value.Select(b => b.Note).Intersect(dinstictNotes).Count() == dinstictNotes.Count)
                .Select(a => a.Key.Replace("Jońska (durowa)  ", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty))
                .ToList();
            scalesContainingChordNotes.ForEach(a => Scales.Add(new(a)));

            //List<MyChordSuper> myScaleSupers = new List<MyChordSuper>();
            //foreach (var scaleNote in durowa)
            //{
            //    myScaleSupers.Add(new MyChordSuper
            //    {
            //        ChordName = scaleNote.Key,
            //        NotesContent = string.Join(" ", scaleNote.Value.Select(a => a.Note))
            //    });
            //}
            //var myCont = string.Join(Environment.NewLine, myScaleSupers);

            //var notesInScale = durowa.Single(a => a.Key.Contains("(B)")).Value.Select(a => a.Note).ToList();

            //StringBuilder globalSb = new StringBuilder();
            //foreach (var item in chords)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(item.Type + '\t');
            //    AddNoteFromChord(notesInScale, item.ChordC, sb);
            //    AddNoteFromChord(notesInScale, item.ChordCSharp, sb);
            //    AddNoteFromChord(notesInScale, item.ChordD, sb);
            //    AddNoteFromChord(notesInScale, item.ChordDSharp, sb);

            //    AddNoteFromChord(notesInScale, item.ChordE, sb);
            //    AddNoteFromChord(notesInScale, item.ChordF, sb);
            //    AddNoteFromChord(notesInScale, item.ChordFSharp, sb);
            //    AddNoteFromChord(notesInScale, item.ChordG, sb);
            //    AddNoteFromChord(notesInScale, item.ChordGSharp, sb);
            //    AddNoteFromChord(notesInScale, item.ChordA, sb);
            //    AddNoteFromChord(notesInScale, item.ChordASharp, sb);
            //    AddNoteFromChord(notesInScale, item.ChordB, sb);

            //    globalSb.AppendLine(sb.ToString());
            //}

            //var myText = globalSb.ToString();
            //chords.ForEach(a =>
            //{
            //    var notesInChord2 = a.Notes.Split(' ').ToList();
            //    var intersect = notesInChord.Intersect(notesInChord2).ToList();
            //    if (intersect.Count == notesInChord.Count)
            //    {
            //        var scaleName = durowa.Single(b => b.Value

            //scalesContainingChordNotes.ForEach(a => Scales.Add(new(a)));
            //scalesContainingChordNotes.ForEach(a => Scales.Add(new(a)));
            //StringBuilder globalSb = new StringBuilder();
            //var fitChords = new List<string>();
        }

        private static void AddNoteFromChord(List<string> notesInScale, NotesViewModelLiteVersion item, StringBuilder sb)
        {
            var notesInChordx = item.NoteOctaves.Select(a => a.Name).Where(b => !string.IsNullOrEmpty(b)).ToList();
            if (notesInChordx.Distinct().Count() == notesInScale.Intersect(notesInChordx).Count())
            {
                sb.Append(item.ChordName + ", ");
            }
        }

        //public override string ToString()
        //{
        //    //return $"Name: {RootNoteName}{ChordType} Groups: {NotesGroup.Count} ChordIntervalsDetails";
        //}

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