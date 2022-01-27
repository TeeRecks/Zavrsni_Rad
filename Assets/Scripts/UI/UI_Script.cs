using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Script : MonoBehaviour
{
    public CanvasGroup uiCanvasGroup;
    public CanvasGroup menuCanvasGroup;
    public CanvasGroup confirmQuitCanvasGroup;
    public CanvasGroup confirmMainMenuCanvasGroup;

    public GameObject MainCamera;
    public CameraAndControlsScript controlsScript;
    public TileMap tileMap;

    private void Awake()
    {
        menuCanvasGroup.gameObject.SetActive(true);
        confirmQuitCanvasGroup.gameObject.SetActive(true);
        confirmMainMenuCanvasGroup.gameObject.SetActive(true);
    }

    private void Start()
    {
        activateUI();

        deactivateCanvasGroup(menuCanvasGroup);
        deactivateCanvasGroup(confirmQuitCanvasGroup);
        deactivateCanvasGroup(confirmMainMenuCanvasGroup);
    }

    private void deactivateCanvasGroup(CanvasGroup canvas, float alpha = 0)
    {
        canvas.alpha = alpha;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    private void activateCanvasGroup(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    private void deactivateUI(float alpha = 0.5f)
    {
        uiCanvasGroup.alpha = alpha;
        uiCanvasGroup.interactable = false;
    }

    private void activateUI()
    {
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
    }

    public void DisableControls()
    {

        controlsScript.DisableMovement();
        tileMap.DisableAttackMove();
    }

    public void EnableControls()
    {
        controlsScript.EnableMovement();
        tileMap.EnableAttackMove();
    }

    // UI gumbi
    public void OpenMenu()
    {
        deactivateUI();
        activateCanvasGroup(menuCanvasGroup);
        DisableControls();
    }

    // menu gumbi
    public void ResumeGameButton()
    {
        activateUI();
        //uiCanvasGroup.alpha = 1;
        //uiCanvasGroup.interactable = true;

        deactivateCanvasGroup(menuCanvasGroup);
        //menuCanvasGroup.alpha = 0;
        //menuCanvasGroup.interactable = false;
        //menuCanvasGroup.blocksRaycasts = false;

        EnableControls();
    }

    public void MainMenuButton()
    {
        deactivateCanvasGroup(menuCanvasGroup, 0.5f);
        activateCanvasGroup(confirmMainMenuCanvasGroup);
    }

    public void QuitButton()
    {
        //smanjenje vidljivosti menija i isključivanje interakcije

        deactivateCanvasGroup(menuCanvasGroup, 0.5f);
        //menuCanvasGroup.alpha = 0.5f;
        //menuCanvasGroup.interactable = false;
        //menuCanvasGroup.blocksRaycasts = false;

        //omogućiti interakciju i vidljivost pop-up menija
        activateCanvasGroup(confirmQuitCanvasGroup);
        //confirmQuitCanvasGroup.alpha = 1;
        //confirmQuitCanvasGroup.interactable = true;
        //confirmQuitCanvasGroup.blocksRaycasts = true;
    }

    // za main menu pop-up

    public void MainMenuNO()
    {
        activateCanvasGroup(menuCanvasGroup);
        deactivateCanvasGroup(confirmMainMenuCanvasGroup);
    }

    public void MainMenuYES()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // za exit pop-up
    public void QuitGameNO()
    {
        // povećavanje vidljivosti menija i omogućavanje interakcije
        activateCanvasGroup(menuCanvasGroup);
        //menuCanvasGroup.alpha = 1;
        //menuCanvasGroup.interactable = true;
        //menuCanvasGroup.blocksRaycasts = true;

        // micanje pop-up menija
        deactivateCanvasGroup(confirmQuitCanvasGroup);
        //confirmQuitCanvasGroup.alpha = 0;
        //confirmQuitCanvasGroup.interactable = false;
        //confirmQuitCanvasGroup.blocksRaycasts = false;
    }

    public void QuitGameYES()
    {
        Application.Quit();
    }
}
