using System;
using System.Collections.Generic;
using UnityEngine;
public class HistoryQueue<T> {
    Node[] queue;
    public List<T> getQueue {get{
        List<T> r = new List<T>();
        foreach(Node t in queue){
            if(t!=null)
                r.Add(t.content);
        }
        return r;
    }}
    public int size {get{return queue.Length;}}

    public HistoryQueue(int size){
        queue = new Node[size];
        for(int i = 0; i < size; ++i){
            queue[i]=null;
        }
    }

    public void Clear(){
        queue = new Node[queue.Length];
        for(int i = 0; i < size; ++i){
            queue[i]=null;
        }
    }

    public void UpdateTime(float deltaTime){
        List<Node> newQueue = new List<Node>();
        for(int i = 0; i < size; ++i){
            if(queue[i] == null)
                break;
            queue[i].time -= deltaTime;
            if(queue[i].time < 0){
                queue[i] = null;
            }
            else{
                newQueue.Add(queue[i]);
            }
        }
        for(int i = 0; i < size; ++i){
            queue[i] = i < newQueue.Count ? newQueue[i] : null;
        }
    }

    public void Log(T obj,float lifeTime){
        for(int i = 0;i < size-1; ++i){
            queue[i+1] = queue[i];
        }
        queue[0]=new Node(obj, lifeTime);
    }

    class Node {
        public T content;
        public float time;

        public Node(T content, float time){
            this.content = content;
            this.time = time;
        }
    }
}