using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgClPlugin028_HealSCPOnStop
{
    public class Main : Plugin<Config>
    {
        public static Main Instance;
        public override string Name => "SCPHealOnStop";
        public override string Author => "AgCl";
        public override Version Version => new Version(1, 0, 0);

        public Dictionary<Player, int> HealDelay = new Dictionary<Player, int>();

        private CoroutineHandle counter = new CoroutineHandle();

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();
            Exiled.Events.Handlers.Server.RoundStarted += StartHealingCounter;
            Exiled.Events.Handlers.Player.Hurting += Hurting;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Clear;
            Exiled.Events.Handlers.Player.Dying += Dying;
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
            Exiled.Events.Handlers.Server.RoundStarted -= StartHealingCounter;
            Exiled.Events.Handlers.Player.Hurting -= Hurting;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Clear;
            Exiled.Events.Handlers.Player.Dying -= Dying;
        }

        private void StartHealingCounter()
        {
            counter = Timing.RunCoroutine(Counter());
        }

        private void Clear()
        { 
            HealDelay.Clear();
            Timing.KillCoroutines(counter);
        }

        private void Dying(DyingEventArgs ev)
        {
            RemoveRecord(ev.Player);
        }

        private void Record(Player player)
        {
            if (!HealDelay.ContainsKey(player))
            {
                HealDelay.Add(player, Config.HealDelay);
            }
        }

        private void RemoveRecord(Player player)
        {
            if (HealDelay.ContainsKey(player))
            {
                HealDelay.Remove(player);
            }
        }

        private void Hurting(HurtingEventArgs ev)
        {
            if (ev.Player == null || ev.Attacker == null)
                return;
            if (!ev.Player.IsScp)
                return;
            Record(ev.Player);
            HealDelay[ev.Player] = Config.HealDelay;
        }

        private IEnumerator<float> Counter()
        { 
            while(!Round.IsEnded)
            {
                foreach (var kvp in HealDelay.ToList())
                {
                    var player = kvp.Key;
                    var counter = kvp.Value;
                    if (player.Velocity.x <= 0.01 && player.Velocity.y <= 0.01 && player.Velocity.z <= 0.01)
                    {
                        if (counter <= 0)
                        {
                            player.Heal(5);
                        }
                        else
                        {
                            HealDelay[player] = counter - 1;
                        }
                    }
                }
                yield return Timing.WaitForSeconds(1.1f);
            }
        }
    }
}
