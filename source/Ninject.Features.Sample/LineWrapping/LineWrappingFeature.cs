namespace Ninject.Features.Sample.LineWrapping
{
    using System.Collections.Generic;

    using Ninject.Modules;

    public class LineWrappingFeature : Feature<ILineWrappingFeatureFactory>
    {
        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new LineWrappingFeatureModule();
            }
        }
    }
}