using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace VAProject.Core.Utils
{
    public static class AsyncArrayExtensions
    {
        public static async Task<List<T>> FilterAsync<T> (
            this IEnumerable<T> source, 
            Func<T, CancellationToken, Task<bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            List<T> result = new List<T>();

            foreach (T item in source)
            {
                cancellationToken.ThrowIfCancellationRequested();

                bool isMatch = await predicate(item, cancellationToken);

                if (isMatch)
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
