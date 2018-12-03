using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public interface IChoser
    {
        string makeRandomeMove(ChoseTree tree);
        void takeChanse(char figure);
        void useChanse();

    }
}
