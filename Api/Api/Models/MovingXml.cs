using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Api.Models
{
    public class MovingXml
    {
        [XmlAttribute]
        public string possiton;
        [XmlAttribute]
        public Dictionary<string, int> chnase;
    }
}
