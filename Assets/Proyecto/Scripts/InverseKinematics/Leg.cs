using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Leg : MonoBehaviour
{
    Transform miTrans = default;
    [SerializeField] Transform transAMover = default;
    // La posicion y rotacion en la que queremos estar cerca
    [SerializeField] Transform transInicio = default;
    // permanece a esta distancia del inicio
    [SerializeField] float quieroPasoADistancia = 0.2f;
    // cuanto dura un paso en completarse
    [SerializeField] float duracionMovim = 0.2f;

    [SerializeField] float jumpStrngth = 0.9f;
    // Esta la pierna Moviendose
    public bool moviendo = false;

    private void Awake(){
        miTrans = transform;
    }
    private void Update(){
        if((transInicio.position - miTrans.position).sqrMagnitude > quieroPasoADistancia * quieroPasoADistancia && !moviendo)
            moverAInicio();
    }

    void moverAInicio(){
        moviendo = true;
        Vector3 overShootVector = transInicio.position-transAMover.position;
        overShootVector = Vector3.ClampMagnitude(overShootVector,quieroPasoADistancia*0.9f);

        transAMover.DOJump(transInicio.position + overShootVector, jumpStrngth, 1,duracionMovim,false).OnComplete(movTerminado);
    }

    void movTerminado(){
        moviendo=false;
    }
}
