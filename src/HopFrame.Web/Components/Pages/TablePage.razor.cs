using HopFrame.Core.Configuration;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HopFrame.Web.Components.Pages;

public partial class TablePage(IConfigAccessor accessor, NavigationManager navigator) : ComponentBase {
    private const int PerPage = 25;
    
    [Parameter]
    public string TableRoute { get; set; } = null!;

    public TableConfig Table { get; set; } = null!;

    protected override void OnInitialized() {
        base.OnInitialized();

        var table = accessor.GetTableByRoute(TableRoute);

        if (table is null) {
            navigator.NavigateTo("/admin", true);
            return;
        }

        Table = table;
    }
}