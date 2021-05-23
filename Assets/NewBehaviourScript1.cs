using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Test()
    {
        //string text = Console.ReadLine();
        string text = "(0+0)+0";

        if (text.Length == 1 && text[0] == '0')
        {
            Console.WriteLine("Possible");
            return;
        }

        Stack<char> stack1 = new Stack<char>();
        Stack<char> stack2 = new Stack<char>(); 
        for (int i=0; i<text.Length; i++)
        {
            if (text[i] == '(')
            {
                stack1.Push('(');
            }
            else if (text[i] == ')')
            {
                if (stack1.Count == 0)
                {
                    Console.WriteLine("Impossible");
                    return;
                }
                else
                {
                    stack1.Pop();
                }
            }
        }

        if (stack1.Count > 0)
        {
            Console.WriteLine("Impossible");
        }
        else
        {
            Console.WriteLine("Possible");
        }
    }
}
