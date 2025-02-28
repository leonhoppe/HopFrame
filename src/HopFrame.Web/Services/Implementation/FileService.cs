using HopFrame.Web.Components.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace HopFrame.Web.Services.Implementation;

internal sealed class FileService(IJSRuntime runtime) : IFileService {
    
    public async Task DownloadFile(string name, byte[] data) {
        using var stream = new DotNetStreamReference(new MemoryStream(data));

        await runtime.InvokeVoidAsync("downloadFileFromStream", name, stream);
    }
    
    public Task<IBrowserFile> UploadFile() {
        var result = new TaskCompletionSource<IBrowserFile>();
        
        if (HopFrameTablePage.CurrentInstance is null)
            result.SetException(new InvalidOperationException("No table page visible"));

        HopFrameTablePage.CurrentInstance!.OnFileUpload = files => {
            result.SetResult(files.First());
            HopFrameTablePage.CurrentInstance.OnFileUpload = null;
            return Task.CompletedTask;
        };

        runtime.InvokeVoidAsync("triggerClick", HopFrameTablePage.CurrentInstance.FileInputElement!.Element);
        return result.Task;
    }
    
}