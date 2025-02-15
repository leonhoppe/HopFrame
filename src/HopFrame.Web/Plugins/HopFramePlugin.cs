using HopFrame.Web.Components.Pages;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace HopFrame.Web.Plugins;

public abstract class HopFramePlugin {

    /// <summary>
    /// Downloads a file using a helper js function
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <param name="data">The content of the file</param>
    /// <param name="runtime">The js reference for invoking the function (Injectable)</param>
    protected async Task DownloadFile(string fileName, byte[] data, IJSRuntime runtime) {
        using var stream = new DotNetStreamReference(new MemoryStream(data));

        await runtime.InvokeVoidAsync("downloadFileFromStream", fileName, stream);
    }

    protected Task<IBrowserFile> UploadFile(HopFrameTablePage targetPage, IJSRuntime runtime) {
        var result = new TaskCompletionSource<IBrowserFile>();

        targetPage.OnFileUpload = files => {
            result.SetResult(files.First());
            targetPage.OnFileUpload = null;
            return Task.CompletedTask;
        };

        runtime.InvokeVoidAsync("triggerClick", targetPage.FileInputElement!.Element);
        return result.Task;
    }
    
}
