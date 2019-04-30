using System.Collections.Generic;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2;
using scp4aiur;
using Smod2.EventSystem.Events;
using System.Linq;
using UnityEngine;


namespace BOOM
{
    partial class PlayersEvents : IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerThrowGrenade, IEventHandlerSetSCPConfig, IEventHandlerSetConfig,
        IEventHandlerCheckRoundEnd, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerElevatorUse,
        IEventHandlerPlayerPickupItem, IEventHandlerRoundRestart, IEventHandlerPlayerDropItem

       {
        /////////////////////////////////////////////////////////////////Variables////////////////////////////////////////////////////////////////
        static Dictionary<string, int> Jugadores = new Dictionary<string, int>();
        static Dictionary<string, int> Jugadoresh = new Dictionary<string, int>();
        private Boom plugin;

        public PlayersEvents(Boom plugin)
        {
            this.plugin = plugin;
        }
        
        int contador = 0; int Scientists = 0; int Dboys = 0; int contadorpos = 0;     
        static string MVP = "nadie"; string mejor = "Nadie";
        int var1; int var2; Vector grandapos;

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            Jugadoresh[ev.Player.SteamId] = 0;
            
            if (ev.Player.TeamRole.Role == Role.SCIENTIST)
            {
                Timing.Run(Respawn(ev.Player));

            }
            if((ev.Player.TeamRole.Role == Role.CLASSD)&&(contadorpos == 0))
            {
                Timing.Run(Respawn(ev.Player));
                contadorpos = 1;
            }
            if ((ev.Player.TeamRole.Role == Role.CLASSD) && (contadorpos == 1))
            {
                Timing.Run(RespawnD1(ev.Player));
                contadorpos = 2;
            }
            if ((ev.Player.TeamRole.Role == Role.CLASSD) && (contadorpos == 2))
            {
                Timing.Run(RespawnD2(ev.Player));
                contadorpos = 0;
            }
            
                
            if ((ev.Killer.SteamId != ev.Player.SteamId)&&(ev.Killer.TeamRole.Role != ev.Player.TeamRole.Role)){ Jugadores[ev.Killer.SteamId] = Jugadores[ev.Killer.SteamId]+ 1; Jugadoresh[ev.Killer.SteamId] = Jugadoresh[ev.Killer.SteamId] + 1; }
               
                ev.Player.SendConsoleMessage("Has muerto, tu asesino fue: " + ev.Killer.Name, "green");
               
                if ((ev.Player.TeamRole.Role == Role.SCIENTIST) && ((ev.Killer.TeamRole.Role == Role.SPECTATOR)||(ev.Killer.TeamRole.Team == Team.CLASSD)) && (ev.Killer.SteamId != ev.Player.SteamId))
                {
                    ev.Killer.GiveItem(ItemType.FRAG_GRENADE);
                    Dboys = Dboys + 1;
                   
                }
                if ((ev.Player.TeamRole.Role == Role.CLASSD) && ((ev.Killer.TeamRole.Team == Team.SCIENTIST)||(ev.Killer.TeamRole.Role == Role.SPECTATOR)) && (ev.Killer.SteamId != ev.Player.SteamId))
                {
                    ev.Killer.GiveItem(ItemType.FRAG_GRENADE);
                    Scientists = Scientists + 1;
                   
                }
              if (Jugadoresh[ev.Killer.SteamId] >= 1) { ev.Killer.AddHealth(50); }
            if (Jugadoresh[ev.Killer.SteamId] == 2) { ev.Killer.GiveItem(ItemType.COIN);
                PluginManager.Manager.Server.Map.Broadcast(2,ev.Killer.Name + " esta en racha de 2",true);
            }
            if (Jugadoresh[ev.Killer.SteamId] == 7)
            {
                ev.Killer.GiveItem(ItemType.CUP);
                PluginManager.Manager.Server.Map.Broadcast(2,"CUIDADO, " + ev.Killer.Name + " esta en racha de 7", true);
            }

        }
        

