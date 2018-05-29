using UnityEngine;
using System.Collections;
using System;

public class ActionQueue : MonoBehaviour
{
    Queue AQueue = new Queue();

    static ActionQueue _instance;
    public static ActionQueue Instance
    {
        get
        {
            return _instance;
        }
    }


    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lock(AQueue)
        {
            if (AQueue.Count > 0)
            {
                var act = AQueue.Dequeue() as Action;
                if (act != null)
                {
                    try
                    {
                        act.Invoke();
                    }
                    catch (Exception ex)
                    {
                        //LogWriter.Instance.ActionLogger.Warn( ex.Message);
                        print("Cao");
                    }
                }

            }
        }

    }

    public void QueueIn(Action action)
    {
        lock(AQueue)
        {
            AQueue.Enqueue(action);
        }
    }

   
}
