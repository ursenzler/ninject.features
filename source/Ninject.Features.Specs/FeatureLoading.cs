// <copyright file="FeatureLoading.cs" company="Ninject.Features">
//   Copyright (c)  2013-2015
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//
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

namespace Ninject.Features.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using Ninject.Extensions.Factory;
    using Ninject.Modules;
    using Xbehave;

    using Xunit;

    public class FeatureLoading
    {
        private static readonly List<NinjectModule> BoundModules = new List<NinjectModule>();

        [Scenario]
        public void Load(
            IKernel kernel,
            FeatureLoader loader)
        {
            "establish a Ninject kernel"._(() =>
                kernel = new StandardKernel());

            "establish feature module loader"._(() =>
                loader = new FeatureLoader(kernel));

            "established binding for shared dependency instances"._(() =>
                kernel.Bind<ISharedSingletonDependency>().To<SharedSingletonDependency>().InSingletonScope());

            "when loading features"._(() =>
                loader.Load(
                    new FeatureA(
                        new TransientTypeDependency<ITransientDependency, TransientDependency>(),
                        new Dependency<IDependencyOfSharedFeature>(bind => bind.To<DependencyOfSharedFeature>().InSingletonScope()),
                        new KernelGetDependency<ISharedSingletonDependency>(),
                        new FeatureWideDependency<IFeatureWideDependency, FeatureWideDependency>()),
                    new FeatureB(
                        new TransientTypeDependency<ITransientDependency, TransientDependency>(),
                        new Dependency<IDependencyOfSharedFeature>(bind => bind.To<DependencyOfSharedFeature>().InSingletonScope()),
                        new KernelGetDependency<ISharedSingletonDependency>(),
                        new FeatureWideDependency<IFeatureWideDependency, FeatureWideDependency>()),
                    new FeatureC(),
                    new FacadeLessFeature()));

            "it should load all modules"._(() =>
                kernel.GetModules().Select(x => x.Name.Split('+').Last())
                    .Should().Contain(
                        nameof(ModuleA),
                        nameof(SharedModule),
                        nameof(ModuleC),
                        nameof(ModuleD),
                        nameof(FacadeLessFeatureModule)));

            "it should load all extension modules"._(() =>
                kernel.GetModules().Select(x => x.Name.Split('+').Last())
                    .Should().Contain(
                        nameof(ExtensionModuleA),
                        nameof(ExtensionModuleB),
                        nameof(ExtensionModuleC)));

            "it should load extension modules prior to the other modules"._(() =>
                    BoundModules.Select(x => x.Name.Split('+').Last())
                        .Should().ContainInOrder( // modules from feature A
                            nameof(ExtensionModuleA),
                            nameof(ExtensionModuleB),
                            nameof(FeatureAModule),
                            nameof(ModuleA),
                            nameof(SharedModule))
                        .And.ContainInOrder( // modules from feature B
                            nameof(ExtensionModuleB),
                            nameof(ExtensionModuleC),
                            nameof(FeatureBModule)));

            "it should load each module only once"._(() =>
                kernel.GetModules()
                    .GroupBy(x => x.Name)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key).Should().BeEmpty());

            "it should support transient dependencies"._(() =>
                {
                    var a = kernel.Get<IFeatureFactoryA>().CreateTransientDependency();
                    var b = kernel.Get<IFeatureFactoryB>().CreateTransientDependency();

                    a.Should().NotBeSameAs(b);
                });

            "it should support feature wide instances (singleton inside feature)"._(() =>
                {
                    var tree = kernel.Get<IFeatureFactoryA>().CreateObjectTree();

                    tree.Child1.Dependency.Should()
                        .BeSameAs(tree.Child2.Dependency, "because inside a feature, the same instance should be used");

                    var dependencyFromOtherFeature = kernel.Get<IFeatureFactoryB>().CreateFeatureWideDependency();

                    dependencyFromOtherFeature.Should()
                        .NotBeSameAs(tree.Child1.Dependency, "because in different features, different instances should be used");
                });

            "it should support system wide singleton dependencies"._(() =>
                {
                    var a = kernel.Get<IFeatureFactoryA>().CreateSharedSingletonDependency();
                    var b = kernel.Get<IFeatureFactoryB>().CreateSharedSingletonDependency();

                    a.Should().BeSameAs(b);
                });

            "it should not allow to get dependencies directly (without call to feature factory)."._(() =>
                {
                    var dependencyOfSharedFeature = kernel.TryGet<IDependencyOfSharedFeature>();
                    dependencyOfSharedFeature.Should().BeNull();
                });
        }

        [Scenario]
        public void BindsFactoriesOnlyOnce(
            IKernel kernel,
            FeatureLoader loader)
        {
            "establish a Ninject kernel"._(() =>
                kernel = new StandardKernel());

            "establish feature module loader"._(() =>
                loader = new FeatureLoader(kernel));

            "when loading features with shared sub features"._(() =>
                loader.Load(
                    new FeatureA(
                        new TransientTypeDependency<ITransientDependency, TransientDependency>(),
                        new Dependency<IDependencyOfSharedFeature>(bind => bind.To<DependencyOfSharedFeature>().InSingletonScope()),
                        new KernelGetDependency<ISharedSingletonDependency>(),
                        new FeatureWideDependency<IFeatureWideDependency, FeatureWideDependency>()),
                    new FeatureB(
                        new TransientTypeDependency<ITransientDependency, TransientDependency>(),
                        new Dependency<IDependencyOfSharedFeature>(bind => bind.To<DependencyOfSharedFeature>().InSingletonScope()),
                        new KernelGetDependency<ISharedSingletonDependency>(),
                        new FeatureWideDependency<IFeatureWideDependency, FeatureWideDependency>()),
                    new FeatureC()));

            "it should bind feature factory of features that occur multiple times in feature tree, only once"._(() =>
                Record.Exception(() => kernel.Get<ISubFeatureFactoryA>()).Should().BeNull());
        }

        [Scenario]
        public void MissingFeatureFactory(
            IKernel kernel,
            FeatureLoader loader,
            Exception exception)
        {
            "establish a Ninject kernel"._(() =>
                kernel = new StandardKernel());

            "establish feature module loader"._(() =>
                loader = new FeatureLoader(kernel));

            "when loading a feature that misses the binding for its factory"._(() =>
                exception = Record.Exception(() => loader.Load(new FeatureWithoutFactoryBinding())));

            "it should throw an InvalidOperationException"._(() =>
                exception
                    .Should().BeOfType<InvalidOperationException>()
                    .Which.Message
                        .Should().Match($"Missing*{nameof(IFeatureFactoryA)}*"));
        }

        public class FeatureA : Feature<IFeatureFactoryA>
        {
            private Dependency<IDependencyOfSharedFeature> dependencyOfSharedFeature;

            public FeatureA(
                Dependency<ITransientDependency> a,
                Dependency<IDependencyOfSharedFeature> dependencyOfSharedFeature,
                Dependency<ISharedSingletonDependency> c,
                Dependency<IFeatureWideDependency> featureWide)
                : base(a, c, featureWide)
            {
                this.dependencyOfSharedFeature = dependencyOfSharedFeature;
            }

            public override IEnumerable<Feature> NeededFeatures
            {
                get
                {
                    yield return new SharedSubFeature(this.dependencyOfSharedFeature);
                }
            }

            public override IEnumerable<INinjectModule> NeededExtensions
            {
                get
                {
                    yield return new ExtensionModuleA();
                    yield return new ExtensionModuleB();
                }
            }

            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new FeatureAModule();
                    yield return new ModuleA();
                    yield return new SharedModule();
                }
            }
        }

        public class FeatureAModule : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);

                this.Bind<ITree>().To<Tree>().InTransientScope();
            }
        }

        public class FeatureB : Feature<IFeatureFactoryB>
        {
            private readonly Dependency<IDependencyOfSharedFeature> dependencyOfSharedFeature;

            public FeatureB(
                Dependency<ITransientDependency> a,
                Dependency<IDependencyOfSharedFeature> dependencyOfSharedFeature,
                Dependency<ISharedSingletonDependency> c,
                Dependency<IFeatureWideDependency> featureWide)
                : base(a, c, featureWide)
            {
                this.dependencyOfSharedFeature = dependencyOfSharedFeature;
            }

            public override IEnumerable<Feature> NeededFeatures
            {
                get
                {
                    yield return new SharedSubFeature(this.dependencyOfSharedFeature);
                    yield return new SubFeatureOfB();
                }
            }

            public override IEnumerable<INinjectModule> NeededExtensions
            {
                get
                {
                    yield return new ExtensionModuleB();
                    yield return new ExtensionModuleC();
                }
            }

            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new FeatureBModule();
                }
            }
        }

        public class FeatureBModule : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class FeatureC : Feature<IFeatureFactoryC>
        {
            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new FeatureCModule();
                    yield return new ModuleC();
                    yield return new ModuleD();
                }
            }
        }

        public class FeatureCModule : NinjectModule
        {
            public override void Load()
            {
            }
        }

        public class SharedSubFeature : Feature<ISubFeatureFactoryA>
        {
            public SharedSubFeature(Dependency<IDependencyOfSharedFeature> b)
                : base(b)
            {
            }

            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new SharedModule();
                    yield return new ModuleC();
                }
            }
        }

        public class SubFeatureOfB : Feature<ISubFeatureFactoryB>
        {
            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new SubFeatureBModule();
                    yield return new SharedModule();
                }
            }
        }

        public class SubFeatureBModule : NinjectModule
        {
            public override void Load()
            {
                this.Bind<ISubFeatureFactoryB>().ToFactory();
            }
        }

        public class ModuleA : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class SharedModule : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class ModuleC : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class ModuleD : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class ExtensionModuleA : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class ExtensionModuleB : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public class ExtensionModuleC : NinjectModule
        {
            public override void Load()
            {
                BoundModules.Add(this);
            }
        }

        public interface ITransientDependency
        {
        }

        public interface IDependencyOfSharedFeature
        {
        }

        public interface ISharedSingletonDependency
        {
        }

        public class TransientDependency : ITransientDependency
        {
        }

        public class DependencyOfSharedFeature : IDependencyOfSharedFeature
        {
        }

        public class SharedSingletonDependency : ISharedSingletonDependency
        {
        }

        public interface IFeatureFactoryA
        {
            ITransientDependency CreateTransientDependency();

            ISharedSingletonDependency CreateSharedSingletonDependency();

            ITree CreateObjectTree();
        }

        public interface IFeatureFactoryB
        {
            ITransientDependency CreateTransientDependency();

            ISharedSingletonDependency CreateSharedSingletonDependency();

            IFeatureWideDependency CreateFeatureWideDependency();
        }

        public interface ISubFeatureFactoryA
        {
            IDependencyOfSharedFeature CreateB();
        }

        public interface ISubFeatureFactoryB
        {
        }

        public interface IFeatureFactoryC
        {
        }

        public interface ITree
        {
            Child1 Child1 { get; }

            Child2 Child2 { get; }
        }

        public class Tree : ITree
        {
            public Tree(Child1 child1, Child2 child2)
            {
                this.Child1 = child1;
                this.Child2 = child2;
            }

            public Child1 Child1 { get; }

            public Child2 Child2 { get; }
        }

        public interface IFeatureWideDependency
        {
        }

        public class FeatureWideDependency : IFeatureWideDependency
        {
        }

        public class Child1
        {
            public Child1(IFeatureWideDependency dependency)
            {
                this.Dependency = dependency;
            }

            public IFeatureWideDependency Dependency { get; }
        }

        public class Child2
        {
            public Child2(IFeatureWideDependency dependency)
            {
                this.Dependency = dependency;
            }

            public IFeatureWideDependency Dependency { get; }
        }

        public class FeatureWithoutFactoryBinding : Feature<IFeatureFactoryA>
        {
            public override void BindFeatureFactory(IKernel kernel)
            {
            }
        }

        public class FacadeLessFeature : Feature
        {
            public override IEnumerable<INinjectModule> Modules
            {
                get
                {
                    yield return new FacadeLessFeatureModule();
                }
            }
        }

        public class FacadeLessFeatureModule : NinjectModule
        {
            public override void Load()
            {
            }
        }
    }
}