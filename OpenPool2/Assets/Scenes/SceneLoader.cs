using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGamesMenu() => SceneManager.LoadScene("Games Menu");
    public void LoadSettingsMenu() => SceneManager.LoadScene("Settings Menu");
}
