using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviour
{  

    private void Awake() {
        Application.targetFrameRate = 60;
    }

    public void LoadGame(int i)
    {
        SceneManager.LoadScene(i);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
