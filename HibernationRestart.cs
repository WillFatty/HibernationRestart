using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace HibernationRestart;

public class HibernationRestartConfig : BasePluginConfig
{
    [JsonPropertyName("ExecServer")]
    public bool ExecServerEnabled { get; set; } = false;
}

public class HibernationRestart : BasePlugin, IPluginConfig<HibernationRestartConfig>
{
    private bool _wasHibernating = true; // assume hibernating so we don't restart on first load when already awake

    public override string ModuleName => "Hibernation Restart";
    public override string ModuleDescription => "Runs a command when the server wakes from hibernation.";
    public override string ModuleAuthor => "Fatty";
    public override string ModuleVersion => "1.1.0";

    public HibernationRestartConfig Config { get; set; } = null!;

    public void OnConfigParsed(HibernationRestartConfig config)
    {
        Config = config;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{ModuleName}] Config loaded. ExecServer: {config.ExecServerEnabled}");
        Console.ResetColor();
    }

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnServerHibernationUpdate>(OnServerHibernationUpdate);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{ModuleName}] Loaded. Will run {(Config?.ExecServerEnabled == true ? "exec server.cfg" : "mp_restartgame 1")} when server wakes from hibernation.");
        Console.ResetColor();
    }

    private void OnServerHibernationUpdate(bool isHibernating)
    {
        if (_wasHibernating && !isHibernating)
        {
            var command = Config.ExecServerEnabled ? "exec server.cfg" : "mp_restartgame 1";
            Server.ExecuteCommand(command);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{ModuleName}] Server woke from hibernation — executed {command}");
            Console.ResetColor();
        }
        else if (!_wasHibernating && isHibernating)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{ModuleName}] Server went into hibernation.");
            Console.ResetColor();
        }
        _wasHibernating = isHibernating;
    }
}
