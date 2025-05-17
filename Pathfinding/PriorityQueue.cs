using System;
using System.Collections.Generic;

namespace PathFindingClassDiagram.PathFinding
{
    /// <summary>
    /// A priority queue implementation for efficiently ordering items by priority.
    /// Used specifically for Dijkstra's algorithm.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the queue.</typeparam>
    public class PriorityQueue<T>
    {
        private readonly List<(T Item, double Priority)> _elements;

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class.
        /// </summary>
        public PriorityQueue()
        {
            _elements = new List<(T, double)>();
        }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Adds an item to the queue with the specified priority.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="priority">The priority of the item (lower values have higher priority).</param>
        public void Enqueue(T item, double priority)
        {
            // Binary search to find insertion point
            int left = 0;
            int right = _elements.Count - 1;
            int insertIndex = _elements.Count;

            while (left <= right)
            {
                int middle = left + (right - left) / 2;
                if (_elements[middle].Priority > priority)
                {
                    insertIndex = middle;
                    right = middle - 1;
                }
                else
                {
                    left = middle + 1;
                }
            }

            _elements.Insert(insertIndex, (item, priority));
        }

        /// <summary>
        /// Removes and returns the item with the highest priority (lowest priority value).
        /// </summary>
        /// <returns>The highest priority item.</returns>
        public T Dequeue()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            var item = _elements[0].Item;
            _elements.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Returns the item with the highest priority without removing it.
        /// </summary>
        /// <returns>The highest priority item.</returns>
        public T Peek()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return _elements[0].Item;
        }

        /// <summary>
        /// Checks if the queue contains a specific item.
        /// </summary>
        /// <param name="item">The item to check for.</param>
        /// <returns>True if the item is in the queue, false otherwise.</returns>
        public bool Contains(T item)
        {
            return _elements.Exists(e => EqualityComparer<T>.Default.Equals(e.Item, item));
        }

        /// <summary>
        /// Updates the priority of an existing item in the queue.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="newPriority">The new priority value.</param>
        public void UpdatePriority(T item, double newPriority)
        {
            int index = _elements.FindIndex(e => EqualityComparer<T>.Default.Equals(e.Item, item));
            if (index != -1)
            {
                _elements.RemoveAt(index);
                Enqueue(item, newPriority);
            }
        }

        /// <summary>
        /// Removes all items from the queue.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }
    }
}