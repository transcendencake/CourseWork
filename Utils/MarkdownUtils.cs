using HeyRed.MarkdownSharp;

namespace CourseWork
{
    public static class MarkdownUtils
    {
        private static Markdown parser;
        static MarkdownUtils()
        {
            parser = new Markdown();
        }
        public static string MarkdownParser(string src)
        {
            return parser.Transform(src);
        }
    }
}
