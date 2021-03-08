using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork
{
    public static class TagsUtils
    {
        public static string[] NormalizeTags(string tags)
        {
            string[] tagsArr = tags?.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int length = tagsArr == null ? 0 : tagsArr.Length;
            var result = new string[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = tagsArr[i].Split('"')[3];
            }
            return result;
        }
    }
}
