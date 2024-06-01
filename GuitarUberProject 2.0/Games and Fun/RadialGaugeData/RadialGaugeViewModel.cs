using System.Collections.ObjectModel;

namespace GitarUberProject.Games_and_Fun.RadialGaugeData
{
    public class RadialGaugeViewModel
    {
        public ObservableCollection<RadialGaugeModel> Data { get; set; } = new ObservableCollection<RadialGaugeModel>();

        public RadialGaugeViewModel()
        {
            //if(!Data.Any())
            //    Data.Add(new RadialGaugeModel { Name = "", Count = 0 });
        }
    }
}