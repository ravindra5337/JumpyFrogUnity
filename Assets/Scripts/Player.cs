using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Player : MonoBehaviour
{

    [SerializeField]
    ParticleSystem mParticle;

    public delegate void ScoredEvent(bool isTopPlayer);

    public ScoredEvent OnScored;

    float mUnitJumpDistance = 0f;

    Vector3 mInitialScale;

    int mSourceLaneId;
    int mDestinationLaneId;
    float mJumpTime = 0.4f;
    bool mIsInUse = false;
    bool mTopPlayer = false;
    public void Initialize(bool inTopPlayer, float inJumpTime,   float inJumpStrength,int inSourceId,int inDestinationId)
    {
        mParticle.Stop();
        mJumpTime = inJumpTime;
       mTopPlayer = inTopPlayer;
        mIsInUse = true;
        mUnitJumpDistance = inJumpStrength;
        mInitialScale = this.gameObject.transform.localScale;
        mJumping = false;
        mDead = false;
        mIsOnPlank = false;
         mSourceLaneId= inSourceId;
         mDestinationLaneId= inDestinationId;

        this.gameObject.GetComponent<SpriteRenderer>().color = inSourceId == 0 ? Color.yellow : Color.red;

    }

    bool mDead = false;
    bool mJumping = false;

   

    bool mIsOnPlank = false;

    Vector3 mJumpDestination;
   public void Jump()
    {

        if (!mJumping && !mDead && mIsInUse)
        {
            mJumping = true;
            mIsOnPlank = false;
            this.gameObject.transform.parent = null;
            this.gameObject.transform.DOMoveY(mUnitJumpDistance, mJumpTime).SetRelative().OnComplete(() =>
            {
                mJumping = false;
            });
        }

    }

    void OnTriggerExit2D(Collider2D col)
    {
      
        if (!mDead && mIsInUse)
        {
            Lane colLane = col.GetComponent<Lane>();

            if (colLane != null)
            {

                Vector3 pos = this.gameObject.transform.position;
                float widthhalf = Camera.main.aspect * Camera.main.orthographicSize;

                if (Mathf.Abs(pos.x) > widthhalf)
                {
                    Debug.Log("Dead");
                    mDead = true;
                    mIsInUse = false;
                    this.gameObject.transform.position = new Vector3(1000f, 0f, 0f);
                    this.gameObject.transform.parent = null;
                }




            }

        }
    }

        void OnTriggerEnter2D(Collider2D col)
    {

        if (!mDead && mIsInUse)
        {
            Lane colLane = col.GetComponent<Lane>();
            Plank plank = col.GetComponent<Plank>();
            Player othrplayer = col.GetComponent<Player>();

            if (othrplayer != null && othrplayer.mTopPlayer != mTopPlayer && !mJumping)
            {
                Debug.Log("Dead");
                mDead = true;
                DeathAnimation();
            }
            if(!mDead){ 
            if (plank != null)
            {
                mIsOnPlank = true;
                this.gameObject.transform.parent = plank.gameObject.transform;

                Vector3 plankScale = plank.gameObject.transform.localScale;
                // this.gameObject.transform.localScale =new Vector3(plankScale.x/ mInitialScale.x , plankScale.y/mInitialScale.y ,0);


            }

            if (colLane != null)
            {
                if (colLane.IsRiver)
                {
                    if (!mIsOnPlank)
                    {
                        Debug.Log("Dead");
                        mDead = true;
                        DeathAnimation();
                    }
                    else
                    {
                        Debug.Log("Is on Plank");
                    }
                }
                else
                {
                    if (colLane.Id == mDestinationLaneId)
                    {
                        mDead = true;
                        ScoreAnimation();
                            OnScored?.Invoke(mTopPlayer);
                    }
                }
            }
        }
        }
    }


    void DeathAnimation()
    {
        mParticle.Play();
        Sequence deathSeq = DOTween.Sequence();
        deathSeq.Append(transform.DOScale(Vector3.zero,1f))
            .Insert(0, transform.DORotate(new Vector3(0, 0, 180), deathSeq.Duration())).OnComplete(()=> { mIsInUse = false;

                transform.DOKill();
            });
    }

    void ScoreAnimation()
    {
        
        Sequence deathSeq = DOTween.Sequence();
        deathSeq.Append(transform.DOScale(Vector3.zero, 1f))
            .Insert(0, transform.DORotate(new Vector3(0, 0, 180), deathSeq.Duration())).OnComplete(() => { mIsInUse = false; transform.DOKill(); });
    }


    public bool IsInUse
    {
        get
        {
            return mIsInUse;
        }

    }

    public bool IsTopPlayer
    {
        get
        {
            return mTopPlayer;
        }

    }


}
