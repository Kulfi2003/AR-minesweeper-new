using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TileManager : MonoBehaviour
{
    int rowPosition;
    int columnPosition;
    public bool isMine;
    public bool isMarked = false;
    public bool opened = false;
    public int minesInVicinity;

    GameManager gm;

    [SerializeField] TextMeshPro number;
    [SerializeField] GameObject mine;
    [SerializeField] GameObject cover;
    [SerializeField] GameObject flag;
    [SerializeField] GameObject selection;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void SetTile(int column, int row, int gridValue, bool gridMine)
    {
        minesInVicinity = gridValue;
        isMine = gridMine;
        rowPosition = row;
        columnPosition = column;

        if (isMine)
        {
            number.gameObject.SetActive(false);
            mine.SetActive(true);
        }
        else
        {
            number.gameObject.SetActive(true);
            mine.SetActive(false);
            number.text = minesInVicinity.ToString();
            if (minesInVicinity == 0 )
            {
                number.text = "";
            }
        }
    }

    public void OpenTile()
    {
        if (!isMarked && !opened)
        {
            gm.gameStarted = true;

            if (!isMine)
            {
                cover.SetActive(false);
                opened = true;

                gm.CheckIfGameOverTile(columnPosition, rowPosition);

                GetComponent<BoxCollider>().enabled = false;

                if (minesInVicinity == 0)
                {
                    gm.openNearbyTiles(columnPosition, rowPosition);
                    
                }
            }

            if (isMine)
            {
                cover.SetActive(false);
                opened = true;
                GetComponent<BoxCollider>().enabled = false;


                Debug.Log("This is being called.");
                //Code to make all the mines explode
                gm.HandleGameLost();
            }
        }
    }

    public void toggleFlag()
    {
        if (isMarked)
        {
            isMarked = false;
            flag.SetActive(false);
            gm.flagsLeft++;
        }
        else if (gm.flagsLeft > 0)
        {
            isMarked = true;
            flag.SetActive(true);
            gm.flagsLeft--;
            gm.CheckIfGameOverFlag();
        }
    }

    public void TileTouchingStartEnd(bool touch)
    {
        selection.SetActive(touch);
    }
}