using NAudio.Wave;
using Pv;
using System.IO;
using System.Runtime.InteropServices;
using VAProject.Logger;

namespace VAProject.Audio
{
    internal class AudioCapturer
    {
        private readonly ILogger _logger;
        private WaveInEvent _waveIn;
        private short[] _frameBuffer;

        private Porcupine _porcupine;
        private readonly string accessKey = Environment.GetEnvironmentVariable("PORCUPINE_ACCESS_KEY", EnvironmentVariableTarget.User);
        private readonly string keywordPath = "Models\\wakeWords\\Alex_en_windows_v4_0_0.ppn";

        private bool isCommandRec = false;
        private MemoryStream cmdAudioStream;
        private readonly int cmdByteLength = 96000;

        public event Action<byte[]> OnCommandAudioCaptured;

        public AudioCapturer(ILogger logger)
        {
            _logger = logger;
        }

        public void StartListening()
        {
            _porcupine = Porcupine.FromKeywordPaths(accessKey, new List<string> {keywordPath});
            _frameBuffer = new short[_porcupine.FrameLength];
            _waveIn = new WaveInEvent    
            {
                WaveFormat = new WaveFormat(16000, 16, 1),
                BufferMilliseconds = 32
            };

            _waveIn.DataAvailable += OnDataAvailable;

            _logger.Log("Live", LogLevel.Debug);
            _waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            ReadOnlySpan<byte> byteSpan = e.Buffer.AsSpan(0, e.BytesRecorded);

            if (isCommandRec)
            {
                cmdAudioStream.Write(e.Buffer, 0, e.BytesRecorded);

                if (cmdAudioStream.Length >= cmdByteLength)
                {
                    isCommandRec = false;
                    _logger.Log("Command recorded", LogLevel.Debug);
                    ProcessCommandAudio();
                }
            }

            ReadOnlySpan<short> shortSpan = MemoryMarshal.Cast<byte, short>(byteSpan);

            int frameLength = _porcupine.FrameLength;

            for (int i = 0; i + frameLength <= shortSpan.Length; i += frameLength)
            {
                ReadOnlySpan<short> chunk = shortSpan.Slice(i, frameLength);
                chunk.CopyTo(_frameBuffer);

                int keywordIndex = _porcupine.Process(_frameBuffer);

                if (keywordIndex >= 0)
                {
                    _logger.Log("Ping", LogLevel.Debug);
                    isCommandRec = true;
                    cmdAudioStream = new MemoryStream(cmdByteLength);
                }
            }
        }

        private void ProcessCommandAudio()
        {
            cmdAudioStream.Position = 0;
            byte[] audioData = cmdAudioStream.ToArray();

            _logger.Log($"Command audio length: {audioData.Length} bytes", LogLevel.Debug);

            OnCommandAudioCaptured(audioData);
        }

        public void StopListening()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _porcupine?.Dispose();
        }
    }
}
