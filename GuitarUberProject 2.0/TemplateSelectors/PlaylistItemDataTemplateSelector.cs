using GitarUberProject.Models;
using System.Windows;
using System.Windows.Controls;

namespace GitarUberProject.TemplateSelectors
{
    public class PlaylistItemDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is KlocekChordModel)
            {
                KlocekChordModel klocekChordModel = item as KlocekChordModel;

                if (klocekChordModel.IsChord)
                {
                    var template = element.FindResource("PlayListChordTemplate") as DataTemplate;
                    return template;
                }
                else
                {
                    var template = element.FindResource("PlayListNoteTemplate") as DataTemplate;
                    return template;
                }
            }

            return null;
        }
    }
}