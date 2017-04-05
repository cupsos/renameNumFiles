using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace renameNumFiles
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
                goto HelpAndExit;
            string dir = args[0];
            IEnumerable<string> everFiles;

            try
            {
                everFiles =
                from longFilename in Directory.GetFiles(dir)
                select Path.GetFileName(longFilename);
            }
            catch
            {
                goto HelpAndExit;
            }

            IEnumerable<string> oldNumFiles =
            from file in everFiles
            where FileNameRegex.RightmostInt.IsMatch(file)
            orderby SortHash.Gen(file)
            select file;

            var headers = oldNumFiles.Select(file => FileNameRegex.GetHead(file)).Distinct();            
            Dictionary<string, int> NameCounter = new Dictionary<string, int>();
            Dictionary<string, string> CharLength = new Dictionary<string, string>();
            foreach (string h in headers)//Init Dic
            {
                NameCounter.Add(h, 0);
                CharLength.Add(h,string.Format("D{0}", (oldNumFiles.Where(file => FileNameRegex.GetHead(file) == h).Count() / 10 + 1)));
            }

            IEnumerable<string> newNumFiles = oldNumFiles.Select(oldfile => 
            {
                string head = FileNameRegex.GetHead(oldfile);
                string ret = FileNameRegex.RightmostInt.Replace(oldfile, NameCounter[head].ToString(CharLength[head]), 1);
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
                Console.WriteLine($"rename : {p.Item1} => {p.Item2}");
                File.Move(Path.Combine(dir, p.Item1), Path.Combine(dir, p.Item2));
            }
            return;
        HelpAndExit:
            const string HelpText = "renameNumFiles [path]";
            Console.WriteLine(HelpText);
            return;
        }
    }
}