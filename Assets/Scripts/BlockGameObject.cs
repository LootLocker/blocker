using UnityEngine;

[System.Serializable]
public class Block
{
    // Position
    public float x;
    public float y;
    public float z;

    // Color
    public float r;
    public float g;
    public float b;

    public void Init(Vector3 position, Color color)
    {
        x = position.x;
        y = position.y;
        z = position.z;

        r = color.r;
        g = color.g;
        b = color.b;
    }
}
public class BlockGameObject : MonoBehaviour
{
    public Block block;
}
