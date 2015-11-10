namespace Ninject.Features.Sample.UmlautReplacing
{
    using System;

    public enum UmlautReplacingOptions
    {
        Lowercase,
        Uppercase,
        All
    }

    public class UmlautsReplacer : IUmlautsReplacer
    {
        private readonly UmlautReplacingOptions options;

        public UmlautsReplacer(UmlautReplacingOptions options)
        {
            this.options = options;
        }

        public string ReplaceUmlauts(string content)
        {
            Console.WriteLine($"replacing umlauts in `{content}` with option `{this.options}`.");

            var replaceLowercase = this.options == UmlautReplacingOptions.Lowercase || this.options == UmlautReplacingOptions.All;
            var replaceUppercase = this.options == UmlautReplacingOptions.Uppercase || this.options == UmlautReplacingOptions.All;

            if (replaceLowercase)
            {
                content = content
                    .Replace("ä", "ae")
                    .Replace("ö", "oe")
                    .Replace("ü", "ue");
            }

            if (replaceUppercase)
            {
                content = content
                    .Replace("Ä", "Ae")
                    .Replace("Ö", "Oe")
                    .Replace("Ü", "Ue");
            }

            return content;
        }
    }
}