using AudioMaker.NAudiox.Services;
using GuitarUberProject.Mappers;

namespace GuitarUberProject
{
    public static class DependencyInjection
    {
        public static PlaySoundService PlaySoundService { get; set; } = new PlaySoundService();
        public static PlaySoundMapper PlaySoundMapper { get; set; } = new PlaySoundMapper();
        public static PlaylistMapper PlaylistMapper { get; set; } = new PlaylistMapper();
    }
}