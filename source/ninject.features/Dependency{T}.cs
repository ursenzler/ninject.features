namespace Ninject.Features
{
    using System;

    using Ninject.Syntax;

    public class Dependency<T> : Dependency
    {
        private readonly Action<IBindingToSyntax<T>> bind;

        public Dependency(Action<IBindingToSyntax<T>> bind)
        {
            this.bind = bind;
        }

        public sealed override void Bind(IKernel kernel)
        {
            this.bind(kernel.Bind<T>());
        }
    }
}