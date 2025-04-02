using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance;
    public GameObject ecoIcon;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
