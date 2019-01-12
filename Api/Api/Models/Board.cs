using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Api.Models
{
    public class Board
    {
        [XmlAttribute]
        public char turn { get; set; } //w or b
        [XmlAttribute]
        public string Possiotion { get; set; }
        [XmlAttribute]
        public string[] possiblemoves { get; set; }
    }
}
