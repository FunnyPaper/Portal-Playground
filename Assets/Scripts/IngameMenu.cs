using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] GameObject Menu = null;
    [SerializeField] GameObject Main = null;
    [SerializeField] GameObject Options = null;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);
            Main.SetActive(!Main.activeSelf);

            if (Menu.activeSelf)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Resume();
            }
        }
    }
    public void Resume()
    {
        Menu.SetActive(false);
        Options.SetActive(false);
        Main.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ShowOptions()
    {
        Main.SetActive(false);
        Options.SetActive(true);
    }

    public void HideOptions()
    {
        Main.SetActive(true);
        Options.SetActive(false);
    }

    public void SetVolume(Slider slider)
    {
        AudioListener.volume = slider.value;
    }

}
