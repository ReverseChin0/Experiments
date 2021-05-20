using System.Collections;
using System.Collections.Generic;
    #if UNITY_EDITOR
    using UnityEditor;//contiene handles
    #endif
using UnityEngine;


public class InverseKinematics : MonoBehaviour
{
        /// LongitudDeCadenadeHuesos
        public int longitudCadena = 2;

        /// Objetivo hacia el que se debe doblar
        public Transform Objetivo;
        public Transform Pole;

        /// iteraciones por Update
        [Header("Parameters de Solucion")]
        public int iteraciones = 10;

        
        public float Delta = 0.001f;/// Distancia a la que para

        /// Fuerza para regresar a posicion Inicial
        [Range(0, 1)]
        public float fuerzaSnapInicio = 1f;


        protected float[] LongitudHuesos; //Objetivo a Origen
        protected float LongitudCompleta;
        protected Transform[] Huesos;
        protected Vector3[] Posiciones;
        protected Vector3[] StartDirectionSucc;
        protected Quaternion[] RotInicioHueso;
        protected Quaternion StartRotationObjetivo;
        protected Transform Raiz;


        void Awake()
        {
            Inicializar();
        }

        void Inicializar()
        {
            //Inicializarial array
            Huesos = new Transform[longitudCadena + 1]; //porque si son 3 huesos serian 4 puntos
            Posiciones = new Vector3[longitudCadena + 1];
            LongitudHuesos = new float[longitudCadena];
            StartDirectionSucc = new Vector3[longitudCadena + 1];
            RotInicioHueso = new Quaternion[longitudCadena + 1];

            //Encuentra la Raiz
            Raiz = transform;
            for (var i = 0; i <= longitudCadena; i++)
            {
                if (Raiz == null)
                    throw new UnityException("El valor de cadena es mayor que la cadena de ancestros!");
                Raiz = Raiz.parent;
            }

            //Inicializar Objetivo
            if (Objetivo == null)
            {
                Objetivo = new GameObject(gameObject.name + " Objetivo").transform;
                AsignarPosEspacioRaiz(Objetivo, ObtenerPosEspacioRaiz(transform));
            }
            StartRotationObjetivo = ObtenerRotEspacioRaiz(Objetivo);


            //Inicializar data
            var actual = transform;
            LongitudCompleta = 0;
            for (var i = Huesos.Length - 1; i >= 0; i--)
            {
                Huesos[i] = actual;
                RotInicioHueso[i] = ObtenerRotEspacioRaiz(actual);

                if (i == Huesos.Length - 1)
                {
                    //hoja
                    StartDirectionSucc[i] = ObtenerPosEspacioRaiz(Objetivo) - ObtenerPosEspacioRaiz(actual);
                }
                else
                {
                    //hueso en medio
                    StartDirectionSucc[i] = ObtenerPosEspacioRaiz(Huesos[i + 1]) - ObtenerPosEspacioRaiz(actual);
                    LongitudHuesos[i] = StartDirectionSucc[i].magnitude;
                    LongitudCompleta += LongitudHuesos[i];
                }

                actual = actual.parent;
            }

        }

        // Update is called once per frame
        void LateUpdate()
        {
            ResolveIK();
        }

