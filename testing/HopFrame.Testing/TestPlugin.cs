using HopFrame.Core.Config;
using HopFrame.Testing.Models;
using HopFrame.Web.Plugins;
using HopFrame.Web.Plugins.Annotations;
using HopFrame.Web.Plugins.Events;

namespace HopFrame.Testing;

public class TestPlugin : HopFramePlugin {

    [PluginConfigurator]
    public static void OnConfiguring(HopFrameConfigurator configurator) {
        Console.WriteLine("Configurator invoked!");
        configurator.GetDbContext<DatabaseContext>()!
            .Table<User>()
            .Property(u => u.Email)
            .SetDisplayName("Modified by Plugin!");
    }

    [EventHandler]
    public void OnDelete(DeleteEntryEvent e) {
        Console.WriteLine("Event called!");
        e.SetCancelled(true);
    }
    
}