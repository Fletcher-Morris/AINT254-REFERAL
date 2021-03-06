// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Portal"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Tear("Tear", 2D) = "white" {}
		_Completion("Completion", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float4 screenPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _PortalPrevewTex;
		uniform float _Completion;
		uniform sampler2D _Tear;
		uniform float4 _Tear_ST;
		uniform float _Cutoff = 0.5;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 0.0,10.0,32.0);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime57 = _Time.y * 2.0;
			float temp_output_60_0 = ( sin( ( ( ase_vertex3Pos.y * 100.0 ) + mulTime57 ) ) * 0.01 );
			float3 appendResult51 = (float3(temp_output_60_0 , temp_output_60_0 , 0.0));
			v.vertex.xyz += appendResult51;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			o.Emission = tex2D( _PortalPrevewTex, ase_screenPosNorm.xy ).rgb;
			o.Alpha = 1;
			float2 uv_Tear = i.uv_texcoord * _Tear_ST.xy + _Tear_ST.zw;
			float4 tex2DNode3 = tex2D( _Tear, uv_Tear );
			float ifLocalVar19 = 0;
			if( ( _Completion * _Completion * _Completion ) > ( ( tex2DNode3.r + tex2DNode3.b ) / 2.0 ) )
				ifLocalVar19 = tex2DNode3.g;
			float clampResult41 = clamp( ifLocalVar19 , 0.0 , 1.0 );
			clip( clampResult41 - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
171;224;1349;655;1011.715;582.072;1.3;True;True
Node;AmplifyShaderEditor.PosVertexDataNode;53;-896,128;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;55;-864,272;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;100;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;57;-640,352;Float;False;1;0;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-672,128;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-1248,-144;Float;True;Property;_Tear;Tear;2;0;Create;True;0;0;False;0;0259111f4eb22ed4abcff8612ff92497;0259111f4eb22ed4abcff8612ff92497;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-432,128;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-941.4194,-107.5269;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-880,-240;Float;False;Property;_Completion;Completion;3;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-272,352;Float;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;54;-272,128;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-576,-256;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;40;-649.2903,-112;Float;True;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;19;-400,-144;Float;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-32,128;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-192,-352;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;41;-128,-48;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;80,-336;Float;True;Global;_PortalPrevewTex;_PortalPrevewTex;0;0;Create;True;0;0;True;0;143ac18a926a8b7408621aae002b76e3;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceBasedTessNode;62;206.3247,429.2079;Float;False;3;0;FLOAT;32;False;1;FLOAT;0;False;2;FLOAT;10;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;51;214.2284,118.0166;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;43;528,-272;Float;False;True;6;Float;ASEMaterialInspector;0;0;Unlit;Custom/Portal;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;32;0;5;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;56;0;53;2
WireConnection;56;1;55;0
WireConnection;58;0;56;0
WireConnection;58;1;57;0
WireConnection;39;0;3;1
WireConnection;39;1;3;3
WireConnection;54;0;58;0
WireConnection;21;0;7;0
WireConnection;21;1;7;0
WireConnection;21;2;7;0
WireConnection;40;0;39;0
WireConnection;19;0;21;0
WireConnection;19;1;40;0
WireConnection;19;2;3;2
WireConnection;60;0;54;0
WireConnection;60;1;59;0
WireConnection;41;0;19;0
WireConnection;1;1;2;0
WireConnection;51;0;60;0
WireConnection;51;1;60;0
WireConnection;43;2;1;0
WireConnection;43;10;41;0
WireConnection;43;11;51;0
WireConnection;43;14;62;0
ASEEND*/
//CHKSM=03366BF5B9E5EAFBE99BD7B97BCF3E9510924628