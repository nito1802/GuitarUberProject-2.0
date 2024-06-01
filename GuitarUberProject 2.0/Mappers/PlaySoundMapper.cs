using AudioMaker.Interfaces.Models.PlaySound;
using GitarUberProject.Models;
using GitarUberProject.ViewModels;

namespace GuitarUberProject.Mappers
{
    public interface IPlaySoundMapper
    {
        PlaysoundStrumViewModel MapStrumViewModel(StrumViewModel strumViewModel);

        PlaysoundMixerModel MapMixerModel(MixerModel mixerModel);

        PlaysoundKlocekChordModel MapKlocekChordModel(KlocekChordModel klocekChordModel);
    }

    public class PlaySoundMapper : IPlaySoundMapper
    {
        public PlaysoundKlocekChordModel MapKlocekChordModel(KlocekChordModel klocekChordModel) => new()
        {
            XPos = klocekChordModel.XPos,
            ChannelNr = klocekChordModel.ChannelNr,
            Mp3Name = klocekChordModel.Mp3Name,
            IsChord = klocekChordModel.IsChord,
            StrumViewModel = MapStrumViewModel(klocekChordModel.StrumViewModel)
        };

        public PlaysoundMixerModel MapMixerModel(MixerModel mixerModel) => new()
        {
            Vol = mixerModel.Vol
        };

        public PlaysoundStrumViewModel MapStrumViewModel(StrumViewModel strumViewModel) => new()
        {
            StrumPattern = strumViewModel.StrumPattern.Select(MapStrumModel).ToList(),
        };

        private PlaysoundStrumModel MapStrumModel(StrumModel strumViewModel) => new()
        {
            PlayedNotes = strumViewModel.PlayedNotes.Select(MapStrumNoteDetails).ToList(),
        };

        private PlaysoundStrumNoteDetails MapStrumNoteDetails(StrumNoteDetails strumViewModel) => new()
        {
            Path = strumViewModel.Path,
            Name = strumViewModel.Name,
            DelayMs = strumViewModel.DelayMs,
            PlayTime = strumViewModel.PlayTime
        };
    }
}