using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchBat : MonoBehaviour
{
    public GameObject[] bats;
    private int batIndex;

    public void SwitchBat()
    {
        batIndex++;

        bats[0].SetActive(false);
        bats[1].SetActive(false);
        bats[2].SetActive(false);
        bats[3].SetActive(false);
        bats[4].SetActive(false);
        bats[5].SetActive(false);

        bats[batIndex].SetActive(true);
    }
}
