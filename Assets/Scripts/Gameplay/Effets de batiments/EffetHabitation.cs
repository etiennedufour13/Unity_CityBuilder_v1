using UnityEngine;

public class EffetHabitation : MonoBehaviour, IBuildingEffect
{
    public float nbHabitants;

    //----------------------------------------------------------------------- Direct ---
    void Start()
    {

    }

    public void ApplyEffect()
    {
        //les impacts directs
        CityFactors.Instance.ModifyFactor(5, nbHabitants);
    }
}
