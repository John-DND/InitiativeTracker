using System.Collections.Generic;
using System.Xml.Serialization;

namespace InitiativeTracker.Compat
{
    public class EntityGenerator : EntityContainer
    {
        public bool NumericPostfix { get; set; }

        public bool RandomizeHealth { get; set; }

        public bool RandomizeImages { get; set; }

        public int DieSides { get; set; }

        public int DieCount { get; set; }

        [XmlArray]
        public List<string> ImageSources { get; set; }

        public EntityGenerator(string name, bool numericPostfix, 
            bool randomizeHealth, bool randomizeImages, int dieSides, int dieCount, 
            List<string> imageSources) : base(name, new List<EntityBase>())
        {
            NumericPostfix = numericPostfix;
            RandomizeHealth = randomizeHealth;
            RandomizeImages = randomizeImages;
            DieSides = dieSides;
            DieCount = dieCount;
            ImageSources = imageSources;
        }

        public EntityGenerator() : base()
        {
            ImageSources = new List<string>();
        }
    }
}