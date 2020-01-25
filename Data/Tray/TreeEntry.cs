using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Documents;
using InitiativeTracker.Data.Stack;
using InitiativeTracker.Data.Util;
using InitiativeTracker.Data.XML;

namespace InitiativeTracker.Data.Tray
{
    public enum EntryType
    {
        Entity,
        Generator,
        Folder
    }

    public delegate void OnDeletionEventHandler(object sender, EventArgs e);

    public class TreeEntry : Notifier, IComparable<TreeEntry>
    {
        //private fields
        EntryType _type;
        string _displayName;
        SortedTreeEntryList _contents;
        Entity _attachedEntity;
        TreeEntry _parent;

        /*
         * when pushing a TreeEntry, this holds a reference
         * to Parent. used to return an entity to its old parent, if it
         * still exists. is null for generators and folders
         */
        TreeEntry _savedParent;

        /*
         * these fields are all exclusively used by generators
         */
        bool _numericPostfix;
        bool _randomizeHealth;
        bool _randomizeImages;

        int _dieSides;
        int _dieCount;
        int _baseHealth;
        int _baseArmorClass;
        int _baseDexterity;
        ObservableCollection<string> _imageSources;

        bool _canSave;

        /// <summary>
        /// The type of TreeEntry. "Entity" types will have
        /// an null Contents property and a non-null
        /// AttachedEntity. Generator and Folder types will have a null
        /// AttachedEntity and a non-null Contents.
        /// </summary>
        public EntryType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        /// <summary>
        /// Holds a reference to the Entity object that can be pushed to the tray.
        /// Generators and folders have this value set to null.
        /// </summary>
        public Entity AttachedEntity
        {
            get { return _attachedEntity; }
            set { SetProperty(ref _attachedEntity, value); }
        }

        public TreeEntry Parent
        {
            get { return _parent; }
            private set { SetProperty(ref _parent, value); }
        }

        public TreeEntry SavedParent
        {
            get { return _savedParent; }
            private set { SetProperty(ref _savedParent, value); }
        }

        /// <summary>
        /// Should be null for EntryType.Entity — reference AttachedEntity.Name instead
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }

        /// <summary>
        /// Contains child TreeEntries, in the case of "folder" tree entries. This will exist
        /// for both Generators and Folders.
        /// </summary>
        public SortedTreeEntryList Contents
        {
            get { return _contents; }
            private set { SetProperty(ref _contents, value); }
        }

        //generator-exclusive properties
        public bool NumericPostfix
        {
            get { return _numericPostfix; }
            set { SetProperty(ref _numericPostfix, value); }
        }

        public bool RandomizeHealth
        {
            get { return _randomizeHealth; }
            set { SetProperty(ref _randomizeHealth, value); }
        }

        public bool RandomizeImages
        {
            get { return _randomizeImages; }
            set { SetProperty(ref _randomizeImages, value); }
        }

        public int DieSides
        {
            get { return _dieSides; }
            set { SetProperty(ref _dieSides, value); }
        }

        public int DieCount
        {
            get { return _dieCount; }
            set { SetProperty(ref _dieCount, value); }
        }

        public int BaseHealth
        {
            get { return _baseHealth; }
            set { SetProperty(ref _baseHealth, value); }
        }

        public int BaseArmorClass
        {
            get { return _baseArmorClass; }
            set { SetProperty(ref _baseArmorClass, value); }
        }

        public int BaseDexterity
        {
            get { return _baseDexterity; }
            set { SetProperty(ref _baseDexterity, value); }
        }

        public ObservableCollection<string> ImageSources
        {
            get { return _imageSources; }
            set { SetProperty(ref _imageSources, value); }
        }

        /// <summary>
        /// Used by the serializer to determine whether or not to save the entity. This
        /// value should be "true" for all entities when we're not in the selective save
        /// dialog. Changing this property on a Generator or Folder automatically sets
        /// the property on all child elements of type Entity to "True".
        /// </summary>
        public bool CanSave
        {
            get { return _canSave; }
            set { SetProperty(ref _canSave, value); }
        }

        public event OnDeletionEventHandler OnDeletion;

