using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public static Action<float, float> ItemSpeedBoost; // (속도, 지속시간)

    public static Func<float, bool> Run;

}
