namespace Ninject.Features.Sample
{
    using System;

    using Ninject.Features.Sample.TimeStamping;

    public class LocalLongTimeProvider : ITimeProvider
    {
        public string GetCurrentTime()
        {
            return DateTime.Now.ToLongTimeString();
        }
    }
}