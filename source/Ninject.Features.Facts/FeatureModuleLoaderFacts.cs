//-------------------------------------------------------------------------------
// <copyright file="FeatureModuleLoaderFacts.cs" company="Appccelerate">
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

namespace Ninject.Features.Facts
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using FluentAssertions;
    using Ninject.Modules;
    using NUnit.Framework;

    [TestFixture]
    public class FeatureModuleLoaderFacts
    {
        private FeatureModuleLoader testee;
        private IKernel kernel;

        public interface IDependencyA
        {
        }

        public interface IDependencyB
        {
        }

        public FeatureModuleLoaderFacts()
        {
            this.kernel = A.Fake<IKernel>();

            this.testee = new FeatureModuleLoader(this.kernel);
        }

        [Test]
        public void CollectsAllBindingsAndBindsThemDistinctOnKernel()
        {
            IEnumerable<INinjectModule> loadedModules = null;

            A.CallTo(() => this.kernel.Load(A<IEnumerable<INinjectModule>>._))
                .Invokes((IEnumerable<INinjectModule> modules) => loadedModules = modules);

            var dependencyA = new DependencyA();
            var dependencyB = new DependencyB();

            var dependencyDefinitionA = new Dependency<IDependencyA>(bind => bind.ToConstant(dependencyA).InTransientScope());
            var dependencyDefinitionB = new Dependency<IDependencyB>(bind => bind.ToConstant(dependencyB).InSingletonScope());

            this.testee.Load(
                new FeatureA(
                    dependencyDefinitionA,
                    dependencyDefinitionB),
                new FeatureB(
                    dependencyDefinitionA,
                    dependencyDefinitionB),
                new FeatureC());

            loadedModules.Select(m => m.GetType().Name)
                .Should().BeEquivalentTo(
                    new[] 
                    {
                        typeof(ModuleA),
                        typeof(ModuleB),
                        typeof(ModuleC),
                        typeof(ModuleD)
                    }.Select(_ => _.Name))
                .And.HaveCount(4);
        }

        [Test]
        public void ExecutesAllDistinctDependencies()
        {
            var dependencyA = new DependencyA();
            var dependencyB = new DependencyB();

            var dependencyDefinitionA = new Dependency<IDependencyA>(bind => bind.ToConstant(dependencyA).InTransientScope());

            this.testee.Load(
                new FeatureA(
                    dependencyDefinitionA,
                    new Dependency<IDependencyB>(bind => bind.ToConstant(dependencyB).InSingletonScope())),
                new FeatureB(
                    dependencyDefinitionA,
                    new Dependency<IDependencyB>(bind => bind.ToConstant(dependencyB).InSingletonScope())),
                new FeatureC());

            A.CallTo(() => this.kernel.Bind<IDependencyA>()).MustHaveHappened();
            A.CallTo(() => this.kernel.Bind<IDependencyB>()).MustHaveHappened();
        }

        public class FeatureA : Feature
        {
            private Dependency<IDependencyB> b;

            public FeatureA(Dependency<IDependencyA> a, Dependency<IDependencyB> b)
                : base(a, b)
            {
                this.b = b;
            }

            public override IEnumerable<Feature> NeededFeatures
            {
                get
                {
                    yield return new SubFeatureA(this.b);
                }
            }

            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new ModuleA();
                    yield return new ModuleB();
                }
            }
        }

        public class FeatureB : Feature
        {
            private readonly Dependency<IDependencyB> b;

            public FeatureB(Dependency<IDependencyA> a, Dependency<IDependencyB> b)
                : base(a)
            {
                this.b = b;
            }

            public override IEnumerable<Feature> NeededFeatures
            {
                get
                {
                    yield return new SubFeatureA(this.b);
                    yield return new SubFeatureB();
                }
            }
        }

        public class FeatureC : Feature
        {
            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new ModuleC();
                    yield return new ModuleD();
                }
            }
        }

        public class SubFeatureA : Feature
        {
            public SubFeatureA(Dependency<IDependencyB> b)
                : base(b)
            {
            }

            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new ModuleB();
                    yield return new ModuleC();
                }
            }
        }

        public class SubFeatureB : Feature
        {
            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new ModuleB();
                }
            }
        }

        public class ModuleA : NinjectModule
        {
            public override void Load()
            {
            }
        }

        public class ModuleB : NinjectModule
        {
            public override void Load()
            {
            }
        }

        public class ModuleC : NinjectModule
        {
            public override void Load()
            {
            }
        }

        public class ModuleD : NinjectModule
        {
            public override void Load()
            {
            }
        }

        public class DependencyA : IDependencyA
        {
        }

        public class DependencyB : IDependencyB
        {
        }
    }
}