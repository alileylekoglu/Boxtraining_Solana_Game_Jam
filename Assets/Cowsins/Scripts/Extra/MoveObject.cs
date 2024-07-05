using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Vector3 pointA; // Starting point
    public Vector3 pointB; // Ending point
    public float duration = 5.0f; // Duration of the movement

    private float elapsedTime = 0f;
    private bool movingToB = true;

    void Update()
    {
        // Increment the elapsed time by the time passed since last frame
        elapsedTime += Time.deltaTime;

        // Calculate the interpolation factor
        float t = elapsedTime / duration;

        // Move the object from point A to point B or from point B to point A
        if (movingToB)
        {
            transform.position = Vector3.Lerp(pointA, pointB, t);
        }
        else
        {
            transform.position = Vector3.Lerp(pointB, pointA, t);
        }

        // If the object has reached point B or point A, reverse the direction
        if (t >= 1.0f)
        {
            elapsedTime = 0f; // Reset elapsed time
            movingToB = !movingToB; // Reverse the direction
        }
    }

    // Draw the path in the editor for visualization
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointA, pointB);
    }
}
