namespace Ninject.Features.Sample.TimeStamping
{
    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class TimeStampingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ITimeStampingFeatureFactory>().ToFactory(() => new TypeMatchingArgumentInheritanceInstanceProvider());

            this.Bind<ITimeStamper>().To<TimeStamper>();
        }
    }
}