using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMaker.Interfaces.Models.MidiExport
{
    public class PlaylistKlocekChordViewModel
    {
        //public static int RowsCount { get; } = 6;
        //public static double ItemHeight { get; } = 20;
        //public static double ItemWidth { get; } = 25;

        //private PlaylistKlocekChordModel draggedItem;
        //private List<PlaylistKlocekChordModel> ctrlSelection = new List<PlaylistKlocekChordModel>();
        //private List<PlaylistKlocekChordModel> borderSelection = new List<PlaylistKlocekChordModel>();
        public List<PlaylistKlocekChordModel> Klocki { get; set; }

        //public double Bpm { get; set; }
        //public int CanvasWidth { get; set; }
    }
}