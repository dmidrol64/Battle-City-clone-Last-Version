using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRessurection : MonoBehaviour
{

    [SerializeField] private int playerGlobalLives = 2;
    [SerializeField] private int playerNumberOfHits;


    public int PlayerNumberOfHits
    {
        get { return playerNumberOfHits; }

        set {  playerNumberOfHits = value; }
    }

    public int PlayerGlobalLives
    {  get { return playerGlobalLives; }
        set { playerGlobalLives = value; }
    }
   
}
