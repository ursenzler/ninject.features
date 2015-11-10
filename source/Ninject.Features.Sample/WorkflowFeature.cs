namespace Ninject.Features.Sample
{
    using System.Collections.Generic;

    using Ninject.Features.Sample.LineWrapping;
    using Ninject.Features.Sample.TimeStamping;
    using Ninject.Features.Sample.UmlautReplacing;
    using Ninject.Modules;

    public class WorkflowFeature : Feature<WorkflowFeature.IWorkflowFactory>
    {
        public interface IWorkflowFactory
        {
            IWorkflow CreateWorkflow();
        }

        private readonly Dependency<ITimeProvider> timeProvider;

        public WorkflowFeature(Dependency<ITimeProvider> timeProvider)
        {
            this.timeProvider = timeProvider;
        }

        public override IEnumerable<Feature> NeededFeatures
        {
            get
            {
                yield return new UmlautReplacingFeature();
                yield return new LineWrappingFeature();
                yield return new TimeStampingFeature(this.timeProvider);
            }
        }

        public override IEnumerable<INinjectModule> Modules
        {
            get
            {
                yield return new WorkflowFeatureModule();
            }
        }
    }
}