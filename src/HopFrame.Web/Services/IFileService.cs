using Microsoft.AspNetCore.Components.Forms;

namespace HopFrame.Web.Services;

/// <summary>
/// Provides file handling capabilities for downloading and uploading files.
/// </summary>
public interface IFileService {

    /// <summary>
    /// Initiates a file download with the specified name and data.
    /// </summary>
    /// <param name="name">The name of the file to be downloaded.</param>
    /// <param name="data">The byte array representing the file's content.</param>
    public Task DownloadFile(string name, byte[] data);

    /// <summary>
    /// Allows the user to upload a file and returns the uploaded file for processing.
    /// </summary>
    /// <returns>A task that returns an IBrowserFile representing the uploaded file.</returns>
    public Task<IBrowserFile> UploadFile();

}