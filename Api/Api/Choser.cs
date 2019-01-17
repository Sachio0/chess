using Api.Extensions;
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
        private string choseIndexs;
        private string _fileName;
        private ISession session;
        private Dictionary<char, int?> figuresCount = new Dictionary<char, int?>()
            {
                {'p', 8},
                {'n', 2},
                {'b', 2},
                {'r', 2},
                {'q', 1},
                {'P',8 },
                {'N',2 },
                {'B',2 },
                {'R',2 },
                {'Q',1 }
            };
        private readonly Dictionary<char, int> figuresValue = new Dictionary<char, int>()
        {
            {'P', 1},
            {'N', 3},
            {'B', 3},
            {'R', 5},
            {'Q', 9},
            {'K', 100},
            {'p', -1},
            {'n', -3},
            {'b', -3},
            {'r', -5},
            {'q', -9},
            {'k', -100}
        };
        private string last = String.Empty;
        private string lastChoese = String.Empty;
        private List<MovingXml> fullMoves;
        private bool turn; //black true

        public Choser(HttpContext context, string fileName, char turn)
        {
             this.session = context.Session;
            updateSession();
            _fileName = fileName;
            fillFullMoves();
        }

        private void fillFullMoves()
        {
            FileContorl file = new FileContorl(_fileName);
            fullMoves = file.read();
        }

        
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
            
            string res;
            turn = tree.turn == 'b';
            chengeCountFigures(tree.Possiotion);
            if (!String.IsNullOrEmpty(last))
            {
                var figure = checkFigureCount(lastChoese);
                if (figure != ' ')
                {
                    upDateXml(figure, tree);
                }
            }
            if (!fullMoves.Select(n => n.possiton).Contains(tree.Possiotion))
            {
                Random r = new Random();
                addToFile(tree);
                res = tree.possiblemoves[r.Next(tree.possiblemoves.Length)];
            }
            else
                res = useChanse(tree.Possiotion);

            last = tree.Possiotion;
            lastChoese = res;
            addToSession();
            return lastChoese;
        }
        
        public void CreateChanseToMove(string[] capabilities)
        {
            Dictionary<int, string> MoveChanse = new Dictionary<int, string>();
            foreach (var item in capabilities)
            {

            }
        }
        private string useChaseAlt(string pos,int counter = 2)
        {
            //Dictionary<string, MovingXml> nextMoves = new Dictionary<string, MovingXml>();
            var acualyMove = fullMoves.First(n => n.possiton == pos);
            var nextMoves = fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(pos))
                .ToDictionary(n => n, m => m.PrevPossiton.First(x => x.Position.Contains(pos)));
            if (counter == 1) return shareForOneAlt(nextMoves);
        }

        private string shareForOneAlt(Dictionary<MovingXml, PrevChoes> nextMoves)
        {
            throw new NotImplementedException();
        }

        private string useChanse(string pos, int counter = 2)
        {
            string res = String.Empty;
            List<MovingXml> nextMoves;
            
            List<List<MovingXml>> nextNextMoves = new List<List<MovingXml>>();
            List<List<List<MovingXml>>> nextNextNextMoves = new List<List<List<MovingXml>>>();
            var move = fullMoves.First(n => n.possiton == pos);
            nextMoves = fullMoves.Where(n => n.PrevPossiton.Select(m=>m.Position).Contains(pos)).ToList();
            if (counter == 1) return findConnection(move,shareForOne(nextMoves));
            if(counter > 1)
            {
                foreach (var item in nextMoves.Select(n => n.possiton))
                {
                    nextNextMoves.Add(fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(item)).ToList());
                }
                if(counter == 2) return findConnection(move,shareForTwo(nextNextMoves,nextMoves, move));
            }
            if(counter > 2)
            {
                List<List<MovingXml>> helpedList = new List<List<MovingXml>>();
                foreach (var item in nextNextMoves)
                {
                    foreach (var ob in item.Select(n => n.possiton))
                    {
                        helpedList.Add(fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(ob)).ToList());
                    }
                    nextNextNextMoves.Add(helpedList);
                    helpedList.Clear();
                }
            }
            return res;
        }
        private MovingXml shareForTwo(List<List<MovingXml>> nextNextMoves,List<MovingXml> nextMoves, MovingXml move)
        {
            Dictionary<string, MovingXml> routsMove = new Dictionary<string, MovingXml>();
            List<MovingXml> bestsMoves = new List<MovingXml>();
            foreach (var item in nextNextMoves)
            {
                var choes = shareForOne(item);
                
                bestsMoves.Add(choes);
            }
            var best = shareForOne(bestsMoves);

        }
        private MovingXml shareForOne(List<MovingXml> nextMoves)
        {
            MovingXml best = nextMoves[0];
            foreach (var item in nextMoves)
            {
                if (best == item) continue;
                else best = whoIsTheBest(item, best);
            }
            return best;
        }

        private MovingXml whoIsTheBest(MovingXml item, MovingXml best)
        {
            return figureCounting(item.possiton) > figureCounting(best.possiton) ? item : best;
        }
        private int figureCounting(string possition)
        {
            int value = 0;
            possition = possition.Split(' ')[0];
            foreach (var item in possition)
            {
                if(figuresValue.Select(n=>n.Key).Contains(item))
                {
                    value += figuresValue[item];
                }
            }
            return value;
        }
        private string findConnection(MovingXml one, MovingXml two)
        {
            return two.PrevPossiton.FirstOrDefault(n => n.Position == one.possiton).Move;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="positon"></param>
        /// <returns></returns>
        private char checkFigureCount(string positon)
        {
            positon = preperPosition(positon);
            foreach (var item in figuresCount)
            {
                if (item.Value != positon.Count(n => n == item.Key)) return item.Key;
            }
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
            figuresCount['P'] = positon.Count(n => n == 'P');
            figuresCount['N'] = positon.Count(n => n == 'N');
            figuresCount['B'] = positon.Count(n => n == 'B');
            figuresCount['R'] = positon.Count(n => n == 'R');
            figuresCount['Q'] = positon.Count(n => n == 'Q');
        }


        //TO Change
        private void upDateChanse(char figure)
        {
            
            var index = fullMoves.IndexOf(fullMoves.FirstOrDefault(n => n.possiton == last));
            var currentObject = fullMoves.FirstOrDefault(n => n.possiton == last);
            var dictCurrentObj = currentObject.Chanse;
            dictCurrentObj[lastChoese] = currentObject.Chanse[lastChoese] + figuresValue[figure];
            currentObject.Chanse = dictCurrentObj;
            fullMoves[index] = currentObject;
            FileContorl file = new FileContorl(_fileName);
            file.update(fullMoves);
        }

        private int[] getChoseIndexs()
        {
            var inedxs = choseIndexs.Split(';');
            int[] res = new int[inedxs.Length];
            for (int i = 0; i < inedxs.Length; i++)
            {
                res[i] = Convert.ToInt32(inedxs[i]);
            }
            return res;
        }
        #region alt
        private void upDateXml(char Figure, Board tree)
        {
            var index = fullMoves.IndexOf(fullMoves.FirstOrDefault(n => n.possiton == last));
            var PrevObject = fullMoves[index];
            PrevObject.NextPossiton.Add(tree.Possiotion);
            PrevObject.Chanse = UpdateChanse(PrevObject.Chanse, Figure);
            fullMoves[index] = PrevObject;
            FileContorl file = new FileContorl(_fileName);
            file.update(fullMoves);
        }

        private Dictionary<string, int> UpdateChanse(Dictionary<string, int> chanse, char figure)
        {
            chanse[lastChoese] += figuresValue[figure];
            return chanse;
        }

        private void addMove(char Figure)
        {

        }
        #endregion
        private void reaFile()
        {
            FileContorl file = new FileContorl(_fileName);
            fullMoves = file.read();
        }
        private void addToFile(Board tree)
        {
            Dictionary<string, int> chanse = new Dictionary<string, int>();
            foreach (var item in tree.possiblemoves)
            {
                chanse.Add(item, 10);
            }

            saveAdd(chanse, tree.Possiotion);
        }

        private void saveAdd(Dictionary<string,int> chanse, string positon)
        {
            FileContorl file = new FileContorl(_fileName);
            file.AddMoveToTree(new MovingXml()
            {
                Chanse = chanse,
                possiton = positon,
                PrevPossiton =
                new List<PrevChoes>()
                {
                    new PrevChoes{ Position = last, Move = lastChoese}
                }
            });
            file.save();
        }
        public static void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }


        private void addToSession()
        {
            foreach (var item in figuresCount)
            {
                SessionExtensions.SetInt32(session, item.Key.ToString(), item.Value ?? 0);
            }
            SessionExtensions.SetString(session, "last", last);
            SessionExtensions.SetString(session, "lastChose", lastChoese);
            SessionExtensions.SetString(session, "choseIndexs", choseIndexs);
        }

        private void updateSession()
        {

            last = SessionExtensions.GetString(session, "last");
            lastChoese = SessionExtensions.GetString(session, "lastChose");
            choseIndexs = SessionExtensions.GetString(session, "choseIndexs");
            if (!String.IsNullOrEmpty(last))
                foreach (var item in figuresCount.Select(n => n.Key).ToArray())
                {
                    figuresCount[item] = SessionExtensions.GetInt32(session, item.ToString());
                }
        }
    }
}
