using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EspirituDelMal : MonoBehaviour
{
    public TipoEspiritu tipo;

    public void VerificarCaptura()
    {
        if ((int)tipo == HeadRadialMaskMenu.instance.GetActualIndex())
        {
            SpiritsCollector.instance.AddSpirit((SpiritType)tipo, this.gameObject);
        }
    }
}

public enum TipoEspiritu
{
    jaguar = 0,
    tucan = 1,
    tapir = 2,
    mono = 3
}
