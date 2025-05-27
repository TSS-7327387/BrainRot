using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalDrag : MonoBehaviour
{
    private bool hasBeenReleased = false;
    Rigidbody2D rb;
    public float minX = -2.5f; // Minimum horizontal position
    public float maxX = 2.5f;  // Maximum horizontal position
    public ObjectSpawner oSpawner;
    private float startY;
    Transform target;
    public void StartDraging(GameObject obj)
    {
        rb = obj.GetComponent<Rigidbody2D>();
        rb.simulated = false;
        rb.bodyType = RigidbodyType2D.Kinematic; // Don't fall yet
        hasBeenReleased = false;
        target = obj.GetComponent<Transform>();
        startY = target.position.y;
    }


    void Update()
    {
        
        if (hasBeenReleased || oSpawner.gameOver) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Optimized UI check for 2D: skip processing if touch is over any UI element
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) || touch.position.y > Screen.height - (Screen.height / 4))
                return;

            Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
            touchWorldPos.z = 0;
            
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                float clampedX = Mathf.Clamp(touchWorldPos.x, minX, maxX);
                target.position = new Vector3(clampedX, startY, 0);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                ReleaseObject();
            }
        }
    }

    private void ReleaseObject()
    {
        hasBeenReleased = true;
        rb.bodyType = RigidbodyType2D.Dynamic; // Enable gravity
        rb.simulated = true;
        rb = null;
        DOVirtual.DelayedCall(1, () => { oSpawner.SpawnRandomObject(); });
    }
}
