using Fujitsu.Aspose.Spreadsheets;
using Fujitsu.SLM.DataImporters.Entities;

namespace Fujitsu.SLM.DataImporters.Interfaces
{
    public interface IServiceDecompositionImporter
    {
        ImportListResult<ServiceDecomposition> ImportSpreadsheet(string filename);
    }
}
