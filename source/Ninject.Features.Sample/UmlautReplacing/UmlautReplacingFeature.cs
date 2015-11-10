namespace Ninject.Features.Sample.UmlautReplacing
{
    using System.Collections.Generic;
    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class UmlautReplacingFeature : Feature<IUmlautReplacingFeatureFactory>
    {
        public override void BindFeatureFactory(IKernel kernel)
        {
            kernel.Bind<IUmlautReplacingFeatureFactory>().ToFactory();
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new UmlautReplacingFeatureModule();
            }
        }
    }
}