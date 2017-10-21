using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IAssetService : IService<Asset>
    {
        int GetNumberOfAssetReferences(int id);
    }
}