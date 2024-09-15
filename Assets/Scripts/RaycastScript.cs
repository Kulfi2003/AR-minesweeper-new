using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;


public class RaycastScript : MonoBehaviour
{
    public GameObject[] prefabToSpawn; // Assign your prefab here in the Inspector
    private ARRaycastManager _raycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    [SerializeField] private ARPlaneManager _planeManager;
    //game state bools
    public bool positioningGame = true;
    public bool gameOver = false;

    public GraphicRaycaster uiRaycaster; // Assign this in the inspector (Canvas GraphicRaycaster)
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Start()
    {
        // Initialize the EventSystem
        eventSystem = EventSystem.current;

        if (!PlayerPrefs.HasKey("difficulty")) PlayerPrefs.SetInt("difficulty", 0); // setting the difficulty in case none is specified
        _raycastManager = GetComponent<ARRaycastManager>(); // getting raycast manager reference
    }

    void Update()
    {
        if (!gameOver)
        {
            if (positioningGame) PositionGame();
            else GamePlay();
        }
    }

    void PositionGame()
    {
        // Check if the user is interacting with the UI
        if (IsTouchOverUI())
        {
            // If true, skip raycasting
            return;
        }

        if (Input.touchCount == 1)
        {
            // Single touch: Position the object
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                if (_raycastManager.Raycast(touchPosition, _hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = _hits[0].pose;

                    GameObject existingPrefab = GameObject.FindWithTag("GameManager");

                    if (existingPrefab != null)
                    {
                        existingPrefab.transform.position = hitPose.position;
                        existingPrefab.transform.rotation = hitPose.rotation;
                    }
                    else
                    {
                        Debug.Log("Difficulty level should be set to : " + PlayerPrefs.GetInt("difficulty"));
                        GameObject newPrefab = Instantiate(prefabToSpawn[PlayerPrefs.GetInt("difficulty")], hitPose.position, hitPose.rotation);
                        newPrefab.transform.localScale = Vector3.one * 0.1f;
                        newPrefab.tag = "GameManager";
                    }
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            // Two fingers: Scale and rotate the object
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Calculate previous and current positions of touches
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Scale the object
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            GameObject existingPrefab = GameObject.FindWithTag("GameManager");
            if (existingPrefab != null)
            {
                float scaleFactor = 0.0001f * deltaMagnitudeDiff; // Adjust scale speed here
                existingPrefab.transform.localScale += Vector3.one * scaleFactor;

                // Prevent scaling from becoming too small or too large
                existingPrefab.transform.localScale = Vector3.Max(existingPrefab.transform.localScale, Vector3.one * 0.1f * 0.05f);
                existingPrefab.transform.localScale = Vector3.Min(existingPrefab.transform.localScale, Vector3.one * 2.0f * 0.05f);
            }

            // Rotate the object
            Vector2 prevDirection = touchZeroPrevPos - touchOnePrevPos;
            Vector2 currentDirection = touchZero.position - touchOne.position;
            float angle = Vector2.SignedAngle(prevDirection, currentDirection);

            if (existingPrefab != null)
            {
                existingPrefab.transform.Rotate(Vector3.down, angle, Space.World);
            }
        }
    }



    private GameObject touchedTile; // To store the tile touched on "TouchPhase.Began"
    [SerializeField] private float pressTime = 0f;

    void GamePlay()
    {
        // Check if the user is interacting with the UI
        if (IsTouchOverUI())
        {
            // If true, skip raycasting
            return;
        }

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = touch.position;

            // Check if the touch has just begun
            if (touch.phase == TouchPhase.Began)
            {
                pressTime = 0f;
                CheckTileTouched(touchPosition);
                StartCoroutine(StartTime());
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (_raycastManager.Raycast(touchPosition, _hits))
                {
                    Ray ray = Camera.main.ScreenPointToRay(touchPosition);
                    RaycastHit raycastHit;

                    if (Physics.Raycast(ray, out raycastHit))
                    {
                        // Check if the object hit has the tag "Tile"
                        if (raycastHit.collider.gameObject.CompareTag("Tile"))
                        {
                            // Store the tile that was touched
                            if (touchedTile != raycastHit.collider.gameObject)
                            {
                                //Highlight tile
                                touchedTile.GetComponent<TileManager>().TileTouchingStartEnd(false);
                            }
                        }
                    }
                }
            }

            // Check if the touch ended (lifted finger)
            if (touch.phase == TouchPhase.Ended)
            {
                //Remove highlight from tile
                touchedTile.GetComponent<TileManager>().TileTouchingStartEnd(false);

                CheckTilePressed(touchPosition);
                StopAllCoroutines();
            }
        }
    }

    private IEnumerator StartTime()
    {
        while (true)
        {
            pressTime += Time.deltaTime;
            yield return null;
        }
    }

    // Check if the touch started on a "Tile" object
    private void CheckTileTouched(Vector2 touchPosition)
    {
        // Perform an AR raycast to check for planes and objects
        if (_raycastManager.Raycast(touchPosition, _hits))
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                // Check if the object hit has the tag "Tile"
                if (raycastHit.collider.gameObject.CompareTag("Tile"))
                {
                    // Store the tile that was touched
                    touchedTile = raycastHit.collider.gameObject;

                    //Highlight tile
                    touchedTile.GetComponent<TileManager>().TileTouchingStartEnd(true);
                }
            }
        }
    }

    // Check if the touch ended on the same "Tile" object (registering a press)
    private void CheckTilePressed(Vector2 touchPosition)
    {
        if (_raycastManager.Raycast(touchPosition, _hits, TrackableType.Planes))
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                // Check if the object hit has the tag "Tile" and if it's the same object as touchedTile
                if (raycastHit.collider.gameObject.CompareTag("Tile") && raycastHit.collider.gameObject == touchedTile)
                {
                    // The tile was pressed (touched and released on the same tile)
                    HandleTilePress(raycastHit.collider.gameObject);
                }
            }
        }

        // Clear the reference after the touch ends
        touchedTile = null;
    }

    // This method handles the object with the "Tile" tag being pressed
    private void HandleTilePress(GameObject tile)
    {
        if (pressTime > 0.4f)
        {
            Debug.Log("Pressed Tile: " + tile.name);
            tile.GetComponent<TileManager>().toggleFlag();
        }
        else
        {
            Debug.Log("Touched Tile: " + tile.name);
            tile.GetComponent<TileManager>().OpenTile();
        }
    }

    public void RepositionGame(bool reposition)
    {
        positioningGame = reposition;
        _planeManager.SetTrackablesActive(reposition);
    }

    private bool IsTouchOverUI()
    {
        // Set up a new PointerEventData
        pointerData = new PointerEventData(eventSystem);

        // Use the current touch position
        if (Input.touchCount > 0)
        {
            pointerData.position = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButton(0)) // For testing in the editor
        {
            pointerData.position = Input.mousePosition;
        }
        else
        {
            return false;
        }

        // Create a list to hold the results of the raycast
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerData, results);

        // Return true if any UI elements were hit
        return results.Count > 0;
    }

}
