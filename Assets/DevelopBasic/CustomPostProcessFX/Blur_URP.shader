Shader "Hidden/Custom/Blur_URP"
{
	Properties 
	{
	    _MainTex ("Main Texture", 2D) = "white" {}
	}
    
    HLSLINCLUDE
// StdLib.hlsl holds pre-configured vertex shaders (VertDefault), varying structs (VaryingsDefault), and most of the data you need to write common effects.
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);

    float _BlurSize;
    int _Sample;

    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
    };

    struct Varyings
    {
        float2 uv        : TEXCOORD0;
        float4 vertex : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    
    Varyings vert(Attributes input)
    {
        Varyings o = (Varyings)0;
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o)
        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        o.vertex = vertexInput.positionCS;
        o.uv = input.uv;
        
        return o;
    }
    float4 frag (Varyings input) : SV_Target 
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    	float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
    	return lerp(color, float4(0,0,0,1), _BlurSize);
    }

    float4 Frag_Vertical(Varyings i) : SV_Target
    {
        float4 col = 0;
        for(float index = 0; index < _Sample; index++){
            float2 uv = i.uv + float2(0, (index/(_Sample-1) - 0.5) * _BlurSize);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        col /= _Sample;

        return col;
    }
    float4 Frag_Horizontal(Varyings i) : SV_Target
    {
        float invAspect = _ScreenParams.y/_ScreenParams.x;
        float4 col = 0;
        for(float index = 0; index < _Sample; index++){
            float2 uv = i.uv + float2((index/(_Sample-1) - 0.5f) * _BlurSize * invAspect, 0);
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        col = col/_Sample;
        return col;
    }
    ENDHLSL
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment Frag_Vertical
            ENDHLSL
        }
        Pass
        {
            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment Frag_Horizontal
            ENDHLSL
        }
    }
}