        /// <summary>
        /// This constructor is called by SerializationManager during a file load.
        /// </summary>
        /// <param name="entry"></param>
        public TreeEntry(SerializableTreeEntry entry)
        {
            _type = entry.Type;
            _canSave = true;

            if (_type == EntryType.Entity)
            {
                _attachedEntity = new Entity(entry.AttachedEntity);
                _attachedEntity.AttachedEntry = this;
            }
            else if (_type == EntryType.Folder)
            {
                _contents = new SortedTreeEntryList();
                _displayName = entry.DisplayName;

                foreach (SerializableTreeEntry treeEntry in entry.Contents)
                {
                    TreeEntry newEntry = new TreeEntry(treeEntry);
                    newEntry.Parent = this;

                    Contents.Add(newEntry);
                }

                _contents.CollectionChanged += Contents_OnCollectionChanged;
                PropertyChanged += CanSave_PropertyChanged;
            }
            else
            {
                _contents = new SortedTreeEntryList();
                _imageSources = new ObservableCollection<string>();
                _displayName = entry.DisplayName;

                foreach (SerializableTreeEntry treeEntry in entry.Contents)
                {
                    TreeEntry newEntry = new TreeEntry(treeEntry);
                    newEntry.Parent = this;

                    Contents.Add(newEntry);
                }

                foreach (string imageSource in entry.ImageSources)
                {
                    ImageSources.Add(imageSource);
                }

                _numericPostfix = entry.NumericPostfix;
                _randomizeHealth = entry.RandomizeHealth;
                _randomizeImages = entry.RandomizeImages;
                _dieSides = entry.DieSides;
                _dieCount = entry.DieCount;

                _baseHealth = entry.BaseHealth;
                _baseArmorClass = entry.BaseArmorClass;
                _baseDexterity = entry.BaseDexterity;

                _contents.CollectionChanged += Contents_OnCollectionChanged;
                PropertyChanged += CanSave_PropertyChanged;
            }
        }

        public TreeEntry(Entity entity)
        {
            _type = EntryType.Entity;
            _canSave = true;

            entity.AttachedEntry = this;
            _attachedEntity = entity;
        }

        public TreeEntry(string displayName)
        {
            _type = EntryType.Folder;
            _canSave = true;
            _contents = new SortedTreeEntryList();
            _displayName = displayName;
            _contents.CollectionChanged += Contents_OnCollectionChanged;
            PropertyChanged += CanSave_PropertyChanged;
        }

        public TreeEntry(string displayName, int baseHealth, int baseDexterity, int baseArmorClass, bool numericPostfix, bool randomizeHealth, bool randomizeImages, int dieSides, int dieCount)
        {
            _type = EntryType.Generator;
            _canSave = true;
            _contents = new SortedTreeEntryList();
            _displayName = displayName;
            _contents.CollectionChanged += Contents_OnCollectionChanged;
            PropertyChanged += CanSave_PropertyChanged;

            _baseHealth = baseHealth;
            _baseDexterity = baseDexterity;
            _baseArmorClass = baseArmorClass;

            _numericPostfix = numericPostfix;
            _randomizeHealth = randomizeHealth;
            _randomizeImages = randomizeImages;

            _dieSides = dieSides;
            _dieCount = dieCount;
            _imageSources = new ObservableCollection<string>();
        }

