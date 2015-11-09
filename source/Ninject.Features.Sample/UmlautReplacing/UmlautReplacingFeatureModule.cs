namespace Ninject.Features.Sample.UmlautReplacing
{
    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class UmlautReplacingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IUmlautReplacingFeatureFactory>().ToFactory();

            this.Bind<IUmlautsReplacer>().To<UmlautsReplacer>().InTransientScope();
        }
    }
}