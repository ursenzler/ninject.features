// <copyright file="TimeStamper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Ninject.Features.Sample.TimeStamping
{
    using System;
    using System.Linq;

    public class TimeStamper : ITimeStamper
    {
        private readonly ITimeProvider timeProvider;

        public TimeStamper(ITimeProvider timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public string TimeStampEachLine(string content)
        {
            return content
                .GetLines()
                .Select(x => $"[{this.timeProvider.GetCurrentTime()}] {x}")
                .Aggregate("", (a, v) => a + v + Environment.NewLine)
                .TrimEnd('\n', '\r');
        }
    }
}