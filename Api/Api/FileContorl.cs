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

        public ChoseTree Tree { get => _tree; }

        public FileContorl(string filename)
        {

            _fileName = filename;
            _tree = new ChoseTree();
            _xml = read();
        }
        public void addWinLouse(bool isWin)
        {
            if(isWin)
            {
                _tree.wins++;
                _tree.winsARow++;
                _tree.louseARow = 0;
            }
            else
            {
                _tree.louse++;
                _tree.louseARow++;
                _tree.winsARow = 0;
            }
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
            Tree.moves = _xml.ToArray();
            using (var xmlWriter = new StreamWriter(_fileName))
            {
                new XmlSerializer(typeof(ChoseTree)).Serialize(xmlWriter, Tree);
            }
        }
        
        public List<MovingXml> read()
        {
            if (!File.Exists(_fileName))
            {

                return new List<MovingXml>();
            }
            using (Stream reader = new FileStream(_fileName, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(ChoseTree));
                _tree = (ChoseTree)serializer.Deserialize(reader);
            }
            return Tree.moves.ToList();
        }
        

    }
}
