using EditChordsWindow;
using GitarUberProject.HelperWindows;
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
    public class ChordRow : INotifyPropertyChanged
    {
        private RelayCommand addRow;
        private RelayCommand changeOrder;
        private RelayCommand editRowHeaderName;
        private RelayCommand resetIntervals;
        private RelayCommand removeRow;
        private string type;
        private int chordIndex;
        private List<int> intervals;
        private string intervalsText;
        private int fontSizeIntervalText = 10; //read 10, normal 13
        public ChordRow()
        {
        }

        public ChordRow(string type)
        {
            Type = type;
            ChordC = new NotesViewModelLiteVersion("C", Type);
            ChordCSharp = new NotesViewModelLiteVersion("C#", Type);
            ChordD = new NotesViewModelLiteVersion("D", Type);
            ChordDSharp = new NotesViewModelLiteVersion("D#", Type);
            ChordE = new NotesViewModelLiteVersion("E", Type);
            ChordF = new NotesViewModelLiteVersion("F", Type);
            ChordFSharp = new NotesViewModelLiteVersion("F#", Type);
            ChordG = new NotesViewModelLiteVersion("G", Type);
            ChordGSharp = new NotesViewModelLiteVersion("G#", Type);
            ChordA = new NotesViewModelLiteVersion("A", Type);
            ChordASharp = new NotesViewModelLiteVersion("A#", Type);
            ChordB = new NotesViewModelLiteVersion("B", Type);

            ChordC.InitNotes();
            ChordCSharp.InitNotes();
            ChordD.InitNotes();
            ChordDSharp.InitNotes();
            ChordE.InitNotes();
            ChordF.InitNotes();
            ChordFSharp.InitNotes();
            ChordG.InitNotes();
            ChordGSharp.InitNotes();
            ChordA.InitNotes();
            ChordASharp.InitNotes();
            ChordB.InitNotes();

            InitIntervalsActions();
        }

        public static Action<string, string> OnAddRowAction { get; set; }
        public static Action<string> OnRemoveRowAction  { get; set; }
        public static Func<string, bool> OnItemWithNameAlreadyExist { get; set; }
        public static Action<ChordRow, int> ReorderRowsAction { get; set; }

        public NotesViewModelLiteVersion ChordC { get; set; }
        public NotesViewModelLiteVersion ChordCSharp { get; set; }
        public NotesViewModelLiteVersion ChordD { get; set; }
        public NotesViewModelLiteVersion ChordDSharp { get; set; }
        public NotesViewModelLiteVersion ChordE { get; set; }
        public NotesViewModelLiteVersion ChordF { get; set; }

        public NotesViewModelLiteVersion ChordFSharp { get; set; }
        public NotesViewModelLiteVersion ChordG { get; set; }
        public NotesViewModelLiteVersion ChordGSharp { get; set; }
        public NotesViewModelLiteVersion ChordA { get; set; }
        public NotesViewModelLiteVersion ChordASharp { get; set; }
        public NotesViewModelLiteVersion ChordB { get; set; }

        public override string ToString()
        {
            return $"{Type} GitarUberProject.Models.ChordRow";
        }

        public void InitIntervalsActions()
        {
            var allChords = GetAllChords();

            foreach (var item in allChords)
            {
                item.ChordReadModeBackground = NotesHelper.ChordColor[item.ChordName];
                item.GetIntervalsFromParent = () => Intervals;
                item.SetIntervalInParent = (a) => Intervals = a;
                var noteOctaves = item.NoteOctaves.Reverse().ToList();
                item.NoteOctavesDataGrid.Clear();
                noteOctaves.ForEach(a => item.NoteOctavesDataGrid.Add(a));
            }
        }

        private NotesViewModelLiteVersion GetSelectedChord(string chord)
        {
            NotesViewModelLiteVersion result = null;

            switch (chord)
            {
                case "C":
                    result = ChordC;
                    break;
                case "C#":
                    result = ChordCSharp;
                    break;
                case "D":
                    result = ChordD;
                    break;
                case "D#":
                    result = ChordDSharp;
                    break;
                case "E":
                    result = ChordE;
                    break;
                case "F":
                    result = ChordF;
                    break;
                case "F#":
                    result = ChordFSharp;
                    break;
                case "G":
                    result = ChordG;
                    break;
                case "G#":
                    result = ChordGSharp;
                    break;
                case "A":
                    result = ChordA;
                    break;
                case "A#":
                    result = ChordASharp;
                    break;
                case "B":
                    result = ChordB;
                    break;
                default:
                    break;
            }

            return result;
        }

        [JsonIgnore]
        public ICommand AddRow
        {
            get
            {
                if (addRow == null)
                {
                    addRow = new RelayCommand(param =>
                    {
                        string textPrefix = "Nowy akord: ";
                        string text = "";

                        List<CustomMessageBoxValidation> errorsValidation = new List<CustomMessageBoxValidation>()
                        {
                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Taka nazwa już istnieje",
                                ErrorCondition = (arg) =>
                                {
                                    if(OnItemWithNameAlreadyExist != null)
                                    {
                                        return OnItemWithNameAlreadyExist(arg);
                                    }

                                    return false;
                                }
                            },

                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Nazwa nie może być pusta",
                                ErrorCondition = (arg) =>
                                {
                                    return string.IsNullOrEmpty(arg);
                                }
                            }
                        };

                        CustomMessageBox customMessageBox = new CustomMessageBox(textPrefix, text, errorsValidation);
                        customMessageBox.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        customMessageBox.Width *= App.CustomScaleX;
                        customMessageBox.Height *= App.CustomScaleY;

                        customMessageBox.ShowDialog();
                        
                        if (customMessageBox.DialogResult == true)
                        {
                            OnAddRowAction?.Invoke(customMessageBox.Text, Type);
                        }   
                    }
                     , param => true);
                }
                return addRow;
            }
        }

        [JsonIgnore]
        public ICommand ChangeOrder
        {
            get
            {
                if (changeOrder == null)
                {
                    changeOrder = new RelayCommand(param =>
                    {
                        string textPrefix = "Nowa nazwa: ";
                        string text = ChordIndex.ToString();

                        List<CustomMessageBoxValidation> errorsValidation = new List<CustomMessageBoxValidation>()
                        {
                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Nazwa nie może być pusta",
                                ErrorCondition = (arg) =>
                                {
                                    return string.IsNullOrEmpty(arg);
                                }
                            },
                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Musi być dodatnia liczba",
                                ErrorCondition = (arg) =>
                                {
                                    bool res = int.TryParse(arg, out var parsed);

                                    if(res == false) return true;
                                    return parsed <= 0;
                                }
                            }
                        };


                        CustomMessageBox customMessageBox = new CustomMessageBox(textPrefix, text, errorsValidation);

                        customMessageBox.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        customMessageBox.Width *= App.CustomScaleX;
                        customMessageBox.Height *= App.CustomScaleY;

                        customMessageBox.ShowDialog();

                        if (customMessageBox.DialogResult == true)
                        {
                            var orderIdx = int.Parse(customMessageBox.Text);

                            ReorderRowsAction(this, orderIdx);
                        }
                    }
                     , param => true);
                }
                return changeOrder;
            }
        }

        [JsonIgnore]
        public ICommand EditRowHeaderName
        {
            get
            {
                if (editRowHeaderName == null)
                {
                    editRowHeaderName = new RelayCommand(param =>
                    {
                        string textPrefix = "Nowa nazwa: ";
                        string text = Type;

                        List<CustomMessageBoxValidation> errorsValidation = new List<CustomMessageBoxValidation>()
                        {
                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Taka nazwa już istnieje",
                                ErrorCondition = (arg) =>
                                {
                                    if(OnItemWithNameAlreadyExist != null)
                                    {
                                        return OnItemWithNameAlreadyExist(arg);
                                    }

                                    return false;
                                }
                            },

                            new CustomMessageBoxValidation()
                            {
                                ErrorText = "Nazwa nie może być pusta",
                                ErrorCondition = (arg) =>
                                {
                                    return string.IsNullOrEmpty(arg);
                                }
                            }
                        };


                        CustomMessageBox customMessageBox = new CustomMessageBox(textPrefix, text, errorsValidation);

                        customMessageBox.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        customMessageBox.Width *= App.CustomScaleX;
                        customMessageBox.Height *= App.CustomScaleY;

                        customMessageBox.ShowDialog();

                        if(customMessageBox.DialogResult == true)
                        {
                            Type = customMessageBox.Text;
                        }
                    }
                     , param => true);
                }
                return editRowHeaderName;
            }
        }

        [JsonIgnore]
        public ICommand ResetIntervals
        {
            get
            {
                if (resetIntervals == null)
                {
                    resetIntervals = new RelayCommand(param =>
                    {
                        Intervals.Clear();
                        IntervalsText = "";

                        NotesViewModelLiteVersion.UpdateIntervalsAction?.Invoke(Type, Intervals);
                    }
                     , param => true);
                }
                return resetIntervals;
            }
        }

        [JsonIgnore]
        public ICommand RemoveRow
        {
            get
            {
                if (removeRow == null)
                {
                    removeRow = new RelayCommand(param =>
                    {
                        CustomYesNoBox customYesNoBox = new CustomYesNoBox($"Czy na pewno chcesz usunąć chord \"{Type}\"?");

                        customYesNoBox.MGrid.LayoutTransform = new ScaleTransform(App.CustomScaleX, App.CustomScaleY, 0, 0);
                        customYesNoBox.Width *= App.CustomScaleX;
                        customYesNoBox.Height *= App.CustomScaleY;

                        customYesNoBox.ShowDialog();

                        if (customYesNoBox.DialogResult == true)
                        {
                            OnRemoveRowAction?.Invoke(Type);
                        }
                    }
                     , param => true);
                }
                return removeRow;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public int ChordIndex
        {
            get
            {
                return chordIndex;
            }

            set
            {
                chordIndex = value;
                OnPropertyChanged("ChordIndex");
            }
        }

        public List<int> Intervals
        {
            get => intervals;
            set
            {
                intervals = value;
                IntervalsText = intervals != null ? string.Join(", ", intervals) : null;
            }
        }
        public string IntervalsText
        {
            get => intervalsText;
            set
            {
                intervalsText = value;
                OnPropertyChanged("IntervalsText");
            }
        }

        public int FontSizeIntervalText
        {
            get => fontSizeIntervalText;
            set
            {
                fontSizeIntervalText = value;
                OnPropertyChanged("FontSizeIntervalText");
            }
        }

        public void DeselectAllChords()
        {
            List<NotesViewModelLiteVersion> chords = new List<NotesViewModelLiteVersion>
            {
                ChordC, ChordCSharp, ChordD, ChordDSharp, ChordE, ChordF, ChordFSharp, ChordG, ChordGSharp, ChordA, ChordASharp, ChordB
            };

            foreach (var item in chords)
            {
                item.IsSelected = false;
                item.FocusedChord = false;
            }
        }

        public void ActionForAllChords(Action<NotesViewModelLiteVersion> action)
        {
            if (action == null) return;

            List<NotesViewModelLiteVersion> chords = new List<NotesViewModelLiteVersion>
            {
                ChordC, ChordCSharp, ChordD, ChordDSharp, ChordE, ChordF, ChordFSharp, ChordG, ChordGSharp, ChordA, ChordASharp, ChordB
            };

            int counter = 0;
            foreach (var item in chords)
            {
                action(item);
                counter++;
            }
        }

        public List<NotesViewModelLiteVersion> GetAllChords()
        {
            List<NotesViewModelLiteVersion> chords = new List<NotesViewModelLiteVersion>
            {
                ChordC, ChordCSharp, ChordD, ChordDSharp, ChordE, ChordF, ChordFSharp, ChordG, ChordGSharp, ChordA, ChordASharp, ChordB
            };

            return chords;
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