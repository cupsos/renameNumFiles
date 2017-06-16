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
            if (args.Length == 0) goto NoArgument;
            string dir = args.Last();
            if (!Directory.Exists(dir)) goto InvalidPath;

            Options everyOption = new Options(args);
            if (everyOption.isHelp) goto HelpAndExit;

            IEnumerable<string> everyFile =
                from longFilename in Directory.GetFiles(dir)
                select Path.GetFileName(longFilename);
            
            if (everyOption.isPattern) everyFile = everyFile.Where(File => everyOption.All(File));

            IEnumerable<string> oldNumFiles =
            from file in everyFile
            where FileNameRegex.RightmostInt.IsMatch(file)
            orderby SortHash.Gen(file)
            select file;

            var headers = oldNumFiles.Select(file => FileNameRegex.GetHead(file)).Distinct();
            Dictionary<string, int> NameCounter = new Dictionary<string, int>();
            Dictionary<string, string> CharLength = new Dictionary<string, string>();
            foreach (string h in headers)//Init Dic
            {
                NameCounter.Add(h, 0);
                CharLength.Add(h, $"D{oldNumFiles.Where(file => FileNameRegex.GetHead(file) == h).Count() / 10 + 1}");
            }

            IEnumerable<string> newNumFiles = oldNumFiles.Select(oldfile =>
            {
                string head = FileNameRegex.GetHead(oldfile);
                string ret = FileNameRegex.RightmostInt.Replace(oldfile, NameCounter[head].ToString(CharLength[head]), 1);
                NameCounter[head]++;
                return ret;
            });

            var mvArgs = Enumerable.Zip(oldNumFiles, newNumFiles, (oldName, newName)
                             => new {oldName = oldName, newName = newName});

            foreach (var mArg in mvArgs)
            {
                Console.WriteLine($"rename : {mArg.oldName} => {mArg.newName}");
                if(everyOption.isForce && !everyOption.isSim)
                    File.Move(Path.Combine(dir, mArg.oldName), Path.Combine(dir, mArg.newName));
            }
            if(everyOption.isForce && !everyOption.isSim)
                Console.WriteLine($"{mvArgs.Count()} file(s) moved");
            else
                Console.WriteLine($"{mvArgs.Count()} file(s) simulated");
            return;
        HelpAndExit:
            const string HelpText =
                    "renameNumFiles [<options>] <path>\n" +
                    "----------------------------------------------\n" +
                    "OPTIONS\n" +
                    "\t--version\n" +
                    "\t--help\n" +
                    "\t-g\n\t\tPATTERN\n" +
                    "\t-gi\n\t\tPATTERN. Ignore case\n" +
                    "\t-gx\n\t\tPATTERN. Regex\n" +
                    "\t-n\n\t\tDon't actually rename anything, just show what would be done. It is default\n" +
                    "\t-f\n\t\tDo file rename\n";
            Console.Write(HelpText);
            Environment.Exit(1);
        NoArgument:
            Console.WriteLine("No argument");
            Environment.Exit(1);
        InvalidPath:
            Console.WriteLine($"Path {args.Last()} is not exist");
            Environment.Exit(1);
        }
    }
}