using System;
using System.Collections;
using System.Collections.Generic;
namespace renameNumFiles
{
    public static class Extention
    {
        public static IEnumerable<T> dropLastArray<T>(this T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++) yield return array[i];
        }
    }
}