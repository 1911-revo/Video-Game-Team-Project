using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class Loader : MonoBehaviour
{
    public GameObject confirmNewGamePanel;

    void Start()
    {
        confirmNewGamePanel.SetActive(false);
    }

    public void NewGame()
    {
        confirmNewGamePanel.SetActive(true);
    }

    public void ConfirmNewGame()
    {
        // DELETE ALL SAVE DATA:
        PlayerPrefs.DeleteAll();
        File.Delete(Application.persistentDataPath + "/SAVE.sav");

        SceneManager.LoadScene("MissionSelect");
    }

    public void CancelNewGame()
    {
        confirmNewGamePanel.SetActive(false);
    }

    public void load()
    {
        SceneManager.LoadScene("MissionSelect");
    }

    public void quit()
    {
        Application.Quit();
    }
}
