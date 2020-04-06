using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventResultFactory : MonoBehaviour
{
    public static EventResult GetNewEventResult(EventOptionData.Result result)
    {
        EventResult eventResult = new EventResult();
        if (result.Type == EventOptionData.TypeEnum.RecoverHP)
        {
            eventResult = new RecoverHPEvent(result);
        }
        else if (result.Type == EventOptionData.TypeEnum.RecoverMP)
        {
            eventResult = new RecoverMPEvent(result);
        }
        else if (result.Type == EventOptionData.TypeEnum.RecoverBoth)
        {
            eventResult = new RecoverBothEvent(result);
        }
        else if (result.Type == EventOptionData.TypeEnum.Teleport)
        {
            eventResult = new TeleportEvent(result);
        }

        else if (result.Type == EventOptionData.TypeEnum.Money)
        {
            eventResult = new MoneyEvent(result);
        }
        else if (result.Type == EventOptionData.TypeEnum.Battle)
        {
            eventResult = new BattleEvent(result);
        }

        return eventResult;
    }
}
