using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    public enum TypeEnum 
    {
        Recover,
        Telepoet,
    }

    public class Result 
    {
        public string Comment;
        public bool isDoNothing;

        public virtual void Execute() { }
    }

    public string Tile;
    public string Comment;
    public List<string> Options = new List<string>();
    public TypeEnum Type; //存檔之後會忘記自己的 child class, 所以要記錄

    public virtual Result GetResult(int option) 
    {
        return null;
    }
}
