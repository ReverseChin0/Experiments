using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Spider : MonoBehaviour
{
    [SerializeField] Transform spiderBody = default;
    [SerializeField] float turnAcceleration=5.0f,velocidadGiro = 2.0f, maxAngATarget = 90.0f;
    [SerializeField] float velocidadMov = 5.0f, moveAcceleration = 5.0f, distMinTarget=1.0f, distMaxTarget = 10.0f;
    [SerializeField] Transform miTarget = default;
    
    Vector3 direccionUno=default, velocidadActual = default;
    float velocidadAngActual= 0.0f;

    void Start()
    {
        if(spiderBody==null)
            spiderBody = this.transform;
        
    }
    private void LateUpdate() 
    {
        /*
        spiderInputs();
        spiderBody.Translate(direccionUno * velocidad * Time.deltaTime);
        */
        RootMotionUpdate();
    }

    void spiderInputs()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        bool rotIzq = Input.GetKey(KeyCode.Q);
        bool rotDer = Input.GetKey(KeyCode.E);
        int rotDirection =  rotIzq ? 1 : rotDer ? -1 : 0;
       
        direccionUno = vert * spiderBody.right + hori * -spiderBody.forward;

        direccionUno = Vector3.ClampMagnitude(direccionUno,1);
    }

   void RootMotionUpdate()
    {
        // Obten la direccion hacia nuestro objetivo
        Vector3 towardTarget = miTarget.position - spiderBody.position;
        
        towardTarget.y = spiderBody.position.y;

        // Obtener el angulo de nuestro forward al target
        // obtenemos el angulo con signo hacia donde girar (angulo mas corto entre +180 y -180)
        float angATarget = Vector3.SignedAngle(spiderBody.right, towardTarget, transform.up);

        float VelocidadAngularTarget = 0;

        // Si estamos dentro del angulo maximo(osea viendo hacia el objetivo)
        // deja la velocidad angular es el 0
        if (Mathf.Abs(angATarget) > maxAngATarget)
        {
            //Angulos en Unity van con las manecillas, por lo que los positivos van a la derecha
            if (angATarget > 0)
            {
                VelocidadAngularTarget = velocidadGiro;
            }
            // Invertir si es a la izquierda
            else
            {
                VelocidadAngularTarget = -velocidadGiro;
            }
        }

        // Funcion de suavizado para cambiar la velocidad chido
        velocidadAngActual = Mathf.Lerp(
            velocidadAngActual,
            VelocidadAngularTarget,
            1 - Mathf.Exp(-turnAcceleration * Time.deltaTime) //no dependiente de frame rate
        );

        // Rotar el transform en y en world Space
        spiderBody.Rotate(0, Time.deltaTime * velocidadAngActual, 0, Space.World);

        Vector3 velocidadTarget = Vector3.zero;

        // No moverse si no estamos en direccion al target
        if (Mathf.Abs(angATarget) < 90)
        {
            float distToTarget = Vector3.Distance(transform.position, miTarget.position);

            // Si estamos muy cerca, aproximemonos
            if (distToTarget > distMaxTarget)
            {
                velocidadTarget = velocidadMov * towardTarget.normalized;
            }
            // sino alejemonos
            else if (distToTarget < distMinTarget)
            {
                velocidadTarget = velocidadMov * -towardTarget.normalized;
            }
        }

        velocidadActual = Vector3.Lerp(
        velocidadActual,
        velocidadTarget,
        1 - Mathf.Exp(-moveAcceleration * Time.deltaTime)
        );

        // Apply the velocity
        transform.position += velocidadActual * Time.deltaTime;
    }


    /*void checkNewIks(Vector3 newDir)
    {
        int length = puntas.Length;
        for (int i = 0; i < length; i++)
        {
           if((puntas[i].position-targets[i].position).sqrMagnitude > longitudes[i] * longitudes[i])//si la punta esta muy lejos de su longitud maxima
            {
                Vector3 newPos = initialPos[i] + spiderBody.position;
                newPos += newDir * longitudMovimiento;
                targets[i].DOLocalMove(newPos,0.2f).SetEase(suavizadoIks);
            } 
        }
        
    }*/
}
