using System;
using System.Collections;
using UnityEngine;
public class HistoryQueue<T> {
    T[] queue;
    public List<T> getQueue {get{
        List<T> r = new List<T>();
        foreach(T t in queue){
            if(t!=null)
                r.Add(t);
        }
        return r;
    }}


    public HistoryQueue(int size){
        queue = new T[size];
    }

    public void Clear(){
        queue = new T[queue.Length];
    }

    public void Log(T obj){
        for(int i = 0;i < size-1; ++i){
            queue[i+1] = queue[i];
        }
        queue[0]=obj;
    }
}