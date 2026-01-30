using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipAreaTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapsDataSingleton.Instance.LocationAreaName = "Space Ship";
            GameSceneManager.Instance.ShowLocationChangeUIAnim(MapsDataSingleton.Instance.LocationAreaName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapsDataSingleton.Instance.LocationAreaName = "Moon Surface";
            GameSceneManager.Instance.ShowLocationChangeUIAnim(MapsDataSingleton.Instance.LocationAreaName);
        }
    }
}
