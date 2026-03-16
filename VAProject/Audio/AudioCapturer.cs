using NAudio.Wave;
using Pv;
using System;
using System.Runtime.InteropServices;

namespace VAProj.Audio
{
    class AudioCapturer
    {
        private WaveInEvent waveIn;
        private short[] frameBuffer;
        private Porcupine porcupine;

        private readonly string accessKey = Environment.GetEnvironmentVariable("PORCUPINE_ACCESS_KEY", EnvironmentVariableTarget.User);
        private readonly string keywordPath = "C:\\Users\\MAX\\source\\repos\\VAProj\\VAProj\\VAProj\\Audio\\wakeWords\\Alex_en_windows_v4_0_0.ppn";

        public void StartListening()
        {
            porcupine = Porcupine.FromKeywordPaths(accessKey, new List<string> {keywordPath});
            frameBuffer = new short[porcupine.FrameLength];
            waveIn = new WaveInEvent    
            {
                WaveFormat = new WaveFormat(16000, 16, 1),
                BufferMilliseconds = 32
            };

            waveIn.DataAvailable += OnDataAvailable;

            Console.WriteLine("Live");
            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            ReadOnlySpan<byte> byteSpan = e.Buffer.AsSpan(0, e.BytesRecorded);
            ReadOnlySpan<short> shortSpan = MemoryMarshal.Cast<byte, short>(byteSpan);

            int frameLength = porcupine.FrameLength;

            for (int i = 0; i + frameLength <= shortSpan.Length; i += frameLength)
            {
                ReadOnlySpan<short> chunk = shortSpan.Slice(i, frameLength);
                chunk.CopyTo(frameBuffer);

                int keywordIndex = porcupine.Process(frameBuffer);

                if (keywordIndex >= 0)
                {
                    Console.WriteLine("Ping");

                }
            }
        }

        public void StopListening()
        {
            waveIn?.StopRecording();
            waveIn?.Dispose();
            porcupine?.Dispose();
        }
    }
}
