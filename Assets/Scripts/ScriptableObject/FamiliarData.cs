using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Familiar.asset", menuName = "FamiliarObject")]
public class FamiliarData : ScriptableObject
{

    public string familiarType;

    public float speed;

    public float fireDelay;

    public GameObject bulletPrefeb;

}
