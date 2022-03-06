using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TrulyRandom
{
    //Based on joaoportela's code
    /// <summary>
    /// Circular buffer optimized for frequent additions and removals of data
    /// </summary>
    internal class CircularBuffer<T> : IEnumerable<T>
    {
        private T[] buffer;

        /// <summary>
        /// Index of the first element in buffer
        /// </summary>
        private int start;

        /// <summary>
        /// Index after the last element in the buffer
        /// </summary>
        private int end;

        /// <summary>
        /// Buffer size
        /// </summary>
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class
        /// </summary>
        /// <param name='capacity'>Buffer capacity</param>
        public CircularBuffer(int capacity)
            : this(capacity, Array.Empty<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class
        /// </summary>
        /// <param name='capacity'> Buffer capacity</param>
        /// <param name='items'> Items to fill buffer with</param>
        public CircularBuffer(int capacity, T[] items)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(
                    "Circular buffer cannot have negative or zero capacity.", nameof(capacity));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (items.Length > capacity)
            {
                throw new ArgumentException(
                    "Too many items to fit circular buffer", nameof(items));
            }

            buffer = new T[capacity];

            Array.Copy(items, buffer, items.Length);
            count = items.Length;

            start = 0;
            end = count == capacity ? 0 : count;
        }

        /// <summary>
        /// Maximum capacity of the buffer. Elements pushed into the buffer after
        /// maximum capacity is reached (IsFull = true), will remove an element
        /// </summary>
        public int Capacity
        {
            get => buffer.Length;
            set
            {
                T[] data = ReadAll();
                buffer = new T[value];
                Clear();
                Write(data);
            }
        }

        /// <summary>
        ///Shows whether buffer is full
        /// </summary>
        public bool IsFull => Count == Capacity;

        /// <summary>
        /// Shows whether buffer is empty
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// The number of elements in the buffer
        /// </summary>
        public int Count => count;

        /// <summary>
        /// First element of the buffer
        /// </summary>
        /// <returns>The value of the first element of the buffer</returns>
        T First()
        {
            ThrowIfEmpty();
            return buffer[start];
        }

        /// <summary>
        /// Index access to elements in buffer
        /// </summary>
        /// <param name="index">Index of element to access</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when index is outside of [0; Count] interval.</exception>
        public T this[int index]
        {
            get
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                }
                if (index >= count)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, count));
                }
                int actualIndex = InternalIndex(index);
                return buffer[actualIndex];
            }
            set
            {
                if (IsEmpty)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer is empty", index));
                }
                if (index >= count)
                {
                    throw new IndexOutOfRangeException(string.Format("Cannot access index {0}. Buffer size is {1}", index, count));
                }
                int actualIndex = InternalIndex(index);
                buffer[actualIndex] = value;
            }
        }

        /// <summary>
        /// Adds a new element to the end of the buffer
        /// When the buffer is full, the first element will be removed to allow for new element to fit
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Write(T item)
        {
            if (IsFull)
            {
                buffer[end] = item;
                Increment(ref end);
                start = end;
            }
            else
            {
                buffer[end] = item;
                Increment(ref end);
                ++count;
            }
        }

        /// <summary>
        /// Adds several elements to the end of the buffer
        /// When the buffer is full, first elements will be removed to allow for new elements to fit
        /// </summary>
        /// <param name="items">Items to add</param>
        public void Write(IEnumerable<T> items)
        {
            int actualCount = Math.Min(items.Count(), Capacity);

            if (actualCount == 0)
            {
                return;
            }

            if (actualCount < items.Count())
            {
                items = items.Take(actualCount);
            }

            int elementsAtTheEnd = Math.Min(actualCount, Capacity - end);
            int elementsAtTheStart = actualCount - elementsAtTheEnd;

            Array.Copy(items.Take(elementsAtTheEnd).ToArray(), 0, buffer, end, elementsAtTheEnd);
            if (elementsAtTheStart != 0)
            {
                Array.Copy(items.TakeLast(elementsAtTheStart).ToArray(), 0, buffer, 0, elementsAtTheStart);
            }

            end = (end + actualCount) % Capacity;
            start = (start + Math.Max(0, count + actualCount - Capacity)) % Capacity;
            count = Math.Min(count + actualCount, Capacity);
        }

        /// <summary>
        /// Removes first element of the buffer
        /// </summary>
        void RemoveFirst()
        {
            ThrowIfEmpty("Cannot take elements from an empty buffer.");
            Increment(ref start);
            --count;
        }

        /// <summary>
        /// Reads and removes first element of the buffer
        /// </summary>
        public T Read()
        {
            T result = First();
            RemoveFirst();
            return result;
        }

        /// <summary>
        /// Reads and removes first <code>count</code> elements of the buffer
        /// </summary>
        /// <param name="count">Count of elements to be removed</param>
        public T[] Read(int count)
        {
            int actualCount = Math.Min(count, this.count);
            if (actualCount == 0)
            {
                return Array.Empty<T>();
            }
            T[] result = new T[actualCount];

            int elementsAtTheEnd = Math.Min(actualCount, Capacity - start);
            int elementsAtTheStart = actualCount - elementsAtTheEnd;

            Array.Copy(buffer, start, result, 0, elementsAtTheEnd);

            if (elementsAtTheStart != 0)
            {
                Array.Copy(buffer, 0, result, elementsAtTheEnd, elementsAtTheStart);
            }

            start = (start + actualCount) % Capacity;
            this.count -= actualCount;

            return result;
        }

        /// <summary>
        /// Reads and removes all elements of the buffer
        /// </summary>
        public T[] ReadAll()
        {
            return Read(Count);
        }

        /// <summary>
        /// Clears the contents of the array
        /// </summary>
        public void Clear()
        {
            start = 0;
            end = 0;
            count = 0;
        }

        /// <summary>
        /// Copies the buffer contents to an array
        /// </summary>
        /// <returns>A new array with a copy of the buffer contents.</returns>
        public T[] ToArray()
        {
            T[] newArray = new T[Count];
            int newArrayOffset = 0;
            IList<ArraySegment<T>> segments = ToArraySegments();
            foreach (ArraySegment<T> segment in segments)
            {
                Array.Copy(segment.Array, segment.Offset, newArray, newArrayOffset, segment.Count);
                newArrayOffset += segment.Count;
            }
            return newArray;
        }

        /// <summary>
        /// Get the contents of the buffer as 2 ArraySegments.
        /// Respects the logical contents of the buffer, where
        /// each segment and items in each segment are ordered
        /// according to insertion.
        ///
        /// Fast: does not copy the array elements.
        /// Useful for methods like <c>Send(IList&lt;ArraySegment&lt;Byte&gt;&gt;)</c>.
        /// 
        /// <remarks>Segments may be empty.</remarks>
        /// </summary>
        /// <returns>An IList with 2 segments corresponding to the buffer content.</returns>
        public IList<ArraySegment<T>> ToArraySegments()
        {
            return new[] { ArrayOne(), ArrayTwo() };
        }

        #region IEnumerable<T> implementation
        /// <summary>
        /// Returns an enumerator that iterates through this buffer.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate this collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            IList<ArraySegment<T>> segments = ToArraySegments();
            foreach (ArraySegment<T> segment in segments)
            {
                for (int i = 0; i < segment.Count; i++)
                {
                    yield return segment.Array[segment.Offset + i];
                }
            }
        }
        #endregion
        #region IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        private void ThrowIfEmpty(string message = "Cannot access an empty buffer.")
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Increments the provided index variable by one, wrapping
        /// around if necessary.
        /// </summary>
        /// <param name="index"></param>
        private void Increment(ref int index)
        {
            if (++index == Capacity)
            {
                index = 0;
            }
        }

        /// <summary>
        /// Converts the index in the argument to an index in <code>_buffer</code>
        /// </summary>
        /// <returns>
        /// The transformed index.
        /// </returns>
        /// <param name='index'>
        /// External index.
        /// </param>
        private int InternalIndex(int index)
        {
            return start + (index < (Capacity - start) ? index : index - Capacity);
        }

        // doing ArrayOne and ArrayTwo methods returning ArraySegment<T> as seen here: 
        // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1957cccdcb0c4ef7d80a34a990065818d
        // http://www.boost.org/doc/libs/1_37_0/libs/circular_buffer/doc/circular_buffer.html#classboost_1_1circular__buffer_1f5081a54afbc2dfc1a7fb20329df7d5b
        // should help a lot with the code.

        #region Array items easy access.
        // The array is composed by at most two non-contiguous segments, 
        // the next two methods allow easy access to those.

        private ArraySegment<T> ArrayOne()
        {
            if (IsEmpty)
            {
                return new ArraySegment<T>(Array.Empty<T>());
            }
            else if (start < end)
            {
                return new ArraySegment<T>(buffer, start, end - start);
            }
            else
            {
                return new ArraySegment<T>(buffer, start, buffer.Length - start);
            }
        }

        private ArraySegment<T> ArrayTwo()
        {
            if (IsEmpty)
            {
                return new ArraySegment<T>(Array.Empty<T>());
            }
            else if (start < end)
            {
                return new ArraySegment<T>(buffer, end, 0);
            }
            else
            {
                return new ArraySegment<T>(buffer, 0, end);
            }
        }
        #endregion
    }
}
