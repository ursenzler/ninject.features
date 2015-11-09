namespace Ninject.Features.Sample.Specs
{
    using System.Collections;

    using FluentAssertions;

    using Ninject.Features.Sample.TimeStamping;

    using Xbehave;

    public class DocumentWorkflowSpecs
    {
        [Scenario] 
        public void ProcessDocument(
            IWorkflowFactory factory,
            string output)
        {
            const string Input = "some text to process in the specification";
            const int MaxLineLength = 10;
            const int ExtraCharactersInLine = 5; // brackets, space + \r\n

            "establish the document workflow feature"._(() =>
                {
                    var timeProvider = new TransientTypeDependency<ITimeProvider, FakeTimeProvider>();

                    var kernel = new StandardKernel();
                    var loader = new FeatureModuleLoader(kernel);
                    
                    loader.Load(new WorkflowFeature(timeProvider));
                    factory = kernel.Get<IWorkflowFactory>();
                });
            
            "when processing a document"._(() => 
                output = factory.CreateWorkflow().Process(Input, MaxLineLength));
                
            "it should replace all umlauts"._(() => 
                output
                    .Should().NotContain("ä")
                    .And.NotContain("ö")
                    .And.NotContain("ü"));

            "it should wrap lines"._(() =>
                output.GetLines()
                    .Should().OnlyContain(x => x.Length < MaxLineLength + FakeTimeProvider.FakeTime.Length + ExtraCharactersInLine));

            "it should stamp each line with local time"._(() =>
                output.GetLines()
                    .Should().OnlyContain(x => x.StartsWith($"[{FakeTimeProvider.FakeTime}] ")));
        }

        public class FakeTimeProvider : ITimeProvider
        {
            public const string FakeTime = "20:15";

            public string GetCurrentTime()
            {
                return FakeTime;
            }
        }
    }
}