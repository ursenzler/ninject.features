namespace Ninject.Features.Sample
{
    using System;

    using Ninject.Features.Sample.TimeStamping;

    public class Program
    {
        public static void Main(string[] args)
        {
            string content = "this is some content that will be processed by this crazy sample code";
            int maxLineLength = 15;

            Console.WriteLine($"input = {content}");

            // build up
            var timeProvider = new TransientTypeDependency<ITimeProvider, LocalLongTimeProvider>();

            var kernel = new StandardKernel();
            var featureModuleLoader = new FeatureModuleLoader(kernel);
            featureModuleLoader.Load(new WorkflowFeature(timeProvider));

            // run
            var appFactory = kernel.Get<IWorkflowFactory>();
            var workflow = appFactory.CreateWorkflow();
            var processedContent = workflow.Process(content, maxLineLength);

            Console.WriteLine($"output = {processedContent}");
        }
    }
}
