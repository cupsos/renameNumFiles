using System;
using System.Text;
using System.Security.Cryptography;
namespace renameNumFiles
{
    public class SortHash
    {
        private static readonly MD5 md = MD5.Create();
        public static int Gen(string fileName) =>
             BitConverter.ToInt16(md.ComputeHash(Encoding.UTF8.GetBytes(FileNameRegex.GetHead(fileName))),0)
             + FileNameRegex.GetNum(fileName);
    }
}