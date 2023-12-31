using UnityEngine;
using System.Collections;

public class Player_Base : HitObject
{
    protected enum PlayerInput
    {
        Move_Left,
        Move_Up,
        Move_Right,
        Move_Down,
        Shoot,
        EnumMax
    }

    private static readonly float MOVE_ROTATION_Y_LEFT = -90f;
    private static readonly float MOVE_ROTATION_Y_UP = 0f;
    private static readonly float MOVE_ROTATION_Y_RIGHT = 90f;
    private static readonly float MOVE_ROTATION_Y_DOWN = 180f;

    public float MOVE_SPEED = 5.0f;


    public GameObject playerObject = null;
    public GameObject bulletObject = null;


    public GameObject hitEffectPrefab = null;


    private float m_rotationY = 0.0f;

    protected bool[] m_playerInput = new bool[(int)PlayerInput.EnumMax];
    protected bool m_playerDeadFlag = false;

    private void Update()
    {
        if (m_playerDeadFlag)
        {
            return;
        }

        ClearInput();
        GetInput();
        CheckMove();
    }

    private void ClearInput()
    {
        int i;
        for (i = 0; i < (int)PlayerInput.EnumMax; i++)
        {
            m_playerInput[i] = false;
        }
    }

    protected virtual void GetInput()
    {
    }

    private void CheckMove()
    {
        Animator animator = playerObject.GetComponent<Animator>();

        float moveSpeed = MOVE_SPEED;
        bool shootFlag = false;

        {
            if (m_playerInput[(int)PlayerInput.Move_Left])
            {
                m_rotationY = MOVE_ROTATION_Y_LEFT;
            }
            else if (m_playerInput[(int)PlayerInput.Move_Up])
            {
                m_rotationY = MOVE_ROTATION_Y_UP;
            }
            else if (m_playerInput[(int)PlayerInput.Move_Right])
            {
                m_rotationY = MOVE_ROTATION_Y_RIGHT;
            }
            else if (m_playerInput[(int)PlayerInput.Move_Down])
            {
                m_rotationY = MOVE_ROTATION_Y_DOWN;
            }
            else
            {
                moveSpeed = 0f;
            }

            transform.rotation = Quaternion.Euler(0, m_rotationY, 0);

            transform.position += ((transform.rotation * (Vector3.forward * moveSpeed)) * Time.deltaTime);
        }

        {
            if (m_playerInput[(int)PlayerInput.Shoot])
            {
                shootFlag = true;

                Vector3 vecBulletPos = transform.position;

                vecBulletPos += (transform.rotation * Vector3.forward);

                vecBulletPos.y = 2.0f;

                Instantiate(bulletObject, vecBulletPos, transform.rotation);
            }
            else
            {
                shootFlag = false;
            }
        }

        {
            animator.SetFloat("Speed", moveSpeed);
            animator.SetBool("Shoot", shootFlag);
        }
    }

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (false == IsHitOK(hitCollider.gameObject))
        {
            return;
        }

        {
            Animator animator = playerObject.GetComponent<Animator>();

            animator.SetBool("Dead", true); //死んだフラグ
        }

        if (null != hitEffectPrefab)
        {
            Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        }

        m_playerDeadFlag = true;
    }
}