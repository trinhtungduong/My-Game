using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonControllerTest : MonoBehaviourPunCallbacks, IDamageMonster, IPunObservable
{
    [Header("Photon Setting")]
    public PhotonView PV;

    [Header("List Players In Room")]
    public List<PlayerController> listPlayers;

    [Header("Worm Properties")]
    public MonsterLiveState monsLiveState;
    public MonsterBehaviorState monsBehaviorState;
    public float health = 10000f;
    public List<Transform> listBodyParts;
    public float headX;
    private float networkedHeadX;
    private Transform targetPlayer;
    public float dirHead;
    public float headSpeed;
    public float limitHeadX;
    private float timeSkill;

    [Header("Skill")]
    public GameObject fireBall;
    public Transform mouth;

    [Header("UI")]
    public Image healthBar;
    public TMP_Text resultText;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(headX);
            stream.SendNext(dirHead);
        }
        else
        {
            networkedHeadX = (float)stream.ReceiveNext();
            if (Mathf.Abs(networkedHeadX - headX) > 15)
            {
                headX = networkedHeadX;
                dirHead = (float)stream.ReceiveNext();
            }
        }
    }

    private void Start()
    {
        listPlayers = BossTest.instance.listPlayerInRoom;
    }
    private void Update()
    {
        DampingHead();
        LookAtTarget();

        if (PV.IsMine && BossTest.instance.gameState == GameState.Playing)
        {
            FindTarget();
            CoolDownSkill();
        }        
    }   
    public void CoolDownSkill()
    {
        if(monsBehaviorState == MonsterBehaviorState.Idle)
        {
            timeSkill -= Time.deltaTime;
            if(timeSkill < 0f)
            {
                timeSkill = 10f;
                UpdateState(MonsterBehaviorState.CastSkill);
            }
        }
    }
    public void UpdateState(MonsterBehaviorState newState)
    {
        monsBehaviorState = newState;
        switch (monsBehaviorState)
        {
            case MonsterBehaviorState.Idle:
                Idle();
                break;
            case MonsterBehaviorState.Move:
                Move();
                break;
            case MonsterBehaviorState.CastSkill:
                CastSkill();
                break;
            default:
                break;
        }
    }
    public void DampingHead()
    {
        headX += Time.deltaTime * dirHead * headSpeed;
        if (headX > limitHeadX || headX < -limitHeadX) dirHead = -dirHead;
        headX = Mathf.Clamp(headX, -limitHeadX, limitHeadX);
        listBodyParts[0].localPosition = headX * Vector3.right;
    }
    public void FindTarget()
    {
        if (monsBehaviorState != MonsterBehaviorState.Idle) return;

        if(targetPlayer != null)
        {
            return;
        }

        float maxHealth = 10000f;
        int indexPlayer = -1;

        for (int i = 0; i < listPlayers.Count; i++)
        {
            if (listPlayers[i] != null)
            {
                if (indexPlayer < 0)
                    indexPlayer = i;

                if (listPlayers[i].health < maxHealth)
                {
                    maxHealth = listPlayers[i].health;
                    indexPlayer = i;
                }                
            }
        }

        PV.RPC(nameof(RPC_SetTarget), RpcTarget.All, indexPlayer);
    }
    public void LookAtTarget()
    {
        if (targetPlayer == null) return;

        transform.LookAt(targetPlayer);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
    public void Idle()
    {
        
    }
    public void Move()
    {
        
    }
    public void CastSkill()
    {
        PV.RPC(nameof(RPC_FireBall), RpcTarget.All);
    }

    public void TakeDamage()
    {
        if(monsLiveState == MonsterLiveState.Live && monsBehaviorState == MonsterBehaviorState.Idle)
            PV.RPC(nameof(RPC_TakeDamage), RpcTarget.All);
    }

    public void EndGame()
    {
        for (int i = 0; i < listPlayers.Count; i++)
        {
            if (listPlayers[i] != null)
            {
                listPlayers[i].RemovePlayer();
            }
        }
        
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    void RPC_TakeDamage()
    {
        Debug.Log("Took Damage");
        health -= 500f;
        health = Mathf.Clamp(health, 0f, 10000f);
        healthBar.fillAmount = health / 10000f;

        if(health == 0f)
        {
            monsLiveState = MonsterLiveState.Dead;
            BossTest.instance.gameState = GameState.EndGame;
            Destroy(RoomManager.Instance.gameObject);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            resultText.text = "WIN";
            resultText.gameObject.SetActive(true);
            Invoke(nameof(EndGame), 5f);
        }
    }

    [PunRPC]
    void RPC_SetTarget(int index)
    {
        targetPlayer = listPlayers[index].transform;
    }

    [PunRPC]
    void RPC_FireBall()
    {
        fireBall.transform.position = mouth.position;
        fireBall.SetActive(true);
        var dir = Vector3.forward;
        if (targetPlayer != null)
            dir = (targetPlayer.position - mouth.position).normalized;
        fireBall.GetComponent<Rigidbody>().velocity = dir * 15f;
        targetPlayer = null;
        UpdateState(MonsterBehaviorState.Idle);
    }
}

public enum MonsterBehaviorState
{
    Idle,
    Move,
    CastSkill
}

public enum MonsterLiveState
{
    Live,
    Dead
}
