using Microsoft.AspNetCore.Components.Forms;

namespace HopFrame.Web.Services;

public interface IFileService {

    public Task DownloadFile(string name, byte[] data);

    public Task<IBrowserFile> UploadFile();

}