using UnityEngine;
using System.Collections;

public class HitObject : MonoBehaviour
{
    public enum HitGroup
    {
        Player1,
        Player2,
        Other
    }

    public HitGroup m_hitGroup = HitGroup.Player1;

    protected bool IsHitOK(GameObject hittedObject)
    {
        HitObject hit = hittedObject.GetComponent<HitObject>();

        if (null == hit)
        {
            return false;
        }

        if (m_hitGroup == hit.m_hitGroup)
        {
            return false;
        }

        return true;
    }
}