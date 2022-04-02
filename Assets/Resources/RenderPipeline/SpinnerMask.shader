// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
 
Shader "Custom/SpinnerMask"
{
    Properties {
     _Color ("Color", Color) = (1,1,1,1)
     _MainTex ("Albedo (RGB)", 2D) = "white" {}
 }
 SubShader {
 Cull Off
 Pass{
     ZTest Greater
     }
 Pass{
     ZTest Less
 }
 Pass{
     ZTest Always
 }
     Tags { "RenderType"="Opaque" } // https://answers.unity.com/questions/802180/make-gameobjects-visible-when-behind-other-gameobj.html
     LOD 200
     
     CGPROGRAM
     // Physically based Standard lighting model, and enable shadows on all light types
     #pragma surface surf Standard fullforwardshadows
     // Use shader model 3.0 target, to get nicer looking lighting
     #pragma target 3.0
     sampler2D _MainTex;
     struct Input {
         float2 uv_MainTex;
     };
     fixed4 _Color;
     void surf (Input IN, inout SurfaceOutputStandard o) {
         // Albedo comes from a texture tinted by color
         fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
         o.Albedo = c.rgb;
         // Metallic and smoothness come from slider variables
         o.Alpha = c.a;
     }
     ENDCG
 } 
 FallBack "Diffuse"
}
 