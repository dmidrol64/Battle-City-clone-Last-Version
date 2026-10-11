using UnityEngine;
   

public class PlayerInitializer : MonoBehaviour
{
    public void SetupPlayer(PlayerDataSO playerData)
    {
        if (playerData == null) return;

       
        PlayerRessurection ressurection = FindAnyObjectByType<PlayerRessurection>();
        if (ressurection != null)
        {
          
            ressurection.PlayerGlobalLives = playerData.globalLives;
            ressurection.PlayerNumberOfHits = playerData.hits;
        }
        else
        {
            Debug.LogWarning("[ВНИМАНИЕ] Скрипт PlayerRessurection не найден на сцене!");
        }

       
        LivesComponent lives = GetComponent<LivesComponent>();
        if (lives != null)
        {
            lives.InitPlayerHealth(playerData.hits);
        }

        
        PlayerMove move = GetComponent<PlayerMove>();
        if (move != null)
        {
            // Передаем скорость  игрока
 
            move.speed = playerData.moveSpeed;
        }

        
        Shoot weapon = GetComponentInChildren<Shoot>();
        if (weapon != null)
        {
            // Настраиваем пушку игрока на старте
            weapon.levelWeapon = playerData.weaponLevel;

           
        }

        
        if (playerData.VisualPrefab != null)
        {
            // Спавним графику внутрь контейнера
            GameObject visual = Instantiate(playerData.VisualPrefab, transform);

            
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;

           
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;

           // устанавливаем родной масштаб картинки
            visual.transform.localScale = playerData.VisualPrefab.transform.localScale;
        }
    }
}


