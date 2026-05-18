using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VAProject.Core.Utils
{
    public class LargeDataProcessor
    {
        public async IAsyncEnumerable<string> ReadFileInChunksAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
