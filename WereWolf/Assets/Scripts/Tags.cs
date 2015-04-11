using UnityEngine;
using System.Collections;

public class Tags : MonoBehaviour 
{
    //Human Players
    string HUMAN = "Human";

    //Werewolf Players (Possibly bots)
    string WEREWOLF = "Werewolf";

    //Traps that can harm humans
    string TRAP = "Trap";

    //Terrain that NO ONE can pass
    string HARDTERRAIN = "HardTerrain";

    //Terrain that WEREWOLVES can pass
    string SOFTTERRAIN = "SoftTerrain";

    //Towers can potentially be activated to activate the victory zone
    string TOWER = "Tower";

    //HUMANS ONLY. Werewolves cannot enter
    string HAVEN = "Haven";

    //If a human reaches here, they win.
    string VICTORYZONE = "VictoryZone";
	
}
