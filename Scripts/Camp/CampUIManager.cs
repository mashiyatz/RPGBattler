using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    /* TODO:
     * 1. Create different battles on a map, set different parameters for enemies upon selection. 
     * 2. Map button icons include enemy details that can be accessed when clicked. But have to update listeners. 
     * 3. Player details sortie should only be set up once at game start, and then accessed when recreating Game Objects. 
     * ?. Consider doing 1, 2 in AR, selecting enemy instances instead. 
     * ?. Figure out how to create a database of some kind to keep unit information. 
     */

public class CampUIManager : MonoBehaviour
{
    public void EnterBattleScene()
    {
        List<UnitDetails> playerDetails = new List<UnitDetails>();
        List<UnitDetails> enemyDetails = new List<UnitDetails>();

        // player details should come out of a party script
        playerDetails.Add(new UnitDetails(Name.Ali, Title.Rogue, Alignment.Player, AIBehavior.Aggressive, WeaponID.SteelBow, ArmorID.Leather, AccessoryID.GoldRing));
        playerDetails.Add(new UnitDetails(Name.Layla, Title.Warrior, Alignment.Player, AIBehavior.Aggressive, WeaponID.IronSword, ArmorID.Iron, AccessoryID.Glasses));
        playerDetails.Add(new UnitDetails(Name.Musa, Title.Warrior, Alignment.Player, AIBehavior.Aggressive, WeaponID.IronSword, ArmorID.Iron, AccessoryID.GoldRing));

        enemyDetails.Add(new UnitDetails(Name.Musa, Title.Warrior, Alignment.Enemy, AIBehavior.Aggressive));
        enemyDetails.Add(new UnitDetails(Name.Sahara, Title.Mage, Alignment.Enemy, AIBehavior.Strategic));
        enemyDetails.Add(new UnitDetails(Name.Layla, Title.Rogue, Alignment.Enemy, AIBehavior.Aggressive));
        enemyDetails.Add(new UnitDetails(Name.Ali, Title.Mage, Alignment.Enemy, AIBehavior.Strategic));
        enemyDetails.Add(new UnitDetails(Name.Ali, Title.Warrior, Alignment.Enemy, AIBehavior.Strategic));

        Sortie.EnemyUnitDetails = enemyDetails;
        Sortie.PlayerUnitDetails = playerDetails;

        SceneManager.LoadScene("BattleScene");
    }
}
