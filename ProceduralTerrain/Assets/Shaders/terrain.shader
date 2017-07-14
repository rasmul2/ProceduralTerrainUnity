// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Unlit/terrain"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Normal("Texture", 2D) = "white" {}
		_Curvature("Curve", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

			 CGPROGRAM
	        // Surface shader function is called surf, and vertex preprocessor function is called vert
	        // addshadow used to add shadow collector and caster passes following vertex modification
	        #pragma surface surf Lambert vertex:vert addshadow
	 
	        // Access the shaderlab properties
	        uniform sampler2D _MainTex;
	        uniform float _Curvature;
	 
	        // Basic input structure to the shader function
	        // requires only a single set of UV texture mapping coordinates
	        struct Input {
	            float2 uv_MainTex;
	        };
	 
	        // This is where the curvature is applied
	        void vert( inout appdata_full v)
	        {
	            // Transform the vertex coordinates from model space into world space
	            float4 vv = mul( unity_ObjectToWorld, v.vertex );
	 
	            // Now adjust the coordinates to be relative to the camera position
	            vv.xyz -= _WorldSpaceCameraPos.xyz;
	 
	            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
	            // on the square of the distance from the camera in the z axis, multiplied
	            // by the chosen curvature factor
	  
	            vv = float4( (sin(vv.x/vv.z))*_Curvature,(pow(vv.z,2)) *_Curvature, (sin(vv.x/vv.z))*_Curvature, vv.y*-_Curvature/50+.05f);
	           
	 
	            // Now apply the offset back to the vertices in model space
	            v.vertex += mul(unity_WorldToObject, vv);
	        }
	 
	        // This is just a default surface shader
	        void surf (Input IN, inout SurfaceOutput o) {
	            half4 c = tex2D (_MainTex, IN.uv_MainTex);
	            o.Albedo = c.rgb;
	            o.Alpha = c.a;
	        }
	        ENDCG
	    
	}
 
    // FallBack "Diffuse"
}