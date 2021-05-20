Shader "ClaseCG/Shadercito"
{
    Properties
    {
        _Ambientecito("Color ambiente", Color) = (1, 1, 1, 1)
        _Difusito("Color difuso", Color) = (1, 0, 0, 1)
        _Especularcito("Color especular", Color) = (1, 1, 1, 1)
        _Brillito("Coeficiente de brillo", Float) = 10
    }
        SubShader
    {
        Pass {

        CGPROGRAM
        
        // definir 2 shaders
        // vertex shader 
        // fragment shader

        // hay que avisarle al compilador como se llaman
        #pragma vertex vert
        #pragma fragment frag
        // cuando recibamos variables del exterior hay que declarar
        // una variable local del mismo nombre con el modificador uniform
        uniform float4 _Ambientecito;
        
        
        // como definir varios valores de entrada
        // struct - como un objeto pero mas elemental, sólo datos
        struct vInput {
            float4 pos : POSITION;
            // vector normal - vector que apunta hacia "adelante" o
            // "afuera"
            // chequen normalize
            float3 normal : NORMAL;
        };

        // float4 - vector tamaño 4 de floats
        // existen los tipos regulares 
        // SEMANTICS - le indican a GPU como interpretar un dato
        // vertex shader - regresa posición con modificación
        float4 vert(vInput input) : SV_POSITION {
            
            float3 ver = input.pos.xyz + input.normal.xyz * _SinTime.z;
            float3 vertex = UnityObjectToClipPos(ver);
            
            float4 positions = UnityObjectToClipPos(input.pos);
            /*float3 normals = input.normal;
            float3 moved = positions.xyz + normals * _SinTime.z;*/

            /*float4 sinpos = float4(positions.x + _SinTime.z ,positions.y + _SinTime.z, positions.z + _SinTime.z, positions.w); 
            float3 normals = UnityObjectToWorldNormal(input.normal); 
            
            float4 normalsPos = float4(sinpos.x * normals.x,sinpos.y * normals.y,sinpos.z * normals.z,sinpos.w) ;
            float4 result = positions + normalsPos;*/

            
            float4 result = float4(vertex,positions.w);
        
            // sin(), cos()
            //return float4(result.x + _Time.x, result.y, result.z, result.w);
            return result;
            //return result;
        }

        // fragment shader - regresa un color de un fragmento
        float4 frag() : COLOR {
            return _Ambientecito;
        }

        ENDCG
        }
    }
    FallBack "Diffuse"
}
