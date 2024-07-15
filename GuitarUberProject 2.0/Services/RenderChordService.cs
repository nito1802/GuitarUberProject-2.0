using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GitarUberProject.Services
{
    public static class RenderChordService
    {
        public static void WriteToPngReadChord(UIElement element, string filename, double ReadChordWidth, double ReadChordHeight)
        {
            //var rect = new Rect(new Size(145* App.CustomScaleX, 30* App.CustomScaleY));
            var rect = new Rect(new Size(ReadChordWidth, ReadChordHeight));
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            var bitmap = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            try
            {
                using (var file = File.OpenWrite(filename))
                {
                    encoder.Save(file);
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        public static void WriteToPng(UIElement element, string filename, double NormalChordWidth, double NormalChordHeight)
        {
            //var rect = new Rect(new Size(130* App.CustomScaleX, 70* App.CustomScaleY));
            var rect = new Rect(new Size(NormalChordWidth, NormalChordHeight));

            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(element), null, rect);
            }

            var bitmap = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height, 96, 96, PixelFormats.Default);
            bitmap.Render(visual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            try
            {
                using (var file = File.OpenWrite(filename))
                {
                    encoder.Save(file);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}