using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int rows = 10;  // Number of rows in the grid
    [SerializeField] int columns = 10;  // Number of columns in the grid
    [SerializeField] int mineCount = 10;  // Number of mines in the grid
    int[][] gridValues;  // 2D array to store the grid values
    bool[][] gridMines;
    GameObject[][] gridTiles;
    RaycastScript raycastScript;

    public int flagsLeft;
    public float timePassed;
    public bool gameStarted = false;

    private void Awake()
    {
        raycastScript = FindObjectOfType<RaycastScript>();
        InitializeUI();
        InitializeGridValues();
        SpawnGrid();
    }

    void InitializeUI()
    {
        flagsLeft = mineCount;
    }

    [SerializeField] private TextMeshProUGUI flagsLeftText;
    [SerializeField] private TextMeshProUGUI timePassedText;
    private void Update()
    {
        //UI update
        if (gameStarted && !raycastScript.gameOver) timePassed += Time.deltaTime; // counting time
        flagsLeftText.text = "Flags left : " + flagsLeft; // updating flags remaining
        timePassedText.text = ConvertToMinutesAndSeconds(timePassed); // showing time
    }

    public string ConvertToMinutesAndSeconds(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);  // Get the total minutes
        int remainingSeconds = Mathf.FloorToInt(seconds % 60);  // Get the remaining seconds

        // Format the string to be "mins:secs" and pad seconds with a leading zero if needed
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }


    #region initializing the values in the board and printing them
    void InitializeGridValues()
    {
        // Initialize the jagged array with the number of rows
        int[][] a = new int[rows][];

        // Initialize each row with the number of columns
        for (int i = 0; i < rows; i++)
        {
            a[i] = new int[columns];  // Each row is an array of 'columns' length
        }

        // Initialize the jagged array with the number of rows
        bool[][] b = new bool[rows][];

        // Initialize each row with the number of columns
        for (int i = 0; i < rows; i++)
        {
            b[i] = new bool[columns];  // Each row is an array of 'columns' length
        }

        // Initialize the jagged array with the number of rows
        GameObject[][] c = new GameObject[rows][];

        // Initialize each row with the number of columns
        for (int i = 0; i < rows; i++)
        {
            c[i] = new GameObject[columns];  // Each row is an array of 'columns' length
        }



        //Generating values for the arrays:
        // Initializing all elements to 0
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                b[i][j] = false;  
            }
        }

        //setting mines throughout
        for (int i = 0; i < mineCount; i++)
        {
            int x = UnityEngine.Random.Range(0, columns);
            int y = UnityEngine.Random.Range(0, rows);

            if (b[x][y] == false)
            {
                b[x][y] = true;
            }
            else
            {
                i--;
            }
        }

        //Setting values of the numbers: 
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                int minesInVicinity = 0;

                //Incrementing the value if there is a mine in the vicinity
                TryCheck(() => { if (b[i + 1][j]) minesInVicinity++; });
                TryCheck(() => { if (b[i - 1][j]) minesInVicinity++; });
                TryCheck(() => { if (b[i + 1][j - 1]) minesInVicinity++; });
                TryCheck(() => { if (b[i - 1][j + 1]) minesInVicinity++; });
                TryCheck(() => { if (b[i + 1][j + 1]) minesInVicinity++; });
                TryCheck(() => { if (b[i - 1][j - 1]) minesInVicinity++; });
                TryCheck(() => { if (b[i][j + 1]) minesInVicinity++; });
                TryCheck(() => { if (b[i][j - 1]) minesInVicinity++; });

                a[i][j] = minesInVicinity;
            }
        }



        //Assigning the arrays
        gridValues = a;
        gridMines = b;
        gridTiles = c;

        //printing the values to console
        PrintMinesDebug();
        PrintNumbersDebug();
    }

    void TryCheck(Action action)
    {
        try
        {
            action();  // Execute the action that might throw an exception
        }
        catch (Exception ex)
        {
            //Debug.Log(ex);
        }
    }

    void PrintMinesDebug()
    {
        Debug.Log("Mines in the grid : ");

        for (int i = 0;i < rows;i++)
        {
            string print = "";

            for (int j = 0;j < columns;j++)
            {
                print += gridMines[i][j].ToString() + ", ";
            }

            Debug.Log(print);
        }
    }

    void PrintNumbersDebug()
    {
        Debug.Log("Numbers in the grid : ");

        for (int i = 0; i < rows; i++)
        {
            string print = "";

            for (int j = 0; j < columns; j++)
            {
                print += gridValues[i][j].ToString() + ", ";
            }

            Debug.Log(print);
        }
    }
    #endregion

    #region Spawning the grid into the scene


    [SerializeField] GameObject tile; // tile prefab
    void SpawnGrid()
    {
        // Initialize the jagged array with the number of rows
        GameObject[][] a = new GameObject[rows][];

        // Initialize each row with the number of columns
        for (int i = 0; i < rows; i++)
        {
            a[i] = new GameObject[columns];  // Each row is an array of 'columns' length
        }


        for (int i = 0; i  < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                GameObject newTile = Instantiate(tile, gameObject.transform, false);
                gridTiles[i][j] = newTile;
                newTile.transform.localPosition = new Vector3(i - (columns/2), 0, j - (rows/2));
                newTile.name = i.ToString() + ", " + j.ToString();

                TileManager tileManager = newTile.GetComponent<TileManager>();
                tileManager.SetTile(i, j, gridValues[i][j], gridMines[i][j]);
            }
            
        }

    }
    #endregion

    public void openNearbyTiles(int x, int y)
    {
        TryCheck(() => { TileManager a = gridTiles[x + 1][y].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x - 1][y].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x][y+1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x][y-1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x + 1][y+1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x - 1][y+1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x + 1][y-1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
        TryCheck(() => { TileManager a = gridTiles[x - 1][y-1].GetComponent<TileManager>(); a.OpenTile(); Debug.Log(a.name); });
    }

    

    public void CheckIfGameOverFlag()
    {
        int i = 0; // count for the flags

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows;  y++)
            {
                TileManager a = gridTiles[x][y].GetComponent<TileManager>();
                if (a.isMine && a.isMarked)
                {
                    i++;
                }
            }
        }

        if (i == mineCount)
        {
            HandleGameWon();
        }
    }

    int count = 0;
    public void CheckIfGameOverTile(int column, int row)
    {
        int safeTiles = (rows * columns) - mineCount;
        Debug.Log("this function is being called. Safe tiles = " +  safeTiles + " and count = " + count);

        count++;

        if (count == safeTiles)
        {
            HandleGameWon();
        }
    }

    void HandleGameWon()
    {
        raycastScript.gameOver = true;
        Debug.Log("Game is won");
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                StartCoroutine(OpenTileWithDelay(x, y, false));
            }
        }
        StartCoroutine(GameOverUI(true));
    }

    public void HandleGameLost()
    {
        raycastScript.gameOver = true;
        Debug.Log("Game is lost");
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                StartCoroutine(OpenTileWithDelay(x, y, true));
            }
        }
        StartCoroutine(GameOverUI(false));
    }

    private IEnumerator OpenTileWithDelay(int column, int row, bool mine)
    {
        TileManager tm = gridTiles[column][row].GetComponent<TileManager>();

        if (tm.isMine == mine)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
            tm.OpenTile();
        }
    }

    [SerializeField] GameObject winCanvas, loseCanvas;
    private IEnumerator GameOverUI(bool win)
    {
        yield return new WaitForSeconds(1.1f);
        if (win) winCanvas.SetActive(true);
        else loseCanvas.SetActive(true);
    }

    public void GoToHome()
    {
        FindObjectOfType<LevelManager>().GoToHome();
    }

    public void ResetLevel()
    {
        FindObjectOfType<LevelManager>().ResetLevel();

    }
}