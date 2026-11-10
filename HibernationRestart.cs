using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace HibernationRestart;

public class HibernationRestart : BasePlugin
{
    private bool _wasHibernating = true; // assume hibernating so we don't restart on first load when already awake

    public override string ModuleName => "Hibernation Restart";
    public override string ModuleDescription => "Runs mp_restartgame 1 when the server wakes from hibernation.";
    public override string ModuleAuthor => "Fatty";
    public override string ModuleVersion => "1.0.1";

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnServerHibernationUpdate>(OnServerHibernationUpdate);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{ModuleName}] Loaded. Will run mp_restartgame 1 when server wakes from hibernation.");
        Console.ResetColor();
    }

    private void OnServerHibernationUpdate(bool isHibernating)
    {
        if (_wasHibernating && !isHibernating)
        {
            Server.ExecuteCommand("mp_restartgame 1");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{ModuleName}] Server woke from hibernation — executed mp_restartgame 1");
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