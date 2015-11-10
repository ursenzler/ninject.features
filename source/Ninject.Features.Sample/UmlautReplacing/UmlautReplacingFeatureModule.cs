namespace Ninject.Features.Sample.UmlautReplacing
{
    using Ninject.Modules;

    public class UmlautReplacingFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IUmlautsReplacer>().To<UmlautsReplacer>().InTransientScope();
        }
    }
}