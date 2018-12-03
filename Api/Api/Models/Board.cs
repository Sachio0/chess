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
        public String Possiotion { get; set; }
        [XmlAttribute]
        public string[] possiblemoves { get; set; }
    }
}
