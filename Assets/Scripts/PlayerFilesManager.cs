using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System;
using UnityEngine.UI;

[Serializable]
public class SaveFile
{
    public SaveFile()
    {
    }
    public SaveFile(List<Block> blocks, Vector3 playerPosition)
    {
        this.blocks = blocks;
        // Add x,y,z of playerPosition to the playerPosition array
        this.playerPosition[0] = playerPosition.x;
        this.playerPosition[1] = playerPosition.y;
        this.playerPosition[2] = playerPosition.z;
    }
    // Savedata
    //------------
    public List<Block> blocks;
    public float[] playerPosition = new float[3];
    //------------

    public Vector3 GetPlayerPosition()
    {
        return new Vector3(playerPosition[0], playerPosition[1], playerPosition[2]);
    }

}

public class PlayerFilesManager : MonoBehaviour
{
    public SaveFile saveFile;

    public SaveFileUI saveFileUIPrefab;

    public Transform fileCanvasTransform;

    public static PlayerFilesManager instance;

    public BlockController blockController;

    private float autosaveFrequency = 10f;

    private Coroutine autoSaveCoroutine;

    public bool saveOnDisk;

    public Transform playerTransform;

    public Text informationText;

    private string lastSaveTime;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void RemoveAllChildren()
    {
        foreach (Transform child in fileCanvasTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public void GetFiles()
    {
        LootLockerSDKManager.GetAllPlayerFiles((filesResponse) =>
        {
            if(filesResponse.success)
            {
                // Add one "new file" at the top
                AddUIFile("New save file", "", 0);
                for (int i = 0; i < filesResponse.items.Length; i++)
                {
                    LootLockerPlayerFile currentItem = filesResponse.items[i];
                    AddUIFile(currentItem.name, currentItem.url, currentItem.id);
                }
            }
            else
            {
                Debug.Log(filesResponse.Error);
            }
        });
    }

    private void AddUIFile(string saveName, string url, int id)
    {
        SaveFileUI newSaveFile = Instantiate(saveFileUIPrefab, fileCanvasTransform);
        newSaveFile.SetSaveFile(saveName, url, id);
    }

    public void Load(int id)
    {
        LootLockerSDKManager.GetPlayerFile(id, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Retrieved file!");
                lastSaveTime = PlayerPrefs.GetString("lastSaveTime"+id.ToString(), "Never");
                StartCoroutine(Download(response.url, (data) =>
                {
                    SaveFile savefile = null;
                    savefile = JsonConvert.DeserializeObject<SaveFile>(data);
                    List<Block> allBlocks = savefile.blocks;
                    StartCoroutine(blockController.LoadRoutine(savefile.blocks));
                    GameManager.instance.PlayGame(id, savefile.GetPlayerPosition());
                }));
            }
        });
    }

    public void StartAutoSave(int fileID)
    {
        
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave(fileID));
    }

    public void StopAutoSave()
    {
        StopCoroutine(autoSaveCoroutine);
    }

    IEnumerator AutoSave(int fileID)
    {
        if (blockController.dataLoaded == true)
        {
            Debug.Log("AutoSave");
            Save(fileID);
        }
        float timer = 0f;
        while (timer < autosaveFrequency)
        {
            timer += Time.deltaTime;
            informationText.text = "Move with WASD & mouse\n" +
                "Save on disk is " + (saveOnDisk ? "enabled" : "disabled") +"\n"+
                "Last save: <color=green>" + lastSaveTime + "</color>\n" +
                "Autosaving in <color=yellow>" + (autosaveFrequency-timer).ToString("0.##")+ "</color>";
            yield return null;
        }
        autoSaveCoroutine = StartCoroutine(AutoSave(fileID));
    }

    public static string WriteToFile(string fileName, string content)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(content);
        writer.Close();
        Debug.Log(content);
        return path;
    }
    public static string ReadFromFile(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        StreamReader reader = new StreamReader(path);
        string content = reader.ReadToEnd();
        reader.Close();
        Debug.Log(content);
        return content;
    }

    public IEnumerator CreateSaveFileRoutine()
    {
        bool done = false;
        int id = 0;
        // Save as a byte array to not store anything on disk, only in memory
        // Can be good in some cases in WebGL where you are sometimes not allowed to store data for example
        saveFile = new SaveFile();

        byte[] fileByteArray = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SaveFile()));
        if(saveOnDisk)
        {
            // Save the file to disk
            WriteToFile("save.txt", JsonConvert.SerializeObject(saveFile));
        }

        LootLockerSDKManager.UploadPlayerFile(fileByteArray, "savefile", "save.txt", (response) =>
        {
            Debug.Log(response.text);
            
            id = response.id;
            done = true;
        });
        yield return new WaitUntil(() => done == true);

        // Start playing the game with the ID from the returned file
        GameManager.instance.PlayGame(id, Vector3.zero);
    }

    [ContextMenu("LoadFromFile")]
    public void Load()
    {
        string content = ReadFromFile("save.txt");
        SaveFile savefile = JsonConvert.DeserializeObject<SaveFile>(content);
        List<Block> allBlocks = savefile.blocks;
        StartCoroutine(blockController.LoadRoutine(allBlocks));
    }

    [ContextMenu("Save")]
    public void Save(int fileID)
    {
        string saveString = "";
        saveFile = new SaveFile(blockController.blocks, playerTransform.position);
        saveString = JsonConvert.SerializeObject(saveFile, Formatting.None, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        if (saveOnDisk == true)
        {
            // Save as a file and updte it on LootLocker as well
            string path = WriteToFile("save.txt", saveString);
            LootLockerSDKManager.UpdatePlayerFile(fileID, path, (response) =>
            {
                if(response.success)
                {
                    Debug.Log("Updated file!");
                    // Save the time of the last save in the format of MMMM dd, yyyy HH:mm:ss
                    lastSaveTime = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");
                    PlayerPrefs.SetString("lastSaveTime"+fileID.ToString(), lastSaveTime);
                }
                else
                {
                    Debug.Log("Error"+response.Error);
                }
            });
        }
        else
        {
            // Save as a byte array to not store anything on disk, only in memory
            // Can be good in some cases in WebGL where you are sometimes not allowed to store data for example
            Debug.Log(saveString);
            byte[] fileByteArray = Encoding.UTF8.GetBytes(saveString);
            LootLockerSDKManager.UpdatePlayerFile(fileID, fileByteArray, (response) =>
            {
                if(response.success)
                {
                    Debug.Log("Updated file!");
                    // Save the time of the last save in the format of MMMM dd, yyyy HH:mm:ss
                    lastSaveTime = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");
                    PlayerPrefs.SetString("lastSaveTime" + fileID.ToString(), lastSaveTime);
                }
                else
                {
                    Debug.Log("Error"+response.Error);
                }
            });
        }
    }

    IEnumerator Download(string url, System.Action<string> fileContent)
    {
        UnityWebRequest www = new UnityWebRequest(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            fileContent(www.downloadHandler.text);
        }
    }
}
