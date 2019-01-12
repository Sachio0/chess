using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Api.Models
{
    public class PrevChoes
    {
        [XmlAttribute]
        public string Position;
        [XmlAttribute]
        public string Move;
    }
}
