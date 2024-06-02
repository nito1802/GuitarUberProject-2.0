using System.Windows.Input;

namespace GitarUberProject.Models
{
    public class GlobalGitarButtonModel
    {
        private RelayCommand playNote;
        public int StrunaNr { get; set; }
        //public WaveOut NotePlayer { get; set; }
        //public WaveFileReader WaveFileReader { get; set; }

        public GlobalGitarButtonModel(int strunaNr)
        {
            StrunaNr = strunaNr;
        }

        public ICommand PlayNote
        {
            get
            {
                if (playNote == null)
                {
                    playNote = new RelayCommand(param =>
                    {
                        throw new Exception("o tu jesteś!");

                        //if (NotePlayer != null)
                        //{
                        //    WaveFileReader.Position = 0;
                        //    NotePlayer.Play();
                        //}
                    }
                     , param => true);
                }
                return playNote;
            }
        }
    }
}