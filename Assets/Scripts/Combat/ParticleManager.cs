using UnityEngine;
using System.Collections;

/// <summary>
/// manages all the particles. Eliminated for now until new particles can be found and implemented
/// </summary>
public class ParticleManager : Singleton<ParticleManager>
{
	//uncomment this block and drag the new prefabs into the missing spots to re-implement this
	/*
    //[SerializeField]
    public GameObject spellCastPrefab;
    private GameObject spellCastObject;

    public GameObject spellHitPrefab;
    private GameObject spellHitObject;

    public GameObject physicalHitPrefab;
    private GameObject physicalHitObject;

    public GameObject healHitPrefab;
    private GameObject healHitObject;

    public GameObject buffHitPrefab;
    private GameObject buffHitObject;

    public GameObject debuffHitPrefab;
    private GameObject debuffHitObject;

    public GameObject physicalParticlePrefab;
    public GameObject magicalParticlePrefab;
    public GameObject neutralParticlePrefab;
    private GameObject castParticle;

    //private bool destroySpellCast;
    //private float timeToDestroy = 0.0f;

    float speed = 2.5f;
    float particleTravelTime = 0.5f;
    private float startTime;
    private float journeyLength;
    bool isMovingParticle = false;
    Vector3 startPosition = new Vector3(0,0,0); 
    Vector3 endPosition = new Vector3(0, 0, 0);

    protected ParticleManager()
    { // guarantee this will be always a singleton only - can't use the constructor!

    }

    //PhotonView photonView;

    void Awake()
    {
        //photonView = PhotonView.Get(this);
    }

    void Update()
    {
        if( isMovingParticle)
        {
            //Debug.Log("castParticle position: " + castParticle.transform.position);
            //float distCovered = (Time.time - startTime) * speed;
            //float fracJourney = distCovered / journeyLength;
            float distCovered = (Time.time - startTime) / particleTravelTime; //always end within the travel time
            castParticle.transform.position = Vector3.Lerp(startPosition, endPosition, distCovered);
        }
    }

    //ParticleManager.Instance.SetSpellCast();

    public void SetSpellCast(GameObject go, float positionY = 2.0f)
    {
        spellCastObject = Instantiate(spellCastPrefab) as GameObject;
        Vector3 castPosition = go.transform.position;
        castPosition.y += positionY;
        spellCastObject.transform.position = castPosition;
        //spellCastObject.transform.position = new Vector3(2,2,2);
        Destroy(spellCastObject, 1.5f);
        //arrowObject.SetActive(true);
    }

    //[PunRPC]
    public void SetSpellHit(int targetId, int particleType = 0, float positionY = 1.0f)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("SetSpellHit", PhotonTargets.Others, new object[] { targetId, particleType, positionY});
        //}
        GameObject go = PlayerManager.Instance.GetPlayerUnitObject(targetId);
        if( particleType == 1) //physical
        {
            physicalHitObject = Instantiate(physicalHitPrefab) as GameObject;
            Vector3 castPosition = go.transform.position;
            castPosition.y += positionY;
            physicalHitObject.transform.position = castPosition;
            Destroy(physicalHitObject, 0.5f);
        }
        else if( particleType == 2)//heal
        {
            healHitObject = Instantiate(healHitPrefab) as GameObject;
            Vector3 castPosition = go.transform.position;
            castPosition.y += positionY;
            healHitObject.transform.position = castPosition;
            Destroy(healHitObject, 3.0f);
        }
        else if (particleType == 3)//buff
        {
            buffHitObject = Instantiate(buffHitPrefab) as GameObject;
            Vector3 castPosition = go.transform.position;
            castPosition.y += positionY;
            buffHitObject.transform.position = castPosition;
            Destroy(buffHitObject, 3.0f);
        }
        else if (particleType == 4)//debuff
        {
            debuffHitObject = Instantiate(debuffHitPrefab) as GameObject;
            Vector3 castPosition = go.transform.position;
            castPosition.y += positionY;
            debuffHitObject.transform.position = castPosition;
            Destroy(debuffHitObject, 3.0f);
        }
        else
        {
            spellHitObject = Instantiate(spellHitPrefab) as GameObject;
            Vector3 castPosition = go.transform.position;
            castPosition.y += positionY;
            spellHitObject.transform.position = castPosition;
            Destroy(spellHitObject, 3.0f);
        }

    //    public GameObject spellHitPrefab;
    //private GameObject spellHitObject;
    //public GameObject physicalHitPrefab;
    //private GameObject physicalHitObject;
    //public GameObject healHitPrefab;
    //private GameObject healHitObject;
    //public GameObject buffHitPrefab;
    //private GameObject buffHitObject;
    //public GameObject debuffHitPrefab;
    //private GameObject debuffHitObject;
    }
    
    //Other gets the call from master to show a partile
    //[PunRPC]
    public void ReceiveCastParticleRPC(int actorId, int tileX, int tileY, int snIndex)
    {
        GameObject cc = GameObject.Find("CombatController");
        if( cc != null)
        {
            //Debug.Log("found the object");
            Tile targetTile = cc.GetComponent<CombatController>().board.GetTile(tileX, tileY);
            if( targetTile != null)
            {
                //Debug.Log("found the object");
                LaunchCastParticle(PlayerManager.Instance.GetPlayerUnit(actorId), targetTile, SpellManager.Instance.GetSpellNameByIndex(snIndex));
            }
        }
    }

    public void LaunchCastParticle(PlayerUnit actor, Tile targetTile, SpellName sn)
    {
        //if (!PhotonNetwork.offlineMode && PhotonNetwork.isMasterClient)
        //{
        //    photonView.RPC("ReceiveCastParticleRPC", PhotonTargets.Others, new object[] { actor.TurnOrder, targetTile.pos.x, targetTile.pos.y, sn.Index });
        //}

        if ( actor.TileX == targetTile.pos.x && actor.TileY == targetTile.pos.y)
        {
            return;
        }

        GameObject goActor = PlayerManager.Instance.GetPlayerUnitObject(actor.TurnOrder);
        GameObject goTile = targetTile.gameObject;
        Vector3 offset = new Vector3(0.0f, 0.5f, 0.0f);
        int type = sn.PMType;

        if (type == NameAll.SPELL_TYPE_PHYSICAL)
        {
            //Debug.Log("lanching particle");
            castParticle = Instantiate(physicalParticlePrefab, goActor.transform.position + offset, goActor.transform.rotation) as GameObject; //Instantiate (theObject, transform.position + transform.forward * 1.0, transform.rotation);
        }
        else
        {
            return;
        }
        //else if (type == 1)
        //{
        //    castParticle = Instantiate(magicalParticlePrefab, goActor.transform.position + (offset * 2), goActor.transform.rotation) as GameObject; //Instantiate (theObject, transform.position + transform.forward * 1.0, transform.rotation);
        //}
        //else
        //{
        //    castParticle = Instantiate(neutralParticlePrefab, goActor.transform.position + offset, goActor.transform.rotation) as GameObject; //Instantiate (theObject, transform.position + transform.forward * 1.0, transform.rotation);
        //}
        startPosition = goActor.transform.position + offset;
        endPosition = goTile.transform.position + offset;
        journeyLength = Vector3.Distance(startPosition, endPosition);
        startTime = Time.time;
        isMovingParticle = true;

        //Debug.Log("positions are " + startPosition + "asdf " + endPosition);

        StartCoroutine(EndCastParticle(castParticle, particleTravelTime)); //Destroy(castParticle, 1.5f);
    }

  
    private IEnumerator EndCastParticle(GameObject go, float travelTime)
    {
        yield return new WaitForSeconds(travelTime);
        isMovingParticle = false;
        Destroy(go);
    }

    //private IEnumerator DelayedFollowUnit(float seconds, int unitId = 1919)
    //{
    //    yield return new WaitForSeconds(seconds);
    //    //Debug.Log("in delay");
    //    FollowUnit(unitId);
    //}

	*/
}
