using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using InitiativeTracker.Data.Tray;

namespace InitiativeTracker.Data.XML
{
    /// <summary>
    /// SerializableTreeEntry objects are created in order to properly save tray contents.
    /// Making the TreeEntry objects serializable themselves is potentially
    /// problematic since every property raises PropertyChanged events, which could cause
    /// significant lag when loading large, nested trays.
    /// </summary>
    public class SerializableTreeEntry
    {
        public EntryType Type;

        public SerializableEntity AttachedEntity;

        public string DisplayName;

        [XmlArray]
        public List<SerializableTreeEntry> Contents;

        public bool NumericPostfix;

        public bool RandomizeHealth;

        public bool RandomizeImages;

        public int DieSides;

        public int DieCount;

        public int BaseHealth;

        public int BaseArmorClass;

        public int BaseDexterity;

        [XmlArray]
        public List<string> ImageSources;

        public SerializableTreeEntry() { }

        public SerializableTreeEntry(TreeEntry entry)
        {
            Type = entry.Type;
            if(entry.AttachedEntity != null) AttachedEntity = new SerializableEntity(entry.AttachedEntity);
            DisplayName = entry.DisplayName;

            if (entry.Contents != null)
            {
                Contents = new List<SerializableTreeEntry>();
                foreach (TreeEntry treeEntry in entry.Contents)
                {
                    if(treeEntry.CanSave) Contents.Add(new SerializableTreeEntry(treeEntry));
                }
            }

            if (entry.ImageSources != null)
            {
                ImageSources = new List<string>();
                foreach (string source in entry.ImageSources)
                {
                    ImageSources.Add(source);
                }
            }

            NumericPostfix = entry.NumericPostfix;
            RandomizeHealth = entry.RandomizeHealth;
            RandomizeImages = entry.RandomizeImages;
            DieSides = entry.DieSides;
            DieCount = entry.DieCount;

            BaseHealth = entry.BaseHealth;
            BaseArmorClass = entry.BaseArmorClass;
            BaseDexterity = entry.BaseDexterity;
        }
    }
}
