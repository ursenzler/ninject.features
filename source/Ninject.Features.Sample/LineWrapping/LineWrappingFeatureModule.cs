namespace Ninject.Features.Sample.LineWrapping
{
    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class LineWrappingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ILineWrappingFeatureFactory>().ToFactory(() => new TypeMatchingArgumentInheritanceInstanceProvider());

            this.Bind<ILineWrapper>().To<LineWrapper>().InTransientScope();
        }
    }
}