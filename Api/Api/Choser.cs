using Api.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class Choser //: IChoser
    {
        public Choser(HttpContext context)
        {
             this.session = context.Session;
            updateSession();
        }
        private string _fileName;
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
        private string lastChoese = String.Empty;
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

        public string makeRandomeMove(Board tree)
        {
            Random r = new Random();
            chengeCountFigures(tree.Possiotion);
            if (!String.IsNullOrEmpty(last))
            {
                var figure = checkFigureCount(last);
                if (figure != ' ')
                {
                    upDateChanse(figure);
                }
            }
            addToFile(tree.possiblemoves, tree.Possiotion);
            var res = r.Next(tree.possiblemoves.Length);
            last = tree.Possiotion;
            lastChoese = tree.possiblemoves[res];
            addToSession();
            return lastChoese;
        }

        public void takeChanse(char figure, string[] capabilities)
        {
            if (figure == 'p') { }
            if (figure == 'n') { }
            if (figure == 'b') { }
            if (figure == 'r') { }
            if (figure == 'q') { }
        }
        public void CreateChanseToMove(string[] capabilities)
        {
            Dictionary<int, string> MoveChanse = new Dictionary<int, string>();
            foreach (var item in capabilities)
            {

            }
        }
        public void useChanse()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positon"></param>
        /// <returns></returns>
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
            SessionExtensions.SetString(session, "lastChose", lastChoese);
        }

        private void updateSession()
        {

            last = SessionExtensions.GetString(session, "last");
            lastChoese = SessionExtensions.GetString(session, "lastChose");
            if (!String.IsNullOrEmpty(last))
                foreach (var item in figuresCount.Select(n=>n.Key).ToArray())
                {
                    figuresCount[item] = SessionExtensions.GetInt32(session, item.ToString());
                }
        }
        private void upDateChanse(char figure)
        {
            FileContorl file = new FileContorl("Game");
            var FullMoves = file.read();
            var lastObject = FullMoves.Last().Chanse;
            var keyToUpChanse = lastObject.Where(n => n.Key == lastChoese).FirstOrDefault().Key;

            lastObject[keyToUpChanse] = lastObject[keyToUpChanse] + figuresValue[figure];
            FullMoves[FullMoves.Count - 1].Chanse = lastObject;
            file.update(FullMoves);
        }
        private void addToFile(string[] capabilities, string positon)
        {
            Dictionary<string, int> chanse = new Dictionary<string, int>();
            foreach (var item in capabilities)
            {
                chanse.Add(item, 1);
            }
            saveAdd(chanse, positon);
        }

        private void saveAdd(Dictionary<string,int> chanse, string positon)
        {
            FileContorl file = new FileContorl("Game");
            file.AddMoveToTree(new MovingXml() { Chanse = chanse, possiton = positon });
            file.save();
        }
        public static void DeleteFile()
        {
            if (File.Exists("Game.xml"))
                File.Delete("Game.xml");
        }
    }
}
