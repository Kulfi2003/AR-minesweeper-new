using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cameraTransform;

    void Start()
    {
        // Find the main camera
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            // Get the direction from the billboard to the camera
            Vector3 directionToCamera = transform.position - cameraTransform.position;
            directionToCamera.y = 0; // Ignore the Y axis to prevent tilting

            // Ensure the billboard faces the camera
            transform.rotation = Quaternion.LookRotation(directionToCamera, Vector3.up);
        }
    }
}
