using GitarUberProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.ViewModels
{
    public class PianoNotesViewModel
    {
        public  List<PianoNoteModel> Notes { get; set; }

        public PianoNotesViewModel()
        {
            Notes = new List<PianoNoteModel>()
            {

            };


            for (int i = 0; i < 7; i++)
            {
                Notes.Add(new PianoNoteModel());
            }
        }
    }
}
