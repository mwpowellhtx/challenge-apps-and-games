using System.Collections;
using System.Collections.Generic;

namespace Dominoes
{
    /// <summary>
    /// Represents a sparsely populated <see cref="Domino"/> dictionary. Users are free to get and
    /// set virtually any value at any key. Default return values for non-existent keys are null.
    /// </summary>
    public class DominoDictionary : IDictionary<long, Domino>
    {
        private readonly IDictionary<long, Domino> _sparse
            = new Dictionary<long, Domino>();

        public void Add(long key, Domino value)
        {
            _sparse.Add(key, value);
        }

        public bool ContainsKey(long key)
        {
            return _sparse.ContainsKey(key);
        }

        public ICollection<long> Keys
        {
            get { return _sparse.Keys; }
        }

        public bool Remove(long key)
        {
            return _sparse.Remove(key);
        }

        public bool TryGetValue(long key, out Domino value)
        {
            value = _sparse.ContainsKey(key) ? _sparse[key] : null;
            return value != null;
        }

        public ICollection<Domino> Values
        {
            get { return _sparse.Values; }
        }

        public Domino this[long key]
        {
            get { return _sparse.ContainsKey(key) ? _sparse[key] : null; }
            set { _sparse[key] = value; }
        }

        public void Add(KeyValuePair<long, Domino> item)
        {
            _sparse.Add(item);
        }

        public void Clear()
        {
            _sparse.Clear();
        }

        public bool Contains(KeyValuePair<long, Domino> item)
        {
            return _sparse.Contains(item);
        }

        public void CopyTo(KeyValuePair<long, Domino>[] array, int arrayIndex)
        {
            _sparse.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _sparse.Count; }
        }

        public bool IsReadOnly
        {
            get { return _sparse.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<long, Domino> item)
        {
            return _sparse.Remove(item);
        }

        public IEnumerator<KeyValuePair<long, Domino>> GetEnumerator()
        {
            return _sparse.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
