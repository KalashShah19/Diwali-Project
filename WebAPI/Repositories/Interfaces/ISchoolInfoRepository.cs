using WebAPI.Models;

namespace WebAPI.Repositories;

interface ISchoolInfoRepository
{
    public SchoolInfo.Get GetSchoolInfo();
    public void UpdateSchoolInfo(SchoolInfo.Post schoolInfo);
}