using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Eye_Left : BaseEyes
{
    public float tempFarDist;
    public float tempNearDist;

    protected override void Start()
    {
        base.Start();

        stateName = "BlinkEye_Left";
    }

    protected override int GetVer(Vector3 dir)
    {
        float z = dir.z;

        switch (z)
        {
            case float a when a <= -tempFarDist:
                return 0;

            case float a when a > -tempFarDist && a < -tempNearDist:
                return 1;

            case float a when a > -tempNearDist && a < tempNearDist:
                return 2;

            case float a when a > tempNearDist && a < tempFarDist:
                return 3;

            case float a when a >= tempFarDist:
                return 4;

            default:
                return 2;
        }
    }

}
