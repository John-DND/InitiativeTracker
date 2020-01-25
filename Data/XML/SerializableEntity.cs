using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InitiativeTracker.Data.Stack;

namespace InitiativeTracker.Data.XML
{
    public class SerializableEntity
    {
        public string Name;

        public int CurrentHealth;

        public int MaxHealth;

        public int ArmorClass;

        public int Dexterity;

        public int Initiative;

        public bool IsHidden;

        public string ImageSource;

        public SerializableEntity() { }

        public SerializableEntity(Entity from)
        {
            Name = from.Name;
            CurrentHealth = from.CurrentHealth;
            MaxHealth = from.MaxHealth;
            ArmorClass = from.ArmorClass;
            Dexterity = from.Dexterity;
            Initiative = from.Initiative;
            IsHidden = from.IsHidden;
            ImageSource = from.ImageSource;
        }
    }
}
