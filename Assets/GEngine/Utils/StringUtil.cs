/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: StringUtil
 * Date    : 2018/03/16
 * Version : v1.0
 * Describe: 
 */

using System;

namespace GEngine
{
    public class StringUtil
    {
        public static bool IsNull(string source)
        {
            return string.IsNullOrEmpty(source);
        }

        public static string[] Split(string source, char separator)
        {
            return source.Split(separator);
        }

        public static string[] Split(string source, string separator)
        {
            return source.Split(new[] {separator}, StringSplitOptions.None);
        }

        public static bool Parse(string source, out int value)
        {
            if (int.TryParse(source, out value))
            {
                return true;
            }
            value = 0;
            return false;
        }


        public static bool Parse(string source, out int[] value, string separator)
        {
            if (IsNull(source) || IsNull(separator))
            {
                value = new int[0];
                return false;
            }

            string[] vs = Split(source, separator);
            value = new int[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                Parse(vs[i], out value[i]);
            }
            return true;
        }

    }
}