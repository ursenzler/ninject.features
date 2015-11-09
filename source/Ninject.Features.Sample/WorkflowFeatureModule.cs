namespace Ninject.Features.Sample
{
    using System;

    using Ninject.Extensions.Factory;
    using Ninject.Modules;

    public class WorkflowFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IWorkflowFactory>().ToFactory();

            this.Bind<IWorkflow>().To<Workflow>().InTransientScope();
        }
    }
}