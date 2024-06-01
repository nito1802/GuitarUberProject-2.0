using GitarUberProject.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GitarUberProject.ViewModels
{
    public class ToViewSingleStrumViewModels : ICloneable
    {
        private RelayCommand removeStrumPattern;
        private RelayCommand editStrumPattern;
        private RelayCommand clonePattern;

        public static Action<ToViewSingleStrumViewModels> RemoveStrumPatternAction { get; set; }
        public static Action<ToViewSingleStrumViewModels> EditStrumPatternAction { get; set; }
        public static Action<ToViewSingleStrumViewModels> ClonePatternAction { get; set; }

        public ObservableCollection<ToViewStrumModels> ToViewStrumModels { get; set; } = new ObservableCollection<ToViewStrumModels>();

        public ICommand RemoveStrumPattern
        {
            get
            {
                if (removeStrumPattern == null)
                {
                    removeStrumPattern = new RelayCommand(param =>
                    {
                        RemoveStrumPatternAction?.Invoke(this);
                    }
                     , param => true);
                }
                return removeStrumPattern;
            }
        }

        public ICommand EditStrumPattern
        {
            get
            {
                if (editStrumPattern == null)
                {
                    editStrumPattern = new RelayCommand(param =>
                    {
                        EditStrumPatternAction?.Invoke(this);
                    }
                     , param => true);
                }
                return editStrumPattern;
            }
        }

        public ICommand ClonePattern
        {
            get
            {
                if (clonePattern == null)
                {
                    clonePattern = new RelayCommand(param =>
                    {
                        ClonePatternAction?.Invoke(this);
                    }
                     , param => true);
                }
                return clonePattern;
            }
        }

        public object Clone()
        {
            var clone = new ToViewSingleStrumViewModels();

            var clonedList = this.ToViewStrumModels.Select(a => a.Clone()).Cast<ToViewStrumModels>().ToList();

            clonedList.ForEach(a => clone.ToViewStrumModels.Add(a));

            return clone;
        }
    }

    public class ToViewStrumViewModels
    {
        public ObservableCollection<ToViewSingleStrumViewModels> ToViewSingleStrumModels { get; set; } = new ObservableCollection<ToViewSingleStrumViewModels>();

        public void AddStrumModel(List<EditStrumModel> editStrumModels)
        {
            var newStrumPattern = new ToViewSingleStrumViewModels();

            foreach (var item in editStrumModels)
            {
                var newItem = new ToViewStrumModels();

                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 0 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 1 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 2 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 3 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 4 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 5 });

                newItem.DelayBeforeMs = item.DelayBeforeMs;
                newItem.DelayBetweenStrunaMs = item.DelayBetweenStrunaMs;

                for (int i = 0; i < item.Notes.Count; i++)
                {
                    newItem.Notes[i].CheckedNote = item.Notes[i].CheckedNote;
                    newItem.Notes[i].MyBackground = item.Notes[i].MyBackground;
                    newItem.Notes[i].MyIdx = item.Notes[i].MyIdx;
                }

                newStrumPattern.ToViewStrumModels.Add(newItem);
            }

            ToViewSingleStrumModels.Insert(0, newStrumPattern);
        }

        public void EditStrumModel(ToViewSingleStrumViewModels toEdit, List<EditStrumModel> editStrumModels)
        {
            toEdit.ToViewStrumModels.Clear();
            foreach (var item in editStrumModels)
            {
                var newItem = new ToViewStrumModels();
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 0 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 1 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 2 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 3 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 4 });
                newItem.Notes.Add(new ToViewEditStrumNoteDetailsModel() { MyIdx = 5 });

                newItem.DelayBeforeMs = item.DelayBeforeMs;
                newItem.DelayBetweenStrunaMs = item.DelayBetweenStrunaMs;

                for (int i = 0; i < item.Notes.Count; i++)
                {
                    newItem.Notes[i].CheckedNote = item.Notes[i].CheckedNote;
                    newItem.Notes[i].MyBackground = item.Notes[i].MyBackground;
                    newItem.Notes[i].MyIdx = item.Notes[i].MyIdx;
                }

                toEdit.ToViewStrumModels.Add(newItem);
            }
        }
    }
}