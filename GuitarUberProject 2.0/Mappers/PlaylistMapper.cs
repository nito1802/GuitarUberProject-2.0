using AudioMaker.Interfaces.Models.MidiExport;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioMaker.Interfaces.Mappers
{
    public interface IPlaylistMapper
    {
        //PlaylistKlocekChordModel MapKlocekChordModel(KlocekChordModel klocekChordModel);

        PlaylistKlocekChordViewModel MapKlocekChordViewModel(KlocekChordViewModel klocekChordViewModel);

        //PlaylistKlocekNoteDetails MapKlocekNoteDetails(KlocekNoteDetails klocekNoteDetails);
    }

    public class PlaylistMapper : IPlaylistMapper
    {
        public PlaylistKlocekChordModel MapKlocekChordModel(KlocekChordModel klocekChordModel) => new()
        {
            XPos = klocekChordModel.XPos,
            ChordName = klocekChordModel.ChordName,
            IsChord = klocekChordModel.IsChord,
            NotesInChord = klocekChordModel.NotesInChord.Select(MapKlocekNoteDetails).ToList()
        };

        public PlaylistKlocekChordViewModel MapKlocekChordViewModel(KlocekChordViewModel klocekChordViewModel) => new()
        {
            Klocki = klocekChordViewModel.Klocki.Select(MapKlocekChordModel).ToList()
        };

        private PlaylistKlocekNoteDetails MapKlocekNoteDetails(KlocekNoteDetails klocekNoteDetails) => new()
        {
            Name = klocekNoteDetails.Name,
            Octave = klocekNoteDetails.Octave
        };
    }
}