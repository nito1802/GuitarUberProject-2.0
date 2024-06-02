using AudioMaker.Interfaces.Consts;
using AudioMaker.Interfaces.Models.PlaySound;

namespace AudioMaker.Interfaces.Interfaces
{
    public interface IPlaySoundService
    {
        double PlaylistMilisecond();

        PlaylistState GetPlaylistState();

        void PausePlaylist();

        void ResumePlaylist();

        void StopPlaylist();

        void PlayNote(int nrStruny, string mp3Name);

        void TryPlayChordFromPlaylist(PlaysoundStrumViewModel strumManager);

        void MyExtraPlayChordWithStrumPattern(PlaysoundStrumViewModel strumManager);

        void PlayPlaylist(
                    List<PlaysoundKlocekChordModel> klocki,
                    List<PlaysoundMixerModel> mixerModels,
                    double Bpm,
                    double BeatWidth,
                    string recordBasePath,
                    double delayByMs = 0,
                    bool exportToWavMp3 = false
                    );

        void PlayChordPiano(List<string> paths, int delayMs, string recordBasePath, int index = 0, bool exportToWavMp3 = false);

        void PlayChord(List<string> paths, int delayMs);

        void PlayTwoChords(List<string> chordANotes, List<string> chordBNotes, int delayBetweenNotes, int delayBetweenChords);
    }
}