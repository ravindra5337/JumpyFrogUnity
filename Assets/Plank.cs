using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Plank : MonoBehaviour
{
    bool mIsInUse = false;
    public bool IsInUse
    {
        get
        {
            return mIsInUse;
        }

    }


    public void StartMoving(Vector3 fromSource,Vector3 destination,float inSpeed)
    {
        this.gameObject.transform.DOKill();
        this.gameObject.transform.position = fromSource;
        mIsInUse = true;
        this.gameObject.transform.DOMove(destination, inSpeed).OnComplete(() =>
        {
            mIsInUse = false;
        }).SetSpeedBased(true).SetEase(Ease.Linear);
    }
}
