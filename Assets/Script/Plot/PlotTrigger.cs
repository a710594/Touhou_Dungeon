using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotTrigger : MonoBehaviour
{
    public string Plot;

    private ExploreCharacter character;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            character = collision.transform.parent.GetComponent<ExploreCharacter>();
            character.MoveCallback += OnCharacterMoveEnd;
        }
    }
    private void OnCharacterMoveEnd()
    {
        Type t = Type.GetType(Plot);

        Plot plot = (Plot)Activator.CreateInstance(t);
        plot.Start();
        character.MoveCallback -= OnCharacterMoveEnd;
    }
}
