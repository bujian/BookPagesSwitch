// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:33032,y:32667,varname:node_4013,prsc:2|diff-2807-OUT,emission-2807-OUT,clip-336-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32502,y:32479,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Distance,id:8217,x:32472,y:32913,varname:node_8217,prsc:2|A-3807-XYZ,B-48-XYZ;n:type:ShaderForge.SFN_Transform,id:3807,x:32192,y:32792,varname:node_3807,prsc:2,tffrom:0,tfto:1|IN-4311-XYZ;n:type:ShaderForge.SFN_Transform,id:48,x:32165,y:32992,varname:node_48,prsc:2,tffrom:0,tfto:1|IN-7214-XYZ;n:type:ShaderForge.SFN_FragmentPosition,id:4311,x:32017,y:32792,varname:node_4311,prsc:2;n:type:ShaderForge.SFN_If,id:6921,x:32778,y:33000,varname:node_6921,prsc:2|A-8217-OUT,B-4631-OUT,GT-7574-OUT,EQ-7574-OUT,LT-221-OUT;n:type:ShaderForge.SFN_ComponentMask,id:336,x:32857,y:32834,varname:node_336,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-6921-OUT;n:type:ShaderForge.SFN_ValueProperty,id:221,x:32561,y:33183,ptovrint:False,ptlb:less,ptin:_less,varname:node_221,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_ValueProperty,id:7574,x:32549,y:33117,ptovrint:False,ptlb:great,ptin:_great,varname:node_7574,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:4631,x:32549,y:33034,ptovrint:False,ptlb:当前模型与中心距离,ptin:_,varname:node_4631,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:50;n:type:ShaderForge.SFN_Vector4Property,id:7214,x:31958,y:33017,ptovrint:False,ptlb:_中心点位置,ptin:__,varname:node_7214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Tex2d,id:8670,x:32472,y:32668,ptovrint:False,ptlb:node_8670,ptin:_node_8670,varname:node_8670,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2807,x:32770,y:32647,varname:node_2807,prsc:2|A-1304-RGB,B-8670-RGB;proporder:1304-221-7574-4631-7214-8670;pass:END;sub:END;*/

Shader "xumeng/NewShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _less ("less", Float ) = 0.8
        _great ("great", Float ) = 0.1
        _ ("当前模型与中心距离", Float ) = 50
        __ ("_中心点位置", Vector) = (0,0,0,0)
        _node_8670 ("node_8670", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float _less;
            uniform float _great;
            uniform float _;
            uniform float4 __;
            uniform sampler2D _node_8670; uniform float4 _node_8670_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float node_6921_if_leA = step(distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb),_);
                float node_6921_if_leB = step(_,distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb));
                clip(lerp((node_6921_if_leA*_less)+(node_6921_if_leB*_great),_great,node_6921_if_leA*node_6921_if_leB).r - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _node_8670_var = tex2D(_node_8670,TRANSFORM_TEX(i.uv0, _node_8670));
                float3 node_2807 = (_Color.rgb*_node_8670_var.rgb);
                float3 diffuseColor = node_2807;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float3 emissive = node_2807;
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _Color;
            uniform float _less;
            uniform float _great;
            uniform float _;
            uniform float4 __;
            uniform sampler2D _node_8670; uniform float4 _node_8670_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float node_6921_if_leA = step(distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb),_);
                float node_6921_if_leB = step(_,distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb));
                clip(lerp((node_6921_if_leA*_less)+(node_6921_if_leB*_great),_great,node_6921_if_leA*node_6921_if_leB).r - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _node_8670_var = tex2D(_node_8670,TRANSFORM_TEX(i.uv0, _node_8670));
                float3 node_2807 = (_Color.rgb*_node_8670_var.rgb);
                float3 diffuseColor = node_2807;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float _less;
            uniform float _great;
            uniform float _;
            uniform float4 __;
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float node_6921_if_leA = step(distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb),_);
                float node_6921_if_leB = step(_,distance(mul( unity_WorldToObject, float4(i.posWorld.rgb,0) ).xyz.rgb,mul( unity_WorldToObject, float4(__.rgb,0) ).xyz.rgb));
                clip(lerp((node_6921_if_leA*_less)+(node_6921_if_leB*_great),_great,node_6921_if_leA*node_6921_if_leB).r - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
