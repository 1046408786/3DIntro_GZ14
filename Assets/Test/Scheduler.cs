using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    public static Scheduler instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameController").GetComponent<Scheduler>();
            }
            return _instance;
        }
    }
    private static Scheduler _instance;
    private Dictionary<int, Stopwatch> stopwatches = new Dictionary<int, Stopwatch>();
    private int totalWatchCount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        foreach (Stopwatch sw in stopwatches.Values)
        {
            sw.Update();
        }

        for (int i = 0; i < totalWatchCount; i++)
        {
            if (stopwatches.ContainsKey(i))
            {
                if (stopwatches[i].isTimesUp && stopwatches[i].onTimesUp != null)
                {
                    stopwatches[i].onTimesUp();
                    stopwatches[i].isTimesUp = false;
                }

                if (stopwatches[i].canRemove)
                {
                    stopwatches.Remove(i);
                }
            }
        }
    }

    public int Schedule(float interval, bool repeat, System.Action callback)
    {
        //Debug.Log("callback :" + callback);
        int id = totalWatchCount;
        Stopwatch newWatch = new Stopwatch(interval, repeat, callback);
        stopwatches.Add(id,newWatch);
        newWatch.Start();

        totalWatchCount++;
        return id;
    }

    public void Deschedule(int handler)
    {
        if (stopwatches.ContainsKey(handler))
        {
            stopwatches[handler].Stop();
            stopwatches[handler].canRemove = true;
            //stopwatches.Remove(handler);
        }
    }
}
