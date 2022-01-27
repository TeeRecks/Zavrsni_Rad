using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionScript : MonoBehaviour
{
    private string[] lineArray = new string[] { "", "", "", "", "", "" };

    private void Start()
    {
        Output();
    }

    private void Output()
    {
        string output = "";
        foreach (string s in lineArray)
        {
            output += s + "\n";
        }
        this.GetComponent<TextMeshProUGUI>().text = output;
    }

    public void AddLine(string text)
    {
        if (lineArray[5] != "")
        {
            for (int i = 0; i < lineArray.Length - 1; i++)
            {
                lineArray[i] = lineArray[i + 1];
            }
            lineArray[5] = text;
        }
        else
        {
            for (int i = 0; i < lineArray.Length; i++)
            {
                if (lineArray[i] == "")
                {
                    lineArray[i] = text;
                    break;
                }
            }
        }

        Output();
    }
}
