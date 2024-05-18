using GitarUberProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.ViewModels
{
    public class MixerViewModel
    {
        public ObservableCollection<MixerModel> MixerModels { get; set; } = new ObservableCollection<MixerModel>();

        public void InitMixerModels()
        {
            MixerModels.Add(new MixerModel("Rythm", 100, true));
            MixerModels.Add(new MixerModel("Rythm #2", 100, false));
            MixerModels.Add(new MixerModel("Lead", 100, false));
            MixerModels.Add(new MixerModel("Vocal", 100, false));
        }
    }
}
