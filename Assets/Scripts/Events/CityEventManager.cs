using UnityEngine;
using System.Collections.Generic;

public class CityEventManager : MonoBehaviour
{
    [System.Serializable]
    public class CityEvent
    {
        public string eventName;
        public int factorID;
        public float factorValue;
        public float cooldownDuration = 3f;

        [HideInInspector] public float currentCooldown = 0f;

        public UnityEngine.Events.UnityEvent onTrigger;
    }

    public List<CityEvent> cityEvents = new List<CityEvent>();
    public CityFactors cityFactors;



    void Start()
    {
        //pour que EvaluateEvents se lance auto chaque jours
        TimeManager.Instance.OnDayPassed.AddListener(EvaluateEvents);
        TimeManager.Instance.OnDayPassed.AddListener(ReduceCooldowns);
    }

    void ReduceCooldowns()
    {
        foreach (var evt in cityEvents)
        {
            if (evt.currentCooldown > 0f)
                evt.currentCooldown -= 1f;
        }
    }

    void EvaluateEvents()
    {
        CityEvent selected = null;
        float maxDelta = float.MinValue;

        foreach (var evt in cityEvents)
        {
            float factor = cityFactors.cityFactors[evt.factorID].currentValeur;
            if (factor <= evt.factorValue) continue;
            if (evt.currentCooldown > 0f) continue;

            float delta = factor - evt.factorValue;
            if (delta > maxDelta)
            {
                maxDelta = delta;
                selected = evt;
            }
        }

        if (selected != null)
        {
            selected.onTrigger?.Invoke();
            selected.currentCooldown = selected.cooldownDuration;
            Debug.Log("Évènement déclenché : " + selected.eventName);
        }
        else
        {
            Debug.Log("pad d'events");
        }
    }
}
