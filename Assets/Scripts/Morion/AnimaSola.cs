using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimaSola : MonoBehaviour
{
    public Animator animator;

    public void CambiarEstado(EstadosAnimacion estado)
    {
        animator.SetInteger("estado", (int)estado);
    }

    [ContextMenu("Cambiar Idle")]
    public void CambiarAIdle()
    {
        CambiarEstado(EstadosAnimacion.idle);
    }

    [ContextMenu("Cambiar Caminar")]
    public void CambiarACaminar()
    {
        CambiarEstado(EstadosAnimacion.caminando);
    }

    [ContextMenu("CAmbiar Cansado")]
    public void CambiarACansado()
    {
        CambiarEstado(EstadosAnimacion.cansado);
    }

    [ContextMenu("CAmbiar Bailando")]
    public void CambiarABailando()
    {
        CambiarEstado(EstadosAnimacion.bailando);
    }
}

public enum EstadosAnimacion
{
    idle = 0,
    caminando = 3,
    cansado = 1, 
    bailando = 2,
}