using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string text = "12 3";
        string[] words = text.Split(' ');
        Debug.Log(words[0]);
        Debug.Log(words[1]);
        Test();
    }

    static void Test()
    {
        //int D = int.Parse(args[0]);
        //int M = int.Parse(args[1]);

        //string text = Console.ReadLine();
        string text = "8 19";
        string[] words = text.Split(' ');
        int M = int.Parse(words[0]);
        int D = int.Parse(words[1]);

        if ((D / 10 + D % 10 + M) % 2 != 0)
        {
            Console.WriteLine("Not beautiful");
        }

        int days = D;
        for (int i=1; i<M; i++)
        {
            if (i == 1 || i == 3 || i == 5 || i == 7 || i == 8 || i == 9 || i == 11)
            {
                days += 31;
            }
            else if (i == 4 || i == 6 || i == 10 || i == 12)
            {
                days += 30;
            }
            else
            {
                days += 28;
            }
        }

        if (days % 7 == 0 || days % 7 == 6)
        {
            Console.WriteLine("Beautiful");
        }
        else
        {
            Console.WriteLine("Not beautiful");
        }
    }
}
