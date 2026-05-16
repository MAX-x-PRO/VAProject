using System.IO;
using System.Security.Cryptography;
using System.Text;
using VAProject.Core.Logger;

namespace VAProject.Core.Audio
{
    internal class AudioCacher
    {
        private readonly TextToSpeech _tts;

        private readonly string _cacheDirectory;

        private const int CacheTtlDays = 2;

        public AudioCacher(TextToSpeech tts)
        {
            _tts = tts;
            _cacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AudioCache");

            if (!Directory.Exists(_cacheDirectory))
                Directory.CreateDirectory(_cacheDirectory);

            Task.Run(() => CleanupOldCacheFiles(CacheTtlDays));
        }

        public string GetPhrasePath(string text)
        {
            string hash = GetMd5Hash(text);
            string phrasePath = Path.Combine(_cacheDirectory, hash + ".wav");

            if (File.Exists(phrasePath))
            {
                File.SetLastAccessTime(phrasePath, DateTime.Now);
                return phrasePath;
            }

            return _tts.CreatePhraseFile(text, phrasePath);
        }

        private void CleanupOldCacheFiles(int daysOld)
        {
            try
            {
                var directory = new DirectoryInfo(_cacheDirectory);
                var cutoffTime = DateTime.Now.AddDays(-daysOld);

                foreach (var file in directory.GetFiles("*.wav"))
                {
                    if (file.LastAccessTime < cutoffTime)
                    {
                        try
                        {
                            file.Delete();
                            LogManager.Log($"[Cache] Deleted old file: {file.Name}");
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log($"[Cache] Cache clearing error: {ex.Message}");
            }
        }

        private string GetMd5Hash(string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}