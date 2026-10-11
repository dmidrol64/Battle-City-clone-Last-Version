using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHQ : MonoBehaviour,IDamageable
{

  
    [SerializeField] GameObject _eagleHQHonor;
    [SerializeField] GameObject _eagleHQDestrpoyed;


    GameObject honor;
    Vector3 _eaglePosition;

    private int _eagleHit ;

    public int EagleHit
    {
        get { return _eagleHit; }
        set { _eagleHit = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        _eagleHit = 1;
 
        _eaglePosition = new Vector3(0.14f, -13.3f, 0);
   
    }


    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        EagleHit--;

        if (EagleHit < 1)
        {
            EagleDie();
        }
    }

    private void EagleDie()
    {
        Instantiate(_eagleHQDestrpoyed, _eaglePosition, Quaternion.identity);   
        Destroy(gameObject);
        var gameOverMgr = FindAnyObjectByType<GameOverManager>();
        if (gameOverMgr != null)
        {
            gameOverMgr.ShowGameOver();
        }
        else
        {
            Debug.LogError("[Ressurecton] Не найден GameOverManager на сцене!");
        }
        Debug.Log($"У объекта {gameObject.name} осталось попыток: {EagleHit} GameOver");

    }
}
