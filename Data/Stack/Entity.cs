using System;
using InitiativeTracker.Data.Tray;
using InitiativeTracker.Data.Util;
using InitiativeTracker.Data.XML;

namespace InitiativeTracker.Data.Stack
{
    /// <summary>
    /// Object used to model players, monsters, and really anything that has a place
    /// on the initiative stack.
    /// </summary>
    public class Entity : Notifier, IComparable<Entity>
    {
        /*
         * various implicitly private fields, most of which are exposed via properties.
         */
        string _name;
        int _currentHealth;
        int _maxHealth;
        int _armorClass;
        int _dexterity;
        int _initiative;
        bool _isHidden;

        bool _positionLock;

        int _dexMod;

        string _healthDisplay;
        string _dexterityDisplay;

        string _imageSource;

        bool isHealthDisplayStale;
        bool isDexterityDisplayStale;

        bool _isHighlighted;

        public TreeEntry AttachedEntry;

        /*
         * This is a property that can be bound to by a visual component using XAML code.
         * When something changes it, the component will recieve a notification to update itself
         * automatically. See the Notifier class for more detailed information on how this works.
         */
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        /*
         * Returns a formatted string using the pattern [current-health]/[max-health], and caches
         * the result for performance reasons.
         *
         * This method has no SetProperty function, because
         * the call to update bindings can only be triggered by changing the CurrentHealth and
         * MaxHealth properties (HealthDisplay has no accessable set function so it is effectively
         * read-only.) Basically you need to only change this through Current/MaxHealth
         */
        public string HealthDisplay
        {
            get
            {
                if (isHealthDisplayStale)
                {
                    _healthDisplay = $"{_currentHealth}/{_maxHealth}";
                    isHealthDisplayStale = false;
                }

                return _healthDisplay;
            }
            private set { }
        }

        /*
         * CurrentHealth and MaxHealth will be bound in the entity editor, but aren't used at all in the entity stack
         * (except by proxy of HealthDisplay), so we still have it set up to work with bindings
         */
        public int CurrentHealth
        {
            get { return _currentHealth; }
            set
            {
                if (SetProperty(ref _currentHealth, value))
                {
                    isHealthDisplayStale = true;
                    Notify("HealthDisplay"); //notify all bindings on HealthDisplay so the it updates properly...
                }
            }
        }

        public int MaxHealth
        {
            get { return _maxHealth; }
            set
            {
                if (SetProperty(ref _maxHealth, value))
                {
                    isHealthDisplayStale = true;
                    Notify("HealthDisplay"); //...and do the same thing here
                }
            }
        }

        public int ArmorClass
        {
            get { return _armorClass; }
            set { SetProperty(ref _armorClass, value); }
        }


        public int Dexterity
        {
            get { return _dexterity; }
            set
            {
                if (SetProperty(ref _dexterity, value))
                {
                    isDexterityDisplayStale = true;
                    _dexMod = (_dexterity - 10) / 2;
                    Notify("DexterityDisplay");
                }
            }
        }

        public string DexterityDisplay
        {
            get
            {
                if (isDexterityDisplayStale)
                {
                    _dexterityDisplay = $"{_dexterity} ({(_dexMod < 0 ? String.Empty : "+")}{_dexMod})";
                    isDexterityDisplayStale = false;
                }

                return _dexterityDisplay;
            }
            private set { }
        }

        public int Initiative
        {
            get { return _initiative; }
            set { SetProperty(ref _initiative, value); }
        }

        public bool IsHidden
        {
            get { return _isHidden; }
            set { SetProperty(ref _isHidden, value); }
        }

        /*
         * This flag is used to give the DM the freedom to move the entity independent of initiative. When set,
         * it is ignored by the position-determining algorithm, which otherwise could lead to erroneous
         * placement.
         */
        public bool PositionLock
        {
            get { return _positionLock; }
            set { SetProperty(ref _positionLock, value); }
        }

        public string ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        /*
         * Any entity that has this field set to true will have a darker background
         * color (used for showing who's going first)
         */
        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set { SetProperty(ref _isHighlighted, value); }
        }

