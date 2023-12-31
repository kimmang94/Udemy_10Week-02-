using UnityEngine;
using System.Collections;

public class Field : MonoBehaviour
{
    public GameObject m_blockObject = null;

    public GameObject m_player1Object = null;
    public GameObject m_player2Object = null;


    public static readonly int FIELD_GRID_X = 9;
    public static readonly int FIELD_GRID_Y = 9;

    public static readonly float BLOCK_SCALE = 2.0f;
    public static readonly Vector3 BLOCK_OFFSET = new Vector3(1, 1, 1);

    public enum ObjectKind
    {
        Empty,
        Block,
        Player1,
        Player2
    }

    public static readonly int[] GRID_OBJECT_DATA = new int[]
    {
        1, 1, 1, 1, 1, 1, 1, 1, 1,
        1, 2, 0, 0, 0, 0, 0, 0, 1,
        1, 0, 1, 1, 1, 0, 1, 0, 1,
        1, 0, 0, 0, 0, 0, 0, 0, 1,
        1, 0, 1, 0, 1, 1, 1, 0, 1,
        1, 0, 1, 0, 1, 0, 0, 0, 1,
        1, 0, 1, 0, 0, 0, 1, 0, 1,
        1, 0, 0, 0, 1, 0, 0, 3, 1,
        1, 1, 1, 1, 1, 1, 1, 1, 1,
    };


    private GameObject m_blockParent = null;

    private void Awake()
    {
        InitializeField();
    }

    private void InitializeField()
    {
        m_blockParent = new GameObject();
        m_blockParent.name = "BlockParent";
        m_blockParent.transform.parent = transform;

        GameObject originalObject;
        GameObject instanceObject;
        Vector3 position;

        int gridX;
        int gridY;
        for (gridX = 0; gridX < FIELD_GRID_X; gridX++)
        {
            for (gridY = 0; gridY < FIELD_GRID_Y; gridY++)
            {
                switch ((ObjectKind)GRID_OBJECT_DATA[gridX + (gridY * FIELD_GRID_X)])
                {
                    case ObjectKind.Block:
                        originalObject = m_blockObject;
                        break;
                    case ObjectKind.Player1:
                        originalObject = m_player1Object;
                        break;
                    case ObjectKind.Player2:
                        originalObject = m_player2Object;
                        break;
                    default:
                        originalObject = null;
                        break;
                }

                if (null == originalObject)
                {
                    continue;
                }

                position = new Vector3(gridX * BLOCK_SCALE, 0, gridY * BLOCK_SCALE) + BLOCK_OFFSET;

                instanceObject = Instantiate(originalObject, position, originalObject.transform.rotation) as GameObject;

                instanceObject.name = "" + originalObject.name + "(" + gridX + "," + gridY + ")";

                instanceObject.transform.localScale = (Vector3.one * BLOCK_SCALE);

                instanceObject.transform.parent = m_blockParent.transform;
            }
        }
    }
}