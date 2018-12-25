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
    }
}
