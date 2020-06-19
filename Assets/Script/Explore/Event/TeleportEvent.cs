using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEvent : EventResult
{
    public TeleportEvent(EventData.Result result)
    {
        _result = result;
    }

    public override void Execute()
    {
        ExploreController.Instance.TeleportPlayer();
    }
}