        public void OnSetRole(PlayerSetRoleEvent ev)
        {
            if(contador == 0)
            {
                foreach (Player player in PluginManager.Manager.Server.GetPlayers())
                {
                    Jugadores.Add(player.SteamId,0);
                    Jugadoresh.Add(player.SteamId, 0);
                  
                    Timing.Run(Duracion());
                }
                contador = 1;
            }
            if (!Jugadores.ContainsKey(ev.Player.SteamId)){ Jugadores.Add(ev.Player.SteamId, 0); Jugadoresh.Add(ev.Player.SteamId, 0);}
            if(ev.Player.TeamRole.Team != Team.SCP){ ev.Player.GiveItem(ItemType.FRAG_GRENADE); }
        }


        public static IEnumerable<float> Respawn(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");
            
            yield return 5f;
            player.ChangeRole(rolep);          
        }
   

        public static IEnumerable<float> RespawnD1(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");

            yield return 5f;
            player.ChangeRole(rolep);
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.SCIENTIST).First());
        }
        public static IEnumerable<float> RespawnD2(Player player)
        {
            var rolep = player.TeamRole.Role;
            player.SendConsoleMessage("Respawnearas en 5 segundos," + player.Name, "blue");

            yield return 5f;
            player.ChangeRole(rolep);
            player.Teleport(PluginManager.Manager.Server.Map.GetSpawnPoints(Role.SCP_173).First());
        }

        public static IEnumerable<float> Granada(Player player)
        {         
            
            yield return 5f;
            player.GiveItem(ItemType.FRAG_GRENADE);
        }

        public static IEnumerable<float> GranadaC(Player player)
        {
            yield return 35f;
            player.GiveItem(ItemType.FLASHBANG);
        }
        public static IEnumerable<float> GranadaP1(Player player,Vector x)
        {
            yield return 3.5f;
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, x, false, x, true, 0);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, x, false, x, true, 0);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, x, false, x, true, 0);
        }

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            
            grandapos = ev.Direction;
            if (Jugadoresh[ev.Player.SteamId] >= 5) { Timing.Run(GranadaP1(ev.Player,grandapos)); }
            if (ev.GrenadeType == ItemType.FRAG_GRENADE)
            {
                Timing.Run(Granada(ev.Player));
            }
            if (ev.GrenadeType == ItemType.FLASHBANG){ Timing.Run(GranadaC(ev.Player)); }
            if ((Jugadoresh[ev.Player.SteamId] >= 7) && (!ev.Player.HasItem(ItemType.CUP))){ Timing.Run(Multishot(ev.Player,grandapos)); }
        }

        public void OnSetSCPConfig(SetSCPConfigEvent ev)
        {
            ev.Ban049 = true;
            ev.Ban079 = true;
            ev.Ban096 = true;
            ev.Ban106 = true;
            ev.Ban173 = true;
            ev.Ban939_53 = true;
            ev.Ban939_89 = true;
            
        }

        public void OnSetConfig(SetConfigEvent ev)
        {
            switch (ev.Key)
            {
                case "team_respawn_queue":                 
                    ev.Value = "3434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343434343";
                    break;
                case "auto_warhead_start":
                    ev.Value = 1800;
                    break;
                case "auto_warhead_start_lock":
                    ev.Value = false;
                    break;
                case "default_item_classd":
                    ev.Value = new int[] { 25,26 };
                    break;
                case "default_item_scientist":
                    ev.Value = new int[] { 25,26 };
                    break;
                case "minimum_MTF_time_to_spawn":
                    ev.Value = 10000;
                    break;
                case "maximum_MTF_time_to_spawn":
                    ev.Value = 10000;
                    break;
            }
        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
           if(Scientists > Dboys) { ev.Status = ROUND_END_STATUS.MTF_VICTORY;  }
           if (Dboys > Scientists) { ev.Status = ROUND_END_STATUS.CI_VICTORY; }

        }

    

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.RefreshConfig();
        }
        public static IEnumerable<float> Duracion()
        {
            yield return 900f;
            foreach(Player player in PluginManager.Manager.Server.GetPlayers())
            {
                player.Kill(DamageType.FRAG);
            }
        
        }
        
        public void OnRoundEnd(RoundEndEvent ev)
        {
       
            foreach (KeyValuePair<string,int> key in Jugadores)
            {
                var1 = key.Value;
                if(var1 > var2) { var2 = var1; MVP = key.Key; }               
            }
            foreach (Player player in PluginManager.Manager.Server.GetPlayers())
            {
                if(MVP == player.SteamId){ mejor = player.Name; }
            }

            if (Scientists > Dboys)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "Ganan Los Científicos, el mejor jugador fué " + mejor, false);
            }
            if(Dboys > Scientists)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "Ganan Los Clases D, el mejor jugador  fué " + mejor, true);
            }
            if(Dboys == Scientists)
            {
                PluginManager.Manager.Server.Map.Broadcast(6, "¡EMPATE xD!, El mejor jugador fué " + mejor, true);
            }

        }

        public void OnElevatorUse(PlayerElevatorUseEvent ev)
        {
            ev.AllowUse = false;
        }

        public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
        {
            if(ev.Item.ItemType == ItemType.COIN) { ev.Item.Remove(); }
           if((ev.Item.ItemType == ItemType.FRAG_GRENADE)&&(ev.Item.InWorld == true)) { ev.Allow = false; }
            if((ev.Item.ItemType == ItemType.COM15)|| (ev.Item.ItemType == ItemType.USP) || (ev.Item.ItemType == ItemType.P90) || (ev.Item.ItemType == ItemType.LOGICER) || (ev.Item.ItemType == ItemType.E11_STANDARD_RIFLE) || (ev.Item.ItemType == ItemType.MP4))
            {
                ev.ChangeTo = ItemType.FRAG_GRENADE;
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            plugin.RefreshConfig();
        }


        public static IEnumerable<float> Granandat(Player player)
        {
             yield return 10f;
            player.GiveItem(ItemType.FRAG_GRENADE);
        }

   

        public static IEnumerable<float> Multishot(Player player,Vector z)
        {
            Vector pos = player.GetPosition();

            player.GiveItem(ItemType.CUP);
            yield return 0.5f;
            pos = player.GetPosition();
            player.ThrowGrenade(ItemType.FLASHBANG, true,z,false,pos,true,0);
            yield return 0.5f;
            pos = player.GetPosition();
            player.ThrowGrenade(ItemType.FRAG_GRENADE, false, z, false,pos, true, 1);
            yield return 0.5f;
            pos = player.GetPosition();
            player.ThrowGrenade(ItemType.FRAG_GRENADE, false, z, false,pos, true, 2);

        }

        public void OnPlayerDropItem(PlayerDropItemEvent ev)
        {
            Timing.Run(Granandat(ev.Player));
            if((Jugadoresh[ev.Player.SteamId] >= 2)&&(ev.Item.ItemType == ItemType.COIN)){ Timing.Run(Bombatemp(ev.Player, ev.Item.GetPosition())); }
          
        }

        public static IEnumerable<float> Bombatemp(Player player, Vector y)
        {
            Vector posicionini = player.GetPosition();
            yield return 10f;
            player.SetGodmode(true);
            player.Teleport(y);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, y, true, y,true,0);          
            player.Teleport(posicionini);
            player.SetGodmode(false);
            yield return 1f;
            player.SetGodmode(true);
            player.Teleport(y);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, y, false, y, true, 0);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, y, false, y, true, 0);
            player.ThrowGrenade(ItemType.FRAG_GRENADE, true, y, false, y, true, 0);
            player.Teleport(posicionini);
            player.GiveItem(ItemType.COIN);
            player.SetGodmode(false);
        }
    }
}



