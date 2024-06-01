using GitarUberProject.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GitarUberProject.ViewModels
{
    public class ScaleNotesViewModel : INotifyPropertyChanged
    {
        private Dictionary<string, List<int>> ScalesDict { get; set; }
        private Dictionary<string, List<ScaleNoteModel>> CachedScaleValue = new Dictionary<string, List<ScaleNoteModel>>();

        public ScaleNotesViewModel()
        {
            ScalesDict = new Dictionary<string, List<int>>
            {
                {"Jońska (durowa)", new List<int> {2, 2, 1, 2, 2, 2, 1 } },
                { "Eolska (molowa)", new List<int> {2, 1, 2, 2, 1, 2, 2 } },
                {"Dorycka (sad)", new List<int> {2, 1, 2, 2, 2, 1, 2 } },
                {"Frygijska (sad)", new List<int> {1, 2, 2, 2, 1, 2, 2 } },
                {"Lidyjska (happy)", new List<int> {2, 2, 2, 1, 2, 2, 1 } },
                {"Miksolidyjska (happy)", new List<int> {2, 2, 1, 2, 2, 1, 2 } },
                {"Lokrycka (sad)", new List<int> {1, 2, 2, 1, 2, 2, 2 } },
            };

            ScalesNames = ScalesDict.Keys.ToList();

            PopularChordProgression = new List<string>
            {
                "I-III-IV", "VI-II", "IV-II-III", "IV-V-III-I", "IV-V-III-II"
            };

            ScaleName = ScalesNames.First();
            SelectedNote = "C";
        }

        public List<string> ScalesNames { get; set; }
        public List<string> PopularChordProgression { get; set; }

        public List<string> AllNotes { get; set; } = new List<string> { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        private string scaleName;
        private string selectedNote;
        private bool disableNotesOutsideScale;
        public ObservableCollection<ScaleNoteModel> ScaleNotes { get; set; } = new ObservableCollection<ScaleNoteModel>();
        public ObservableCollection<ScaleNoteModel> HarmonizedChords { get; set; } = new ObservableCollection<ScaleNoteModel>();

        public string ScaleName
        {
            get
            {
                return scaleName;
            }

            set
            {
                scaleName = value;
                OnPropertyChanged("ScaleName");
                ExecuteScale();
            }
        }

        public string SelectedNote
        {
            get
            {
                return selectedNote;
            }

            set
            {
                selectedNote = value;
                OnPropertyChanged("SelectedNote");
                ExecuteScale();
            }
        }

        public bool DisableNotesOutsideScale
        {
            get
            {
                return disableNotesOutsideScale;
            }

            set
            {
                disableNotesOutsideScale = value;
                OnPropertyChanged("DisableNotesOutsideScale");
            }
        }

        public void GetAllScaleNotes()
        {
            if (CachedScaleValue.Any()) return;

            foreach (var currentScale in ScalesDict)
            {
                foreach (var item in NotesHelper.AllNotes)
                {
                    var res = GetScaleNotes(currentScale.Key, item);
                    CachedScaleValue.Add($"{currentScale.Key}  ({item})", res);
                }
            }
        }

        public Dictionary<string, List<ScaleNoteModel>> GetMyDict() => CachedScaleValue;

        public List<string> GetPossibleScales(List<string> notesInPlaylist)
        {
            List<string> res = new List<string>();

            foreach (var item in CachedScaleValue)
            {
                var exceptNotes = notesInPlaylist.Except(item.Value.Select(a => a.Note)).ToList();

                if (exceptNotes.Count == 0)
                {
                    res.Add(item.Key);
                }
            }

            return res;
        }

        private List<ScaleNoteModel> GetScaleNotes(string scaleName, string note)
        {
            var scalePattern = ScalesDict[scaleName];

            //var majScaleNotes = new List<string> { "F", "G", "B", "D#", "E", "A" };
            var res = new List<ScaleNoteModel>();

            int idxSelectedNote = AllNotes.IndexOf(note);
            res.Add(new ScaleNoteModel(AllNotes[idxSelectedNote], "", 1));

            int idx = idxSelectedNote;
            int stopienIdx = 2;
            foreach (var item in scalePattern)
            {
                string currentNote = AllNotes[idx];

                if ((idx + item) >= AllNotes.Count)
                {
                    int firstPart = AllNotes.Count - idx;
                    idx = item - firstPart;
                }
                else
                {
                    idx += item;
                }

                res.Add(new ScaleNoteModel(AllNotes[idx], item == 2 ? "T" : "P", stopienIdx));

                stopienIdx++;

                if (stopienIdx == 8)
                {
                    stopienIdx = 1;
                }
            }

            return res;
        }

        public void ExecuteScale()
        {
            if (string.IsNullOrEmpty(SelectedNote) || string.IsNullOrEmpty(ScaleName)) return;

            var res = GetScaleNotes(ScaleName, SelectedNote);

            ScaleNotes.Clear();
            res.ForEach(a => ScaleNotes.Add(a));
            //Harmonizuj();
        }

        //public void Harmonizuj()
        //{
        //    var harmonized = new List<ScaleNoteModel>
        //    {
        //        new ScaleNoteModel("Am", "I"),
        //        new ScaleNoteModel("G", "II"),
        //        new ScaleNoteModel("F", "III"),
        //        new ScaleNoteModel("Dm", "IV")
        //    };

        //    HarmonizedChords.Clear();
        //    harmonized.ForEach(a => HarmonizedChords.Add(a));
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