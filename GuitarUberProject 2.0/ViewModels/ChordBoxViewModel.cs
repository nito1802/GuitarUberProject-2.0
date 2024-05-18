using EditChordsWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.ViewModels
{
    public class ChordBoxViewModel
    {
        public ObservableCollection<NotesViewModelLiteVersion> ChordsInBox { get; set; } = new ObservableCollection<NotesViewModelLiteVersion>();
    }
}
