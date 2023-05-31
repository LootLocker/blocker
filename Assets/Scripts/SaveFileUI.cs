using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileUI : MonoBehaviour
{
    [SerializeField] private Text saveFileText;

    private string url;

    [SerializeField] private Button button;

    public string URL => url;

    private int ID;

    public void SetSaveFile(string saveFileName, string url, int id)
    {
        saveFileText.text = saveFileName;
        this.url = url;
        this.ID = id;
        button.onClick.AddListener(delegate { LoadSaveFile(); });
    }

    void LoadSaveFile()
    {
        // No save file, create a new one
        if(url == "")
        {
            GameManager.instance.CreateSaveFileAndPlay();
        }
        else
        {
            PlayerFilesManager.instance.Load(ID);
        }
    }
}
