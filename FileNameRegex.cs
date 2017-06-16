using System.Text.RegularExpressions;

namespace renameNumFiles
{
    public class FileNameRegex
    {
        public static readonly Regex RightmostInt = new Regex(@"[0-9]+", RegexOptions.RightToLeft);
        public static string GetHead(string numstring)
        {
            Match LM = FileNameRegex.RightmostInt.Match(numstring);
            return numstring.Remove(LM.Index, LM.Length);
        }
        public static int GetNum(string numstring) =>
                int.Parse(FileNameRegex.RightmostInt.Match(numstring).Value);
    }
}