using EditChordsWindow;
using System.Collections.ObjectModel;

namespace GitarUberProject
{
    public class CustomChordsViewModel
    {
        public ObservableCollection<NotesViewModelLiteVersion> Chords { get; set; }

        public CustomChordsViewModel()
        {
            Chords = new ObservableCollection<NotesViewModelLiteVersion>();

            if (NotesViewModelLiteVersion.DeselectAllOtherChords == null)
            {
                NotesViewModelLiteVersion.DeselectAllOtherChords = () =>
                {
                    foreach (var item in Chords)
                    {
                        item.IsSelected = false;
                        item.FocusedChord = false;
                    }
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
            inputViewModelFacade.NotesO[3] = NotesOStates.None;

            NotesViewModelLiteVersion NotesVM = new NotesViewModelLiteVersion();

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

            Chords = new ObservableCollection<NotesViewModelLiteVersion>()
            {
                NotesVM
            };
        }
    }
}