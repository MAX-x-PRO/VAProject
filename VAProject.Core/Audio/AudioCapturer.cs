using NAudio.Wave;
using Vosk;
using VAProject.Core.Logger;
using System.IO;
using System.Media;

namespace VAProject.Core.Audio
{
    public class AudioCapturer
    {
        #region 1. Events and dependency injection
        public event Action<byte[]> OnCommandAudioCaptured;
        public event Action<MicStates> OnMicStateChanged;
        #endregion

        #region 2. Vosk
        private readonly Model _voskModel;
        private readonly VoskRecognizer _wakeRecognizer;
        private readonly string _wakeWord = "alex";
        #endregion

        #region 3. Audio
        private WaveInEvent _waveIn;
        private readonly string _activationSoundPath;
        #endregion

        #region 4. Dynamic data
        private MemoryStream _cmdAudioStream;
        private bool _isCommandRec = false;
        private int _silenceTimerMs = 0;
        private int _totalRecordMs = 0;
        #endregion

        #region 5. VAD configuration
        private readonly int _maxSilenceMs = 1000;
        private readonly int _maxCommandLengthMs = 10000;
        private readonly float _silenceThreshold = 0.03f;
        #endregion

        public AudioCapturer()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;

            string modelPath = Path.Combine(exeDir, "Models", "vosk-model-en-us-0.22-lgraph");
            _activationSoundPath = Path.Combine(exeDir, "Audio", "Files", "activation.wav");

            if (!Directory.Exists(modelPath))
                LogManager.Log($"Vosk model directory not found at: {modelPath}", LogLevel.Error);

            Vosk.Vosk.SetLogLevel(-1);
            _voskModel = new Model(modelPath);

            _wakeRecognizer = new VoskRecognizer(_voskModel, 16000.0f);
        }

        public void StartListening()
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 16, 1),
            };

            _waveIn.DataAvailable += OnDataAvailable;

            LogManager.Log("Live", LogLevel.Debug);
            _waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (_isCommandRec)
            {
                _cmdAudioStream.Write(e.Buffer, 0, e.BytesRecorded);

                int chunkMs = e.BytesRecorded / 32;
                _totalRecordMs += chunkMs;

                float maxVolume = 0;
                for (int i = 0; i < e.BytesRecorded; i += 2)
                {
                    short sample = BitConverter.ToInt16(e.Buffer, i);
                    float sample32 = Math.Abs(sample / 32768f);
                    if (sample32 > maxVolume)
                        maxVolume = sample32;
                }

                if (maxVolume < _silenceThreshold)
                    _silenceTimerMs += chunkMs;
                else
                    _silenceTimerMs = 0;

                if (_silenceTimerMs > _maxSilenceMs || _totalRecordMs > _maxCommandLengthMs)
                {
                    _isCommandRec = false;
                    LogManager.Log($"Command recorded: {_totalRecordMs} ms", LogLevel.Debug);
                    OnMicStateChanged?.Invoke(MicStates.Inactive);
                    ProcessCommandAudio();
                }
                return;
            }

            _wakeRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded);

            string partialResult = _wakeRecognizer.PartialResult();
            if (partialResult.Contains(_wakeWord))
            {
                LogManager.Log("Wake word detected", LogLevel.Debug);

                PlayActivationSound();

                _isCommandRec = true;
                _cmdAudioStream = new MemoryStream();

                _silenceTimerMs = 0;
                _totalRecordMs = 0;

                OnMicStateChanged?.Invoke(MicStates.Active);

                _wakeRecognizer.Reset();
            }
        }

        private void ProcessCommandAudio()
        {
            _cmdAudioStream.Position = 0;
            byte[] audioData = _cmdAudioStream.ToArray();
            _cmdAudioStream.Dispose();

            LogManager.Log($"Command audio length: {audioData.Length} bytes", LogLevel.Debug);

            OnCommandAudioCaptured?.Invoke(audioData);
        }

        public void StopListening()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _wakeRecognizer?.Dispose();
        }

        private void PlayActivationSound()
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer(_activationSoundPath))
                {
                    player.Play();
                }

            }
            catch (Exception ex)
            {
                LogManager.Log($"Error playing beep sound: {ex.Message}", LogLevel.Warning);
            }
        }
    }
}
