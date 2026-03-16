using NAudio.Wave;
using Pv;
using System;
using System.IO;
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

        private bool isCommandRec = false;
        private MemoryStream cmdAudioStream;
        private readonly int cmdByteLength = 96000;

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

            if (isCommandRec)
            {
                cmdAudioStream = new MemoryStream(cmdByteLength);
                cmdAudioStream.Write(e.Buffer, 0, cmdByteLength);

                if (cmdAudioStream.Length >= cmdByteLength)
                {
                    isCommandRec = false;
                    Console.WriteLine("Command recorded");
                    ProcessCommandAudio();
                }
            }

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
                    isCommandRec = true;
                }
            }
        }

        private void ProcessCommandAudio()
        {
            cmdAudioStream.Position = 0;
            byte[] audioData = cmdAudioStream.ToArray();

            // iMPLEMENT RECOGNOITION
             Console.WriteLine($"Command audio length: {audioData.Length} bytes");
        }

        public void StopListening()
        {
            waveIn?.StopRecording();
            waveIn?.Dispose();
            porcupine?.Dispose();
        }
    }
}
