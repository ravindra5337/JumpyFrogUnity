using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GridManager : MonoBehaviour
{

    [SerializeField]
    GameObject pLanes;

    
 [SerializeField]
    TextMeshProUGUI mRemainingGameTimeText;
    [SerializeField]
    TextMeshProUGUI mGameStartCountDownText;
    [SerializeField]
    TextMeshProUGUI mTopPlayerScoreGameoverText;
    [SerializeField]
    TextMeshProUGUI mBottomPlayerScoreGameoverText;

  


    [SerializeField]
    TextMeshProUGUI mTopPlayerScoreText;
    [SerializeField]
    TextMeshProUGUI mBottomPlayerScoreText;

    [SerializeField]
    GameObject mMainMenu ;

    [SerializeField]
    GameObject mInGameControllerUI;

    [SerializeField]
    GameObject mGamePlayScreen;


    [SerializeField]
    GameObject mGameOverMenu;

    [SerializeField]
    GameObject pPlayer;


    float mJumpTime = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
       
    }


    List<Lane> mLanes=new List<Lane>();
    List<Player> mPooledPlayer = new List<Player>();
    Player GetPooledPlayer()
    {
        Player retval = null;
        for (int i = 0; i < mPooledPlayer.Count; i++)
        {
            if (!mPooledPlayer[i].IsInUse)
            {
                retval = mPooledPlayer[i];
                break;
            }
        }
        if (retval == null)
        {

           
            GameObject curPlayer = Instantiate(pPlayer) as GameObject;
            retval = curPlayer.GetComponent<Player>();
            mPooledPlayer.Add(retval);

        }
        else
        {
        
        }
       


        return retval;
    }


    void Jump(bool inTopPlayerCharcters)
    {
        bool coolDowned = inTopPlayerCharcters ? mTopPlayerRemainingCoolDownTime <= 0f : mBottomPlayerRemainingCoolDownTime <= 0f;
        if (coolDowned)
        {
            if(inTopPlayerCharcters)
            mTopPlayerRemainingCoolDownTime = mJumpTime;
            else
                mBottomPlayerRemainingCoolDownTime = mJumpTime;
            for (int i = 0; i < mPooledPlayer.Count; i++)
            {
                if (mPooledPlayer[i].IsTopPlayer == inTopPlayerCharcters)
                    mPooledPlayer[i].Jump();
            }

            CreatePlayer(inTopPlayerCharcters);
        }
    }

    float mRemainingCountDown = 0;

    public void OnClickPlay()
    {
        mGameOverMenu.SetActive(false);
        mGamePlayScreen.SetActive(true);
        mMainMenu.SetActive(false);
        mRemainingCountDown = 3f;
        mGameStartCountDownText.text = "3";
        mInGameControllerUI.SetActive(false);
        CreateGrid();
         mTopPlayerScore = 0;
         mBottomPlayerScore = 0;
        StartGame();

        mRemainingGameTime = mTotalGameTime;
    }

    [SerializeField]
    float mTotalGameTime = 120f;


    public void OnClickRestart()
    {

         mTotalTopPlayerCharSpawned = 0;
         mTotalBottomPlayerCharSpawned = 0;
        mGameOverMenu.SetActive(false);
        mGamePlayScreen.SetActive(true);
        mMainMenu.SetActive(false);
        mRemainingCountDown = 3f;
        mGameStartCountDownText.text = "3";
        mInGameControllerUI.SetActive(false);

        mRemainingGameTime = mTotalGameTime;
        mTopPlayerScore = 0;
        mBottomPlayerScore = 0;

        mTopPlayerScoreText.text = mTopPlayerScore.ToString();
        mBottomPlayerScoreText.text = mBottomPlayerScore.ToString();
        mTopPlayerScoreGameoverText.text = mTopPlayerScore.ToString();
        mBottomPlayerScoreGameoverText.text = mBottomPlayerScore.ToString();

    }


    void OnScored(bool isTpPlayer)
    {
        if (isTpPlayer)
            mTopPlayerScore++;
        else
            mBottomPlayerScore++;

        mTopPlayerScoreText.text = mTopPlayerScore.ToString();
        mBottomPlayerScoreText.text = mBottomPlayerScore.ToString();
        mTopPlayerScoreGameoverText.text = mTopPlayerScore.ToString();
        mBottomPlayerScoreGameoverText.text = mBottomPlayerScore.ToString();
    }

    int mTopPlayerScore = 0;
    int mBottomPlayerScore = 0;



    public void OnClickJump(bool isTopPlayer)
    {
        Jump(isTopPlayer);
    }
    // Update is called once per frame

    float mTopPlayerRemainingCoolDownTime = 0f;
    float mBottomPlayerRemainingCoolDownTime = 0f;

    float mRemainingGameTime;

    void GameOver()
    {

        mGamePlayScreen.SetActive(false);
        mGameOverMenu.SetActive(true);
    }
    void Update()
    {
        if (mRemainingCountDown >= 0f)
        {
            mGameStartCountDownText.text = string.Format("{0:0}", mRemainingCountDown);
            mRemainingCountDown -= Time.deltaTime;
        }
        else
        {
            mGameStartCountDownText.text = "";
            mInGameControllerUI.SetActive(true);

            if (mGamePlayScreen.activeSelf)
            {
                if (mRemainingGameTime <= 0)
                {
                    GameOver();
                }
                else
                {
                    mRemainingGameTimeText.text = string.Format("{0:0.}:{1:00.}", (int)(mRemainingGameTime/60),mRemainingGameTime%60);
                    mRemainingGameTime -= Time.deltaTime;
                }
            }

        }
        if (mTopPlayerRemainingCoolDownTime > 0f)
            mTopPlayerRemainingCoolDownTime -= Time.deltaTime;

        if (mBottomPlayerRemainingCoolDownTime > 0f)
            mBottomPlayerRemainingCoolDownTime -= Time.deltaTime;

     

    }

    int mTotalTopPlayerCharSpawned = 0;
    int mTotalBottomPlayerCharSpawned = 0;

    void CreatePlayer(bool isTopPlayer)
    {
        if (isTopPlayer)
            mTotalTopPlayerCharSpawned++;
        else
            mTotalBottomPlayerCharSpawned++;

        float playerSize = mLanes[0].transform.localScale.y;
        playerSize = playerSize > (mLanes[0].transform.localScale.x / 7) ? (mLanes[0].transform.localScale.x / 7) : playerSize;

        Player curPlayer = GetPooledPlayer();
        curPlayer.transform.parent = null;
        curPlayer.transform.localScale = new Vector3(playerSize, playerSize, 0);

        int sourceLaneId = isTopPlayer ? 0 : mLanes.Count - 1;
        int destLaneId = isTopPlayer ? mLanes.Count - 1 :0 ;

        Vector3 pos= mLanes[sourceLaneId].transform.position;

        int offsetVal = 0;
        if (isTopPlayer)
            offsetVal = mTotalTopPlayerCharSpawned % 3;
        else
            offsetVal = (mTotalBottomPlayerCharSpawned+1) % 3;

        if (offsetVal == 2)
            offsetVal = -1;

        if (isTopPlayer)
            offsetVal *= -1;

        pos.x += (mLanes[0].transform.lossyScale.x / 5) * offsetVal;


        curPlayer.transform.position = pos;

      float jumpStrength=  mLanes[0].transform.localScale.y * 2f;
        jumpStrength = jumpStrength*(sourceLaneId == 0 ? -1f : 1f);
        curPlayer.OnScored -= OnScored;
        curPlayer.OnScored += OnScored;
        curPlayer.Initialize(isTopPlayer, mJumpTime,jumpStrength, sourceLaneId, destLaneId);

    }

    public int mTotalLanes=11;




    void CreateGrid()
    {
        Camera c = Camera.main;
        float height = c.orthographicSize * 2f;
        float width = c.aspect * height;



        float heightOfLanes = height / mTotalLanes;
        float topMostY = (mTotalLanes / 2) * heightOfLanes;

        for(int i = 0; i < mTotalLanes; i++)
        {
            GameObject curLane= Instantiate(pLanes) as GameObject;
            curLane.transform.localScale = new Vector3(width, heightOfLanes, 0);
            curLane.transform.position = new Vector3(0, topMostY - i * heightOfLanes);
            curLane.name = "Lane_" + i;
            curLane.transform.parent = this.transform;
            Lane laneScrpt = curLane.GetComponent<Lane>();
            bool isRiver = i > 0 && i < mTotalLanes - 1;
            laneScrpt.Initialize(i,isRiver, isRiver?i%2==0:false,null);
            mLanes.Add(laneScrpt);


        }

        CreatePlayer(false);
        CreatePlayer(true);
    }


    void StartGame()
    {
        for (int i = 0; i < mTotalLanes; i++)
        {
            mLanes[i].StartPlankGeneration();
        }
    }
   
}
