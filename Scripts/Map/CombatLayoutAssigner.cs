using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatLayoutAssigner : MonoBehaviour
{
    #region Singleton/Awake
    private static CombatLayoutAssigner _instance;
    public static CombatLayoutAssigner Instance { get { return _instance; } }

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
        TextAsset[] layoutJsons = Resources.LoadAll<TextAsset>("BoardLayouts");
        foreach (TextAsset boardText in layoutJsons)
        {
            NPCBoardState board = JsonUtility.FromJson<NPCBoardState>(boardText.text);
            if (layouts.ContainsKey(board.difficulty))
            {
                layouts[board.difficulty].Add(board);
            }
            else
            {
                layouts[board.difficulty] = new List<NPCBoardState>() { board };
            }
            Resources.UnloadAsset(boardText);
        }
    }
    #endregion

    private Dictionary<int, List<NPCBoardState>> layouts = new Dictionary<int, List<NPCBoardState>>();

    public void AssignLayout(Node node)
    {
        List<NPCBoardState> possibleLayouts = new List<NPCBoardState>();

        int range = 0;

        // For the first level only assign difficulty 1 stages
        if (node.Level == 1)
        {
            int randLayout = Random.Range(0, layouts[1].Count);
            node.Layout = layouts[1][randLayout];
            return;
        }

        bool levelChecked = false;

        // combats will have a difficulty of +/- 1 from the level the node is on
        // if there are no layouts for the difficulty then keep expanding the range by 1
        while (possibleLayouts.Count < 1)
        {
            if (!levelChecked)
            {
                levelChecked = true;
                if (layouts.ContainsKey(node.Level))
                {
                    possibleLayouts.AddRange(layouts[node.Level]);
                }
            }
            range++;
            if (layouts.ContainsKey(node.Level + range))
            {
                possibleLayouts.AddRange(layouts[node.Level + range]);
            }

            if (layouts.ContainsKey(node.Level - range))
            {
                possibleLayouts.AddRange(layouts[node.Level - range]);
            }
        }

        int rand = Random.Range(0, possibleLayouts.Count);
        node.Layout = possibleLayouts[rand];
    }
}
