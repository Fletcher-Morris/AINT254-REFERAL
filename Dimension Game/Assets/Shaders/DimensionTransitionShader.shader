Shader "Custom/DimensionTransitionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intensity ("Intensity", Range(0.0, 1.0)) = 0.0
		_1 ("1", 2D) = "white" {}
		_2 ("2", 2D) = "white" {}
		_3 ("3", 2D) = "white" {}
		_4 ("4", 2D) = "white" {}
		_5 ("5", 2D) = "white" {}
		_6 ("6", 2D) = "white" {}
		_7 ("7", 2D) = "white" {}
		_8 ("8", 2D) = "white" {}
		_9 ("9", 2D) = "white" {}
		_10 ("10", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Intensity;
			int _Img;

			sampler2D _1;
			sampler2D _2;
			sampler2D _3;
			sampler2D _4;
			sampler2D _5;
			sampler2D _6;
			sampler2D _7;
			sampler2D _8;
			sampler2D _9;
			sampler2D _10;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 effect = tex2D(_1, i.uv);
				if(_Img >= 1){effect = tex2D(_2, i.uv);}
				if(_Img >= 2){effect = tex2D(_3, i.uv);}
				if(_Img >= 3){effect = tex2D(_4, i.uv);}
				if(_Img >= 4){effect = tex2D(_5, i.uv);}
				if(_Img >= 5){effect = tex2D(_6, i.uv);}
				if(_Img >= 6){effect = tex2D(_7, i.uv);}
				if(_Img >= 7){effect = tex2D(_8, i.uv);}
				if(_Img >= 8){effect = tex2D(_9, i.uv);}
				if(_Img >= 9){effect = tex2D(_10, i.uv);}

				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col.rgb = col.rgb + (effect.rgb * _Intensity);
				return col;
			}
			ENDCG
		}
	}
}