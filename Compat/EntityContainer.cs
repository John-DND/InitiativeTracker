using System.Collections.Generic;
using System.Xml.Serialization;

namespace InitiativeTracker.Compat
{
    public class EntityContainer : EntityBase
    {
        [XmlArray]
        public List<EntityBase> Contents { get; set; }

        public EntityContainer(string name, List<EntityBase> contents) : base(name, 0, 0, 0)
        {
            if(contents != null) Contents = contents;
            else Contents = new List<EntityBase>();
        }

        public EntityContainer() { Contents = new List<EntityBase>(); }
    }
}
