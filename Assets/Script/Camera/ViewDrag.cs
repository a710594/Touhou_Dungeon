using UnityEngine;
using System.Collections;

public class ViewDrag : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;

    private bool _isDrag = false;

    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    float z = 0.0f;

    public void StartDrag(Vector3 mousePosition)
    {
        hit_position = mousePosition;
        camera_position = transform.position;
        _isDrag = true;
    }

    public void OnDrag(Vector3 mousePosition)
    {
        current_position = mousePosition;
        LeftMouseDrag();
    }

    public void EndDrag()
    {
        _isDrag = false;
    }

    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.  
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        direction = Vector2.ClampMagnitude(direction * -10, 20);
        if (direction.magnitude < 1)
        {
            direction = Vector2.zero;
        }
        Rigidbody2D.velocity = direction;
        //Vector3 position = camera_position + direction;
        //transform.position = position;
    }

    private void Update()
    {
        if (!_isDrag)
        {
            Rigidbody2D.velocity = Rigidbody2D.velocity * 0.95f;
        }
    }
}