        /*
         * I made sure to set the property fields rather than the properties themselves.
         * this is because notifying each binding will do nothing before object creation. The
         * bindings don't even exist at all!
         *
         * When a new binding is created, the get accessor will be called once for each property
         * automatically (without notification), which serves our needs perfectly. After that,
         * it will need to be updated as usual via PropertyChanged
         */
        public Entity(string name, int currentHealth, int maxHealth, int armorClass, int dexterity, int initiative, string imageSource)
        {
            _name = name;
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;
            _armorClass = armorClass;
            _dexterity = dexterity;
            _initiative = initiative;
            _imageSource = imageSource;

            _dexMod = _dexterity == Int32.MinValue ? 0 : (_dexterity - 10) / 2;

            //we will need to update display-related things
            isHealthDisplayStale = true;
            isDexterityDisplayStale = true;
        }

        /*
         * these are just a few useful constructor overloads that
         * reference the one above with varying numbers of arguments and default values
         */
        public Entity(string name) : this(name, 0, 0, 0, 0, 0, null) { }
        public Entity(string name, int health) : this(name, health, health, 0, 0, 0, null) { }
        public Entity(string name, int health, int armorClass) : this(name, health, health, armorClass, 0, 0, null) { }
        public Entity(string name, int health, int armorClass, int dexterity) : this(name, health, health, armorClass, dexterity, 0, null) { }
        public Entity(string name, int currentHealth, int maxHealth, int armorClass, int dexterity) : this(name, currentHealth, maxHealth, armorClass, dexterity, 0, null) { }
        public Entity(string name, int currentHealth, int maxHealth, int armorClass, int dexterity, string imageSource) : this(name, currentHealth, maxHealth, armorClass, dexterity, 0, imageSource) { }
        public Entity(SerializableEntity entity) : this(entity.Name, entity.CurrentHealth, entity.MaxHealth, entity.ArmorClass, entity.Dexterity, 0, entity.ImageSource) { }

        /// <summary>
        /// Special method used by the sorting algorithm to determine if one entity should
        /// go after or before another in the initiative stack.
        /// </summary>
        /// <param name="other">The entity we're comparing against</param>
        /// <returns>-1 if the current entity goes after the one we're comparing; 0 if there's
        /// a tie, and 1 if the current entity goes before the one we're comparing.</returns>
        public int CompareTo(Entity other)
        {
            if (other.Initiative < _initiative) return 1;
            else if (other.Initiative > _initiative) return -1;
            else return 0; //pass off tie handling to whatever algorithm is sorting the entities
        }

        /// <summary>
        /// Calculate the entity's initiative by rolling a die and adding its dexterity modifier.
        /// </summary>
        public void RollInitiative()
        {
            Initiative = RngProvider.d20() + _dexMod;
        }

        /// <summary>
        /// Used for debugging purposes (it makes entities look a lot better when they show up
        /// in Visual Studio's object tracing tool)
        /// </summary>
        /// <returns>Relatively detailed, readable information about the entity.</returns>
        public override string ToString()
        {
            return $"NAME: {Name}, HP: {HealthDisplay}, AC: {_armorClass}, DEX: {DexterityDisplay}, INIT: {Initiative}, OOP: {_positionLock.ToString()}";
        }

        /// <summary>
        /// Removes the entity from the stack and also tries to return it
        /// to its parent container/generator in the tray, if there is one. if there isn't,
        /// it adds it to the top level (TreeEntryStack). also unloads the BitmapImage
        /// associated with this entity, if there is one
        /// </summary>
        public void Pull()
        {
            EntityStackProvider.Instance.InitiativeStack.Remove(this);

            if (AttachedEntry.SavedParent != null) AttachedEntry.SavedParent.Contents.Add(AttachedEntry);
            else TreeEntryStackProvider.Instance.TreeEntryStack.Add(AttachedEntry);

            BitmapManager.Remove(ImageSource);
        }

        public void Delete()
        {
            EntityStackProvider.Instance.InitiativeStack.Remove(this);
            AttachedEntry?.Delete();
            BitmapManager.Remove(ImageSource);
        }

        public void Reset()
        {
            Initiative = Int32.MinValue;
            CurrentHealth = MaxHealth;
        }
    }
}