// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/SuperGrass"
{
	Properties
	{
		_AlphaMask("AlphaMask", 2D) = "white" {}
		_BaseColour("BaseColour", Color) = (0.3215686,0.227451,0.1176471,1)
		_TipColour("TipColour", Color) = (0.5843138,0.7490196,0.2078432,1)
		_Offset("Offset", Float) = 0
		_PlayerPos("PlayerPos", Vector) = (0,0,0,0)
		_Radius("Radius", Float) = 0
		_Clamp("Clamp", Range( 0 , 10)) = 0
		_FixRoots("FixRoots", Range( 0 , 1)) = 1
		_BendGradient("BendGradient", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" }
		Cull Off
		AlphaToMask On
		CGPROGRAM
		#pragma target 3.5
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform sampler2D _BendGradient;
		uniform float4 _BendGradient_ST;
		uniform float _Offset;
		uniform float _Radius;
		uniform float3 _PlayerPos;
		uniform float _Clamp;
		uniform float _FixRoots;
		uniform float4 _BaseColour;
		uniform float4 _TipColour;
		uniform sampler2D _AlphaMask;
		uniform float4 _AlphaMask_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_BendGradient = v.texcoord * _BendGradient_ST.xy + _BendGradient_ST.zw;
			float lerpResult49 = lerp( tex2Dlod( _BendGradient, float4( uv_BendGradient, 0, 0.0) ).r , 1.0 , 0.3);
			float blendOpSrc47 = v.texcoord.xy.y;
			float blendOpDest47 = lerpResult49;
			float lerpResult14 = lerp( ( saturate( ( blendOpSrc47 * blendOpDest47 ) )) , 1.0 , 0.0);
			float OffsetXZ13 = lerpResult14;
			float MultXZ12 = _Offset;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float temp_output_55_0 = ( _PlayerPos.y + 2.0 );
			float3 appendResult26 = (float3(_PlayerPos.x , (( ase_worldPos.y > temp_output_55_0 ) ? temp_output_55_0 :  (( ase_worldPos.y < 0.0 ) ? 0.0 :  ase_vertex3Pos.y ) ) , _PlayerPos.z));
			float clampResult29 = clamp( ( _Radius - distance( ase_vertex3Pos , appendResult26 ) ) , 0.0 , _Clamp );
			float3 appendResult21 = (float3(_PlayerPos.x , ase_vertex3Pos.y , _PlayerPos.z));
			float3 normalizeResult22 = normalize( ( ase_vertex3Pos - appendResult21 ) );
			float3 _Gravity = float3(0,1,0);
			float3 normalizeResult60 = normalize( _Gravity );
			float temp_output_39_0 = v.texcoord.xy.y;
			float blendOpSrc40 = temp_output_39_0;
			float blendOpDest40 = temp_output_39_0;
			float lerpResult33 = lerp( ( saturate( ( blendOpSrc40 * blendOpDest40 ) )) , 1.0 , ( 1.0 - _FixRoots ));
			v.vertex.xyz += ( ( OffsetXZ13 * MultXZ12 * ( clampResult29 * normalizeResult22 ) ) + ( -clampResult29 * normalizeResult60 * lerpResult33 ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult5 = lerp( _BaseColour , _TipColour , i.uv_texcoord.y);
			o.Albedo = lerpResult5.rgb;
			float2 uv_AlphaMask = i.uv_texcoord * _AlphaMask_ST.xy + _AlphaMask_ST.zw;
			o.Alpha = tex2D( _AlphaMask, uv_AlphaMask ).a;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
237;92;1292;655;3306.976;221.5762;1.729213;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;52;-3205.366,51.62775;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;56;-3197.174,191.3453;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;16;-3860,544;Float;False;Property;_PlayerPos;PlayerPos;5;0;Create;True;0;0;False;0;0,0,0;0,0,-3;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCCompareLower;53;-2967.974,181.4453;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-2887.974,325.4453;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;57;-2727.974,101.4452;Float;False;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-2256,-176;Float;True;Property;_BendGradient;BendGradient;9;0;Create;True;0;0;False;0;None;de24223707f11f34e9c21b57659f9130;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;19;-2256,608;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;26;-2176,384;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;24;-2176,256;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;48;-1952,-336;Float;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;49;-1904,-144;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2064,720;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1920,192;Float;False;Property;_Radius;Radius;6;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;34;-1792,816;Float;True;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceOpNode;25;-1920,320;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;-1920,608;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;28;-1744,240;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-1472,1056;Float;False;Property;_FixRoots;FixRoots;8;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;47;-1712.457,-279.8777;Float;True;Multiply;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;39;-1552,816;Float;True;True;True;True;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1744,368;Float;False;Property;_Clamp;Clamp;7;0;Create;True;0;0;False;0;0;0.6666667;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;40;-1280,816;Float;True;Multiply;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;58;-1476,656;Float;False;Constant;_Gravity;Gravity;8;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;14;-1386,-52;Float;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;22;-1776,608;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;42;-1184,1056;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1200,352;Float;False;Property;_Offset;Offset;4;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;29;-1552,288;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;60;-1152,656;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1056,352;Float;False;MultXZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;46;-912,656;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1008,480;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;33;-1008,816;Float;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1083.2,118.3998;Float;False;OffsetXZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-832,-160;Float;False;Property;_TipColour;TipColour;3;0;Create;True;0;0;False;0;0.5843138,0.7490196,0.2078432,1;0.5843138,0.7490196,0.2078432,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-832,0;Float;False;Property;_BaseColour;BaseColour;2;0;Create;True;0;0;False;0;0.3215686,0.227451,0.1176471,1;0.3215685,0.227451,0.1176471,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-752,720;Float;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;3;-800,160;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-768,416;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-592.5583,436.5923;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;5;-544,-16;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;-592,240;Float;True;Property;_AlphaMask;AlphaMask;1;0;Create;True;0;0;False;0;None;19ddafd070e265d48895d584ee6aa29b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;59;-1296,656;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-240,128;Float;False;True;3;Float;ASEMaterialInspector;0;0;Standard;Custom/SuperGrass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;False;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;-0.34;0,0,0,0;VertexScale;False;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;52;2
WireConnection;53;3;56;2
WireConnection;55;0;16;2
WireConnection;57;0;52;2
WireConnection;57;1;55;0
WireConnection;57;2;55;0
WireConnection;57;3;53;0
WireConnection;26;0;16;1
WireConnection;26;1;57;0
WireConnection;26;2;16;3
WireConnection;49;0;51;1
WireConnection;21;0;16;1
WireConnection;21;1;19;2
WireConnection;21;2;16;3
WireConnection;25;0;24;0
WireConnection;25;1;26;0
WireConnection;20;0;19;0
WireConnection;20;1;21;0
WireConnection;28;0;27;0
WireConnection;28;1;25;0
WireConnection;47;0;48;2
WireConnection;47;1;49;0
WireConnection;39;0;34;2
WireConnection;40;0;39;0
WireConnection;40;1;39;0
WireConnection;14;0;47;0
WireConnection;22;0;20;0
WireConnection;42;0;41;0
WireConnection;29;0;28;0
WireConnection;29;2;30;0
WireConnection;60;0;58;0
WireConnection;12;0;11;0
WireConnection;46;0;29;0
WireConnection;23;0;29;0
WireConnection;23;1;22;0
WireConnection;33;0;40;0
WireConnection;33;2;42;0
WireConnection;13;0;14;0
WireConnection;31;0;46;0
WireConnection;31;1;60;0
WireConnection;31;2;33;0
WireConnection;10;0;13;0
WireConnection;10;1;12;0
WireConnection;10;2;23;0
WireConnection;9;0;10;0
WireConnection;9;1;31;0
WireConnection;5;0;2;0
WireConnection;5;1;1;0
WireConnection;5;2;3;2
WireConnection;59;0;58;0
WireConnection;0;0;5;0
WireConnection;0;9;8;4
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=312E103286650A0FD39B0D39DB7D485543CB609F