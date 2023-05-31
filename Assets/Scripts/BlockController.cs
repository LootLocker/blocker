using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LootLocker.Requests;
using System.IO;
using UnityEngine.Networking;
using System.Text;

public class BlockController : MonoBehaviour
{
    public List<Block> blocks = new List<Block>();

    public static BlockController instance;

    public GameObject blockPrefab;

    public GameObject destroyBlockEffect;

    public UIController uiController;

    public bool dataLoaded = false;

    void Awake()
    {
        instance = this;
    }

    public void RemoveBlock(GameObject blockToRemove)
    {
        // Instantiate a new particle effect at the position of the block being removed
        GameObject newParticleEffect = Instantiate(destroyBlockEffect, blockToRemove.transform.position, Quaternion.identity);
        // Get the main settings of the particle system component of the new particle effect
        ParticleSystem.MainModule settings = newParticleEffect.GetComponent<ParticleSystem>().main;

        // Get the Block component of the block being removed
        Block hittedBlock = blockToRemove.GetComponent<BlockGameObject>().block;
        // Create a color variable with the color of the block being removed
        Color blockColor = new Vector4(hittedBlock.r, hittedBlock.g, hittedBlock.b, 1f);
        // Set the start color of the particle system to the color of the block being removed
        settings.startColor = new ParticleSystem.MinMaxGradient(blockColor);
        // Remove the block from the list of blocks
        blocks.Remove(blockToRemove.GetComponent<BlockGameObject>().block);
        // Destroy the block game object
        Destroy(blockToRemove);
    }

    public IEnumerator LoadRoutine(List<Block> allBlocks)
    {
        for (int i = 0; i < allBlocks.Count; i++)
        {
            SpawnSavedBlock(allBlocks[i]);
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
        dataLoaded = true;
    }

    public void SpawnSavedBlock(Block spawnedBlock)
    {
        // Create a Vector3 variable for the spawn position using the x, y, and z values from the "spawnedBlock" parameter
        Vector3 spawnPos = new Vector3(spawnedBlock.x, spawnedBlock.y, spawnedBlock.z); ;
        // Instantiate a new block object using the "blockPrefab" prefab, the "spawnPos" position, and the identity rotation, as a child of the current transform
        GameObject newBlockObject = Instantiate(blockPrefab, spawnPos, Quaternion.identity, transform);

        // Create a Vector4 variable for the block color using the r, g, b values from the "spawnedBlock" parameter
        Vector4 blockColor = new Vector4(spawnedBlock.r, spawnedBlock.g, spawnedBlock.b, 1f);

        // Get the Block component of the new block object
        Block newBlock = newBlockObject.GetComponent<BlockGameObject>().block;
        // Initialize the new block with the "spawnPos" position and "blockColor" color
        newBlock.Init(spawnPos, blockColor);

        // Get the MeshRenderer component of the child of the new block object and set its material color to "blockColor"
        newBlockObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", blockColor);
        // Add the new block to the list of blocks
        blocks.Add(newBlock);
    }


    public void CreateBlock(Vector3 position)
    {
        // Instantiate a new block object using the "blockPrefab" prefab, the "position" position, and the identity rotation, as a child of the BlockController's transform
        GameObject newBlockObject = Instantiate(blockPrefab, position, Quaternion.identity, BlockController.instance.transform);

        // Get the Block component of the new block object
        Block newBlock = newBlockObject.GetComponent<BlockGameObject>().block;
        // Initialize the new block with the "position" position and the current color of the UI controller
        newBlock.Init(position, uiController.currentColor);

        // Get the MeshRenderer component of the child of the new block object and set its material color to the current color of the UI controller
        newBlockObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", uiController.currentColor);
        // Add the new block to the list of blocks
        blocks.Add(newBlock);
    }

    public void DestroyAllBlocks()
    {
        StartCoroutine(DestroyAllBlocksRoutine());
    }

    IEnumerator DestroyAllBlocksRoutine()
    {
        Debug.Log(transform.childCount);
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            RemoveBlock(transform.GetChild(i).gameObject);
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }

}
