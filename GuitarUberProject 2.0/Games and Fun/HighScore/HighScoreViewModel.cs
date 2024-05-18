using GitarUberProject.Games_and_Fun.HighScoreStats;
using GitarUberProject.Games_and_Fun.RadialGaugeData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.Games_and_Fun
{
    public class HighScoreViewModel
    {
        [JsonIgnore]
        public int TopScoreCount { get; } = 5;
        public static string RootFilePath { get; set; }
        [JsonIgnore]
        public string FileName { get; set; }

        public HighScoreStatsModel AllTimeStats { get; set; } = new HighScoreStatsModel();
        [JsonIgnore]
        public HighScoreStatsModel TodayStats { get; set; } = new HighScoreStatsModel();

        public ObservableCollection<HighScoreModel> BestScores { get; set; }
        [JsonIgnore]
        public ObservableCollection<HighScoreModel> NetworkBestScores { get; set; }

        public void AddBestScore(int score)
        {
            var toAdd = new HighScoreModel("local", score);

            BestScores.Add(toAdd);

            var bestScores = BestScores.OrderByDescending(a => a.Score).Take(TopScoreCount).ToList();
            int idx = 1;
            bestScores.ForEach(a => a.Index = idx++);

            BestScores.Clear();
            bestScores.ForEach(a => BestScores.Add(a));
        }

        public void Save()
        {
            if (!Directory.Exists(RootFilePath)) Directory.CreateDirectory(RootFilePath);
            string fullFilePath = Path.Combine(RootFilePath, FileName);

            string serializedText = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(fullFilePath, serializedText);
        }

        public HighScoreViewModel Load()
        {
            string fullFilePath = Path.Combine(RootFilePath, FileName);

            if (!File.Exists(fullFilePath))
            {
                return null;
            }

            string jsonContent = File.ReadAllText(fullFilePath);

            HighScoreViewModel highScoreViewModel = null;
            try
            {
                highScoreViewModel = JsonConvert.DeserializeObject<HighScoreViewModel>(jsonContent);
                
                if(!highScoreViewModel.TodayStats.RadialViewModel.Data.Any())
                {
                    highScoreViewModel.TodayStats.RadialViewModel.Data.Add(new RadialGaugeModel { Name = "", Count = 0 });
                }

                if (!highScoreViewModel.AllTimeStats.RadialViewModel.Data.Any())
                {
                    highScoreViewModel.AllTimeStats.RadialViewModel.Data.Add(new RadialGaugeModel { Name = "", Count = 0 });
                }
            }
            catch (Exception ex)
            {
            }

            return highScoreViewModel;
        }
    }
}
