namespace Ninject.Features.Sample
{
    using System;
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
        public static IEnumerable<string> GetLines(this string content)
        {
            return content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }
    }
}