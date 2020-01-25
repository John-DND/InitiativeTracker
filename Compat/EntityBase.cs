using System.Xml.Serialization;

namespace InitiativeTracker.Compat
{
    [XmlInclude(typeof(Entity))]
    [XmlInclude(typeof(EntityContainer))]
    [XmlInclude(typeof(EntityGenerator))]
    public abstract class EntityBase
    {
        public string Name { get; set; }

        public int Health { get; set; }

        public int ArmorClass { get; set; }

        public int Dexterity { get; set; }

        public EntityBase(string name, int health, int armorClass, int dexterity)
        {
            Name = name;
            Health = health;
            ArmorClass = armorClass;
            Dexterity = dexterity;
        }

        public EntityBase() { }
    }
}
