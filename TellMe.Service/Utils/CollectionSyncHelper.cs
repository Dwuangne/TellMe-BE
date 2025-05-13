using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Service.Utils
{
    public static class CollectionSyncHelper
    {
        public class SyncResult<T>
        {
            public List<T> ToAdd { get; set; } = new();
            public List<T> ToUpdate { get; set; } = new();
            public List<T> ToDelete { get; set; } = new();
        }

        public static SyncResult<T> SyncCollections<T, TKey>(
            IEnumerable<T> existingItems,
            IEnumerable<T> incomingItems,
            Func<T, TKey> keySelector)
            where TKey : IEquatable<TKey>
        {
            var existingDict = existingItems.ToDictionary(keySelector);
            var incomingDict = incomingItems.ToDictionary(keySelector);

            var toAdd = incomingDict.Where(kvp => !existingDict.ContainsKey(kvp.Key)).Select(kvp => kvp.Value).ToList();
            var toDelete = existingDict.Where(kvp => !incomingDict.ContainsKey(kvp.Key)).Select(kvp => kvp.Value).ToList();
            var toUpdate = incomingDict.Where(kvp => existingDict.ContainsKey(kvp.Key)).Select(kvp => kvp.Value).ToList();

            return new SyncResult<T>
            {
                ToAdd = toAdd,
                ToDelete = toDelete,
                ToUpdate = toUpdate
            };
        }
    }

}
