using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class Choser : IChoser
    {
        public Choser(HttpContext context)
        {
             this.session = context.Session;
            updateSession();
        }
        private ISession session;
        private Dictionary<char, int?> figuresCount = new Dictionary<char, int?>()
            {
                {'p', 8},
                {'n', 2},
                {'b', 2},
                {'r', 2},
                {'q', 1},
            };
        private readonly Dictionary<char, int> figuresValue = new Dictionary<char, int>()
        {
            {'p', 1},
            {'n', 3},
            {'b', 3},
            {'r', 5},
            {'q', 9},
            {'k', 100}
        };
        private string last = String.Empty;
        //private List<string> lasts = new List<string>();
        //Description
        //P - pionek
        //N - skoczek
        //B - goniec
        //R - wierza
        //Q - królowa
        //K - król
        //w - ruch białych
        //b - ruch czarnych
        //roszady KQ -białe kq - czarne
        //
        //
        public string makeRandomeMove(ChoseTree tree)
        {
            Random r = new Random();
            chengeCountFigures(tree.Position);
            if (!String.IsNullOrEmpty(last))
            {
                var figure = checkFigureCount(last);
                if (figure != ' ')
                {
                    takeChanse(figure);
                }
            }
            
            var res = r.Next(tree.Capabilities.Length);
            last = tree.Position;
            addToSession();
            return tree.Capabilities[res];
        }

        public void takeChanse(char figure)
        {
            if (figure == 'p') { }
            if (figure == 'n') { }
            if (figure == 'b') { }
            if (figure == 'r') { }
            if (figure == 'q') { }
        }

        public void useChanse()
        {

        }

        private char checkFigureCount(string positon)
        {
            positon = preperPosition(positon);
            if(figuresCount['p'] != positon.Count(n => n == 'p')) return 'p';
            if(figuresCount['n'] != positon.Count(n => n == 'n')) return 'n';
            if(figuresCount['b'] != positon.Count(n => n == 'b')) return 'b';
            if(figuresCount['r'] != positon.Count(n => n == 'r')) return 'r';
            if(figuresCount['q'] != positon.Count(n => n == 'q')) return 'q';
            return ' ';
        }

        private string preperPosition(string positon)
        {
            return positon.Substring(0, positon.IndexOf(' '));
        }

        private void chengeCountFigures(string positon)
        {
            positon = preperPosition(positon);
            figuresCount['p'] = positon.Count(n => n == 'p');
            figuresCount['n'] = positon.Count(n => n == 'n');
            figuresCount['b'] = positon.Count(n => n == 'b');
            figuresCount['r'] = positon.Count(n => n == 'r');
            figuresCount['q'] = positon.Count(n => n == 'q');
        }

        private void addToSession()
        {
            foreach (var item in figuresCount)
            {
                SessionExtensions.SetInt32(session, item.Key.ToString(), item.Value??0);
            }
            SessionExtensions.SetString(session, "last", last);
            
        }

        private void updateSession()
        {
            last = SessionExtensions.GetString(session, "last");
            if(!String.IsNullOrEmpty(last))
                foreach (var item in figuresCount.Select(n=>n.Key).ToArray())
                {
                    figuresCount[item] = SessionExtensions.GetInt32(session, item.ToString());
                }
            
        }
    }
}
