using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffDisplayController : MonoBehaviour
{
    [SerializeField] private GameObject pointsBuff;
    [SerializeField] private GameObject damageBuff;
    [SerializeField] private GameObject noBuff;


    public void TogglePointsBuff(bool _isActive, int _index)
    {
        switch (_index)
        {
            case 0:
                pointsBuff.SetActive(_isActive);
                break;
            case 1:
                damageBuff.SetActive(_isActive);
                break;
        }

        if (!damageBuff.activeInHierarchy && !pointsBuff.activeInHierarchy)
            noBuff.SetActive(true);
        else
            noBuff.SetActive(false);
    }
}