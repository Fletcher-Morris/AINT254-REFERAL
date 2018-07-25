// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/SuperGrass"
{
	Properties
	{
		_AlphaMask("AlphaMask", 2D) = "white" {}
		_Offset("Offset", Float) = 0
		_PlayerPos("PlayerPos", Vector) = (0,0,0,0)
		_Radius("Radius", Float) = 0
		_Clamp("Clamp", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" }
		Cull Off
		AlphaToMask On
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Offset;
		uniform float _Radius;
		uniform float3 _PlayerPos;
		uniform float _Clamp;
		uniform sampler2D _AlphaMask;
		uniform float4 _AlphaMask_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float MultXZ12 = _Offset;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 appendResult26 = (float3(_PlayerPos.x , 0.0 , _PlayerPos.z));
			float clampResult29 = clamp( ( _Radius - distance( ase_vertex3Pos , appendResult26 ) ) , 0.0 , _Clamp );
			float3 appendResult21 = (float3(_PlayerPos.x , ase_vertex3Pos.y , _PlayerPos.z));
			float3 normalizeResult22 = normalize( ( ase_vertex3Pos - appendResult21 ) );
			v.vertex.xyz += ( ( MultXZ12 * ( clampResult29 * normalizeResult22 ) ) + float3( 0,0,0 ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 lerpResult5 = lerp( float4(0.3215686,0.227451,0.1176471,1) , float4(0.5843138,0.7490196,0.2078432,1) , i.uv_texcoord.y);
			o.Albedo = lerpResult5.rgb;
			float2 uv_AlphaMask = i.uv_texcoord * _AlphaMask_ST.xy + _AlphaMask_ST.zw;
			o.Alpha = tex2D( _AlphaMask, uv_AlphaMask ).a;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
7;29;1906;1004;3738.57;834.3825;2.595892;True;True
Node;AmplifyShaderEditor.Vector3Node;16;-2560,768;Float;False;Property;_PlayerPos;PlayerPos;2;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;19;-2256,672;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;24;-2176,256;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;26;-2176,384;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;21;-2032,800;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;25;-1920,320;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-1920,192;Float;False;Property;_Radius;Radius;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;28;-1744,240;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1744,368;Float;False;Property;_Clamp;Clamp;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;20;-1872,672;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;22;-1728,672;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1200,352;Float;False;Property;_Offset;Offset;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;29;-1552,288;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-1056,352;Float;False;MultXZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1152,512;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;3;-688,-64;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-768,416;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;1;-720,-384;Float;False;Constant;_TipColour;TipColour;0;0;Create;True;0;0;False;0;0.5843138,0.7490196,0.2078432,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-720,-224;Float;False;Constant;_BaseColour;BaseColour;0;0;Create;True;0;0;False;0;0.3215686,0.227451,0.1176471,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;14;-1280,96;Float;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-592.5583,436.5923;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;15;-1472,144;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;5;-432,-240;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;8;-640,112;Float;True;Property;_AlphaMask;AlphaMask;0;0;Create;True;0;0;False;0;None;19ddafd070e265d48895d584ee6aa29b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-1083.2,118.3998;Float;False;OffsetXZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-128,128;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Custom/SuperGrass;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;False;0;False;Opaque;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;26;0;16;1
WireConnection;26;2;16;3
WireConnection;21;0;16;1
WireConnection;21;1;19;2
WireConnection;21;2;16;3
WireConnection;25;0;24;0
WireConnection;25;1;26;0
WireConnection;28;0;27;0
WireConnection;28;1;25;0
WireConnection;20;0;19;0
WireConnection;20;1;21;0
WireConnection;22;0;20;0
WireConnection;29;0;28;0
WireConnection;29;2;30;0
WireConnection;12;0;11;0
WireConnection;23;0;29;0
WireConnection;23;1;22;0
WireConnection;10;0;12;0
WireConnection;10;1;23;0
WireConnection;14;2;15;0
WireConnection;9;0;10;0
WireConnection;5;0;2;0
WireConnection;5;1;1;0
WireConnection;5;2;3;2
WireConnection;13;0;14;0
WireConnection;0;0;5;0
WireConnection;0;9;8;4
WireConnection;0;11;9;0
ASEEND*/
//CHKSM=A59DF484A574871DC61DABD4155BC55760D64DA7