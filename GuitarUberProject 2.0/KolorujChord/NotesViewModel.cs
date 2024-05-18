using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace EditChordsWindow
{
    public class NotesViewModel
    {
        public int RowSize { get; } = 6;
        public int ColumnSize { get; } = 6;
        private CheckedFinger hoverFinger;
        public List<NoteModel> Notes { get; set; } = new List<NoteModel>();
        public string ChordName { get; set; } = "pols";

        public NotesViewModel()
        {
            NoteModel.FingerDict = new Dictionary<CheckedFinger, SolidColorBrush>()
            {
                {CheckedFinger.None, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) },
                {CheckedFinger.firstFinger,(SolidColorBrush)(new BrushConverter().ConvertFrom("#E29A05")) },
                {CheckedFinger.secondFinger,(SolidColorBrush)(new BrushConverter().ConvertFrom("#B71DE9")) },
                {CheckedFinger.thirdFinger,(SolidColorBrush)(new BrushConverter().ConvertFrom("#00A0E8")) },
                {CheckedFinger.fourthFinger,(SolidColorBrush)(new BrushConverter().ConvertFrom("#E16449")) },
                {CheckedFinger.Other, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF036C50")) }
            };
            
            NoteModel.DefaultBtnBackground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD"));
            NoteModel.DefaultBtnHover = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBEE6FD")); ;

            for (int i = 0; i < RowSize; i++)
            {
                for (int j = 0; j < ColumnSize; j++)
                {
                    NoteModel note = new NoteModel(i + 1, j);
                    Notes.Add(note);
                }
            }

            HoverFinger = CheckedFinger.firstFinger;

            //HoverFinger = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFBEE6FD"));
        }

        public CheckedFinger HoverFinger
        {
            get
            {
                return hoverFinger;
            }

            set
            {
                hoverFinger = value;

                foreach (var item in Notes)
                {
                    item.HoverFinger = value;
                }
            }
        }
        
        public void ClearButtons(int fingerIdx)
        {
            foreach (var item in Notes)
            {
                item.CheckedFinger = CheckedFinger.None;
                item.HoverFinger = (CheckedFinger)fingerIdx + 1; ;
            }
        }

        public void RefreshForLiteVersion()
        {
            MoveCheckedNotesByOffset(1);
        }

        public void MoveCheckedNotesByOffset(int offset)
        {
            var checkedNotesFromEdit = Notes.Where(a => a.CheckedFinger != CheckedFinger.None).Select(a => (NoteModel)a.Clone()).ToList();


        }
    }
}
