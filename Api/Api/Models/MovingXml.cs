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
        public string[] moves;
        [XmlArray]
        public int[] chanses;
        [XmlArray]
        public List<PrevChoes> PrevPossiton;
        
        [XmlIgnore]
        public Dictionary<string, int> Chanse {
            get => parseTwoArrayToDictionarty();
            set => parseDictionaryToTwoArray(value);
        }
        

        private void parseDictionaryToTwoArray(Dictionary<string, int> chanse)
        {
            moves = new string[chanse.Count];
            chanses = new int[chanse.Count];
            int counter = 0;
            foreach (var item in chanse)
            {
                moves[counter] = item.Key;
                chanses[counter] = item.Value;
                counter++;
            }
        }
        private Dictionary<string, int> parseTwoArrayToDictionarty()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            for (int i = 0; i < moves.Length; i++)
            {
                result.Add(moves[i], chanses[i]);
            }
            return result;
        }

    }
}
