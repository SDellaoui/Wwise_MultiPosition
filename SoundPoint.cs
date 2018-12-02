using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoint : MonoBehaviour {

    float initTime = 3.0f;
    Vector3 position;
    float coolDown;

    public bool m_isPlaying { get; set; }

    public string eventName;
    public string stopEventName;
    public bool isParentgameObject;
    public bool allowMultiPosition;
	// Use this for initialization
	void Start () {
        coolDown = initTime;
        position = transform.position;
        if(eventName != "" && stopEventName != "" && allowMultiPosition)
            SoundPointManager.Instance.AddSoundPoint(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
        coolDown -= Time.deltaTime;
        if (coolDown <= 0f)
        {
            float x = Random.Range(position.x - 5, position.x + 5);
            float y = 0.9f;
            float z = Random.Range(position.z - 5, position.z + 5);
            Vector3 pos = new Vector3(x, y, z);
            transform.position = pos;
            coolDown = initTime;
        }
        if (transform.hasChanged && allowMultiPosition)
            SoundPointManager.Instance.UpdateMultiPositionTransform(this);
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
}
