using Microsoft.AspNetCore.Components;

namespace HopFrame.Web.Components;

public class CancellableComponent : ComponentBase, IDisposable {
    protected CancellationTokenSource TokenSource { get; } = new();
    
    public void Dispose() {
        TokenSource.Dispose();
    }
}