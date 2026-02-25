using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Events;

namespace HibernationRestart;

public class HibernationRestart : BasePlugin
{
    private bool _wasHibernating = true; // assume hibernating so we don't restart on first load when already awake
    private bool _restartPendingAfterSpawn;

    public override string ModuleName => "Hibernation Restart";
    public override string ModuleDescription => "Runs mp_restartgame 1 when the server wakes from hibernation (after a player spawns).";
    public override string ModuleAuthor => "";
    public override string ModuleVersion => "1.0.0";

    public override void Load(bool hotReload)
    {
        Console.WriteLine($"[{ModuleName}] Loaded. Will run mp_restartgame 1 after first player spawns when server wakes from hibernation.");
    }

    [ListenerHandler<Listeners.OnServerHibernationUpdate>]
    public void OnServerHibernationUpdate(bool isHibernating)
    {
        if (_wasHibernating && !isHibernating)
        {
            _restartPendingAfterSpawn = true;
            Console.WriteLine($"[{ModuleName}] Server woke from hibernation — will restart game after a player spawns.");
        }
        _wasHibernating = isHibernating;
    }

    [GameEventHandler]
    public HookResult OnPlayerSpawned(EventPlayerSpawned @event, GameEventInfo info)
    {
        if (!_restartPendingAfterSpawn)
            return HookResult.Continue;

        _restartPendingAfterSpawn = false;
        Server.ExecuteCommand("mp_restartgame 1");
        Console.WriteLine($"[{ModuleName}] Player spawned — executed mp_restartgame 1");
        return HookResult.Continue;
    }
}