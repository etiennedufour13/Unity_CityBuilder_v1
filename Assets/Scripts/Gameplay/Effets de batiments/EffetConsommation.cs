using UnityEngine;

public class EffetConsommation : MonoBehaviour, IBuildingEffect
{
    public float consommationElectrique;
    public float productionDechets;


    //----------------------------------------------------------------------- Direct ---
    void Start()
    {

    }

    public void ApplyEffect()
    {
        //les impacts directs
        CityFactors.Instance.ModifyFactor(3, consommationElectrique);
        CityFactors.Instance.ModifyFactor(7, productionDechets);
    }
}
