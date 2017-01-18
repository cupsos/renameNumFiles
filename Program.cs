using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
namespace renameNumFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No Input");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("Too many args");
                return;
            }

            string dir = args[0];
            string[] everFiles;

            try
            {
                everFiles = Directory.GetFiles(dir);
                everFiles = everFiles.Select(file => Path.GetFileName(file)).ToArray();
            }
            catch
            {
                Console.WriteLine("Not vaild dir");
                return;
            }

            IEnumerable<string> oldNumFiles = everFiles.Where(file => Regex.IsMatch(file, @"\d+"));
            var headers = oldNumFiles.Select(file => GetHead(file)).Distinct();
            
            Dictionary<string, int> NameCounter = new Dictionary<string, int>();
            Dictionary<string, string> CharLength = new Dictionary<string, string>();
            foreach (string h in headers)//Init Dic
            {
                NameCounter.Add(h, 0);
                CharLength.Add(h,string.Format("D{0}", (oldNumFiles.Where(file => GetHead(file) == h).Count() / 10 + 1)));
            }

            IEnumerable<string> newNumFiles = oldNumFiles.Select(oldfile => 
            {
                string head = GetHead(oldfile);
                Regex rgx = new Regex(GetNum(oldfile), RegexOptions.RightToLeft);
                string ret = rgx.Replace(oldfile, NameCounter[head].ToString(CharLength[head]), 1);
                NameCounter[head]++;
                return ret;
            });

            List<Tuple<string, string>> pairs = new List<Tuple<string, string>>();
            IEnumerator<string> IEold = oldNumFiles.GetEnumerator();
            IEnumerator<string> IEnew = newNumFiles.GetEnumerator();
            while (IEold.MoveNext() && IEnew.MoveNext())
                pairs.Add(new Tuple<string, string>(IEold.Current, IEnew.Current));

            foreach (var p in pairs)
            {
                Console.WriteLine(string.Format("rename : {0} => {1}", p.Item1, p.Item2));
                File.Move(Path.Combine(dir, p.Item1), Path.Combine(dir, p.Item2));
            }
        }
        
        private static Match FindLastMatch(string input, string pattern)
        {
            Regex rgx = new Regex(pattern, RegexOptions.RightToLeft);
            return rgx.Match(input);
        }
        private static string GetHead(string numstring)
        {
            Match LM = FindLastMatch(numstring, @"[0-9]+");
            return numstring.Remove(LM.Index, LM.Length);
        }
        private static string GetNum(string numstring)
        {
            Match LM = FindLastMatch(numstring, @"[0-9]+");
            return LM.Value;
        }
    }
}
