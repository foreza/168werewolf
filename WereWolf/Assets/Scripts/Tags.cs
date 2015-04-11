using UnityEngine;
using System.Collections;

public class Tags : MonoBehaviour 
{
    //Human Players
    public const string HUMAN = "Human";

    //Werewolf Players (Possibly bots)
    public const string WEREWOLF = "Werewolf";

    //Traps that can harm humans
    public const string TRAP = "Trap";

    //Terrain that NO ONE can pass
    public const string HARDTERRAIN = "HardTerrain";

    //Terrain that WEREWOLVES can pass
    public const string SOFTTERRAIN = "SoftTerrain";

    //Towers can potentially be activated to activate the victory zone
    public const string TOWER = "Tower";

    //HUMANS ONLY. Werewolves cannot enter
    public const string HAVEN = "Haven";

    //If a human reaches here, they win.
    public const string VICTORYZONE = "VictoryZone";
	
}
