namespace Ninject.Features.Sample.TimeStamping
{
    using System.Collections.Generic;

    using Ninject.Modules;

    public class TimeStampingFeature : Feature<ITimeStampingFeatureFactory>
    {
        public TimeStampingFeature(Dependency<ITimeProvider> timeProvider)
            : base(timeProvider)
        {
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new TimeStampingFeatureModule();
            }
        }
    }
}