        /// <summary>
        /// This event handler keeps the Parent field updated. It's called whenever a member of Contents is
        /// added, removed, or the collection is reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contents_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //iterate through all removed items, updating them to reflect their new "orphaned" status
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (TreeEntry entry in e.OldItems) entry.Parent = null;
            }

            //iterate through all added items to reflect their new parent (this)
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (TreeEntry entry in e.NewItems) entry.Parent = this;
            }
        }

        private void CanSave_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("CanSave"))
            {
                if (CanSave)
                {
                    foreach (TreeEntry entry in _contents)
                    {
                        if (entry.Type == EntryType.Entity)
                        {
                            entry.CanSave = true;
                        }
                    }
                }
                else
                {
                    foreach (TreeEntry entry in _contents) entry.CanSave = false;
                }
            }
        }

        /// <summary>
        /// Returns the DisplayName if the TreeEntry is a Generator or Folder, returns AttachedEntity.Name otherwise
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            if (_type == EntryType.Entity) return AttachedEntity.Name;
            else return _displayName;
        }

        /// <summary>
        /// Standard lexicographic comparison, sorts entries alphabetically.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(TreeEntry other)
        {
            return other.GetName().CompareTo(GetName());
        }

        /// <summary>
        /// Searches up the tree to see if the specified TreeEntry is a parent of the
        /// invoking entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool HasParent(TreeEntry entry)
        {
            if (entry == null)
            {
                if (Parent == null) return true;
                else return false;
            }

            TreeEntry parent = this;
            do
            {
                parent = parent.Parent;
                if (ReferenceEquals(parent, entry)) return true;
            }
            while (parent != null);
            return false;
        }

        /// <summary>
        /// Transfers the invoking TreeEntry into the contents folder of the specified entity.
        /// Used by drag and drop code.
        /// </summary>
        /// <param name="target"></param>
        public void MoveTo(TreeEntry target)
        {
            if (target != null) //target is not top level
            {
                if (target.Type != EntryType.Entity) //target is a valid drop target type
                {
                    if (!ReferenceEquals(target, this)) //target isn't dragging onto itself
                    {
                        if (!ReferenceEquals(Parent, target)) //item isn't being redundantly dragged to its current parent
                        {
                            if (!target.HasParent(this)) //a folder isn't being dragged into one of its sub-folders
                            {
                                Detach();
                                target.Contents.Add(this); //add to target container
                            }
                        }
                    }
                }
            }
            else //dragging to top level
            {
                if (Parent != null) //can't move to top level if we're already there
                {
                    Detach();
                    TreeEntryStackProvider.Instance.TreeEntryStack.Add(this);
                }
            }
        }

        /// <summary>
        /// Removes TreeEntry from its parent, or the StackProvider if Parent == null.
        /// used internally and does not raise deletion events
        /// </summary>
        void Detach()
        {
            if (Parent == null) TreeEntryStackProvider.Instance.TreeEntryStack.Remove(this);
            else Parent.Contents.Remove(this);
        }

        /// <summary>
        /// Call this to remove a TreeEntry permanently. raises deletion events
        /// </summary>
        public void Delete()
        {
            Detach();

            if (_contents != null)
            {
                for (int i = Contents.Count - 1; i > -1; i--)
                {
                    Contents[i].Delete(); //ensures all sub entities are properly deleted
                }
            }

            //notify any potential child elements that we've been deleted
            OnDeletion?.Invoke(this, EventArgs.Empty); 
        }

        /// <summary>
        /// When a TreeEntry is pushed to the stack, this event will activate
        /// if its saved parent is deleted. basically keeps the SavedParent property
        /// accurate and cleans up the event delegate we previously assigned
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSavedParentDeletion(object sender, EventArgs e)
        {
            if(_savedParent != null) _savedParent.OnDeletion -= OnSavedParentDeletion;
            _savedParent = null;
        }

        /// <summary>
        /// "Moves" an entry to the initiative stack. a reference to the entry's current parent, if
        /// any, is stored in SavedParent. it will be updated to null if for whatever reason the
        /// parent entry ceases to exist. Used for returning pulled entities to their proper location.
        ///
        /// If this is called on an EntryType.Folder or EntryType.Generator, Push() will be called on every
        /// child element.
        /// </summary>
        public void Push()
        {
            if (_type == EntryType.Entity)
            {
                _savedParent = Parent;
                if(_savedParent != null) _savedParent.OnDeletion += OnSavedParentDeletion;

                Detach(); 
                AttachedEntity.RollInitiative();

                EntityStackProvider.Instance.InitiativeStack.Add(AttachedEntity);
                BitmapManager.Load(AttachedEntity.ImageSource); //bring the entity image into memory
            }
            else
            {
                if (Contents != null)
                {
                    /*
                     * we can't using foreach() because we will be modifying the list. also, iterate backwards
                     * so removing elements doesn't mess with the indexing
                     */
                    for (int i = Contents.Count - 1; i > -1; i--)
                    {
                        Contents[i].Push();
                    }
                }
            }
        }

        public void DeleteChildren()
        {
            if (Contents != null)
            {
                for (int i = Contents.Count - 1; i > -1; i--)
                {
                    Contents[i].Delete();
                }
            }
        }

        public void Generate(int count)
        {
            if (Type == EntryType.Generator)
            {
                for (int i = 0; i < count; i++)
                {
                    int healthResult = BaseHealth;
                    string imageResult = ImageSources.Count == 0 ? null : ImageSources[0];

                    if (RandomizeHealth)
                    {
                        healthResult = BaseHealth + RngProvider.XdY(DieCount, DieSides);
                    }

                    if (RandomizeImages && ImageSources.Count > 0)
                    {
                        imageResult = ImageSources[RngProvider.RNG.Next(0, ImageSources.Count)];
                    }

                    Contents.Add(
                        new TreeEntry(
                            new Entity(
                                NumericPostfix ? DisplayName + " " + (i + 1) : DisplayName,
                                healthResult, healthResult,
                                BaseArmorClass,
                                BaseDexterity, imageResult)
                            )
                        );
                }
            }
        }
    }
}