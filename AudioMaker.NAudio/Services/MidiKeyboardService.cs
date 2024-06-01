using NAudio.Midi;
using System.Diagnostics;

//using System.Windows.Threading;

namespace GitarUberProject.Services
{
    public class MidiKeyboardService
    {
        public MidiIn MidiKeyboard { get; set; }

        //TODO: W razie kontynuowania przerobić na Actiony i tam wywołać Dispatchera (żeby nie dodawać tu referencji do WPF)
        //public Dispatcher Dispatcher { get; set; }

        public MidiKeyboardService(/*Dispatcher Dispatcher*/)
        {
            //this.Dispatcher = Dispatcher;

            Init();
        }

        private void Init()
        {
            var midiCounter = MidiIn.NumberOfDevices;

            if (midiCounter > 0)
            {
                try
                {
                    MidiKeyboard = new MidiIn(0);
                    MidiKeyboard.MessageReceived += MidiKeyboard_MessageReceived;
                    MidiKeyboard.Start();
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void MidiKeyboard_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOn)
            {
                string noteName = ((NAudio.Midi.NoteOnEvent)e.MidiEvent).NoteName;

                //this.Dispatcher.Invoke(() =>
                //{
                //    NotesViewModel.NotesNameDict[noteName].IsSelected = true;
                //});
            }
            else if (e.MidiEvent.CommandCode == MidiCommandCode.NoteOff)
            {
                string text22 = e.MidiEvent.ToString();

                string noteName = ((NAudio.Midi.NoteEvent)e.MidiEvent).NoteName;

                //this.Dispatcher.Invoke(() =>
                //{
                //    NotesViewModel.NotesNameDict[noteName].IsSelected = false;
                //});
            }

            string text = e.MidiEvent.ToString();
            Debug.WriteLine(text);
        }
    }
}