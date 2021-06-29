using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBoardLoader : MonoBehaviour
{
    #region Singleton
    private static NPCBoardLoader _instance;
    public static NPCBoardLoader Instance { get { return _instance; } }

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

    private List<UnitData> allUnits = new List<UnitData>();
    [SerializeField]
    private Transform enemyBoardParent;

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        UnitData[] unitArray = Resources.LoadAll<UnitData>("UnitDataBase");
        foreach (UnitData unit in unitArray)
        {
            allUnits.Add(unit);
        }

        unitArray = Resources.LoadAll<UnitData>("UnitDataLeveled");
        foreach (UnitData unit in unitArray)
        {
            allUnits.Add(unit);
        }
    }
    public void LoadBoard(NPCBoardState layout)
    {
        BonusManager.Instance.ClearNPCBonuses(); // Clear all the bonuses from the previous board
        foreach (NPCSavedUnit loadUnit in layout.units)
        {
            // Use raycast to determine Y
            RaycastHit hit;
            Vector3 newPos = new Vector3(loadUnit.boardX, 3, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)));
            if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
            {
                // TODO: make this more efficient by finding based on tier/level of unit to be spawned and using a different datastructure to hold the units
                UnitData unit = allUnits.Find((x) => x.name == loadUnit.name);

                GameObject newUnit = UnitSpawner.Instance.SpawnUnit(new Vector3(loadUnit.boardX, hit.point.y, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)) + Board.Instance.GetNPCZOffset()), unit, true);
                newUnit.GetComponent<Unit>().SetAsNPC();
                Board.Instance.AddNPCUnit(loadUnit.boardX, loadUnit.boardZ, newUnit);

                newUnit.transform.SetParent(enemyBoardParent);


                //// Instantiate the object at the correct position, set it as an NPC unit and add it to the board
                //GameObject newUnit = Instantiate(unitResource, new Vector3(loadUnit.boardX, hit.point.y, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)) + Board.Instance.GetNPCZOffset()), Quaternion.Euler(0, 180, 0));
                //newUnit.GetComponent<Unit>().SetAsNPC();
                //Board.Instance.AddNPCUnit(loadUnit.boardX, loadUnit.boardZ, newUnit);

            }
        }
    }
}