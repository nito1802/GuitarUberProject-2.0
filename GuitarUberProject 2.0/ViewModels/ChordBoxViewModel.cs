using EditChordsWindow;
using System.Collections.ObjectModel;

namespace GitarUberProject.ViewModels
{
    public class ChordBoxViewModel
    {
        public ObservableCollection<NotesViewModelLiteVersion> ChordsInBox { get; set; } = new ObservableCollection<NotesViewModelLiteVersion>();
    }
}