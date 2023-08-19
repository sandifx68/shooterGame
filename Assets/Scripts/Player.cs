using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get {return _isDead;}
        protected set { _isDead = value; }
    }
    
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public float GetHealthPct()
    {
        return (float)currentHealth/maxHealth;
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    private bool firstSetup = true;

    public void SetupPlayer()
    {
        if(isLocalPlayer)
        {
            //Switch cameras
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();
    }
    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    //Command + client rpc inseamna ca toti clientii primesc comanda
    private void RpcSetupPlayerOnAllClients()
    {
        if(firstSetup==true)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for(int i=0;i< wasEnabled.Length; i++)
            {
                wasEnabled[i]=disableOnDeath[i].enabled;
            }
            firstSetup=false;
        }

        SetDefaults();
    }

    // void Update()
    // {
    //     if(!isLocalPlayer)
    //         return;
    //     if(Input.GetKeyDown(KeyCode.K))
    //     {
    //         RpcTakeDamage(9999);
    //     }
    // }

    [ClientRpc]
    public void RpcTakeDamage(int _amount)//EP 8, nush de ce face asta
    {
        if(isDead)
            return;
        currentHealth -=_amount;

        Debug.Log(transform.name + " now has "+currentHealth+" health.");

        if(currentHealth<=0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead  = true;
        

        //Disable components
        for(int i=0;i<disableOnDeath.Length;i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //Disable game objects
        for(int i=0;i<disableGameObjectsOnDeath.Length;i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        
        //Disable collider
        Collider _col = GetComponent<Collider>();
        if(_col!=null)
            _col.enabled = false;
        //Spawn a deeath effect
        GameObject _gfxIns = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy (_gfxIns,3f);

        //Switch cameras
        if(isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        Debug.Log(transform.name + " is dead :(!");

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);
        
        SetupPlayer();

        Debug.Log(transform.name + " respawned!");
    }

    public void SetDefaults()
    {
        isDead=false;
        currentHealth = maxHealth;
        //Enable components
        for(int i=0;i<disableOnDeath.Length;i++)
        {
            disableOnDeath[i].enabled=wasEnabled[i];
        }
        //Enable the gameobjects
        for(int i=0;i<disableGameObjectsOnDeath.Length;i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }
        //Enable collider
        Collider _col = GetComponent<Collider>();
        if(_col!=null)
            _col.enabled = true;//facem separat ca nu este un behaviour

        
    }
}
