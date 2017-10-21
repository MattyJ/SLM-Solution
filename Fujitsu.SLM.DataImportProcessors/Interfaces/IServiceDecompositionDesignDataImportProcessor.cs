using System;

namespace Fujitsu.SLM.DataImportProcessors.Interfaces
{
    public interface IServiceDecompositionDesignDataImportProcessor : IDisposable
    {
        void Execute(string serviceDecompositionFilePath, string filename, int serviceDeskId);
        void Save();
        void Rollback();
    }
}