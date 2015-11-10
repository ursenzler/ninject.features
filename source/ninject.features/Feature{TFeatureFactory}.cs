namespace Ninject.Features
{
    using System;

    using Ninject.Extensions.Factory;

    public abstract class Feature<TFeatureFactory> : Feature
        where TFeatureFactory : class
    {
        protected Feature(params Dependency[] dependencies)
            : base(dependencies)
        {
        }

        public override Type FactoryType => typeof(TFeatureFactory);

        public override void BindFeatureFactory(IKernel kernel)
        {
            kernel.Bind<TFeatureFactory>().ToFactory(() => new TypeMatchingArgumentInheritanceInstanceProvider());
        }
    }
}