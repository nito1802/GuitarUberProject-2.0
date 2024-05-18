using GitarUberProject.Games_and_Fun.RadialGaugeData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GitarUberProject.Games_and_Fun.HighScoreStats
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }

    public class HighScoreStatsModel : INotifyPropertyChanged
    {
        private int allCounter;
        public RadialGaugeViewModel RadialViewModel { get; set; } = new RadialGaugeViewModel();
        public int Proc0 { get; set; }
        public int Proc10 { get; set; }
        public int Proc20 { get; set; }
        public int Proc30 { get; set; }
        public int Proc40 { get; set; }
        public int Proc50 { get; set; }
        public int Proc60 { get; set; }
        public int Proc70 { get; set; }
        public int Proc80 { get; set; }
        public int Proc90 { get; set; }
        public int Proc100 { get; set; }

        public HighScoreStatsModel()
        {
        }


        public void AddResult(bool value)
        {
            if (value)
            {
                Proc100++;
            }
            else
            {
                Proc0++;
            }

            AllCounter = Proc0 + Proc10 + Proc20 + Proc30 + Proc40 + Proc50 + Proc60 + Proc70 + Proc80 + Proc90 + Proc100;

            var radialGaugeValue = RadialViewModel.Data.FirstOrDefault();

            if(radialGaugeValue == null)
            {
                RadialViewModel.Data.Add(new RadialGaugeModel { Name = "", Count = 0 });
                radialGaugeValue = RadialViewModel.Data.First();
            }
            radialGaugeValue.Count = GetPercentCounter();
        }

        public void AddResult(int interval)
        {
            if(interval == 0)
            {
                Proc100++;
            }
            else if(interval == 1)
            {
                Proc90++;
            }
            else if (interval == 2)
            {
                Proc80++;
            }
            else if (interval == 3)
            {
                Proc70++;
            }
            else if (interval == 4)
            {
                Proc60++;
            }
            else if (interval == 5)
            {
                Proc50++;
            }
            else if (interval == 6)
            {
                Proc40++;
            }
            else if (interval == 7)
            {
                Proc30++;
            }
            else if (interval == 8)
            {
                Proc20++;
            }
            else if (interval == 9)
            {
                Proc10++;
            }
            else
            {
                Proc0++;
            }

            AllCounter = Proc0 + Proc10 + Proc20 + Proc30 + Proc40 + Proc50 + Proc60 + Proc70 + Proc80 + Proc90 + Proc100;

            var radialGaugeValue = RadialViewModel.Data.First();
            radialGaugeValue.Count = GetPercentCounter();
        }

        private double GetPercentCounter()
        {
            double wazoneValue = 0;
            wazoneValue += Proc10 * 0.1;
            wazoneValue += Proc20 * 0.2;
            wazoneValue += Proc30 * 0.3;
            wazoneValue += Proc40 * 0.4;
            wazoneValue += Proc50 * 0.5;
            wazoneValue += Proc60 * 0.6;
            wazoneValue += Proc70 * 0.7;
            wazoneValue += Proc80 * 0.8;
            wazoneValue += Proc90 * 0.9;
            wazoneValue += Proc100;

            double procValue = wazoneValue * 100 / AllCounter;

            double res = Math.Round(procValue);
            return res;
        }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime Date { get; set; }

        public int AllCounter
        {
            get
            {
                return allCounter;
            }

            set
            {
                allCounter = value;
                OnPropertyChanged("AllCounter");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; //INotifyPropertyChanged

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
