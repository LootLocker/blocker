using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LootLocker.Requests;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public CurveAnimatorUI loginCanvas;
    public CurveAnimatorUI playCanvas;
    public CurveAnimatorUI filesCanvas;

    public PlayerFilesManager playerFilesManager;

    public PlayerController playerController;

    public float menuRotationSpeed;

    private int currentFileID;

    private void Awake()
    {
        instance = this;
    }

    public enum GameState { Menu, Playing };
    public GameState gameState;

    public void PlayGame(int fileID, Vector3 playerPosition)
    {
        loginCanvas.Hide();
        playCanvas.Show();
        gameState = GameState.Playing;
        playerController.LockMouse();
        currentFileID = fileID;
        playerController.transform.position = playerPosition;
        playerFilesManager.StartAutoSave(fileID);
    }

    public void BackToFileSelect()
    {
        // Save when player quits
        playerFilesManager.StopAutoSave();
        playerFilesManager.Save(currentFileID);
        BlockController.instance.dataLoaded = false;
        gameState = GameState.Menu;
        playCanvas.Hide();
        loginCanvas.Show();
        filesCanvas.Show();
        playerController.UnlockMouse();
        playerController.ResetRotation();
        BlockController.instance.DestroyAllBlocks();
        playerFilesManager.RemoveAllChildren();
        BlockController.instance.dataLoaded = false;
        playerFilesManager.GetFiles();
    }

    void Update()
    {
        if(gameState == GameState.Menu)
        {
            playerController.transform.localEulerAngles += new Vector3(0, Time.deltaTime* menuRotationSpeed, 0);
        }
    }

    public void CreateSaveFileAndPlay()
    {
        StartCoroutine(playerFilesManager.CreateSaveFileRoutine());
    }

    
}
