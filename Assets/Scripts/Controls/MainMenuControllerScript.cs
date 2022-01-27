using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControllerScript : MonoBehaviour
{
    public CanvasGroup menuCanvasGroup;
    public CanvasGroup confirmationPopupCanvasGroup;

    public GameObject howToPlayWindow;

    public InputField inputField;

    public Text ActualSeed;

    int seed;

    private void Awake()
    {
        QuitGameNO();
    }

    private void Start()
    {
        ValueChangeCheck();
    }

    /// <summary>
    /// Odlazak na scenu "Game"
    /// </summary>
    public void ButtonStartNewGame()
    {
        GameValues.InintializeValues();
        GameValues.mapSeed = seed;
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Pop-up za izlaz iz igre
    /// </summary>
    public void ButtonQuit()
    {
        Debug.Log("Check form quit confirmation");

        //smanjenje vidljivosti menija i isključivanje interakcije
        menuCanvasGroup.alpha = 0.5f;
        menuCanvasGroup.interactable = false;
        menuCanvasGroup.blocksRaycasts = false;

        //omogućiti interakciju i vidljivost pop-up menija
        confirmationPopupCanvasGroup.alpha = 1;
        confirmationPopupCanvasGroup.interactable = true;
        confirmationPopupCanvasGroup.blocksRaycasts = true;
    }

    // za pop-up
    /// <summary>
    /// Pritisak na NO
    /// </summary>
    public void QuitGameNO()
    {
        Debug.Log("Back to the game");

        menuCanvasGroup.alpha = 1;
        menuCanvasGroup.interactable = true;
        menuCanvasGroup.blocksRaycasts = true;

        confirmationPopupCanvasGroup.alpha = 0;
        confirmationPopupCanvasGroup.interactable = false;
        confirmationPopupCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Pritisak na YES
    /// </summary>
    public void QuitGameYES()
    {
        Application.Quit();
    }

    // private
    private bool IsOnlyDigits(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    private int Limit8Digits(int i)
    {
        if (i > 99999999)
        {
            i %= 100000000;
        }
        return i;
    }

    public void ValueChangeCheck()
    {
        string seedT = inputField.text;
        if (seedT == "")
        {
            seed = Random.Range(0, 99999999);
        }
        else if (IsOnlyDigits(seedT))
        {
            seed = int.Parse(seedT);
            seed = Limit8Digits(seed);
        }
        else
        {
            seed = Mathf.Abs(seedT.GetHashCode());
            seed = Limit8Digits(seed);

        }
        ActualSeed.text = seed.ToString();
        Debug.Log(seed);
    }

    // za manual random seed
    public void GenerateRandomSeed()
    {
        seed = Random.Range(0, 99999999);
        ActualSeed.text = seed.ToString();
        inputField.text = seed.ToString();
        Debug.Log(seed);
    }

    // za How to play
    public void ToggleHowToPlay()
    {
        if (howToPlayWindow.activeSelf)
        {
            howToPlayWindow.SetActive(false);
        }
        else
        {
            howToPlayWindow.SetActive(true);
        }
    }
}