using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NodeDisplay : MonoBehaviour
{
    public Node Node { get; set; }

    [SerializeField]
    private GameObject particleParent;

    private GameObject lineObject;
    private LineRenderer lr;

    public void DrawLine(GameObject LineParent)
    {
        //GetComponentInChildren<TextMeshProUGUI>().text = Node.Name;

        // Draw a line to child nodes
        foreach (Node child in Node.Next)
        {
            lineObject = new GameObject("LineObject")
            {
                // Set layer to UI
                layer = 5
            };
            // TODO: Parent line objects to something
            lineObject.transform.parent = LineParent.transform;

            //lineObject.transform.parent = transform;
            lr = lineObject.AddComponent<LineRenderer>();
            lr.startWidth = 1f;
            lr.endWidth = 1f;
            lr.positionCount = 2;
            lr.sortingLayerName = "Top";
            lr.sortingOrder = -1;

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            lr.material = mat;
            lr.useWorldSpace = false;

            Vector3[] checkpoints = new Vector3[2] { transform.position, child.NodeDisplay.transform.position };
            lr.SetPositions(checkpoints);
            //Debug.Log("x: " + checkpoints[0].x + " y: " + checkpoints[0].y + " x: " + checkpoints[1].x + " y: " + checkpoints[1].y);
        }
    }

    private void OnMouseDown()
    {
        if (Node.IsActivated() && GameManager.Instance.GetPhase() is MapPhase && !MapManager.Instance.mapAnimator.IsMapMoving())
        {
            MapManager.Instance.MoveToNode(Node);
            GameManager.Instance.EndCurrent();
        }
    }


    private void OnDestroy()
    {
        // TODO: should be part of the ondestroy on the line object (if not done can lead to memory leak)
        //Destroy(lr.material);
        Destroy(lineObject);
    }

    public void ActivateParticles()
    {
        particleParent.SetActive(true);
    }

    public void DeactivateParticles()
    {
        particleParent.SetActive(false);
    }

    public void SetupTooltip()
    {
        TooltipTrigger toolTip = GetComponent<TooltipTrigger>();
        toolTip.header = Node.NodeEvent.Name;

        if (Node.Layout!= null)
        {
            toolTip.content = Node.Layout.setNames.ToString() + " " + Node.Layout.setCounts.ToString();
        }
    }
}
