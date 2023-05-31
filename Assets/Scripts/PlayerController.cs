using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject blockPrefab;

    public Transform head;
    Vector2 rotation = Vector2.zero;
    public float lookSpeed = 3;

    public float clampMinX = 0f;
    public float clampMaxX = 0f;

    bool mouseLocked = true;

    public Rigidbody rb;
    public float movementSpeed = 3;

    Vector3 movement;

    bool jump;

    public float jumpForce;

    public LayerMask groundLayerMask;
    public LayerMask blockLayerMask;

    public Camera headCamera;

    public Transform current3DCursor;

    public GameObject cursor3DPrefab;

    public bool mouseIsLocked;

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
    void Update()
    {
        if(GameManager.instance.gameState == GameManager.GameState.Menu)
        {
            return;
        }

        BlockPlacing();
        Movement();
    }
    public void LockMouse()
    {
        if (mouseIsLocked == false)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            mouseIsLocked = true;
        }
    }

    public void UnlockMouse()
    {
        if (mouseIsLocked == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            mouseIsLocked = false;
        }
    }


    void BlockPlacing()
    {
        RaycastHit hit;

        // Blocks
        if (Physics.Raycast(headCamera.transform.position, headCamera.transform.forward, out hit, 5f, blockLayerMask))
        {
            Vector3 snappedPosition = hit.collider.gameObject.transform.position;
            snappedPosition.x = Mathf.RoundToInt(snappedPosition.x);
            snappedPosition.y = Mathf.RoundToInt(snappedPosition.y);
            snappedPosition.z = Mathf.RoundToInt(snappedPosition.z);

            if (current3DCursor != null)
            {
                if (current3DCursor.position != hit.collider.gameObject.transform.position + hit.normal)
                {
                    current3DCursor.gameObject.GetComponent<MaterialFader>().Disappear();
                    current3DCursor = null;
                }
            }
            if (current3DCursor == null)
            {
                current3DCursor = Instantiate(cursor3DPrefab, hit.collider.gameObject.transform.position + hit.normal, Quaternion.LookRotation(hit.normal * 90f)).transform;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 blockPlacePosition = hit.collider.gameObject.GetComponent<BlockGameObject>().transform.position;
                BlockController.instance.CreateBlock(hit.collider.gameObject.transform.position + hit.normal);
            }
            if (Input.GetMouseButtonDown(1))
            {
                BlockController.instance.RemoveBlock(hit.collider.gameObject);
            }
        }
        else if (Physics.Raycast(headCamera.transform.position, headCamera.transform.forward, out hit, 5f, groundLayerMask))
        {
            Vector3 snappedPosition = hit.point;
            snappedPosition.x = Mathf.RoundToInt(snappedPosition.x);
            snappedPosition.y = Mathf.RoundToInt(snappedPosition.y) - 1;
            snappedPosition.z = Mathf.RoundToInt(snappedPosition.z);
            if (current3DCursor != null)
            {
                if (current3DCursor.position != snappedPosition + hit.normal)
                {
                    current3DCursor.gameObject.GetComponent<MaterialFader>().Disappear();
                    current3DCursor = null;
                }
            }
            if (current3DCursor == null)
            {
                current3DCursor = Instantiate(cursor3DPrefab, snappedPosition + hit.normal, Quaternion.LookRotation(hit.normal * 90f)).transform;
            }
            // Change back offset
            snappedPosition.y += 1;
            if (Input.GetMouseButtonDown(0))
            {
                BlockController.instance.CreateBlock(snappedPosition);
            }
        }
        else
        {
            if (current3DCursor != null)
            {
                current3DCursor.gameObject.GetComponent<MaterialFader>().Disappear();
                current3DCursor = null;
            }
        }
    }

    void Movement()
    {
        rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
        rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotation.x = Mathf.Clamp(rotation.x, clampMinX, clampMaxX);

        head.rotation = Quaternion.Euler((Vector2)rotation);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMouse();
        }

        movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayerMask) || Physics.Raycast(transform.position, Vector3.down, out hit, 2f, blockLayerMask))
            {
                jump = true;
            }
        }
    }



    void FixedUpdate()
    {
        Quaternion newRotation = head.rotation;
        newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);
        Vector3 newVel = (newRotation * movement) * movementSpeed;
        newVel.y = rb.velocity.y;
        rb.velocity = newVel;

        if (jump == true)
        {
            jump = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ToggleMouse()
    {
        mouseLocked = !mouseLocked;
        if (mouseLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
