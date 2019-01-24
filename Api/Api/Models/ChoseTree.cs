using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Api
{
    [XmlRoot]
    public class ChoseTree
    {
        [XmlElement]
        public MovingXml[] moves;

        [XmlAttribute]
        public int wins;

        [XmlAttribute]
        public int louse;

        [XmlAttribute]
        public int winsARow;

        [XmlAttribute]
        public int louseARow;
    }
}
