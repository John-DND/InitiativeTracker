using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using InitiativeTracker.Data.Tray;
using InitiativeTracker.Compat;

namespace InitiativeTracker.Data.XML
{
    public static class SerializationManager
    {
        static XmlSerializer _serializer = new XmlSerializer(typeof(SerializableGroup));
        static XmlSerializer _legacySerializer = new XmlSerializer(typeof(EntityProvider));

        private static void Serialize(string file, object o, XmlSerializer serializer)
        {           
            try
            {
                using (StreamWriter writer = new StreamWriter(file, false))
                {
                    serializer.Serialize(writer, o);
                }
            }
            catch { MessageBox.Show("Unable to save to the file. Is it already in use?", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
        }

        private static object Deserialize(string file, XmlSerializer serializer)
        {
            try
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    return serializer.Deserialize(reader);
                }
            }
            catch { MessageBox.Show("Unable to load from the file. Either the XML data is invalid or the file is in use.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }

            return null;
        }

        public static void Save(string file)
        {
            Serialize(file, TreeEntryStackProvider.Instance.TreeEntryStack, _serializer);
        }

        public static void Save(string file, SerializableGroup treeEntries)
        {
            Serialize(file, treeEntries, _serializer);
        }

        public static void LoadToTray(string file)
        {
            object deserialize = Deserialize(file, _serializer);
            if (deserialize == null) return;

            SerializableGroup tray = (SerializableGroup) deserialize;
            foreach(SerializableTreeEntry entry in tray.TreeEntries) TreeEntryStackProvider.Instance.TreeEntryStack.Add(new TreeEntry(entry));
        }

        public static SortedTreeEntryList LoadToList(string file)
        {
            object deserialize = Deserialize(file, _serializer);
            if (deserialize == null) return null;

            SortedTreeEntryList result = new SortedTreeEntryList();
            SerializableGroup tray = (SerializableGroup) deserialize;

            foreach (SerializableTreeEntry entry in tray.TreeEntries)
            {
                result.Add(new TreeEntry(entry));
            }
            return result;
        }

        static void ParseLegacy(List<EntityBase> entities, IList<SerializableTreeEntry> newEntities)
        {
            foreach (EntityBase entityBase in entities)
            {
                if (entityBase is Entity entity)
                {
                    SerializableTreeEntry nonLegacy =
                        new SerializableTreeEntry(
                            new TreeEntry(
                                new Stack.Entity(
                                    entity.Name, entity.Health, entity.Health, entity.ArmorClass, entity.Dexterity, entity.ImageSource
                                )
                            )
                        );

                    newEntities.Add(nonLegacy);
                }
                else if (entityBase is EntityGenerator generator)
                {
                    SerializableTreeEntry nonLegacy =
                        new SerializableTreeEntry(
                            new TreeEntry(
                                generator.Name, 
                                generator.Health, 
                                generator.Dexterity, 
                                generator.ArmorClass, 
                                generator.NumericPostfix, 
                                generator.RandomizeHealth, 
                                generator.RandomizeImages, 
                                generator.DieSides, 
                                generator.DieCount
                                )
                        );

                    nonLegacy.ImageSources = generator.ImageSources;
                    newEntities.Add(nonLegacy);
                    ParseLegacy(generator.Contents, nonLegacy.Contents); //recursion is fun
                }
                else if (entityBase is EntityContainer container)
                {
                    SerializableTreeEntry nonLegacy =
                        new SerializableTreeEntry(
                            new TreeEntry(container.Name)
                        );

                    newEntities.Add(nonLegacy);
                    ParseLegacy(container.Contents, nonLegacy.Contents);
                }
            }
        }

        public static void ConvertFromLegacy(string file)
        {
            object deserialize = Deserialize(file, _legacySerializer);
            if (deserialize == null) return;

            SerializableGroup nonLegacyGroup = new SerializableGroup(new SortedTreeEntryList());
            EntityProvider legacyGroup = (EntityProvider) deserialize;

            try
            {
                ParseLegacy(legacyGroup.Entities, nonLegacyGroup.TreeEntries);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to convert the file: '{e.Message}'", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            Serialize(file, nonLegacyGroup, _serializer);
        }
    }
}