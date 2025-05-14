using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour

{

    public Image healthBar;

    public TextMeshProUGUI deathText;

    bool respawn = false;

    public float healthAmount = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathText.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (healthAmount <= 0){
            
            Time.timeScale = 0f;

            deathText.text = "You Died!, Press r to Respawn!";
            deathText.gameObject.SetActive(true);

            if (Input.GetKeyDown(KeyCode.R))
            {
                respawn = true; 
                Time.timeScale = 1f;
            }

            if (respawn){
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }

            
            
            
        }
        if (Input.GetKeyDown(KeyCode.Comma)){
            takeDamage(20);
        }
        if (Input.GetKeyDown(KeyCode.Period)){
            heal(5);
        }
        
    }

    public void takeDamage (float damage){
        healthAmount -= damage;
        healthBar.fillAmount = healthAmount / 100f;
    }

    public void heal(float healingAmount){
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);
        
        healthBar.fillAmount = healthAmount / 100f;
    }
}
