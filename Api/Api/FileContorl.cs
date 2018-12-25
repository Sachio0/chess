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
        List<MovingXml> _xml;
        string _fileName;
        ChoseTree _tree;
        

        public FileContorl(string filename)
        {

            _fileName = filename;
            _tree = new ChoseTree();
            _xml = read();
        }

        public void AddMoveToTree(MovingXml xml)
        {
            _xml.Add(xml);
        }
        public void update(List<MovingXml> tree)
        {
            _xml = tree;
            save();
        }

        public void save()
        {
            _tree.moves = _xml.ToArray();
            using (var xmlWriter = new StreamWriter(_fileName +".xml"))
            {
                new XmlSerializer(typeof(ChoseTree)).Serialize(xmlWriter, _tree);
            }
        }
        
        public List<MovingXml> read()
        {
            if (!File.Exists(_fileName + ".xml"))
            {

                return new List<MovingXml>();
            }
            using (Stream reader = new FileStream(_fileName + ".xml", FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(ChoseTree));
                _tree = (ChoseTree)serializer.Deserialize(reader);
            }
            return _tree.moves.ToList();
        }
        

    }
}
