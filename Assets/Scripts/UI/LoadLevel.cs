using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void JustLoadIt()
    {
        SceneManager.LoadScene("Scenes/Main Menu");
    }
}
