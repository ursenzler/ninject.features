namespace Ninject.Features.Sample.LineWrapping
{
    using System.Collections.Generic;
    using Ninject.Modules;

    public class LineWrappingFeature : Feature<LineWrappingFeature.ILineWrappingFeatureFactory>
    {
        public interface ILineWrappingFeatureFactory
        {
            ILineWrapper CreateDocumentManipulator(WrapConfiguration wrapConfiguration);
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new LineWrappingFeatureModule();
            }
        }
    }
}