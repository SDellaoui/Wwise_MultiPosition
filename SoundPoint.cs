using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoint : MonoBehaviour {

    float initTime = 3.0f;
    Vector3 position;
    float coolDown;
    bool m_isActive;

    public bool m_isPlaying { get; set; }

    //public string eventName;
    //public string stopEventName;
    public bool isParentgameObject;
    public bool allowMultiPosition;

    public AK.Wwise.Event startEvent = new AK.Wwise.Event();
    public AK.Wwise.Event stopEvent = new AK.Wwise.Event();

    public float _debugDelay = 2f;

    // Use this for initialization
    void Start () {
        coolDown = initTime;
        position = transform.position;

        if(startEvent.IsValid() && stopEvent.IsValid())
        {
            /*
            if (allowMultiPosition)
                SoundPointManager.Instance.AddSoundPoint(gameObject);
            else
                startEvent.Post(gameObject);
            */
            StartCoroutine("InitSoundPoint");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!m_isActive)
            return;
        
        if (transform.hasChanged && allowMultiPosition)
            SoundPointManager.Instance.UpdateMultiPositionTransform(this);
    }

    void DebugUpdatePositionning()
    {
        coolDown -= Time.deltaTime;
        if (coolDown <= 0f)
        {
            float x = Random.Range(position.x - 5, position.x + 5);
            float y = 0.9f;
            float z = Random.Range(position.z - 5, position.z + 5);
            Vector3 pos = new Vector3(x, 0, 0);
            transform.position = pos;
            coolDown = initTime;
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            DestroySoundPoint();
        }
    }
    public void DestroySoundPoint()
    {
        SoundPointManager.Instance.RemoveSoundPoint(gameObject);
        if (!isParentgameObject)
            Destroy(gameObject);
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator InitSoundPoint()
    {
        yield return new WaitForSeconds(_debugDelay);
        if (allowMultiPosition)
            SoundPointManager.Instance.AddSoundPoint(gameObject);
        else
            startEvent.Post(gameObject);
        m_isActive = true;
        yield return null;
    }
}
