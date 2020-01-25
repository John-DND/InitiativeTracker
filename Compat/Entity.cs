namespace InitiativeTracker.Compat
{
    public class Entity : EntityBase
    {
        public string ImageSource { get; set; }

        public bool IsHidden { get; set; }

        public Entity(string name, int health, int armorClass, int dexterity, 
            string imageSource, bool isHidden) : base(name, health, armorClass, dexterity)
        {
            ImageSource = imageSource;
            IsHidden = isHidden;
        }

        public Entity() : base() { }
    }
}