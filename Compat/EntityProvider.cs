using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InitiativeTracker.Compat
{
    [XmlRoot]
    public class EntityProvider
    {
        [XmlArray, XmlArrayItem(ElementName = "Entity")]
        public List<EntityBase> Entities { get; set; }

        public EntityProvider()
        {
            Entities = new List<EntityBase>();
        }
    }
}
