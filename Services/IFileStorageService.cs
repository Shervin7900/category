using CategoryApi.Dtos.FileStorageServices;
using RestEase;

namespace CategoryApi.Services
{
    public interface IFileStorageService
    {
        [AllowAnyStatusCode]
        [Post("api/v1/InternalFile/UploadB64")]
        Task<ServiceResult<bool>> Upload([Body] UploadFileDto dto);

        [AllowAnyStatusCode]
        [Post("api/v1/InternalFile/File/Delete")]
        Task<ServiceResult<bool>> DeleteFile([Body] DeleteFileInputDto dto);
    }
}