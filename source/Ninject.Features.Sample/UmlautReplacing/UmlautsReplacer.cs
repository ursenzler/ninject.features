namespace Ninject.Features.Sample.UmlautReplacing
{
    using System;

    public class UmlautsReplacer : IUmlautsReplacer
    {
        public string ReplaceUmlauts(string content)
        {
            Console.WriteLine($"replacing umlauts in  `{content}`.");

            return content
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue");
        }
    }
}