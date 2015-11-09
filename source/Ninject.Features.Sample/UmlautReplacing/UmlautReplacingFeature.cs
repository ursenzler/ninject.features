namespace Ninject.Features.Sample.UmlautReplacing
{
    using System.Collections.Generic;

    using Ninject.Modules;

    public class UmlautReplacingFeature : Feature<IUmlautReplacingFeatureFactory>
    {
        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new UmlautReplacingFeatureModule();
            }
        }
    }
}