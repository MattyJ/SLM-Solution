namespace Fujitsu.SLM.DataImportProcessors.Interfaces
{
    public interface IServiceDecompositionTemplateDataImportProcessor
    {
        void Execute(string serviceDecompositionFilePath, string filename, int templateType);
    }
}
