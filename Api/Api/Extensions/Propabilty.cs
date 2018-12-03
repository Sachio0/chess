using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Extensions
{
    public static class Propabilty
    {
        public static string ProbablityRandom(this Dictionary<string,int> chance)
        {
            Dictionary<string, int> addonObject = new Dictionary<string, int>();
            int options = chance.Count;
            int pointSum = chance.Sum(n => n.Value);
            Random random = new Random();
            if (chance.ToArray().AllTheSame())
                return chance.Select(n => n.Key).ToArray()[random.Next(options)];
            foreach (var item in chance)
            {
                if(item.Value > 1)
                    for (int i = 1; i < item.Value; i++)
                    {
                        addonObject.Add(item.Key,item.Value);
                    }
            }
            chance.AddRange(addonObject);
            var arrayOfOptions = chance.Select(n => n.Key).ToArray();
            return arrayOfOptions[random.Next(arrayOfOptions.Length)];
            
        }
        public static bool AllTheSame<T>(this ICollection<T> collection)
        {
            if (collection.Count < 1)
                return false;
            T last = collection.First();
            foreach (var item in collection)
            {
                if (item.Equals(last))
                    return false;
                last = item;
            }
            return true;
        }
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (var element in source)
                target.Add(element);
        }

    }
}
