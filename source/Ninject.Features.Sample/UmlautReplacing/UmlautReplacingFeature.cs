﻿namespace Ninject.Features.Sample.UmlautReplacing
{
    using System.Collections.Generic;
    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class UmlautReplacingFeature : Feature<UmlautReplacingFeature.IUmlautReplacingFeatureFactory>
    {
        public interface IUmlautReplacingFeatureFactory
        {
            IUmlautsReplacer CreateDocumentLoader(UmlautReplacingOptions options);
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new UmlautReplacingFeatureModule();
            }
        }

        public override void BindFeatureFactory(IKernel kernel)
        {
            kernel.Bind<IUmlautReplacingFeatureFactory>().ToFactory();
        }
    }
}