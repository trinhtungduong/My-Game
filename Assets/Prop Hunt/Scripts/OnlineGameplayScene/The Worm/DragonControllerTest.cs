using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;

public class DragonControllerTest : MonoBehaviourPunCallbacks, IDamageMonster, IPunObservable
{
    [Header("Photon Setting")]
    public PhotonView PV;

    [Header("List Players In Room")]
    public List<PlayerController> listPlayers;

    [Header("Worm Properties")]
    public MonsterLiveState monsLiveState;
    public MonsterBehaviorState monsBehaviorState;
    public Form monsForm;
    public float health = 10000f;
    public List<WormBody> listBodyParts;
    public Transform bossHead;
    public float headX;
    private float networkedHeadX;
    private Transform targetPlayer;
    private PlayerController targetPlayerController;
    public float dirHead;
    public float headSpeed;
    public float limitHeadX;
    private float timeSkill;
    private Quaternion lookRotation;

    [Header("Skill")]
    public GameObject fireBall;
    public Transform mouth;

    [Header("UI")]
    public Image healthBar;

    [Header("Rigging")]
    public Rig form1;
    public Rig form2;

    [Header("Particles")]
    public List<ParticleSystem> explores;

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
        listPlayers = MapManager.instance.listPlayerInRoom;
    }
    private void Update()
    {
        if (monsLiveState == MonsterLiveState.Live)
        {
            if (monsForm == Form.First)
            {
                DampingHead();
                LookAtTarget();

                if (PV.IsMine && MapManager.instance.gameState == GameState.Playing)
                {
                    FindTarget();
                    CoolDownSkill();
                }
            }
            else if(monsForm == Form.Second)
            {
                LookAtTarget();
                if (PV.IsMine && MapManager.instance.gameState == GameState.Playing)
                {
                    FindTarget();
                    CoolDownSkill();
                }
            }
        }
    }   
    public void StartBoss()
    {
        PV.RPC(nameof(RPC_StartBoss), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_StartBoss()
    {
        monsLiveState = MonsterLiveState.Live;
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
                ChooseSkill();
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
        bossHead.localPosition = headX * Vector3.right;
    }
    public void FindTarget()
    {
        if (monsBehaviorState != MonsterBehaviorState.Idle) return;

        if(targetPlayer != null && targetPlayerController != null && targetPlayerController.isAlive)
        {
            return;
        }

        float maxHealth = 10000f;
        int indexPlayer = -1;

        for (int i = 0; i < listPlayers.Count; i++)
        {
            if (listPlayers[i] != null && listPlayers[i].isAlive)
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

        targetPlayerController = listPlayers[indexPlayer];
        PV.RPC(nameof(RPC_SetTarget), RpcTarget.All, indexPlayer);
    }
    public void LookAtTarget()
    {
        if (targetPlayer == null) return;

        lookRotation = Quaternion.LookRotation(targetPlayer.position - transform.position);
        lookRotation.eulerAngles = new Vector3(0f, lookRotation.eulerAngles.y, 0f);
        //transform.LookAt(targetPlayer);
        //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if(monsForm == Form.Second)
        {
            foreach(var part in listBodyParts)
            {
                part.LookAt(targetPlayer.position);
            }
        }
    }
    public void Idle()
    {
        
    }
    public void Move()
    {
        
    }
    public void ChangeForm()
    {
        if(monsForm == Form.First)
        {
            if(health < 5000f)
            {
                monsForm = Form.Second;
                monsLiveState = MonsterLiveState.Transform;
                form1.weight = 0f;
                form2.weight = 1f;
                for(int i = 0; i < listBodyParts.Count; i++)
                {
                    listBodyParts[i].active = true;
                }
                targetPlayer = null;
                monsLiveState = MonsterLiveState.Live;
            }
        }
    }
    public void ChooseSkill()
    {
        Skill_FireBall();
    }
    public void Skill_FireBall()
    {
        PV.RPC(nameof(RPC_FireBall), RpcTarget.All);
    }
    public void TakeDamage(float damage)
    {
        if(monsLiveState == MonsterLiveState.Live)
            PV.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, 10000f);
        healthBar.fillAmount = health / 10000f;

        ChangeForm();

        if (health == 0f)
        {
            monsLiveState = MonsterLiveState.Dead;
            for (int i = 0; i < listBodyParts.Count; i++)
            {
                listBodyParts[i].active = false;
            }
            for(int i = 0; i < explores.Count; i++)
            {
                explores[i].transform.SetParent(null);
                explores[i].Play(true);
            }
            MapManager.instance.EndGameWin();
            gameObject.SetActive(false);
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
    Sleep,
    Live,
    Transform,
    Dead
}

public enum Form
{
    First,
    Second
}