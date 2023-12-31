using UnityEngine;
using System.Collections;

public class Player_Key : Player_Base
{
    public static GameObject m_mainPlayer = null;

    private void Awake()
    {
        m_mainPlayer = gameObject;
    }

    public KeyCode KEYCODE_MOVE_LEFT = KeyCode.A;
    public KeyCode KEYCODE_MOVE_UP = KeyCode.W;
    public KeyCode KEYCODE_MOVE_RIGHT = KeyCode.D;
    public KeyCode KEYCODE_MOVE_DOWN = KeyCode.S;
    public KeyCode KEYCODE_SHOOT = KeyCode.Space;

    protected override void GetInput()
    {
        if (Input.GetKey(KEYCODE_MOVE_LEFT))
        {
            m_playerInput[(int)PlayerInput.Move_Left] = true;
        }
        else if (Input.GetKey(KEYCODE_MOVE_RIGHT))
        {
            m_playerInput[(int)PlayerInput.Move_Right] = true;
        }

        if (Input.GetKey(KEYCODE_MOVE_UP))
        {
            m_playerInput[(int)PlayerInput.Move_Up] = true;
        }
        else if (Input.GetKey(KEYCODE_MOVE_DOWN))
        {
            m_playerInput[(int)PlayerInput.Move_Down] = true;
        }

        if (Input.GetKeyDown(KEYCODE_SHOOT))
        {
            m_playerInput[(int)PlayerInput.Shoot] = true;
        }
    }
}