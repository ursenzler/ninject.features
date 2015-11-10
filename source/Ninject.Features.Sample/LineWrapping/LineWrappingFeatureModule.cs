namespace Ninject.Features.Sample.LineWrapping
{
    using Ninject.Modules;

    public class LineWrappingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<ILineWrapper>().To<LineWrapper>().InTransientScope();
        }
    }
}