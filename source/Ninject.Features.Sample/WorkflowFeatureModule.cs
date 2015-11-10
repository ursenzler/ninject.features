namespace Ninject.Features.Sample
{
    using Ninject.Modules;

    public class WorkflowFeatureModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IWorkflow>().To<Workflow>().InTransientScope();
        }
    }
}