using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Tanks/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] public GameObject BasePlayerContainer;
    [SerializeField] public GameObject VisualPrefab;
    [SerializeField] public GameObject bulletPrefab;

    [SerializeField] public int hits = 1;
    [SerializeField] public int globalLives = 2;
    public float moveSpeed = 300f;

    public int weaponLevel;


}
