using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using InitiativeTracker.Data.Util;
using InitiativeTracker.Properties;

namespace InitiativeTracker.Data.Stack
{
    /// <summary>
    /// This is the main class responsible for holding, sorting, removing, and adding entities.
    /// It extends IList, which enables it to be used in foreach loops, among other things. It
    /// also extends INotifyCollectionChanged which is similar to INotifyPropertyChanged, but
    /// for a custom collection.
    /// </summary>
    public class SortedEntityList : Notifier, ICollection, IList<Entity>, INotifyCollectionChanged
    {
        /*
         * backing list used to hold all the entities. couldn't use SortedSet because i want
         * indexing capabilities, and i don't want the overhead associated with having to convert
         * from a Set to a List every time i want to grab the entity at index X.
         */
        List<Entity> _entities;

        bool _autoResolveTie;

        public Entity this[int index]
        {
            get { return _entities[index]; }
            set
            {
                //if you're setting an entity's placement in this way, it should be position-locked
                value.PositionLock = true;

                Entity oldValue = _entities[index];
                _entities[index] = value;

                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index));
            }
        }

        public int Count => _entities.Count;

        public bool IsReadOnly => false;

        public object SyncRoot => ((ICollection)_entities).SyncRoot;

        public bool IsSynchronized => ((ICollection)_entities).IsSynchronized;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /*
         * when an item (or items) are/is added to the stack, this dictionary
         * will hold information about the indices at which the change(s) occured.
         * CollectionChangedEventArgs only conveys the starting index of a given
         * operation so it isn't really optimized.
         */
        public Dictionary<Entity, int> ChangeIndices { get; private set; } 

        public SortedEntityList()
        {
            _entities = new List<Entity>();
            _autoResolveTie = Settings.Default.AutoResolveTie;
            SettingsManager.OnSettingsUpdate += OnSettingsUpdate;
            ChangeIndices = new Dictionary<Entity, int>();
        }

        void OnSettingsUpdate(object sender, SettingsUpdatedEventArgs e)
        {
            if (e.Name.Equals("AutoResolveTie"))
            {
                _autoResolveTie = Settings.Default.AutoResolveTie;
            }
        }

        public void Add(Entity item)
        {
            //list is empty; no need to check anything
            if (_entities.Count == 0)
            {
                _entities.Add(item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
                Notify("Count");
                return;
            } 
            else
            {
                //iterate through _entities. this assumes index 0 = top of stack.
                for (int i = 0; i < _entities.Count; i++)
                {
                    Entity listEntity = _entities[i];
                    int comparisonResult = item.CompareTo(listEntity);

                    //pass by position-locked entities and entities whose initiative is higher
                    if (listEntity.PositionLock || comparisonResult == -1) continue;
                    else if (comparisonResult == 1) //hooray, we found exactly where it needs to go
                    {
                        _entities.Insert(i, item);
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, i));
                        Notify("Count");
                        return;
                    }
                    else
                    {
                        //there's a tie
                        if (_autoResolveTie)
                        {
                            //first, look ahead and find out how many ties there are. we know there's at least one so we start with an offset of 1
                            int ties;
                            for (ties = 1; ties + i < _entities.Count; ties++)
                            {
                                int testIndex = ties + i;
                                Entity tieTest = _entities[testIndex];

                                //ignore items that are position locked (moved by the DM against initiative order).
                                if (tieTest.PositionLock) continue;
                                else if (item.CompareTo(tieTest) != 0) break;
                            }

                            //get a random number between i (inclusive) and i + ties + 1 (exclusive)
                            int randomInsertionIndex = RngProvider.RNG.Next(i, i + ties + 1);
                            _entities.Insert(randomInsertionIndex, item);
                            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, randomInsertionIndex));
                            Notify("Count");
                            return;
                        }
                        else
                        {
                            //TODO: involve the DM in tie resolution assuming the setting is enabled
                        }
                    }
                }

                //we went through the entire list, so it belongs at the bottom
                _entities.Add(item);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _entities.Count - 1));
                Notify("Count");
            }
        }

        public void AddRange(IEnumerable<Entity> entities)
        {
            foreach (Entity item in entities)
            {
                if (_entities.Count == 0)
                {
                    _entities.Add(item);
                    ChangeIndices.Add(item, 0);
                    return;
                }
                else
                {
                    for (int i = 0; i < _entities.Count; i++)
                    {
                        Entity listEntity = _entities[i];
                        int comparisonResult = item.CompareTo(listEntity);

                        if (listEntity.PositionLock || comparisonResult == -1) continue;
                        else if (comparisonResult == 1)
                        {
                            _entities.Insert(i, item);
                            ChangeIndices.Add(item, i);
                            return;
                        }
                        else
                        {
                            if (_autoResolveTie)
                            {
                                int ties;
                                for (ties = 1; ties + i < _entities.Count; ties++)
                                {
                                    int testIndex = ties + i;
                                    Entity tieTest = _entities[testIndex];

                                    if (tieTest.PositionLock) continue;
                                    else if (item.CompareTo(tieTest) != 0) break;
                                }

                                int randomInsertionIndex = RngProvider.RNG.Next(i, i + ties + 1);
                                _entities.Insert(randomInsertionIndex, item);
                                ChangeIndices.Add(item, randomInsertionIndex);
                                return;
                            }
                            else
                            {
                                //TODO: involve the DM in tie resolution assuming the setting is enabled
                            }
                        }
                    }

                    _entities.Add(item);
                    ChangeIndices.Add(item, _entities.Count - 1);
                }
            }
        }

        public void Clear()
        {
            _entities.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            Notify("Count");
        }

        public bool Contains(Entity item)
        {
            return _entities.Contains(item, ReferenceEqualityComparer<Entity>.Default);
        }

        public void CopyTo(Entity[] array, int arrayIndex)
        {
            _entities.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        /// <summary>
        /// Searches the list for the index of the provided entity. Compares
        /// object references by default.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Entity item)
        {
            IEqualityComparer<Entity> comparer = ReferenceEqualityComparer<Entity>.Default;

            for (int i = 0; i < _entities.Count; i++)
            {
                Entity entity = _entities[i];
                if (comparer.Equals(item, entity)) return i;
            }

            return -1;
        }

        /// <summary>
        /// Inserts an entity at a specific place in the list. Passing Count to
        /// index will add the entity to the end of the stack.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, Entity item)
        {
            item.PositionLock = true;

            if(index < _entities.Count) _entities.Insert(index, item);
            else if(index == _entities.Count) _entities.Add(item);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            Notify("Count");
        }

        public bool Remove(Entity item)
        {
            int location = IndexOf(item);

            if (location == -1) return false;
            else _entities.RemoveAt(location);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, location));
            Notify("Count");
            return true;
        }

        public void RemoveAt(int index)
        {
            Entity oldEntity = _entities[index];

            _entities.RemoveAt(index);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldEntity, index));
            Notify("Count");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        /// <summary>
        /// Returns true if the given index is within bounds, false otherwise.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CheckBounds(int index)
        {
            return index > -1 && index < _entities.Count;
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_entities).CopyTo(array, index);
        }
    }
}