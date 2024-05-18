using NAudio.Lame;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject.Helperes
{
    public static class NAudioHelper
    {
        public static void ConvertToFileWavMp3(ISampleProvider sample, string wavPath, string mp3Path)
        {
            WaveFileWriter.CreateWaveFile16(wavPath, sample);

            byte[] data = File.ReadAllBytes(wavPath);

            var mp3Data = NAudioHelper.ConvertWavToMp3(data);

            File.WriteAllBytes(mp3Path, mp3Data);
        }

        private static byte[] ConvertWavToMp3(byte[] wavFile)
        {

            using (var retMs = new MemoryStream())
            using (var ms = new MemoryStream(wavFile))
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, 128))
            {
                rdr.CopyTo(wtr);
                return retMs.ToArray();
            }
        }

        public static void CreateWaveFile(string filename, IWaveProvider sourceProvider)
        {
            using (var writer = new WaveFileWriter(filename, sourceProvider.WaveFormat))
            {
                var buffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
                while (true)
                {
                    int bytesRead = sourceProvider.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // end of source provider
                        break;
                    }
                    // Write will throw exception if WAV file becomes too large
                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }

        public static void MyWriteWavFileToStream(Stream outStream, IWaveProvider sourceProvider)
        {
            using (var writer = new WaveFileWriter(new IgnoreDisposeStream(outStream), sourceProvider.WaveFormat))
            {
                var buffer = new byte[sourceProvider.WaveFormat.AverageBytesPerSecond * 4];
                while (true)
                {
                    var bytesRead = sourceProvider.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // end of source provider
                        outStream.Flush();
                        break;
                    }

                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
