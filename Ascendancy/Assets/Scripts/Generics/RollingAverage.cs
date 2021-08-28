using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingAverage
{
    private const float rollingAverageTimespan = 10;
    private List<RollingDatapoint> averageList = new List<RollingDatapoint>();

    private class RollingDatapoint
    {
        public float timestamp;
        public float amount;
    }

    public float Calculate()
    {
        if (averageList.Count == 0)
            return 0;

        List<RollingDatapoint> newList = new List<RollingDatapoint>(averageList.Count);
        float total = 0;
        foreach(RollingDatapoint rdp in averageList)
        {
            if (Time.time - rdp.timestamp < rollingAverageTimespan)
            {
                total += rdp.amount;
                newList.Add(rdp);
            }
        }
        averageList = newList;
        
        return total / rollingAverageTimespan;
        //averageQueue.Clear();
    }

    public void QueueDatapoint(float value)
    {
        RollingDatapoint dp = new RollingDatapoint() { amount = value, timestamp = Time.time };
        averageList.Add(dp);
    }
}

