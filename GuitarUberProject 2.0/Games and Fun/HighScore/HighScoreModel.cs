using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GitarUberProject.Games_and_Fun
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }

    public class HighScoreModel : INotifyPropertyChanged
    {
        private DispatcherTimer initializeTimer = new DispatcherTimer();
        private DispatcherTimer makeNickBoldTimer = new DispatcherTimer();
        private int index;
        private bool isNewItem;

        public HighScoreModel(string name, int score)
        {
            Score = score;
            InitTimers();
            InitializeNewRow();
        }

        public HighScoreModel()
        {
        }

        private void InitTimers()
        {
            initializeTimer.Tick += (send, er) =>
            {
                IsNewItem = false;
                initializeTimer.Stop();
            };

            initializeTimer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        public void InitializeNewRow()
        {
            IsNewItem = true;
            Date = DateTime.Now;
            initializeTimer.Start();
            makeNickBoldTimer.Start();
        }

        public int Score { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public int Index
        {
            get => index;
            set
            {
                index = value;
                OnPropertyChanged("Index");
            }
        }

        [JsonIgnore]
        public bool IsNewItem
        {
            get
            {
                return isNewItem;
            }

            set
            {
                isNewItem = value;
                OnPropertyChanged("IsNewItem");
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
