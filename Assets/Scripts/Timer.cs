using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour
{
    public float fTimeLimit = 1f;

    private float m_fTimeLeft = 0f;

    private void Awake()
    {
        m_fTimeLeft = fTimeLimit;
    }

    private void Update()
    {
        m_fTimeLeft -= Time.deltaTime;
        if (m_fTimeLeft < 0f)
        {
            Destroy(gameObject);
        }
    }
}