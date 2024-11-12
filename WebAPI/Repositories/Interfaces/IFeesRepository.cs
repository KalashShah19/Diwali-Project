using WebAPI.Models;

namespace WebAPI.Repositories;

interface IFeesRepository
{
    public void AddFeeInfo(FeesInfo.Post feesInfo);
    public List<FeesInfo.Get> GetFeeStructure();
    public List<FeesInfo.Get> GetFeeStructure(int batchYear);
    public FeesInfo.Get GetFeeStructure(int standard, int batchYear);
    public FeesInfo.Get GetFeeStructureById(int feesinfoid);
    public void UpdateFeeInfo(FeesInfo.Get feesInfo);
    public void RemoveFeeInfo(int feesinfoid);
}