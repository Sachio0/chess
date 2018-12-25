using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Api.Models
{
    public class MovingXml
    {
        
        [XmlAttribute("Pos")]
        public string possiton;
        [XmlArray]
        public string[] positions;
        [XmlArray]
        public int[] chanses;
        [XmlIgnore]
        public Dictionary<string, int> Chanse { get => parseTwoArrayToDictionarty(); set => parseDictionaryToTwoArray(value); }
        

        private void parseDictionaryToTwoArray(Dictionary<string, int> chanse)
        {
            positions = new string[chanse.Count];
            chanses = new int[chanse.Count];
            int counter = 0;
            foreach (var item in chanse)
            {
                positions[counter] = item.Key;
                chanses[counter] = item.Value;
                counter++;
            }
        }
        private Dictionary<string, int> parseTwoArrayToDictionarty()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            for (int i = 0; i < positions.Length; i++)
            {
                result.Add(positions[i], chanses[i]);
            }
            return result;
        }

    }
}
