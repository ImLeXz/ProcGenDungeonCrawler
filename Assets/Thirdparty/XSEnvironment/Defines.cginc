sampler2D _MainTex; float4 _MainTex_ST;
sampler2D _MetallicGlossMap; float4 _MetallicGlossMap_ST;
sampler2D _BumpMap; float4 _BumpMap_ST;
sampler2D _EmissionMap; float4 _EmissionMap_ST;
sampler2D _OcclusionMap; float4 _OcclusionMap_ST;
sampler2D _ThicknessMap; float4 _ThicknessMap_ST;

#if defined(SnowCoverage)
sampler2D _SnowNoise; float4 _SnowNoise_ST;
float _InvertSnowCoverage;
#endif

float _Cutoff;
float4 _RotationAxes;
float4 _EmissionColor;
float4 _Color;
float4 _SSColor;
float _SSDistortion;
float _SSPower;
float _SSScale;
float _Metallic;
float _Glossiness;
float _BumpScale;
float _OcclusionStrength;
float _SnowCoverage;
float _GILightmapScalar;
float _SpecularLMOcclusion;
float _SpecLMOcclusionAdjust;
float _TriplanarFalloff;
float _LMStrength;
float _RTLMStrength;
float _LightMapSpecularStrength;
int _UseFakeTransmission;
int _TextureSampleMode;
int _CastShadowsToLightmap;
int _DebugLightmapView;
int _LightProbeMethod;