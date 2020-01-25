using InitiativeTracker.Data.Util;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using InitiativeTracker.Data.Stack;
using System;

namespace InitiativeTracker.Data.Tray
{
    public class SortedTreeEntryList : Notifier, ICollection, IList<TreeEntry>, INotifyCollectionChanged
    {
        List<TreeEntry> _treeEntries;

        public TreeEntry this[int index]
        {
            get { return _treeEntries[index]; }
            set
            {
                TreeEntry oldValue = _treeEntries[index];
                _treeEntries[index] = value;

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }

        public int Count => _treeEntries.Count;

        public bool IsReadOnly => false;

        public object SyncRoot => ((ICollection)_treeEntries).SyncRoot;

        public bool IsSynchronized => ((ICollection)_treeEntries).IsSynchronized;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public SortedTreeEntryList()
        {
            _treeEntries = new List<TreeEntry>();
        }

        /// <summary>
        /// Uses basically the same sorting algorithm as EntityStack, but without tie resolution. New items just
        /// go on top.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TreeEntry item)
        {
            if (_treeEntries.Count == 0)
            {
                _treeEntries.Add(item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
                Notify("Count");
                return;
            }
            else
            {
                for (int i = 0; i < _treeEntries.Count; i++)
                {
                    TreeEntry listEntity = _treeEntries[i];

                    if (item.CompareTo(listEntity) == -1) continue;

                    _treeEntries.Insert(i, item);
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i));
                    Notify("Count");
                    return;
                }

                _treeEntries.Add(item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _treeEntries.Count - 1));
                Notify("Count");
            }
        }

        public void Clear()
        {
            _treeEntries.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            Notify("Count");
        }

        public bool Contains(TreeEntry item)
        {
            return _treeEntries.Contains(item, ReferenceEqualityComparer<TreeEntry>.Default);
        }

        public void CopyTo(TreeEntry[] array, int arrayIndex)
        {
            _treeEntries.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TreeEntry item)
        {
            IEqualityComparer<TreeEntry> comparer = ReferenceEqualityComparer<TreeEntry>.Default;

            for (int i = 0; i < _treeEntries.Count; i++)
            {
                TreeEntry entity = _treeEntries[i];
                if (comparer.Equals(item, entity)) return i;
            }

            return -1;
        }

        public void Insert(int index, TreeEntry item)
        {
            if (index < _treeEntries.Count) _treeEntries.Insert(index, item);
            else if (index == _treeEntries.Count) _treeEntries.Add(item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            Notify("Count");
        }

        public bool Remove(TreeEntry item)
        {
            int location = IndexOf(item);

            if (location == -1) return false;
            else _treeEntries.RemoveAt(location);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, location));
            Notify("Count");

            return true;
        }

        public void RemoveAt(int index)
        {
            TreeEntry oldEntity = _treeEntries[index];

            _treeEntries.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index, oldEntity));
            Notify("Count");
        }

        public IEnumerator<TreeEntry> GetEnumerator()
        {
            return _treeEntries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _treeEntries.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_treeEntries).CopyTo(array, index);
        }
    }
}