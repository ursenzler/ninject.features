//-------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Appccelerate">
//   Copyright (c) 2008-2013
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace Ninject.Features.Sample
{
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
        public override IEnumerable<INinjectModule> NeededExtensions
        {
            get
            {
                yield return new MyExtensionModule();
            }
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new MyFeatureModule();
                yield return new MyInfrastructureModule();
            }
        }
    }

    public class MyExtensionModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyExtensionThing>().ToSelf();
        }
    }

    public class MyOtherFeature : Feature
    {
        public override IEnumerable<INinjectModule> NeededExtensions
        {
            get
            {
                yield return new MyOtherExtensionModule();
            }
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new MyOtherFeatureModule();
                yield return new MyInfrastructureModule();
            }
        }
    }

    public class MyOtherExtensionModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<MyOtherExtensionThing>().ToSelf();
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

    public class MyExtensionThing
    {
    }

    public class MyOtherExtensionThing
    {
    }
}
