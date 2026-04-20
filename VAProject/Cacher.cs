using System.IO;
using System.Security.Cryptography;
using System.Text;
using VAProject.Audio;

namespace VAProject
{
    internal class Cacher
    {
        private readonly TextToSpeech _tts;

        private readonly Dictionary<string, string> _tags;
        private readonly string _cacheDirectory;

        public Cacher(TextToSpeech tts)
        {
            _tts = tts;

            _tags = new Dictionary<string, string>();
            _cacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AudioCache");

            if (!Directory.Exists(_cacheDirectory))
                Directory.CreateDirectory(_cacheDirectory);
        }

        public string GetPhrasePath(string text)
        {
            string hash = GetM5Hash(text);

            string phrasePath = Path.Combine(_cacheDirectory, hash + ".wav");
            if (_tags.ContainsKey(hash))
            {
                return _tags[hash];
            }

            _tags[hash] = phrasePath;
            return _tts.CreatePhraseFile(text, phrasePath);
        }

        private string GetM5Hash(string text)
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
