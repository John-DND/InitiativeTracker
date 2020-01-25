using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using InitiativeTracker.Data.Tray;

namespace InitiativeTracker.Data.XML
{
    [XmlRoot("SavedEntities")]
    public class SerializableGroup
    {
        public List<SerializableTreeEntry> TreeEntries;

        /// <summary>
        /// A parameterless constructor is required for all classes that will be deserialized
        /// </summary>
        public SerializableGroup() { }

        public SerializableGroup(IEnumerable<TreeEntry> entries)
        {
            TreeEntries = new List<SerializableTreeEntry>();
            foreach (TreeEntry entry in entries)
            {
                if(entry.CanSave) TreeEntries.Add(new SerializableTreeEntry(entry));
            }
        }
    }
}
