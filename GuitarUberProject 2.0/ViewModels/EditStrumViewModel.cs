using EditChordsWindow;
using GitarUberProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GitarUberProject.ViewModels
{
    public class EditStrumViewModel
    {
        public ObservableCollection<EditStrumModel> EditStrumModels { get; set; }

        private RelayCommand tryCommand;
        private RelayCommand addStrum;

        public static Action<List<EditStrumModel>> TryPlayChordWithPattern { get; set; }

        public EditStrumViewModel()
        {
            EditStrumModels = new ObservableCollection<EditStrumModel>();
            EditStrumModels.Add(new EditStrumModel());
            AssignActions();
        }

        public void AssignStrumPattern(List<EditStrumModel> models)
        {
            EditStrumModels.Clear();
            models.ForEach(a => EditStrumModels.Add(a));

            if (!EditStrumModels.Any()) EditStrumModels.Add(new EditStrumModel());
        }

        private void AssignActions()
        {
            if (EditStrumModel.InsertSingleStrumAction == null)
            {
                EditStrumModel.InsertSingleStrumAction = (strum) =>
                {
                    int idx = EditStrumModels.IndexOf(strum);

                    if (idx == -1) return;

                    EditStrumModels.Insert(idx + 1, new EditStrumModel());
                };
            }

            if (EditStrumModel.RemoveSingleStrumAction == null)
            {
                EditStrumModel.RemoveSingleStrumAction = (strum) =>
                {
                    EditStrumModels.Remove(strum);

                    if (!EditStrumModels.Any()) EditStrumModels.Add(new EditStrumModel());
                };
            }

            if (EditStrumNoteDetailsModel.SetStruna == null)
            {
                EditStrumNoteDetailsModel.SetStruna = (struna, finger) =>
                {
                    List<EditStrumNoteDetailsModel> myNotes = null;
                    foreach (var item in EditStrumModels)
                    {
                        int isIdx = item.Notes.IndexOf(struna);

                        if (isIdx != -1)
                        {
                            myNotes = item.Notes;
                        }
                    }

                    if (finger == EditChordsWindow.CheckedFinger.secondFinger)
                    {
                        foreach (var item in myNotes)
                        {
                            item.CheckedNote = CheckedFinger.None;
                        }

                        struna.CheckedNote = finger;
                    }
                    else
                    {
                        var firstNotes = myNotes.Where(a => a.CheckedNote == CheckedFinger.firstFinger).ToList();

                        foreach (var item in firstNotes)
                        {
                            item.CheckedNote = CheckedFinger.None;
                        }

                        var secondNotes = myNotes.Where(a => a.CheckedNote == CheckedFinger.secondFinger).ToList();

                        if (secondNotes.Count > 1)
                        {
                        }
                        else if (secondNotes.Count == 1)
                        {
                            int idxSecond = myNotes.IndexOf(secondNotes.First());
                            int idxFirst = myNotes.IndexOf(struna);

                            if (idxFirst == idxSecond)
                            {
                                struna.CheckedNote = finger;
                                return;
                            }
                            else
                            {
                                if (idxFirst > idxSecond)
                                {
                                    for (int i = idxSecond + 1; i < idxFirst + 1; i++)
                                    {
                                        myNotes[i].CheckedNote = CheckedFinger.firstFinger;
                                    }
                                }
                                else
                                {
                                    for (int i = idxFirst; i < idxSecond; i++)
                                    {
                                        myNotes[i].CheckedNote = CheckedFinger.firstFinger;
                                    }
                                }
                            }
                        }
                        else
                        {
                            struna.CheckedNote = finger;
                        }
                    }
                };
            }
        }

        public ICommand TryCommand
        {
            get
            {
                if (tryCommand == null)
                {
                    tryCommand = new RelayCommand(param =>
                    {
                        TryPlayChordWithPattern?.Invoke(EditStrumModels.ToList());
                    }
                     , param => true);
                }
                return tryCommand;
            }
        }

        public ICommand AddStrum
        {
            get
            {
                if (addStrum == null)
                {
                    addStrum = new RelayCommand(param =>
                    {
                        EditStrumModels.Add(new EditStrumModel());
                        //RemoveSingleStrumAction?.Invoke(this);
                    }
                     , param => true);
                }
                return addStrum;
            }
        }

        public void AddEditStrumModel(List<ToViewStrumModels> editStrumModels)
        {
            List<EditStrumModel> newStrumModel = new List<EditStrumModel>();

            foreach (var item in editStrumModels)
            {
                var newItem = new EditStrumModel();

                newItem.DelayBeforeMs = item.DelayBeforeMs;
                newItem.DelayBetweenStrunaMs = item.DelayBetweenStrunaMs;

                for (int i = 0; i < item.Notes.Count; i++)
                {
                    newItem.Notes[i].CheckedNote = item.Notes[i].CheckedNote;
                    newItem.Notes[i].MyBackground = item.Notes[i].MyBackground;
                    newItem.Notes[i].MyIdx = item.Notes[i].MyIdx;
                }

                newStrumModel.Add(newItem);
            }

            AssignStrumPattern(newStrumModel);
        }
    }
}