using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace renameNumFiles
{
    public class Options
    {
        public readonly bool isHelp = false;
        public readonly bool isSim = false;
        public readonly bool isForce = false;
        public bool isPattern => array.Length > 0;
        private Option[] array;
        public Options(string[] args)
        {
            List<Option> optionList = new List<Option>(args.Length / 2);
            var argQueue = new Queue<string>(args.dropLastArray());

            while (argQueue.Any())
            {
                switch (argQueue.Dequeue())
                {
                    case "-h": case "--help":
                        isHelp = true;
                        break;
                    case "-g":
                        optionList.Add(new Grep(argQueue.Dequeue()));
                        break;
                    case "-gi":
                        optionList.Add(new GrepCaseIgnore(argQueue.Dequeue()));
                        break;
                    case "-gx":
                        optionList.Add(new GrepRegex(argQueue.Dequeue()));
                        break;
                    case "-n":
                        isSim = true;
                        break;
                    case "-f": case "--force":
                        isForce = true;
                        break;
                    default:
                        Environment.Exit(1);
                        break;
                }
            }
            this.array = optionList.ToArray();
        }
        public int Length => this.array.Length;
        public bool All(string fileName) =>
            array.All( o => o.isMatch(fileName));
    }
}