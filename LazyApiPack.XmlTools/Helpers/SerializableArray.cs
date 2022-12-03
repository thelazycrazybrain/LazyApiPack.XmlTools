using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace LazyApiPack.XmlTools.Helpers {

    /// <summary>
    /// Provides a proxy class for an array that supports multiple object types and is being serialized or deserialized.
    /// </summary>
    /// <remarks>Supports multiple dimensions and multiple datatypes.</remarks>
    public class SerializableArray : IEnumerator {

        public IEnumerator<object?> GetEnumerator() {
            while (MoveNext()) {
                yield return Current;
            }

        }
        /// <summary>
        /// Contains the last array item.
        /// </summary>
        int[]? _lastArrayItem { get; set; }

        /// <summary>
        /// Contains the current array.
        /// </summary>
        Array _array;

        /// <summary>
        /// Represents the array item type.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        /// Creates an instance of the ArrayDescriptor.
        /// </summary>
        /// <param name="array">The target array.</param>
        public SerializableArray([NotNull] Array array) {
            _array = array;
            ItemType = _array.GetType().GetElementType() ?? throw new ArgumentException("Array type does not contain an element type.");
        }

        /// <summary>
        /// Contains the current item.
        /// </summary>
        public object? Current
        {
            get
            {
                if (CurrentIndex == null) return null;
                return _array.GetValue(CurrentIndex);
            }
            set
            {
                if (CurrentIndex == null) throw new InvalidOperationException("Enumerator is not initialized.");
                _array.SetValue(value, CurrentIndex);
            }
        }

        /// <summary>
        /// The current index as a ; separated string (Multiple array dimensions).
        /// </summary>
        /// <returns>The current index as a ; separated string (Multiple array dimensions).</returns>
        public string? CurrentIndexString
        {
            get
            {
                if (_lastArrayItem == null) return null;
                string result = "";
                for (int i = 0; i < _lastArrayItem.Length; i++) {
                    result += _lastArrayItem[i] + (i == _lastArrayItem.Length - 1 ? "" : ";");
                }
                return result;
            }
        }

        /// <summary>
        /// The current index as a int array (Multiple array dimensions).
        /// </summary>
        public int[] CurrentIndex { get => _lastArrayItem ?? throw new Exception("Iterator not ready."); }



        /// <summary>
        /// Moves the iterator to the next element.
        /// </summary>
        /// <returns>False, if the iterator reached the end of the array.</returns>
        public bool MoveNext() {
            if (_lastArrayItem == null) {
                _lastArrayItem = new int[_array.Rank];
                return true;
            }
            var currentRank = _lastArrayItem.Count() - 1;
            while (true) {
                var maxIndex = _array.GetLength(currentRank);
                if (_lastArrayItem[currentRank] < maxIndex - 1) {
                    _lastArrayItem[currentRank]++;
                    return true;
                } else {
                    if (currentRank > 0) {
                        _lastArrayItem[currentRank] = 0;
                        currentRank--;
                    } else {
                        _lastArrayItem = null; // Reset
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Resets the current iterator.
        /// </summary>
        public void Reset() {
            _lastArrayItem = null;
        }
    }
}