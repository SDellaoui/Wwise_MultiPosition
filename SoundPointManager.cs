using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPMultiPosEvent
{
    public List<SoundPoint> list = new List<SoundPoint>();
    public bool eventIsPlaying = false;
    public SoundPoint parentSoundPoint = null;

    public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
    {
        eventIsPlaying = false;
    }
}

public class SoundPointManager : MonoBehaviour {
    private static SoundPointManager _instance;
    public static SoundPointManager Instance { get { return _instance; } }

    public SortedList<string,List<SoundPoint>> soundpointList;

    static public Dictionary<string, SPMultiPosEvent> multiPosEventTree = new Dictionary<string, SPMultiPosEvent>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Use this for initialization
    void Start(){
        soundpointList = new SortedList<string, List<SoundPoint>>();
    }
	// Update is called once per frame
	void Update () {
		
	}
    public void AddSoundPoint(GameObject soundpointGO)
    {
        
        SoundPoint sp = soundpointGO.GetComponent<SoundPoint>();

        SPMultiPosEvent eventPosList;
        if (multiPosEventTree.TryGetValue(sp.eventName, out eventPosList))
        {
            if (!eventPosList.list.Contains(sp))
            {
                eventPosList.list.Add(sp);
                if (eventPosList.parentSoundPoint == null && sp.isParentgameObject)
                    eventPosList.parentSoundPoint = sp;
            }
        }
        else
        {
            eventPosList = new SPMultiPosEvent();
            eventPosList.list.Add(sp);
            if (eventPosList.parentSoundPoint == null && sp.isParentgameObject)
                eventPosList.parentSoundPoint = sp;
            else if (eventPosList.parentSoundPoint != null && sp.isParentgameObject)
                sp.isParentgameObject = false;
            multiPosEventTree.Add(sp.eventName, eventPosList);
        }
        Debug.Log("for Soundpoint : " + soundpointGO.name+" parent soundpoint : "+eventPosList.parentSoundPoint);
        if (eventPosList.parentSoundPoint != null)
            SetMultiPosition(sp);

    }
    public void RemoveSoundPoint(GameObject soundpointGO)
    {
        SoundPoint sp = soundpointGO.GetComponent<SoundPoint>();
        SPMultiPosEvent eventPosList;
        if (!multiPosEventTree.TryGetValue(sp.eventName, out eventPosList))
            return;
        eventPosList.list.Remove(sp);

        UpdateMultiPosition(sp);

        }
    public void UpdateMultiPositionTransform(SoundPoint soundpoint)
    {
        UpdateMultiPosition(soundpoint);
    }
    //Update multiposition for parent destroyed
    void UpdateMultiPosition(SoundPoint soundpoint)
    {
        SPMultiPosEvent eventPosList = multiPosEventTree[soundpoint.eventName];

        SoundPoint parentSoundPoint = eventPosList.parentSoundPoint;

        if (eventPosList.list.Count == 0)
        {
            DestroySoundPoint(parentSoundPoint);
            return;
        }
        
        AkPositionArray positionArray = BuildMultiplePositions(eventPosList.list);
        AkSoundEngine.SetMultiplePositions(parentSoundPoint.gameObject, positionArray, (ushort)positionArray.Count, AkMultiPositionType.MultiPositionType_MultiSources);

    }

    void SetMultiPosition(SoundPoint soundpoint)
    {
        SPMultiPosEvent multiPosEvent = multiPosEventTree[soundpoint.eventName];
        SoundPoint soundPointParent = multiPosEvent.parentSoundPoint;
        AkGameObj[] gameObj = soundpoint.gameObject.GetComponents<AkGameObj>();
        for (int i = 0; i < gameObj.Length; i++)
            gameObj[i].enabled = false;

        AkPositionArray positionArray = BuildMultiplePositions(multiPosEvent.list);
        AkSoundEngine.SetMultiplePositions(soundPointParent.gameObject, positionArray, (ushort)positionArray.Count, AkMultiPositionType.MultiPositionType_MultiSources);
        if (!multiPosEvent.eventIsPlaying)
        {
            AkSoundEngine.PostEvent(soundpoint.eventName, soundPointParent.gameObject, (uint)AkCallbackType.AK_EndOfEvent, AKCallBackFinishedPlaying, null, 0, null, AkSoundEngine.AK_INVALID_PLAYING_ID);
            multiPosEvent.eventIsPlaying = true;
        }
    }

    void DestroySoundPoint(SoundPoint sp)
    {
        AkSoundEngine.PostEvent(sp.stopEventName, sp.gameObject);
        Destroy(sp.gameObject);
        multiPosEventTree.Remove(sp.eventName);
    }

    public AkPositionArray BuildMultiplePositions(List<SoundPoint> multiPositionList)
    {
        AkPositionArray positionArray = new AkPositionArray((uint)multiPositionList.Count);
        for (int i = 0; i < multiPositionList.Count; i++)
            positionArray.Add(multiPositionList[i].gameObject.transform.position, multiPositionList[i].gameObject.transform.forward, multiPositionList[i].gameObject.transform.up);
        return positionArray;
    }
    public void AKCallBackFinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
    {
        Debug.Log(in_cookie);
    }
}