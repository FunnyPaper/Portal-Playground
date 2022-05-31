using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject OptionsMenuGroup = null;
    [SerializeField] GameObject MainMenuGroup = null;
    
    void Start()
    {
        Debug.Log("Menu start");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowOptions()
    {
        MainMenuGroup.SetActive(false);
        OptionsMenuGroup.SetActive(true);
    }
    public void HideOptions()
    {
        MainMenuGroup.SetActive(true);
        OptionsMenuGroup.SetActive(false);
    }
    public void SetVolume(Slider slider)
    {
        AudioListener.volume = slider.value;
    }
    public void Play()
    {
        SceneManager.LoadScene("Level_1");
    }
    public void Exit()
    {
        Application.Quit();
    }
}
