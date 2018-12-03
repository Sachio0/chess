using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class ChoseTree
    {
        string position;
        double chance;
        string[] capabilities;

        public string Position { get => position; set => position = value; }
        public double Chance { get => chance; set => chance = value; }
        public string[] Capabilities { get => capabilities; set => capabilities = value; }

        
    }
}
