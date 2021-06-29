using System.IO;
//using System.Collections;
using System.Collections.Generic;
//using System;
using UnityEngine;
//using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class NPCBoardSaver : MonoBehaviour
{
    private TMP_InputField saveNameInput;

    private TMP_Dropdown difficultyInput;

    private List<NPCBoardState> layouts = new List<NPCBoardState>();

    private List<UnitData> allUnits = new List<UnitData>();

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        saveNameInput = GetComponentInChildren<TMP_InputField>();
        difficultyInput = GetComponentInChildren<TMP_Dropdown>();
        
        TextAsset[] layoutJsons = Resources.LoadAll<TextAsset>("BoardLayouts");
        foreach (TextAsset board in layoutJsons)
        {
            layouts.Add(JsonUtility.FromJson<NPCBoardState>(board.text));
            Resources.UnloadAsset(board);
        }

        UnitData[] unitArray = Resources.LoadAll<UnitData>("UnitData");
        foreach (UnitData unit in unitArray)
        {
            allUnits.Add(unit);
            //Resources.UnloadAsset(unit);
        }
    }

    // TODO: Check for null values at multiple stages
    public void SaveBoard()
    {
        GameObject[,] units = Board.Instance.GetPlayerPositions();
        NPCBoardState board = new NPCBoardState
        {
            units = new List<NPCSavedUnit>(),
            setNames = new List<string>(),
            setCounts = new List<int>(),
            tribeNames = new List<string>(),
            tribeCounts = new List<int>(),
            difficulty = 0
        };

        for (int x = 0; x < Board.Instance.GetBoardX(); x++)
        {
            for (int z = 0; z < Board.Instance.GetBoardZ(); z++)
            {
                if (units[x, z] != null)
                {
                    board.units.Add(new NPCSavedUnit(x, z, units[x, z].GetComponent<Unit>().UnitData.name));

                }
            }
        }
        foreach (var set in BonusManager.Instance.GetPlayerSetBonuses())
        {
            board.setNames.Add(set.Key.name);
            board.setCounts.Add(set.Value);
        }
        foreach (var tribe in BonusManager.Instance.GetPlayerTribeBonuses())
        {
            board.tribeNames.Add(tribe.Key.name);
            board.tribeCounts.Add(tribe.Value);
        }
        
        board.difficulty = difficultyInput.value + 1;

        string json = JsonUtility.ToJson(board, true);
        Debug.Log(json);

        string name = saveNameInput.text + ".txt";

        File.WriteAllText(Application.dataPath + "/Resources/BoardLayouts/" + name, json);
        AssetDatabase.Refresh();
    }

    public void LoadRandomTest()
    {
        GameObject[] allUnits = Resources.LoadAll<GameObject>("Units");
        TextAsset[] layouts = Resources.LoadAll<TextAsset>("BoardLayouts");

        int randLayout = Random.Range(0, layouts.Length);
        //Debug.Log(layouts[randLayout].text);
        NPCBoardState boardState = JsonUtility.FromJson<NPCBoardState>(layouts[randLayout].text);
        foreach (NPCSavedUnit loadUnit in boardState.units)
        {
            foreach (GameObject unitResource in allUnits)
            {
                if (unitResource.name == loadUnit.name)
                {
                    // Use raycast to determine Y
                    RaycastHit hit;
                    Vector3 newPos = new Vector3(loadUnit.boardX, 3, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)));
                    if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
                    {
                        // Instantiate the object at the correct position, set it as an NPC unit and add it to the board
                        GameObject newUnit = Instantiate(unitResource, new Vector3(loadUnit.boardX, hit.point.y, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)) + Board.Instance.GetNPCZOffset()), Quaternion.Euler(0, 180, 0));
                        newUnit.GetComponent<Unit>().SetAsNPC();
                        Board.Instance.AddNPCUnit(loadUnit.boardX, loadUnit.boardZ, newUnit);

                    }
                }
            }
        }
    }

    public void LoadBoard()
    {
        int randLayout = Random.Range(0, layouts.Count);
        NPCBoardState boardState = layouts[randLayout];

        foreach (NPCSavedUnit loadUnit in boardState.units)
        {
            // Use raycast to determine Y
            RaycastHit hit;
            Vector3 newPos = new Vector3(loadUnit.boardX, 3, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)));
            if (Physics.Raycast(newPos, Vector3.down, out hit, Mathf.Infinity, layerMask) && hit.transform.CompareTag("tile"))
            {
                UnitData unit = allUnits.Find((x) => x.name == loadUnit.name);

                GameObject newUnit = UnitSpawner.Instance.SpawnUnit(new Vector3(loadUnit.boardX, hit.point.y, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)) + Board.Instance.GetNPCZOffset()), unit, true);
                newUnit.GetComponent<Unit>().SetAsNPC();
                Board.Instance.AddNPCUnit(loadUnit.boardX, loadUnit.boardZ, newUnit);


                //// Instantiate the object at the correct position, set it as an NPC unit and add it to the board
                //GameObject newUnit = Instantiate(unitResource, new Vector3(loadUnit.boardX, hit.point.y, Mathf.Abs(loadUnit.boardZ - (Board.Instance.GetBoardZ() - 1)) + Board.Instance.GetNPCZOffset()), Quaternion.Euler(0, 180, 0));
                //newUnit.GetComponent<Unit>().SetAsNPC();
                //Board.Instance.AddNPCUnit(loadUnit.boardX, loadUnit.boardZ, newUnit);

            }
        }
    }
}
