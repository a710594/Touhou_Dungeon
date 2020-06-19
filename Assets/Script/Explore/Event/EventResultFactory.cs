using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResultFactory : MonoBehaviour
{
    public static EventResult GetNewEventResult(EventData.Result result)
    {
        EventResult eventResult = new EventResult();

        if (result.Type == EventData.TypeEnum.Recover)
        {
            eventResult = new RecoverEvent(result);
        }
        else if (result.Type == EventData.TypeEnum.Teleport)
        {
            eventResult = new TeleportEvent(result);
        }

        else if (result.Type == EventData.TypeEnum.Money)
        {
            eventResult = new MoneyEvent(result);
        }
        else if (result.Type == EventData.TypeEnum.Battle)
        {
            eventResult = new BattleEvent(result);
        }

        return eventResult;
    }
}
