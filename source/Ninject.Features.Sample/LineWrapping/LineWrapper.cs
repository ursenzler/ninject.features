// <copyright file="LineWrapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ninject.Features.Sample.LineWrapping
{
    using System.Text;

    public class LineWrapper : ILineWrapper
    {
        private readonly WrapConfiguration wrapConfiguration;

        public LineWrapper(WrapConfiguration wrapConfiguration)
        {
            this.wrapConfiguration = wrapConfiguration;
        }

        public string WrapLines(string content)
        {
            var length = this.wrapConfiguration.LineLength;

            StringBuilder builder = new StringBuilder();

            while (content.Length > length)
            {
                builder.AppendLine(content.Substring(0, length));

                content = content.Substring(length);
            }

            builder.Append(content);

            return builder.ToString();
        }
    }
}