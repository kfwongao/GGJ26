using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapsDataSingleton : Singleton<MapsDataSingleton>
{
    public string MapName { get; set; }
    public string LocationAreaName { get; set; }

    public Vector3 MiniMapPlayerLocation { get; set; }
    public bool isGrounded { get; set; }

}
