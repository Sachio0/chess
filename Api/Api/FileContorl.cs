using Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Api
{
    public class FileContorl
    {
        TextWriter xmlWriter;
        XmlSerializer ser;
        MovingXml xml;

        public MovingXml Xml { get => xml; set => xml = value; }

        FileContorl(string filename)
        {
            xmlWriter = new StreamWriter(filename);
            ser = new XmlSerializer(typeof(MovingXml));
        }
        
        public void save()
        {
            ser.Serialize(xmlWriter, xml);
        }
        //XmlReader xmlReader;
        //public void CreateNewFile(string fileName)
        //{
        //    xmlWriter = XmlWriter.Create(fileName);

        //}
        //public void addMove(string possiton, Dictionary<string,int> capabilities)
        //{
        //    xmlWriter.WriteStartElement("stage");
        //    xmlWriter.WriteStartElement("possiton");
        //    xmlWriter.WriteString(possiton);
        //    foreach (var item in capabilities)
        //    {
        //        xmlWriter.WriteStartElement("Capabilitie");
        //        xmlWriter.WriteStartElement("move");
        //        xmlWriter.WriteString(item.Key);
        //        xmlWriter.WriteStartElement("chanse");
        //        xmlWriter.WriteValue(item.Value);
        //    }

        //}

    }
}
