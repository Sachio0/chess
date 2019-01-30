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
        private int winsARow;
        private int louseARow;
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
            winsARow = file.Tree.winsARow;
            louseARow = file.Tree.louseARow;
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

            string res = String.Empty ;
            //turn = tree.turn == 'b';
            chengeCountFigures(tree.Possiotion);
            if (!String.IsNullOrEmpty(last))
            {
                var figure = checkFigureCount(last);
                if (figure != ' ')
                {
                    upDateXml(figure, tree);
                }
            }
            if (fullMoves.Select(n => n.possiton).Contains(tree.Possiotion))
            {
                res = useChanse(tree.Possiotion);
            }
            else if(String.IsNullOrEmpty(res))
            {
                Random r = new Random();
                addToFile(tree);
                res = tree.possiblemoves[r.Next(tree.possiblemoves.Length)];
            }

            last = tree.Possiotion;
            lastChoese = res;
            addToSession();
            return lastChoese;
        }

        private string useChanse(string pos, int counter = 3)
        {
            int value = 0;
            string res = String.Empty;
            List<MovingXml> nextMoves;

            Dictionary<PrevChoes, List<MovingXml>> nextNextMoves = new Dictionary<PrevChoes, List<MovingXml>>();
            Dictionary<PrevChoes, List<List<MovingXml>>> nextNextNextMoves = new Dictionary<PrevChoes, List<List<MovingXml>>>();
            var move = fullMoves.First(n => n.possiton == pos);
            nextMoves = fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(pos)).ToList();
            if (nextMoves.Count == 0)
            {
                return move.Chanse.ProbablityRandom();
            }
            if (counter > 1)
            {
                foreach (var item in nextMoves)
                {
                    var list = fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(item.possiton)).ToList();
                    var moveToList = item.PrevPossiton.FirstOrDefault(n => n.Position == pos);
                    nextNextMoves.Add(moveToList, list);
                }
                if (nextNextMoves.Count > 0) res = shareForTwo(nextNextMoves, out value);
            }
            else if (counter == 1 || nextNextMoves.Count == 0)
            {
                if (findConnection(move, shareForOne(nextMoves, out value), out string newMove)) res = newMove;
            }
            if (counter > 2)
            {
                List<List<MovingXml>> helpedList = new List<List<MovingXml>>();
                foreach (var item in nextNextMoves)
                {
                    foreach (var ob in item.Value.Select(n => n.possiton))
                    {
                        helpedList.Add(fullMoves.Where(n => n.PrevPossiton.Select(m => m.Position).Contains(ob)).ToList());
                    }
                    nextNextNextMoves.Add(item.Key, helpedList);
                    helpedList.Clear();
                }
                if (nextNextNextMoves.Count > 0)
                {
                    try
                    {
                        res = shareForThree(nextNextNextMoves, out value);
                    }catch
                    {

                    }
                    
                }
                    
            }
            if (String.IsNullOrEmpty(res)) res = move.Chanse.ProbablityRandom();
            return upDateChanseAfterUse(res,move, value);
        }
        private string shareForThree(Dictionary<PrevChoes, List<List<MovingXml>>> nextNextNextMoves, out int value)
        {
            value = 0;
            Dictionary<PrevChoes, List<MovingXml>> nextNextMove = new Dictionary<PrevChoes, List<MovingXml>>();
            Dictionary<string, MovingXml> dyc = new Dictionary<string, MovingXml>();
            foreach (var NextNextMoves in nextNextNextMoves)
            {
                foreach (var item in NextNextMoves.Value)
                {
                    if (item.Count == 0) continue;
                    nextNextMove.Add(NextNextMoves.Key, item);
                }
                var res = forTwo(nextNextMove, out value);
                dyc.Add(res.Key, res.Value);
                nextNextMove.Clear();
            }
            var best = dyc.FirstOrDefault();
            foreach (var item in dyc)
            {
                if (best.Value == whoIsTheBest(item.Value, best.Value, out value)) continue;
                else best = item;
            }
            return best.Key;
        }
        private string shareForTwo(Dictionary<PrevChoes, List<MovingXml>> nextNextMoves, out int value)
        {
            return forTwo(nextNextMoves,out value).Key;
        }
        KeyValuePair<string, MovingXml> forTwo(Dictionary<PrevChoes, List<MovingXml>> nextNextMoves , out int value)
        {
            value = 0;
            Dictionary<string, MovingXml> routsMove = new Dictionary<string, MovingXml>();
            List<MovingXml> bestsMoves = new List<MovingXml>();
            foreach (var item in nextNextMoves)
            {
                if (item.Value.Count == 0) continue;
                var choes = shareForOne(item.Value,out value);
                if(!routsMove.Keys.Contains(item.Key.Move))routsMove.Add(item.Key.Move, choes);

            }
            var best = routsMove.FirstOrDefault();
            foreach (var item in routsMove)
            {
                if (best.Value == whoIsTheBest(item.Value, best.Value,out value)) continue;
                else best = item;
            }

            return best;
        }

        private MovingXml shareForOne(List<MovingXml> nextMoves, out int value)
        {
            value = 0;
            MovingXml best = nextMoves[0];
            foreach (var item in nextMoves)
            {
                if (best == item) continue;
                else best = whoIsTheBest(item, best,out value);
            }
            return best;
        }
        private MovingXml whoIsTheBest(MovingXml item, MovingXml best, out int value)
        {
            var valueItem = figureCounting(item.possiton);
            var valueBest = figureCounting(best.possiton);
            if (valueBest > valueItem) value = valueBest - valueItem;
            if (valueBest < valueItem) value = valueItem - valueBest;
            else value = 0;
            return valueItem > valueBest ? item : best;
        }
        private int figureCounting(string possition)
        {
            int value = 0;
            possition = possition.Split(' ')[0];
            foreach (var item in possition)
            {
                if (figuresValue.Select(n => n.Key).Contains(item))
                {
                    value += figuresValue[item];
                }
            }
            return value;
        }

        private bool findConnection(MovingXml one, MovingXml two, out string move)
        {
            move = two.PrevPossiton.FirstOrDefault(n => n.Position == one.possiton).Move;
            return !string.IsNullOrEmpty(move);
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

        private void upDateXml(char Figure, Board tree)
        {
            var index = fullMoves.IndexOf(fullMoves.FirstOrDefault(n => n.possiton == last));
            var PrevObject = fullMoves[index];
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
        private string upDateChanseAfterUse(string MoveUp, MovingXml currentObject, int value)
        {
            var index = fullMoves.IndexOf(currentObject);
            var dictCurrentObj = currentObject.Chanse;
            if(last!= null)
            {
                var indexprev = fullMoves.IndexOf(fullMoves.FirstOrDefault(n => n.possiton == last));
                var PrevObject = fullMoves[indexprev];
                if (!currentObject.PrevPossiton.Select(n => n.Position).Contains(last))
                {
                    currentObject.PrevPossiton.Add(new PrevChoes { Position = last, Move = lastChoese });
                }
            }
            dictCurrentObj[MoveUp] = currentObject.Chanse[MoveUp] + value;
            currentObject.Chanse = dictCurrentObj;
            
            fullMoves[index] = currentObject;
            FileContorl file = new FileContorl(_fileName);
            file.update(fullMoves);
            return dictCurrentObj.ProbablityRandom();
        }
        //private int[] getChoseIndexs()
        //{
        //    var inedxs = choseIndexs.Split(';');
        //    int[] res = new int[inedxs.Length];
        //    for (int i = 0; i < inedxs.Length; i++)
        //    {
        //        res[i] = Convert.ToInt32(inedxs[i]);
        //    }
        //    return res;
        //}

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
            //SessionExtensions.SetString(session, "choseIndexs", choseIndexs);
        }

        private void updateSession()
        {

            last = SessionExtensions.GetString(session, "last");
            lastChoese = SessionExtensions.GetString(session, "lastChose");
            //choseIndexs = SessionExtensions.GetString(session, "choseIndexs");
            if (!String.IsNullOrEmpty(last))
                foreach (var item in figuresCount.Select(n => n.Key).ToArray())
                {
                    figuresCount[item] = SessionExtensions.GetInt32(session, item.ToString());
                }
        }
    }
}
