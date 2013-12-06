ninject.features
================

Adds feature concept to Ninject.

A feature concept allows to build blocks of Ninject modules for reusability across executables (e.g. when you have a product family). The `FeatureModuleLoader`takes care that each Ninject module is only loaded once.


Example:

```csharp
    using System.Collections.Generic;
    using Ninject.Modules;

    public class Program
    {
        public static void Main(string[] args)
        {
            var kernel = new StandardKernel();

            var featureModuleLoader = new FeatureModuleLoader(kernel);

            featureModuleLoader.Load(new MyAppFeature());

            var thing = kernel.Get<MyInfrastructureThing>();
        }
    }

    public class MyAppFeature : Feature
    {
        public override IEnumerable<Feature> NeededFeatures
        {
            get
            {
                yield return new MyFeature();
                yield return new MyOtherFeature();
            }
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new MyAppModule();
            }
        }
    }

    public class MyFeature : Feature
    {
        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new MyFeatureModule();
                yield return new MyInfrastructureModule();
            }
        }
    }

    public class MyOtherFeature : Feature
    {
        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new MyOtherFeatureModule();
                yield return new MyInfrastructureModule();
            }
        }
    }

    public class MyAppModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyAppThing>().ToSelf();
        }
    }

    public class MyFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyFeatureThing>().ToSelf();
        }
    }

    public class MyOtherFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyOtherFeatureThing>().ToSelf();
        }
    }

    public class MyInfrastructureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyInfrastructureThing>().ToSelf();
        }
    }

    public class MyAppThing
    {
    }

    public class MyFeatureThing
    {
    }

    public class MyOtherFeatureThing
    {
    }

    public class MyInfrastructureThing
    {
    }
```

The infrastructure module is defined in both sub features, but the module is only loaded once by the `FeatureModuleLoader`.
