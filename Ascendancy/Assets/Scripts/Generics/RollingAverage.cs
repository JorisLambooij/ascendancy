using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingAverage
{
    private const float rollingAverageTimespan = 10;
    private List<RollingDatapoint> averageList = new List<RollingDatapoint>();
    private bool useDelta;

    public float average;
    private class RollingDatapoint
    {
        public float timestamp;
        public float amount;
    }

    public RollingAverage(bool useDelta)
    {
        this.useDelta = useDelta;
    }

    public float Calculate()
    {
        if (averageList.Count == 0)
            return 0;

        List<RollingDatapoint> newList = new List<RollingDatapoint>(averageList.Count);
        float total = 0;
        for (int i = 0; i < averageList.Count; i++)
        {
            RollingDatapoint rdp = averageList[i];
            if (Time.time - rdp.timestamp < rollingAverageTimespan)
            {
                float delta = rdp.amount - (i > 0 ? averageList[i - 1].amount : 0);
                total += delta;
                newList.Add(rdp);
            }
        }
        averageList = newList;

        average = total / rollingAverageTimespan;
        return average;
        //averageQueue.Clear();
    }

    public void QueueDatapoint(float value)
    {
        RollingDatapoint dp = new RollingDatapoint() { amount = value, timestamp = Time.time };
        averageList.Add(dp);
    }
}

