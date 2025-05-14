using UnityEngine;
using UnityEngine.UI;

public class CurrentWeaponSwitcher : MonoBehaviour
{
    // Reference to the Image component
    public Image uiImage;
    
    // Sprites to switch between
    public Sprite sprite1;
    public Sprite sprite2;

    public Sprite sprite3;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)){  
                uiImage.sprite = sprite1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){  
                uiImage.sprite = sprite2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)){  
                uiImage.sprite = sprite3;
        }

    }
}
