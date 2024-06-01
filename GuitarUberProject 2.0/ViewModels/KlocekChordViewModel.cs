using GitarUberProject.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace GitarUberProject.ViewModels
{
    public class KlocekChordViewModel : INotifyPropertyChanged
    {
        public static int RowsCount { get; } = 6;
        public static double ItemHeight { get; } = 20;
        public static double ItemWidth { get; } = 25;

        private KlocekChordModel draggedItem;
        private List<KlocekChordModel> ctrlSelection = new List<KlocekChordModel>();
        private List<KlocekChordModel> borderSelection = new List<KlocekChordModel>();
        public ObservableCollection<KlocekChordModel> Klocki { get; set; }
        public double Bpm { get; set; }
        public int CanvasWidth { get; set; }

        public KlocekChordViewModel()
        {
            Klocki = new ObservableCollection<KlocekChordModel>()
            {
                //new KlocekChordModel("C", (SolidColorBrush)(new BrushConverter().ConvertFrom("#34A285F0"))),

                //new KlocekChordModel("C", 2, 5, 2),
                //new KlocekChordModel("E", 3, 5, 2),
            };

            DraggedItem = new KlocekChordModel("C", 1, 2, 3);

            RenumerateItems();

            //Klocki[2].YPos = 20;

            //Klocki[4].KlocekOpacity = 0.3;

            DraggedItem.KlocekOpacity = 0.3;
        }

        [JsonIgnore]
        public KlocekChordModel DraggedItem
        {
            get
            {
                return draggedItem;
            }
            set
            {
                draggedItem = value;
                OnPropertyChanged("DraggedItem");
            }
        }

        [JsonIgnore]
        public List<KlocekChordModel> SelectedItems
        {
            get => CtrlSelection.Concat(BorderSelection).ToList();
        }

        [JsonIgnore]
        public List<KlocekChordModel> CtrlSelection
        {
            get => ctrlSelection;
            set
            {
                ctrlSelection = value;
            }
        }

        [JsonIgnore]
        public List<KlocekChordModel> BorderSelection
        {
            get => borderSelection;
            set
            {
                borderSelection = value;
            }
        }

        public void SetKlocekDragged(KlocekChordModel model, Point mousePos)
        {
            if (SelectedItems.Any())
            {
                foreach (var item in SelectedItems)
                {
                    item.KlocekOpacity = 0.3;
                    item.ZIndex = 20;
                    item.DeltaMouse = new Point(mousePos.X - item.XPos, mousePos.Y - item.YPos);
                }
            }
            else
            {
                DraggedItem = model;
                DraggedItem.KlocekOpacity = 0.3;
                DraggedItem.ZIndex = 20;
                DraggedItem.DeltaMouse = new Point(mousePos.X - DraggedItem.XPos, mousePos.Y - DraggedItem.YPos);
            }
        }

        public List<string> GetAllNoteNamesFromKlocki()
        {
            List<string> res = new List<string>();

            var NotesKlocki = Klocki.Where(a => a.IsChord == false).ToList();
            var ChordsKlocki = Klocki.Where(a => a.IsChord).ToList();

            var namesFromNotesKlocki = NotesKlocki.Select(a => a.Name).ToList();
            res.AddRange(namesFromNotesKlocki);

            var namesFromChordsKlocki = ChordsKlocki.SelectMany(a => a.NotesInChord)
                                                    .Select(b => b.Name)
                                                    .ToList();
            res.AddRange(namesFromChordsKlocki);

            var groupedByNoteNames = res.GroupBy(a => a)
                                        .Select(c => new { Note = c.Key, Count = c.Count() })
                                        .OrderByDescending(b => b.Count)
                                        .ToList();

            res = res.Distinct().ToList();

            return res;
        }

        public void LeaveSelectedItems()
        {
            if (SelectedItems.Any())
            {
                foreach (var item in SelectedItems)
                {
                    item.KlocekOpacity = 1;
                    item.ZIndex = 1;
                }

                CtrlSelection.Clear();
                BorderSelection.Clear();
            }
        }

        public void LeaveKlocekDragged()
        {
            if (DraggedItem != null)
            {
                DraggedItem.KlocekOpacity = 1;
                DraggedItem.ZIndex = 1;
                DraggedItem = null;
            }
        }

        internal void KlocekDragging(Point mousePos, double beatsColumnWidth, bool alignToGrid)
        {
            if (SelectedItems.Any())
            {
                foreach (var item in SelectedItems)
                {
                    double translatedMousePos = mousePos.X - item.DeltaMouse.X;

                    if (alignToGrid)
                    {
                        double rest = translatedMousePos % beatsColumnWidth;
                        int fullBeatsColumn = (int)translatedMousePos / (int)beatsColumnWidth;

                        item.XPos = fullBeatsColumn * beatsColumnWidth;
                        if (rest > (beatsColumnWidth / 2))
                        {
                            item.XPos += beatsColumnWidth;
                        }
                    }
                    else
                    {
                        item.XPos = translatedMousePos;
                    }

                    //CalculateStruna(item, mousePos.Y - item.DeltaMouse.Y + (ItemHeight / 2));
                }
            }
            else
            {
                double translatedMousePos = mousePos.X - DraggedItem.DeltaMouse.X;

                if (alignToGrid)
                {
                    double rest = translatedMousePos % beatsColumnWidth;
                    int fullBeatsColumn = (int)translatedMousePos / (int)beatsColumnWidth;

                    DraggedItem.XPos = fullBeatsColumn * beatsColumnWidth;
                    if (rest > (beatsColumnWidth / 2))
                    {
                        DraggedItem.XPos += beatsColumnWidth;
                    }
                }
                else
                {
                    DraggedItem.XPos = translatedMousePos;
                }

                //CalculateStruna(DraggedItem, mousePos.Y - DraggedItem.DeltaMouse.Y + (ItemHeight / 2));
            }
        }

        public void CalculateStruna(KlocekChordModel item, double point)
        {
            double res = -1;
            for (int i = 0; i < RowsCount; i++)
            {
                if ((i + 1) * ItemHeight > point)
                {
                    res = (i + 1) * ItemHeight - ItemHeight;
                    break;
                }
            }

            if (res == -1) res = RowsCount * ItemHeight - ItemHeight;

            item.YPos = res;
        }

        public void CalculateSelection(Rect selection, int selectedChannelIdx)
        {
            BorderSelection.Clear();
            foreach (var item in Klocki)
            {
                var klocekRect = new Rect(item.XPos, item.YPos, ItemWidth, ItemHeight);

                var res = Rect.Intersect(selection, klocekRect);

                if (res != Rect.Empty && !CtrlSelection.Contains(item) && item.ChannelNr == selectedChannelIdx)
                {
                    BorderSelection.Add(item);
                }
                else
                {
                }
            }

            foreach (var item in Klocki.Except(CtrlSelection))
            {
                item.KlocekOpacity = 1;
                item.ZIndex = 1;
            }

            foreach (var item in BorderSelection)
            {
                item.KlocekOpacity = 0.3;
                item.ZIndex = 20;
            }
        }

        //public void CalculatePosition(Rect draggedKlocek)
        //{
        //    var leftItem = Klocki.LastOrDefault(a => a.XPos < draggedKlocek.Left);
        //    int idx = Klocki.IndexOf(leftItem);
        //    if (idx == -1) idx = 0;

        //    int idxDraggedItem = Klocki.IndexOf(DraggedItem);
        //    bool notChange = false;

        //    if (idx < Klocki.Count - 1)
        //    {
        //        var rightItem = Klocki[idx + 1];

        //    }

        //    if (idxDraggedItem != -1)
        //    {
        //        if (idx == idxDraggedItem) notChange = true;
        //        else Klocki.Remove(Klocki[idxDraggedItem]);
        //    }

        //    if (!notChange)
        //    {
        //        Klocki.Insert(idx, DraggedItem);
        //    }

        //    RenumerateItems();
        //}

        private void RenumerateItems()
        {
            double xPosCounter = 0;
            foreach (var item in Klocki)
            {
                item.XPos = xPosCounter += 50;
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