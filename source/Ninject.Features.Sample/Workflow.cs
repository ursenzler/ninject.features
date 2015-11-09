namespace Ninject.Features.Sample
{
    using Ninject.Features.Sample.LineWrapping;
    using Ninject.Features.Sample.TimeStamping;
    using Ninject.Features.Sample.UmlautReplacing;

    public class Workflow : IWorkflow
    {
        private readonly IUmlautReplacingFeatureFactory umlautReplacingFeatureFactory;

        private readonly ILineWrappingFeatureFactory lineWrappingFeatureFactory;

        private readonly ITimeStampingFeatureFactory timeStampingFeatureFactory;

        public Workflow(
            IUmlautReplacingFeatureFactory umlautReplacingFeatureFactory,
            ILineWrappingFeatureFactory lineWrappingFeatureFactory,
            ITimeStampingFeatureFactory timeStampingFeatureFactory)
        {
            this.umlautReplacingFeatureFactory = umlautReplacingFeatureFactory;
            this.lineWrappingFeatureFactory = lineWrappingFeatureFactory;
            this.timeStampingFeatureFactory = timeStampingFeatureFactory;
        }

        public string Process(string content, int maxLineLength)
        {
            content = this.ReplaceUmlauts(content);

            content = this.WrapLines(content, maxLineLength);

            return this.TimeStampEachLine(content);
        }

        private string ReplaceUmlauts(string content)
        {
            var documentLoader = this.umlautReplacingFeatureFactory.CreateDocumentLoader();
            return documentLoader.ReplaceUmlauts(content);
        }

        private string WrapLines(string content, int maxLineLength)
        {
            var documentManipulator = this.lineWrappingFeatureFactory
                .CreateDocumentManipulator(new WrapConfiguration(maxLineLength));
            return documentManipulator.WrapLines(content);
        }

        private string TimeStampEachLine(string content)
        {
            var documentSaver = this.timeStampingFeatureFactory.CreateTimeStamper();
            return documentSaver.TimeStampEachLine(content);
        }
    }
}