using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
    public void NewGame()
    {
        NetworkManager.singleton.StopHost();
        SceneManager.LoadScene(0);
        
            Destroy(GameObject.Find("NetworkManager")); 
    }
}
