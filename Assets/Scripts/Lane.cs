using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    int mId = 0;
    public int Id
    {
        get
        {
            return mId;
        }
    }
    bool mIsCanGeneratePlanks = false;
    // Start is called before the first frame update
    public void Initialize(int inId,bool isRiver,bool isCanGeneratePlanks,List<byte> inPlankSeq=null)
    {
        mId = inId;
        mIsCanGeneratePlanks = isCanGeneratePlanks;
        mIsRiver = isRiver;
         mPlankUnitWidth = this.gameObject.transform.localScale.x/7;

         mPlankUnitHeight = this.gameObject.transform.localScale.y;

        if (mIsRiver && mIsCanGeneratePlanks && (inPlankSeq==null || inPlankSeq.Count==0))
        {

            GenerateRandomPlankSequence();
        }

        mCollider.enabled = (mIsRiver && mIsCanGeneratePlanks) || !mIsRiver; 

      this.gameObject.GetComponent<SpriteRenderer>().color = mIsRiver ? new Color(52f / 255f, 210f / 255f, 235f/255f) : new Color(10f / 255f, 168f / 255f, 52f / 255f);
    }

    bool mIsRiver;

    [SerializeField]
    GameObject pPlank;

    void GenerateRandomPlankSequence()
    {
        if (mPlankGenerationSequence.Count == 0)
        {
            for(int i=0;i<20;i++)
                mPlankGenerationSequence.Add((byte)UnityEngine.Random.Range(1, 4));

        }
    }

    List<byte> mPlankGenerationSequence = new List<byte>();
    public List<byte> PlankGenerationSequence
    {
        get
        {
            return mPlankGenerationSequence;
        }
    }

    int mCurrentId = 0;
    [SerializeField]
    int mFlowDirection =-1;
    float mPlankUnitWidth = 0;
    [SerializeField]
    float mFlowSpeed = 1f;


    [SerializeField]
    Collider2D mCollider;


    float mPlankUnitHeight = 0;
    public Plank GetPlankFromPool()
    {

        Plank retval = null;
        for(int i = 0; i < mPooledPlanks.Count; i++)
        {
            if (!mPooledPlanks[i].IsInUse)
            {
                retval = mPooledPlanks[i];
                break;
            }
        }
        GameObject curPlank = null;
        if (retval == null)
        {
            curPlank = Instantiate(pPlank) as GameObject;
            retval = curPlank.GetComponent<Plank>();
            mPooledPlanks.Add(retval);
        }
        else
        {
            curPlank = retval.gameObject;
        }

        int sizeMultiplier =  mPlankGenerationSequence[mCurrentId];
        curPlank.transform.localScale = new Vector3(mPlankUnitWidth* sizeMultiplier, mPlankUnitHeight, 0);
        curPlank.transform.position = this.transform.position+ mFlowDirection *new Vector3(this.gameObject.transform.localScale.x/2+ curPlank.transform.localScale.x/2,0,0);

        mCurrentId = (mCurrentId + 1) % mPlankGenerationSequence.Count;
        return retval;


    }

    List<Plank> mPooledPlanks = new List<Plank>();

    void StartPlanks()
    {
       
        Plank plank =GetPlankFromPool();
        Vector3 dest = this.transform.position + -1*mFlowDirection * new Vector3(this.gameObject.transform.localScale.x / 2 + plank.gameObject.transform.localScale.x , 0, 0);

        plank.StartMoving(plank.gameObject.transform.position, dest, mFlowSpeed);


        float afterTime = 0f;
        afterTime = mFlowSpeed * ((plank.gameObject.transform.localScale.x+ mPlankUnitWidth) /mPlankUnitWidth) *1f;

        Invoke("StartPlanks", afterTime);

    }

    public void StartPlankGeneration()
    {

        if (mIsRiver && mIsCanGeneratePlanks)
        {
            StartPlanks();
        }

    }
    public bool IsRiver
    {
        get
        {
            return mIsRiver;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
