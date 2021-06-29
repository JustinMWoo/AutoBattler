using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    #region Singleton
    private static Board _instance;
    public static Board Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    // For raycasts
    [SerializeField]
    private LayerMask layerMask;

    private const int BOARDX = 7;
    private const int BOARDZ = 4;

    // Z offset for the NPC's units (npc board is flipped with the start position x:3 z:3 at the bottom middle and x:3 z:0 at the top middle)
    private const int NPCZOFFSET = 5;


    private List<Unit> playerUnits = new List<Unit>();
    private List<Unit> npcUnits = new List<Unit>();

    private List<Unit> allUnits = new List<Unit>();


    // Units on boards start at position x:3 z:3
    private GameObject[,] playerPositions = new GameObject[BOARDX, BOARDZ];
    private GameObject[,] npcPositions = new GameObject[BOARDX, BOARDZ];

    private GameObject[,] preCombatPlayerBoard;
    private List<Unit> preCombatPlayerUnits = new List<Unit>();
    private List<Unit> preCombatNPCUnits = new List<Unit>();

    #region Getters
    public GameObject[,] GetPlayerPositions()
    {
        return playerPositions;
    }
    public List<Unit> GetAllUnits()
    {
        return allUnits;
    }
    public List<Unit> GetPlayerUnits()
    {
        return playerUnits;
    }
    public List<Unit> GetNPCUnits()
    {
        return npcUnits;
    }
    public int GetBoardX()
    {
        return BOARDX;
    }
    public int GetBoardZ()
    {
        return BOARDZ;
    }
    public int GetNPCZOffset()
    {
        return NPCZOFFSET;
    }
    public int PlayerUnitCount()
    {
        return playerUnits.Count();
    }
    public int NPCUnitCount()
    {
        return npcUnits.Count();
    }
    public LayerMask GetTileLayerMask()
    {
        return layerMask;
    }
    #endregion

    // Determines the Y position of the unit to place it above the tile on the board
    private float DetermineY(int x, int z)
    {
        RaycastHit hit;
        Vector3 pos = new Vector3(x, 3, z);
        if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
        {
            return hit.point.y;
        }
        return -1;
    }

    /*
     * Attempts to position a unit on the player's board at the specified position
     * @return null if unit not set, otherwise returns the coordinates of the unit
     */
    public Tuple<int, int> PositionPlayerUnit(int x, int z, GameObject unit, int oldX, int oldZ)
    {
        bool removeOld = true;
        if (oldX >= BOARDX || oldX < 0 || oldZ >= BOARDZ || oldZ < 0)
        {
            removeOld = false;
        }

        if (x >= BOARDX || z >= BOARDZ)
        {
            //Debug.Log("Error");
            return null;
        }

        // If there is a unit there then swap the positions
        if (removeOld && playerPositions[x, z] != null)
        {
            GameObject unit2 = playerPositions[x, z];
            playerPositions[x, z] = unit;

            playerPositions[oldX, oldZ] = unit2;

            // Determine the y position for the unit being swapped
            float yPos = DetermineY(oldX, oldZ);
            if (yPos != -1)
            {
                unit2.transform.position = new Vector3(oldX, yPos, oldZ);
                return new Tuple<int, int>(x, z);
            }
            else
            {
                // Should never happen but just incase
                Debug.Log("Error when swapping units (no hit detected)");
                return null;
            }
        }


        // Remove the old position from the player positions
        if (removeOld)
        {
            playerPositions[oldX, oldZ] = null;
            ReorganizePlayerBoard(x, z); // Reorganize the board reserving the new position
        }
        //Debug.Log("Calling with x: " + x + " z: " + z);
        Tuple<int, int> newPos = FindNewPosition(x, z);

        if (newPos != null)
        {
            // Set the unit to the found position        
            AddPlayerUnit(newPos.Item1, newPos.Item2, unit);
            //ReorganizePlayerBoard(); // reorganize board
            //Debug.Log("x: " + newPos.Item1 + " z: " + newPos.Item2);

            return newPos;
        }

        // If there is no valid position then re-add the old position and return null
        if (removeOld)
        {
            playerPositions[oldX, oldZ] = unit;
        }
        return null;
    }

    //public void RemovePlayerUnit(int x, int z)
    //{
    //    if (x > BOARDX || x < 0 || z > BOARDZ || z < 0)
    //    {
    //        return;
    //    }
    //    Unit removedUnit = playerPositions[x, z].GetComponent<Unit>();
    //    playerUnits.Remove(removedUnit);
    //    allUnits.Remove(removedUnit);

    //    playerPositions[x, z] = null;
    //    ReorganizePlayerBoard();
    //}

    public void RemovePlayerUnit(Unit unit, bool reorganize = true)
    {
        Tuple<int, int> pos = FindUnitPosition(unit);
        playerUnits.Remove(unit);
        allUnits.Remove(unit);
        if (pos.Item1 != -1)
        {
            playerPositions[pos.Item1, pos.Item2] = null;
        }
        else
        {
            // Unit no longer on board
            //Debug.LogError("Unit could not be found");
            return;
        }

        BonusManager.Instance.RemovePlayerUnit(unit);
        TriplerManager.Instance.RemoveUnit(unit);
        if (reorganize)
        {
            ReorganizePlayerBoard();
        }
    }

    public void RemoveNPCUnit(Unit unit, bool reorganize = true)
    {
        Tuple<int, int> pos = FindUnitPosition(unit);
        npcUnits.Remove(unit);
        allUnits.Remove(unit);
        if (pos.Item1 != -1)
        {
            npcPositions[pos.Item1, pos.Item2] = null;
        }
        else
        {
            //Debug.LogError("Unit could not be found");
            return;
        }
        if (reorganize)
        {
            ReorganizeNPCBoard();
        }
    }

    public void AddPlayerUnit(int x, int z, GameObject unit, bool replace = false)
    {
        if (x > BOARDX || x < 0 || z > BOARDZ || z < 0)
        {
            return;
        }
        if (playerPositions[x, z] != null && !replace)
        {
            Debug.LogError("Unit already there!");
            return;
        }

        playerPositions[x, z] = unit;
        Unit unitScript = unit.GetComponent<Unit>();
        if (!allUnits.Contains(unitScript))
        {
            allUnits.Add(unitScript);
            playerUnits.Add(unitScript);
            // Add to bonus manager
            BonusManager.Instance.AddPlayerUnit(unitScript);
            TriplerManager.Instance.AddUnit(unitScript);
        }
    }

    public void AddNPCUnit(int x, int z, GameObject unit, bool replace = false)
    {
        if (x > BOARDX || x < 0 || z > BOARDZ || z < 0)
        {
            return;
        }
        if (npcPositions[x, z] != null && !replace)
        {
            Debug.LogError("Unit already there!");
            return;
        }

        npcPositions[x, z] = unit;
        Unit unitScript = unit.GetComponent<Unit>();
        allUnits.Add(unitScript);
        npcUnits.Add(unitScript);

        // Add to bonus manager
        if (unitScript.Set != null)
        {
            BonusManager.Instance.AddNPCUnit(unitScript);
        }
    }

    public Tuple<int, int> FindNewPosition(int x, int z)
    {
        int newX = 0;
        // First position open
        if (playerPositions[BOARDX / 2, BOARDZ - 1] == null)
        {
            return new Tuple<int, int>(BOARDX / 2, BOARDZ - 1);
        }
        // Determine column
        if (x > BOARDX / 2)
        {
            for (int col = (BOARDX / 2) + 1; col <= x; col++)
            {
                if (playerPositions[col, BOARDZ - 1] == null)
                {
                    return new Tuple<int, int>(col, BOARDZ - 1);
                }
                else
                {
                    newX = col;
                }
            }
        }
        else if (x < BOARDX / 2)
        {
            for (int col = (BOARDX / 2) - 1; col >= x; col--)
            {
                if (playerPositions[col, BOARDZ - 1] == null)
                {
                    return new Tuple<int, int>(col, BOARDZ - 1);
                }
                else
                {
                    newX = col;
                }
            }
        }
        else // x == 3
        {
            newX = 3;
        }

        for (int row = BOARDZ - 1; row >= z; row--)
        {
            if (playerPositions[newX, row] == null)
            {
                return new Tuple<int, int>(newX, row);
            }
        }

        return null;
    }

    /*
     * Reorganizes the player's board to account for units being shifted around 
     */
    // TODO: Make helper function for the actual moving (repeated code)


    public void ReorganizePlayerBoard()
    {
        bool reorganized = true;
        while (reorganized)
        {
            reorganized = false;

            //Debug.Log("Checking Positioning...");
            for (int row = BOARDZ - 1; row >= 0; row--)
            {
                for (int col = 0; col < BOARDX; col++)
                {
                    // Move unit forward if empty space
                    if (row != BOARDZ - 1 && playerPositions[col, row] != null && playerPositions[col, row + 1] == null)
                    {
                        //Debug.Log("Moved up");
                        reorganized = true;
                        GameObject unit = playerPositions[col, row];
                        playerPositions[col, row] = null;
                        playerPositions[col, row + 1] = unit;

                        float yPos = DetermineY(col, row + 1);
                        if (yPos != -1)
                        {
                            unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row + 1));
                            //unit.transform.position = new Vector3(col, yPos, row + 1);
                        }
                        else
                        {
                            Debug.Log("Error when reorganizing player board");
                        }
                    }
                }
            }

            // Determine empty columns
            bool[] emptyCols = new bool[BOARDX];
            for (int col = 0; col < BOARDX; col++)
            {
                int count = 0;
                for (int row = BOARDZ - 1; row >= 0; row--)
                {
                    if (playerPositions[col, row] == null)
                    {
                        count++;
                    }
                }

                // If the whole column is empty
                if (count == BOARDZ)
                {
                    //Debug.Log("x: " + col + " empty");
                    emptyCols[col] = true;
                }
            }
            int randomSide = UnityEngine.Random.Range(0, 2);

            for (int i = 0; i < 2; i++)
            {
                if (randomSide == 0)
                {
                    for (int col = BOARDX / 2; col < BOARDX; col++)
                    {
                        // If the column is empty and the column to the right is not
                        if (col != BOARDX - 1 && emptyCols[col] && !emptyCols[col + 1])
                        {
                            //Debug.Log("Moved left");
                            reorganized = true;
                            emptyCols[col + 1] = true;
                            emptyCols[col] = false;
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col + 1, row];
                                if (unit != null)
                                {
                                    playerPositions[col + 1, row] = null;
                                    playerPositions[col, row] = unit;

                                    float yPos = DetermineY(col, row);
                                    if (yPos != -1)
                                    {
                                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                        //unit.transform.position = new Vector3(col, yPos, row);
                                    }
                                    else
                                    {
                                        Debug.Log("Error when reorganizing player board");
                                    }
                                }
                            }
                        }
                    }
                    // Switch the side to check
                    randomSide = 1;
                }
                else if (randomSide == 1)
                {
                    for (int col = BOARDX / 2; col > 0; col--)
                    {
                        // If the column is empty and the column to the left is not
                        if (col != 0 && emptyCols[col] && !emptyCols[col - 1])
                        {
                            //Debug.Log("Moved right");
                            reorganized = true;
                            emptyCols[col - 1] = true;
                            emptyCols[col] = false;
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col - 1, row];
                                if (unit != null)
                                {
                                    playerPositions[col - 1, row] = null;
                                    playerPositions[col, row] = unit;

                                    float yPos = DetermineY(col, row);
                                    if (yPos != -1)
                                    {
                                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                        //unit.transform.position = new Vector3(col, yPos, row);
                                    }
                                    else
                                    {
                                        Debug.Log("Error when reorganizing player board");
                                    }
                                }
                            }
                        }
                    }
                    // Switch the side to check
                    randomSide = 0;
                }
            }
        }
        EventManager.Instance.PlayerBoardReorganize();
    }

    /*
     * Reorganizes the player's board to account for units being shifted around while reserving a unit's position
     */
    public void ReorganizePlayerBoard(int x, int z)
    {
        //Debug.Log("Checking Positioning...");
        for (int row = BOARDZ - 1; row >= 0; row--)
        {
            for (int col = 0; col < BOARDX; col++)
            {
                // Move unit forward if empty space
                if (row != BOARDZ - 1 && playerPositions[col, row] != null && playerPositions[col, row + 1] == null)
                {
                    //Debug.Log("Moved up");
                    GameObject unit = playerPositions[col, row];
                    playerPositions[col, row] = null;
                    playerPositions[col, row + 1] = unit;

                    float yPos = DetermineY(col, row + 1);
                    if (yPos != -1)
                    {
                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row + 1));
                        //unit.transform.position = new Vector3(col, yPos, row + 1);
                    }
                    else
                    {
                        Debug.Log("Error when reorganizing player board");
                    }
                }
            }
        }

        // Determine empty columns
        bool[] emptyCols = new bool[BOARDX];
        for (int col = 0; col < BOARDX; col++)
        {
            int count = 0;
            for (int row = BOARDZ - 1; row >= 0; row--)
            {
                if (playerPositions[col, row] == null)
                {
                    count++;
                }
            }

            // If the whole column is empty
            if (count == BOARDZ)
            {
                //Debug.Log("x: " + col + " empty");
                emptyCols[col] = true;
            }
        }
        int randomSide = UnityEngine.Random.Range(0, 2);

        for (int i = 0; i < 2; i++)
        {
            if (randomSide == 0)
            {
                for (int col = BOARDX / 2; col < BOARDX; col++)
                {
                    // If the column is empty and the column to the right is not
                    if (col != BOARDX - 1 && emptyCols[col] && !emptyCols[col + 1])
                    {
                        bool reserved = false;
                        if (col == x)
                        {
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col + 1, row];
                                if (unit != null)
                                {
                                    if (row > z) // If in front of reserved row then shift into new col 
                                    {

                                        playerPositions[col + 1, row] = null;
                                        playerPositions[col, row] = unit;

                                        float yPos = DetermineY(col, row);
                                        if (yPos != -1)
                                        {
                                            unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                            //unit.transform.position = new Vector3(col, yPos, row);
                                        }
                                        else
                                        {
                                            Debug.Log("Error when reorganizing player board");
                                        }

                                    }
                                    else // Move row forward to front
                                    {
                                        reserved = true;
                                        playerPositions[col + 1, row + BOARDZ - 1 - z] = unit;
                                        playerPositions[col + 1, row] = null;

                                        float yPos = DetermineY(col, row);
                                        if (yPos != -1)
                                        {
                                            unit.GetComponent<Unit>().MoveTo(new Vector3(col + 1, yPos, row + BOARDZ - 1 - z));
                                            //unit.transform.position = new Vector3(col, yPos, row);
                                        }
                                        else
                                        {
                                            Debug.Log("Error when reorganizing player board");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col + 1, row];
                                if (unit != null)
                                {
                                    playerPositions[col + 1, row] = null;
                                    playerPositions[col, row] = unit;

                                    float yPos = DetermineY(col, row);
                                    if (yPos != -1)
                                    {
                                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                        //unit.transform.position = new Vector3(col, yPos, row);
                                    }
                                    else
                                    {
                                        Debug.Log("Error when reorganizing player board");
                                    }
                                }
                            }
                        }
                        //Debug.Log("Moved left");
                        emptyCols[col + 1] = reserved;
                        emptyCols[col] = false;

                    }
                }
                // Switch the side to check
                randomSide = 1;
            }
            else if (randomSide == 1)
            {
                for (int col = BOARDX / 2; col > 0; col--)
                {
                    // If the column is empty and the column to the left is not
                    if (col != 0 && emptyCols[col] && !emptyCols[col - 1])
                    {

                        bool reserved = false;
                        if (col == x)
                        {
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col - 1, row];
                                if (unit != null)
                                {
                                    if (row > z) // If in front of reserved row then shift into new col 
                                    {

                                        playerPositions[col - 1, row] = null;
                                        playerPositions[col, row] = unit;

                                        float yPos = DetermineY(col, row);
                                        if (yPos != -1)
                                        {
                                            unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                            //unit.transform.position = new Vector3(col, yPos, row);
                                        }
                                        else
                                        {
                                            Debug.Log("Error when reorganizing player board");
                                        }

                                    }
                                    else // Move row forward to front
                                    {
                                        reserved = true;
                                        playerPositions[col - 1, row + BOARDZ - 1 - z] = unit;
                                        playerPositions[col - 1, row] = null;

                                        float yPos = DetermineY(col, row);
                                        if (yPos != -1)
                                        {
                                            unit.GetComponent<Unit>().MoveTo(new Vector3(col - 1, yPos, row + BOARDZ - 1 - z));
                                            //unit.transform.position = new Vector3(col, yPos, row);
                                        }
                                        else
                                        {
                                            Debug.Log("Error when reorganizing player board");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (int row = BOARDZ - 1; row >= 0; row--)
                            {
                                GameObject unit = playerPositions[col - 1, row];
                                if (unit != null)
                                {
                                    playerPositions[col - 1, row] = null;
                                    playerPositions[col, row] = unit;

                                    float yPos = DetermineY(col, row);
                                    if (yPos != -1)
                                    {
                                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, row));
                                        //unit.transform.position = new Vector3(col, yPos, row);
                                    }
                                    else
                                    {
                                        Debug.Log("Error when reorganizing player board");
                                    }
                                }
                            }
                        }
                        //Debug.Log("Moved right");
                        emptyCols[col - 1] = reserved;
                        emptyCols[col] = false;
                    }
                }
                // Switch the side to check
                randomSide = 0;
            }
        }
        EventManager.Instance.PlayerBoardReorganize();
    }

    /*
    * Reorganizes the NPC's board to account for units being shifted around
    */
    // TODO: Detect if multiple reorganizations need to be done! (if two units die in the same column then must reorganize multiple times)
    // TODO: yposition for npc unit
    public void ReorganizeNPCBoard()
    {
        //Debug.Log("Checking Positioning...");
        for (int row = BOARDZ - 1; row >= 0; row--)
        {
            for (int col = 0; col < BOARDX; col++)
            {
                // Move unit forward if empty space
                if (row != BOARDZ - 1 && npcPositions[col, row] != null && npcPositions[col, row + 1] == null)
                {
                    //Debug.Log("Moved up");
                    GameObject unit = npcPositions[col, row];
                    npcPositions[col, row] = null;
                    npcPositions[col, row + 1] = unit;

                    float yPos = DetermineY(col, row + 1);
                    if (yPos != -1)
                    {
                        unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, Math.Abs(row + 1 - (BOARDZ - 1)) + NPCZOFFSET));
                        //unit.transform.position = new Vector3(col, yPos, row + 1);
                    }
                    else
                    {
                        Debug.Log("Error when reorganizing NPC board");
                    }
                }
            }
        }

        // Determine empty columns
        bool[] emptyCols = new bool[BOARDX];
        for (int col = 0; col < BOARDX; col++)
        {
            int count = 0;
            for (int row = BOARDZ - 1; row >= 0; row--)
            {
                if (npcPositions[col, row] == null)
                {
                    count++;
                }
            }

            // If the whole column is empty
            if (count == BOARDZ)
            {
                //Debug.Log("x: " + col + " empty");
                emptyCols[col] = true;
            }
        }

        for (int col = BOARDX / 2; col < BOARDX; col++)
        {
            // If the column is empty and the column to the right is not
            if (col != BOARDX - 1 && emptyCols[col] && !emptyCols[col + 1])
            {
                //Debug.Log("Moved left");
                emptyCols[col + 1] = true;
                emptyCols[col] = false;
                for (int row = BOARDZ - 1; row >= 0; row--)
                {
                    GameObject unit = npcPositions[col + 1, row];
                    if (unit != null)
                    {
                        npcPositions[col + 1, row] = null;
                        npcPositions[col, row] = unit;

                        float yPos = DetermineY(col, row);
                        if (yPos != -1)
                        {
                            unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, Math.Abs(row - (BOARDZ - 1)) + NPCZOFFSET));
                            //unit.transform.position = new Vector3(col, yPos, row);
                        }
                        else
                        {
                            Debug.Log("Error when reorganizing NPC board");
                        }
                    }
                }
            }
        }

        for (int col = BOARDX / 2; col > 0; col--)
        {
            // If the column is empty and the column to the left is not
            if (col != 0 && emptyCols[col] && !emptyCols[col - 1])
            {
                //Debug.Log("Moved right");
                emptyCols[col - 1] = true;
                emptyCols[col] = false;
                for (int row = BOARDZ - 1; row >= 0; row--)
                {
                    GameObject unit = npcPositions[col - 1, row];
                    if (unit != null)
                    {
                        npcPositions[col - 1, row] = null;
                        npcPositions[col, row] = unit;

                        float yPos = DetermineY(col, row);
                        if (yPos != -1)
                        {
                            unit.GetComponent<Unit>().MoveTo(new Vector3(col, yPos, Math.Abs(row - (BOARDZ - 1)) + NPCZOFFSET));
                            //unit.transform.position = new Vector3(col, yPos, row);
                        }
                        else
                        {
                            Debug.Log("Error when reorganizing player board");
                        }
                    }
                }
            }
        }
        EventManager.Instance.NPCBoardReorganize();
    }


    // Finds the unit's position on the respective board
    // TODO: could make this much more efficient by starting search from the starting location
    public Tuple<int, int> FindUnitPosition(Unit unit)
    {
        if (unit == null)
        {
            return Tuple.Create(-1, -1);
        }
        for (int x = 0; x < BOARDX; x++)
        {
            for (int z = 0; z < BOARDZ; z++)
            {
                //Debug.Log(playerPositions[x, z]);
                if (unit.IsPlayerUnit() && playerPositions[x, z] != null && playerPositions[x, z].Equals(unit.gameObject))
                {
                    return Tuple.Create(x, z);
                }
                else if (!unit.IsPlayerUnit() && npcPositions[x, z] != null && npcPositions[x, z].Equals(unit.gameObject))
                {
                    return Tuple.Create(x, z);
                }
            }
        }

        // Unit not found
        return Tuple.Create(-1, -1);

    }

    public Unit CheckPositionForUnit(int x, int z, bool isPlayerUnit)
    {
        if (x >= BOARDX || z >= BOARDZ || x < 0 || z < 0)
        {
            return null;
        }

        if (isPlayerUnit)
        {
            if (playerPositions[x, z] != null)
            {
                return playerPositions[x, z].GetComponent<Unit>();
            }
            else
            {
                return null;
            }
        }
        else
        {
            if (npcPositions[x, z] != null)
            {
                return npcPositions[x, z].GetComponent<Unit>();
            }
            else
            {
                return null;
            }
        }
    }

    public void SaveBoardState()
    {
        preCombatPlayerBoard = (GameObject[,])playerPositions.Clone();
        preCombatPlayerUnits = new List<Unit>(playerUnits);
        preCombatNPCUnits = new List<Unit>(npcUnits);
    }

    public void ResetBoardAfterCombat()
    {
        // Move each unit to it's position
        foreach (Unit unit in preCombatPlayerUnits)
        {
            unit.ShowUnit();
            Tuple<int, int> oldPos = FindUnitOldPosition(unit);

            unit.transform.position = new Vector3(oldPos.Item1, unit.transform.position.y, oldPos.Item2);
        }

        playerUnits = preCombatPlayerUnits;
        allUnits = new List<Unit>(preCombatPlayerUnits);
        // Reset player board
        playerPositions = preCombatPlayerBoard;

        // Reset NPC board
        Array.Clear(npcPositions, 0, npcPositions.Length);

        foreach (Unit unit in preCombatNPCUnits)
        {
            Destroy(unit.gameObject);
        }
        npcUnits.Clear();
    }

    private Tuple<int, int> FindUnitOldPosition(Unit unit)
    {
        for (int x = 0; x < BOARDX; x++)
        {
            for (int z = 0; z < BOARDZ; z++)
            {
                //Debug.Log(playerPositions[x, z]);
                if (preCombatPlayerBoard[x, z] != null && preCombatPlayerBoard[x, z].Equals(unit.gameObject))
                {
                    return Tuple.Create(x, z);
                }
            }
        }

        // Unit not found
        return null;
    }

}