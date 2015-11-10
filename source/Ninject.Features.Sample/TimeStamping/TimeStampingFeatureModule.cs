namespace Ninject.Features.Sample.TimeStamping
{
    using Ninject.Modules;

    public class TimeStampingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ITimeStamper>().To<TimeStamper>();
        }
    }
}