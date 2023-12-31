using UnityEngine;
using System.Collections;

public class Player_AI : Player_Base
{
    private enum CheckDir
    {
        Left,
        Up,
        Right,
        Down,
        EnumMax
    }

    private enum CheckData
    {
        X,
        Y,
        EnumMax
    }

    private static readonly int[][] CHECK_DIR_LIST = new int[(int)CheckDir.EnumMax][]
    {
        new int[(int)CheckData.EnumMax] { -1, 0 }, new int[(int)CheckData.EnumMax] { 0, 1 },
        new int[(int)CheckData.EnumMax] { 1, 0 }, new int[(int)CheckData.EnumMax] { 0, -1 }
    };

    private static readonly int AI_PRIO_MIN = 99;


    private static readonly float AI_INTERVAL_MIN = 0.5f;
    private static readonly float AI_INTERVAL_MAX = 0.8f;

    private static readonly float AI_IGNORE_DISTANCE = 2.0f;

    private static readonly float SHOOT_INTERVAL = 1.0f;


    private float m_aiInterval = 0f;
    private float m_shootInterval = 0f;


    private PlayerInput m_pressInput = PlayerInput.Move_Left;

    protected override void GetInput()
    {
        GameObject mainObject = Player_Key.m_mainPlayer;
        if (null == mainObject)
        {
            return;
        }

        m_aiInterval -= Time.deltaTime;

        m_shootInterval -= Time.deltaTime;

        Vector3 aiSubPosition = (transform.position - mainObject.transform.position);
        aiSubPosition.y = 0f;

        if (aiSubPosition.magnitude > AI_IGNORE_DISTANCE)
        {
            if (m_aiInterval < 0f)
            {
                m_aiInterval = Random.Range(AI_INTERVAL_MIN, AI_INTERVAL_MAX);

                int[] prioTable = GetMovePrioTable();

                int highest = AI_PRIO_MIN;
                int i;
                for (i = 0; i < (int)CheckDir.EnumMax; i++)
                {
                    if (highest > prioTable[i])
                    {
                        highest = prioTable[i];
                    }
                }

                PlayerInput pressInput = PlayerInput.Move_Left;
                if (highest == prioTable[(int)CheckDir.Left])
                {
                    pressInput = PlayerInput.Move_Left;
                }
                else if (highest == prioTable[(int)CheckDir.Right])
                {
                    pressInput = PlayerInput.Move_Right;
                }
                else if (highest == prioTable[(int)CheckDir.Up])
                {
                    pressInput = PlayerInput.Move_Up;
                }
                else if (highest == prioTable[(int)CheckDir.Down])
                {
                    pressInput = PlayerInput.Move_Down;
                }

                m_pressInput = pressInput;
            }

            m_playerInput[(int)m_pressInput] = true;
        }

        if (m_shootInterval < 0f)
        {
            if ((Mathf.Abs(aiSubPosition.x) < 1f) || (Mathf.Abs(aiSubPosition.z) < 1f))
            {
                m_playerInput[(int)PlayerInput.Shoot] = true;

                m_shootInterval = SHOOT_INTERVAL;
            }
        }
    }


    private int GetGridX(float posX)
    {
        return Mathf.Clamp((int)((posX) / Field.BLOCK_SCALE), 0, (Field.FIELD_GRID_X - 1));
    }

    private int GetGridY(float posZ)
    {
        return Mathf.Clamp((int)((posZ) / Field.BLOCK_SCALE), 0, (Field.FIELD_GRID_Y - 1));
    }


    private int[] GetMovePrioTable()
    {
        int i, j;

        Vector3 aiPosition = transform.position;
        int aiX = GetGridX(aiPosition.x);
        int aiY = GetGridY(aiPosition.z);

        GameObject mainObject = Player_Key.m_mainPlayer;

        Vector3 playerPosition = mainObject.transform.position;

        int playerX = GetGridX(playerPosition.x);
        int playerY = GetGridY(playerPosition.z);
        int playerGrid = playerX + (playerY * Field.FIELD_GRID_X);

        int[] calcGrid = new int[(Field.FIELD_GRID_X * Field.FIELD_GRID_Y)];

        for (i = 0; i < (Field.FIELD_GRID_X * Field.FIELD_GRID_Y); i++)
        {
            calcGrid[i] = AI_PRIO_MIN;
        }

        calcGrid[playerGrid] = 1;

        int checkPrio = 1;
        int checkX;
        int checkY;
        int tempX;
        int tempY;
        int tempGrid;
        bool update;
        do
        {
            update = false;

            for (i = 0; i < (Field.FIELD_GRID_X * Field.FIELD_GRID_Y); i++)
            {
                if (checkPrio != calcGrid[i])
                {
                    continue;
                }

                checkX = (i % Field.FIELD_GRID_X);
                checkY = (i / Field.FIELD_GRID_X);

                for (j = 0; j < (int)CheckDir.EnumMax; j++)
                {
                    tempX = (checkX + CHECK_DIR_LIST[j][(int)CheckData.X]);
                    tempY = (checkY + CHECK_DIR_LIST[j][(int)CheckData.Y]);

                    if ((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y))
                    {
                        continue;
                    }

                    tempGrid = (tempX + (tempY * Field.FIELD_GRID_X));

                    if (Field.ObjectKind.Block == (Field.ObjectKind)Field.GRID_OBJECT_DATA[tempGrid])
                    {
                        continue;
                    }

                    if (calcGrid[tempGrid] > (checkPrio + 1))
                    {
                        calcGrid[tempGrid] = (checkPrio + 1);
                        update = true;
                    }
                }
            }

            checkPrio++;
        } while (update);

        int[] prioTable = new int[(int)CheckDir.EnumMax];

        for (i = 0; i < (int)CheckDir.EnumMax; i++)
        {
            tempX = (aiX + CHECK_DIR_LIST[i][(int)CheckData.X]);
            tempY = (aiY + CHECK_DIR_LIST[i][(int)CheckData.Y]);

            if ((tempX < 0) || (tempX >= Field.FIELD_GRID_X) || (tempY < 0) || (tempY >= Field.FIELD_GRID_Y))
            {
                prioTable[i] = AI_PRIO_MIN;
                continue;
            }

            tempGrid = (tempX + (tempY * Field.FIELD_GRID_X));
            prioTable[i] = calcGrid[tempGrid];
        }

        {
            string temp = "";

            temp += "PRIO TABLE\n";
            for (tempY = 0; tempY < Field.FIELD_GRID_Y; tempY++)
            {
                for (tempX = 0; tempX < Field.FIELD_GRID_X; tempX++)
                {
                    temp += "\t\t" + calcGrid[tempX + ((Field.FIELD_GRID_Y - 1 - tempY) * Field.FIELD_GRID_X)] + "";

                    if ((aiX == tempX) && (aiY == (Field.FIELD_GRID_Y - 1 - tempY)))
                    {
                        temp += "*";
                    }
                }

                temp += "\n";
            }

            temp += "\n";

            temp += "RESULT\n";
            for (i = 0; i < (int)CheckDir.EnumMax; i++)
            {
                temp += "\t" + prioTable[i] + "\t" + (CheckDir)i + "\n";
            }

            Debug.Log("" + temp);
        }

        return prioTable;
    }
}