        private void ResolveIK()
        {
            if (Objetivo == null)
                return;

            if (LongitudHuesos.Length != longitudCadena)
                Inicializar();

            //Fabric

            //  Raiz
            //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
            //   x--------------------x--------------------x---...

            //get posicion
            for (int i = 0; i < Huesos.Length; i++)
                Posiciones[i] = ObtenerPosEspacioRaiz(Huesos[i]);

            var Objetivoposicion = ObtenerPosEspacioRaiz(Objetivo);
            var ObjetivoRotation = ObtenerRotEspacioRaiz(Objetivo);

            //1ro Es posible alcanzar el objetivo?
            if ((Objetivoposicion - ObtenerPosEspacioRaiz(Huesos[0])).sqrMagnitude >= LongitudCompleta * LongitudCompleta) //no
            {
                //solo estirate
                Vector3 direction = (Objetivoposicion - Posiciones[0]).normalized;
                //setea todo despues de raiz 
                for (int i = 1; i < Posiciones.Length; i++)
                    Posiciones[i] = Posiciones[i - 1] + direction * LongitudHuesos[i - 1];
            }
            else //si
            {
                for (int i = 0; i < Posiciones.Length - 1; i++)
                    Posiciones[i + 1] = Vector3.Lerp(Posiciones[i + 1], Posiciones[i] + StartDirectionSucc[i], fuerzaSnapInicio );

                for (int iteration = 0; iteration < iteraciones; iteration++)
                {
                    //atras
                    for (int i = Posiciones.Length - 1; i > 0; i--)
                    {
                        if (i == Posiciones.Length - 1)
                            Posiciones[i] = Objetivoposicion; //setealo a Objetivo
                        else
                            Posiciones[i] = Posiciones[i + 1] + (Posiciones[i] - Posiciones[i + 1]).normalized * LongitudHuesos[i]; //set in line on distance
                    }

                    //adelante
                    for (int i = 1; i < Posiciones.Length; i++)
                        Posiciones[i] = Posiciones[i - 1] + (Posiciones[i] - Posiciones[i - 1]).normalized * LongitudHuesos[i - 1];

                    //close enough?
                    if ((Posiciones[Posiciones.Length - 1] - Objetivoposicion).sqrMagnitude < Delta * Delta)
                        break;
                }
            }

            //move towards pole
            if (Pole != null)
            {
                var poleposicion = ObtenerPosEspacioRaiz(Pole);
                for (int i = 1; i < Posiciones.Length - 1; i++)
                {
                    var plane = new Plane(Posiciones[i + 1] - Posiciones[i - 1], Posiciones[i - 1]);
                    var projectedPole = plane.ClosestPointOnPlane(poleposicion);
                    var projectedBone = plane.ClosestPointOnPlane(Posiciones[i]);
                    var angle = Vector3.SignedAngle(projectedBone - Posiciones[i - 1], projectedPole - Posiciones[i - 1], plane.normal);
                    Posiciones[i] = Quaternion.AngleAxis(angle, plane.normal) * (Posiciones[i] - Posiciones[i - 1]) + Posiciones[i - 1];
                }
            }

            //set posicion & rotation
            for (int i = 0; i < Posiciones.Length; i++)
            {
                if (i == Posiciones.Length - 1)
                    AsignarRotEspacioRaiz(Huesos[i], Quaternion.Inverse(ObjetivoRotation) * StartRotationObjetivo * Quaternion.Inverse(RotInicioHueso[i]));
                else
                    AsignarRotEspacioRaiz(Huesos[i], Quaternion.FromToRotation(StartDirectionSucc[i], Posiciones[i + 1] - Posiciones[i]) * Quaternion.Inverse(RotInicioHueso[i]));
                AsignarPosEspacioRaiz(Huesos[i], Posiciones[i]);
            }
        }

        private Vector3 ObtenerPosEspacioRaiz(Transform actual)
        {
            if (Raiz == null)
                return actual.position;
            else
                return Quaternion.Inverse(Raiz.rotation) * (actual.position - Raiz.position);
        }

        private void AsignarPosEspacioRaiz(Transform actual, Vector3 posicion)
        {
            if (Raiz == null)
                actual.position = posicion;
            else
                actual.position = Raiz.rotation * posicion + Raiz.position;
        }

        private Quaternion ObtenerRotEspacioRaiz(Transform actual)
        {
            //inverse(after) * before => rot: before -> after
            if (Raiz == null)
                return actual.rotation;
            else
                return Quaternion.Inverse(actual.rotation) * Raiz.rotation;
        }

        private void AsignarRotEspacioRaiz(Transform actual, Quaternion rotation)
        {
            if (Raiz == null)
                actual.rotation = rotation;
            else
                actual.rotation = Raiz.rotation * rotation;
        }

        private void OnDrawGizmosSelected() {
              #if UNITY_EDITOR
                var actual = this.transform;
                for (int i = 0; i < longitudCadena && actual != null && actual.parent != null; i++)
                {
                    var escala = Vector3.Distance(actual.position, actual.parent.position) * 0.1f;
                    Handles.matrix = Matrix4x4.TRS(actual.position, Quaternion.FromToRotation(Vector3.up, actual.parent.position - actual.position), new Vector3(escala, Vector3.Distance(actual.parent.position, actual.position), escala));
                    Handles.color = Color.green;
                    Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
                    actual = actual.parent;
                }
            #endif
        }

        public float GetLongitudCompleta(){
            return LongitudCompleta;
        }

        public Transform getTip()
        {
            return Huesos[0];
        }
}
