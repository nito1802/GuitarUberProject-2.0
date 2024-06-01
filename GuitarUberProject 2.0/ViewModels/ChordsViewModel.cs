using EditChordsWindow;
using GitarUberProject.Models;
using System.Collections.ObjectModel;

namespace GitarUberProject
{
    public class ChordsViewModel
    {
        public static Action RefreshItems { get; set; }

        public ObservableCollection<ChordRow> Chords { get; set; }

        public ChordsViewModel()
        {
            Chords = new ObservableCollection<ChordRow>();
            //InitExampleChords();

            if (ChordRow.OnAddRowAction == null)
            {
                ChordRow.OnAddRowAction = (arg, toInsertType) =>
                {
                    int idx = -1;
                    for (int i = 0; i < Chords.Count; i++)
                    {
                        if (Chords[i].Type == toInsertType)
                        {
                            idx = i;
                            break;
                        }
                    }

                    var newChord = new ChordRow(arg);

                    if (idx != -1)
                    {
                        Chords.Insert(idx + 1, newChord);
                    }
                    else
                    {
                        Chords.Add(newChord);
                    }

                    for (int i = 0; i < Chords.Count; i++)
                    {
                        var chord = Chords[i];
                        chord.ChordIndex = i + 1;
                    }

                    //RefreshItems?.Invoke();
                };
            }

            if (ChordRow.OnRemoveRowAction == null)
            {
                ChordRow.OnRemoveRowAction = (arg) =>
                {
                    //Chords.ToList().RemoveAll(a => a.Type == arg);
                    List<int> indexesToRemove = new List<int>();

                    for (int i = 0; i < Chords.Count; i++)
                    {
                        if (Chords[i].Type == arg)
                            indexesToRemove.Add(i);
                    }

                    for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                    {
                        Chords.RemoveAt(indexesToRemove[i]);
                    }

                    int aa = 2;
                };
            }

            if (ChordRow.ReorderRowsAction == null)
            {
                ChordRow.ReorderRowsAction = (chordRow, orderIdx) =>
                {
                    Chords.Remove(chordRow);
                    Chords.Insert(orderIdx - 1, chordRow);

                    for (int i = 0; i < Chords.Count; i++)
                    {
                        var chord = Chords[i];
                        chord.ChordIndex = i + 1;
                    }

                    int aa = 2;
                };
            }

            if (ChordRow.OnItemWithNameAlreadyExist == null)
            {
                ChordRow.OnItemWithNameAlreadyExist = (arg) =>
                {
                    foreach (var item in Chords)
                    {
                        if (item.Type == arg) return true;
                    }

                    return false;
                };
            }
        }

        public void InitExampleChords()
        {
            InputViewModelFacade inputViewModelFacade = new InputViewModelFacade();
            inputViewModelFacade.Name = "GDM 5.0";

            inputViewModelFacade.InputNotes[0, 0].CheckedFingerProp = CheckedFinger.firstFinger;
            inputViewModelFacade.InputNotes[1, 0].CheckedFingerProp = CheckedFinger.firstFinger;
            inputViewModelFacade.InputNotes[2, 0].CheckedFingerProp = CheckedFinger.firstFinger;
            inputViewModelFacade.InputNotes[3, 0].CheckedFingerProp = CheckedFinger.firstFinger;

            inputViewModelFacade.InputNotes[0, 2].CheckedFingerProp = CheckedFinger.thirdFinger;
            inputViewModelFacade.InputNotes[3, 4].CheckedFingerProp = CheckedFinger.fourthFinger;

            inputViewModelFacade.Fr = 0;
            for (int i = 0; i < inputViewModelFacade.NotesO.Length; i++)
            {
                inputViewModelFacade.NotesO[i] = NotesOStates.None;
            }

            NotesViewModelLiteVersion NotesVM = new NotesViewModelLiteVersion();

            NotesVM.InitNotes();

            int counter = 0;
            for (int i = 0; i < InputViewModelFacade.StrunyCount; i++)
            {
                for (int j = 0; j < InputViewModelFacade.ProgCount; j++)
                {
                    NoteModelLiteVersion noteModel = new NoteModelLiteVersion(inputViewModelFacade.InputNotes[i, j].Struna, inputViewModelFacade.InputNotes[i, j].Prog);
                    noteModel.CheckedFinger = inputViewModelFacade.InputNotes[i, j].CheckedFingerProp;

                    NotesVM.Notes[counter] = noteModel;
                    counter++;
                }
            }

            NotesVM.Fr = 3;

            //Chords[0].ChordA = NotesVM;

            InputViewModelFacade inputViewModelFacade2 = new InputViewModelFacade();
            inputViewModelFacade2.Name = "GDM 5.0";

            inputViewModelFacade2.InputNotes[0, 0].CheckedFingerProp = CheckedFinger.firstFinger;
            inputViewModelFacade2.InputNotes[1, 0].CheckedFingerProp = CheckedFinger.fourthFinger;
            inputViewModelFacade2.InputNotes[2, 0].CheckedFingerProp = CheckedFinger.firstFinger;
            inputViewModelFacade2.InputNotes[3, 0].CheckedFingerProp = CheckedFinger.firstFinger;

            inputViewModelFacade2.InputNotes[0, 2].CheckedFingerProp = CheckedFinger.thirdFinger;
            inputViewModelFacade2.InputNotes[3, 4].CheckedFingerProp = CheckedFinger.fourthFinger;

            inputViewModelFacade2.Fr = 0;

            for (int i = 0; i < inputViewModelFacade2.NotesO.Length; i++)
            {
                inputViewModelFacade2.NotesO[i] = NotesOStates.None;
            }

            NotesViewModelLiteVersion NotesVM2 = new NotesViewModelLiteVersion();
            NotesVM2.InitNotes();

            int counter2 = 0;
            for (int i = 0; i < InputViewModelFacade.StrunyCount; i++)
            {
                for (int j = 0; j < InputViewModelFacade.ProgCount; j++)
                {
                    NoteModelLiteVersion noteModel = new NoteModelLiteVersion(inputViewModelFacade2.InputNotes[i, j].Struna, inputViewModelFacade2.InputNotes[i, j].Prog);
                    noteModel.CheckedFinger = inputViewModelFacade2.InputNotes[i, j].CheckedFingerProp;

                    NotesVM2.Notes[counter2] = noteModel;
                    counter2++;
                }
            }

            NotesVM2.Fr = 2;

            Chords = new ObservableCollection<ChordRow>
            {
                new ChordRow()
                {
                    Type = "maj",
                    ChordC = NotesVM,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM2,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                /*
                new ChordRow()
                {
                    Type = "maj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },

                new ChordRow()
                {
                    Type = "dsus4",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "nnb",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "jyj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "rge",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "maj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },

                new ChordRow()
                {
                    Type = "dsus4",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "nnb",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "jyj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "rge",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "maj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },

                new ChordRow()
                {
                    Type = "dsus4",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "nnb",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "jyj",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "rge",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },
                new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },new ChordRow()
                {
                    Type = "fe",
                    ChordC = NotesVM2,
                    ChordCSharp = NotesVM,
                    ChordD = NotesVM,
                    ChordDSharp = NotesVM2,
                    ChordE = NotesVM,
                    ChordF = NotesVM,
                    ChordFSharp = NotesVM2,
                    ChordG = NotesVM,
                    ChordGSharp = NotesVM,
                    ChordA = NotesVM,
                    ChordASharp = NotesVM,
                    ChordB = NotesVM,
                },*/
            };
        }
    }
}