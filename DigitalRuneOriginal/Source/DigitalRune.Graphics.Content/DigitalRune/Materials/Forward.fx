//-----------------------------------------------------------------------------
// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.
//-----------------------------------------------------------------------------
//
/// \file Forward.fx
/// Renders a model in a single pass ("forward rendering").
/// Supports:
/// - Diffuse color/texture
/// - Specular color/texture
/// - Specular power
/// - Alpha blending
//
// ----- Defines
// This effect can be configured using various defines (see section "Defines").
// Default settings: 1 ambient + 1 directional light (without shadow) + fog
//
// ----- Alpha transparency
// Alpha values can be specified in
// - Diffuse texture (alpha channel)
// - Effect parameter "Alpha"
// 
// Variant #1: "Alpha for transparent materials"
// When rendering transparent materials, such as glass, alpha should only be
// applied to the diffuse component.
//
// Variant #2: "Fade in/out using alpha"
// When using alpha to fade an object in/out, alpha needs to be applied to all
// components (diffuse, specular, emissive).
//
// The current implementation applies alpha to the entire object (Variant #2)!
//
// ----- Morphing (morph target animation)
// When morphing is enabled the binormal is excluded from the vertex attributes
// to stay within the max number of vertex attributes. The binormal is derived
// from normal and tangent in the vertex shader.
// References:
// - C. Beeson & K. Bjorke: Curtis Beeson: Skin in the "Dawn" Demo,
//   http://http.developer.nvidia.com/GPUGems/gpugems_ch03.html
// - C. Beeson, NVIDIA: Animation in the "Dawn" Demo,
//   http://http.developer.nvidia.com/GPUGems/gpugems_ch04.html
//
//-----------------------------------------------------------------------------

#include "../Common.fxh"
#include "../Lighting.fxh"
#include "../Material.fxh"
#include "../Fog.fxh"
//#include "../Lod.fxh"
//#include "../Parallax.fxh"
#include "../Noise.fxh"

#define CLAMP_TEXCOORDS_TO_SHADOW_MAP_BOUNDS 1
#include "../ShadowMap.fxh"


#include "../Encoding.fxh"



#include "../Deferred.fxh"



//--------------------------------------------------------
// Defines
//--------------------------------------------------------

// Use the following #define directives to enable/disable certain features.
// (Most features are disabled by default. See default values below.)

// ---- General
//#define DEPTH_TEST 1                // Perform manual depth test using G-buffer.
//#define GAMMA_CORRECTION 1          // Apply gamma correction to output.

// ----- Material
//#define TWO_SIDED 1                 // Render geometry as two-sided (back faces before front faces).
//#define PREMULTIPLIED_ALPHA 1       // Diffuse texture uses premultiplied alpha.
//#define VERTEX_COLORS 1             // Enable vertex colors.
//#define EMISSIVE 1                  // Enable emissive material.
//#define DIFFUSE_TEXTURE 1           // Enable diffuse texture.
//#define OPACITY_TEXTURE 1           // Enable opacity texture (stored in A channel of diffuse texture).
//#define SPECULAR_TEXTURE 1          // Enable specular texture.
//#define EMISSIVE_TEXTURE 1          // Enable emissive texture (stored in A channel of specular texture).
//#define NORMAL_TEXTURE 1            // Enable normal texture.

// ----- Animation
//#define MORPHING 1                  // Enable morph targets.
//#define SKINNING 1                  // Enable mesh skinning.

// ----- Environment
//#define ENVIRONMENT_TEXTURE 1       // Enable environment texture.
//#define FOG 1                       // Enable fog.

// ----- Lights
//#define AMBIENT_LIGHT_COUNT n       // Max number of ambient lights.
//#define DIRECTIONAL_LIGHT_COUNT n   // Max number of directional lights.
//#define POINT_LIGHT_COUNT n         // Max number of point lights.
//#define SPOTLIGHT_COUNT n           // Max number of spotlights.
//#define PROJECTOR_LIGHT_COUNT 1     // Max number of projector lights.

//#define DIRECTIONAL_LIGHT_TEXTURE 1 // Enable texture for first directional light.
//#define POINT_LIGHT_TEXTURE 1       // Enable texture for first point light.
//#define SPOTLIGHT_TEXTURE 1         // Enable texture for first spotlight.

//#define DIRECTIONAL_LIGHT_SHADOW 1  // Enable shadows for first directional light.

// ----- Default features:
// Diffuse (RGB + A) + Specular textures
// 1 ambient light
// 1 directional light
// 0 point lights
// 0 spotlights
// 0 projector lights
// Fog enabled


#define DIFFUSE_TEXTURE 1



#define OPACITY_TEXTURE 1



#define SPECULAR_TEXTURE 1



#define AMBIENT_LIGHT_COUNT 1



#define DIRECTIONAL_LIGHT_COUNT 1



#define POINT_LIGHT_COUNT 0



#define SPOTLIGHT_COUNT 0



#define PROJECTOR_LIGHT_COUNT 0


#error "Forward.fx supports max. 1 projector light."



#define FOG 1



#define REQUIRES_TEXCOORD 1



//--------------------------------------------------------
// Constants
//--------------------------------------------------------

float4x4 World : WORLD;
float4x4 View : VIEW;
float4x4 Projection : PROJECTION;
float3 CameraPosition : CAMERAPOSITION;


float2 ViewportSize : VIEWPORTSIZE;
DECLARE_UNIFORM_GBUFFER(GBuffer0, 0);
float CameraFar : CAMERAFAR;



//--------------------------------------------------------
// Material Parameters
//--------------------------------------------------------

float3 DiffuseColor : DIFFUSECOLOR;
float3 SpecularColor : SPECULARCOLOR;
float SpecularPower : SPECULARPOWER;

float3 EmissiveColor : EMISSIVECOLOR;

float Alpha : ALPHA = 1;
float BlendMode : BLENDMODE = 1;    // 0 = Additive alpha-blending, 1 = normal alpha-blending


DECLARE_UNIFORM_DIFFUSETEXTURE      // Diffuse (RGB) + Opacity (A)


DECLARE_UNIFORM_SPECULARTEXTURE     // Specular (RGB) + Emissive (A)


DECLARE_UNIFORM_NORMALTEXTURE



float MorphWeight0 : MORPHWEIGHT0;
float MorphWeight1 : MORPHWEIGHT1;
float MorphWeight2 : MORPHWEIGHT2;
float MorphWeight3 : MORPHWEIGHT3;
float MorphWeight4 : MORPHWEIGHT4;



float4x3 Bones[72] : BONES;



//--------------------------------------------------------
// Light Parameters
//--------------------------------------------------------

// ----- Ambient light

float3 AmbientLight[AMBIENT_LIGHT_COUNT] : AMBIENTLIGHT;
float AmbientLightAttenuation[AMBIENT_LIGHT_COUNT] : AMBIENTLIGHTATTENUATION;
float3 AmbientLightUp[AMBIENT_LIGHT_COUNT] : AMBIENTLIGHTUP;


// ----- Directional lights

float3 DirectionalLightDiffuse[DIRECTIONAL_LIGHT_COUNT] : DIRECTIONALLIGHTDIFFUSE;
float3 DirectionalLightSpecular[DIRECTIONAL_LIGHT_COUNT] : DIRECTIONALLIGHTSPECULAR;
float3 DirectionalLightDirection[DIRECTIONAL_LIGHT_COUNT] : DIRECTIONALLIGHTDIRECTION;

texture DirectionalLightTexture0 : DIRECTIONALLIGHTTEXTURE0;
sampler DirectionalLightTexture0Sampler = sampler_state
{
  Texture = (DirectionalLightTexture0);
  AddressU  = WRAP;
  AddressV  = WRAP;
  MinFilter = LINEAR;
  MagFilter = LINEAR;
  MipFilter = LINEAR;
};
float4x4 DirectionalLightTextureMatrix0 : DIRECTIONALLIGHTTEXTUREMATRIX0;


// Shadow parameters given as struct.
//CascadedShadowParameters ShadowParam : DIRECTIONALLIGHTSHADOWPARAMETERS0;

// MonoGame does not yet support struct parameters.
// Effect parameters need to be specified individually.
int DirectionalLightShadowNumberOfCascades : DIRECTIONALLIGHTSHADOWNUMBEROFCASCADES0;

float4 DirectionalLightShadowCascadeDistances : DIRECTIONALLIGHTSHADOWCASCADEDISTANCES0;

float4x4 DirectionalLightShadowViewProjections[4] : DIRECTIONALLIGHTSHADOWVIEWPROJECTIONS0;
float4 DirectionalLightShadowDepthBias : DIRECTIONALLIGHTSHADOWDEPTHBIAS0;
float4 DirectionalLightShadowNormalOffset : DIRECTIONALLIGHTSHADOWDNORMALOFFSET0;
float2 DirectionalLightShadowMapSize : DIRECTIONALLIGHTSHADOWMAPSIZE0;
float DirectionalLightShadowFilterRadius : DIRECTIONALLIGHTSHADOWFILTERRADIUS0;
float DirectionalLightShadowJitterResolution : DIRECTIONALLIGHTSHADOWJITTERRESOLUTION0;
float DirectionalLightShadowFadeOutRange : DIRECTIONALLIGHTSHADOWFADEOUTRANGE0;
float DirectionalLightShadowMaxDistance : DIRECTIONALLIGHTSHADOWMAXDISTANCE0;
float DirectionalLightShadowFog : DIRECTIONALLIGHTSHADOWFOG0;

DECLARE_UNIFORM_SHADOWMAP(DirectionalLightShadowMap, DIRECTIONALLIGHTSHADOWMAP0);

float JitterMapSize : JITTERMAPSIZE;
DECLARE_UNIFORM_JITTERMAP(JitterMap);



// ----- Point lights

float3 PointLightDiffuse[POINT_LIGHT_COUNT] : POINTLIGHTDIFFUSE;
float3 PointLightSpecular[POINT_LIGHT_COUNT] : POINTLIGHTSPECULAR;
float3 PointLightPosition[POINT_LIGHT_COUNT] : POINTLIGHTPOSITION;
float PointLightRange[POINT_LIGHT_COUNT] : POINTLIGHTRANGE;
float PointLightAttenuation[POINT_LIGHT_COUNT] : POINTLIGHTATTENUATION;

DECLARE_UNIFORM_LIGHTTEXTURE(PointLightTexture0, POINT_LIGHT_TEXTURE0);
float4x4 PointLightTextureMatrix0 : POINTLIGHTTEXTUREMATRIX0;



// ----- Spotlights

float3 SpotlightDiffuse[SPOTLIGHT_COUNT] : SPOTLIGHTDIFFUSE;
float3 SpotlightSpecular[SPOTLIGHT_COUNT] : SPOTLIGHTSPECULAR;
float3 SpotlightPosition[SPOTLIGHT_COUNT] : SPOTLIGHTPOSITION;
float3 SpotlightDirection[SPOTLIGHT_COUNT] : SPOTLIGHTDIRECTION;
float SpotlightAttenuation[SPOTLIGHT_COUNT] : SPOTLIGHTATTENUATION;
float SpotlightRange[SPOTLIGHT_COUNT] : SPOTLIGHTRANGE;
float SpotlightFalloffAngle[SPOTLIGHT_COUNT] : SPOTLIGHTFALLOFFANGLE;
float SpotlightCutoffAngle[SPOTLIGHT_COUNT] : SPOTLIGHTCUTOFFANGLE;

DECLARE_UNIFORM_LIGHTTEXTURE(SpotlightTexture0, SPOTLIGHT_TEXTURE0);
float4x4 SpotlightTextureMatrix0 : SPOTLIGHTTEXTUREMATRIX0;



// ----- Projector light

float3 ProjectorLightDiffuse : PROJECTORLIGHTDIFFUSE;
float3 ProjectorLightSpecular : PROJECTORLIGHTSPECULAR;
float3 ProjectorLightPosition : PROJECTORLIGHTPOSITION;
float ProjectorLightRange : PROJECTORLIGHTRANGE;
float ProjectorLightAttenuation : PROJECTORLIGHTATTENUATION;
DECLARE_UNIFORM_LIGHTTEXTURE(ProjectorLightTexture, PROJECTORLIGHTTEXTURE);
float4x4 ProjectorLightTextureMatrix : PROJECTORLIGHTTEXTUREMATRIX;



texture EnvironmentMap : ENVIRONMENTMAP;
samplerCUBE EnvironmentSampler = sampler_state
{
  Texture = <EnvironmentMap>;
  MinFilter = LINEAR;
  MagFilter = LINEAR;
  MipFilter = LINEAR;
  AddressU = CLAMP;
  AddressV = CLAMP;
};
float EnvironmentMapSize : ENVIRONMENTMAPSIZE;
float3 EnvironmentMapDiffuse : ENVIRONMENTMAPDIFFUSE;
float3 EnvironmentMapSpecular : ENVIRONMENTMAPSPECULAR;
float EnvironmentMapRgbmMax : ENVIRONMENTMAPRGBMMAX;
float4x4 EnvironmentMapMatrix : ENVIRONMENTMAPMATRIX;



//--------------------------------------------------------
// Parallax Occlusion Mapping (POM)
//--------------------------------------------------------

//// The height scale for the height map (used in the parallax occlusion mapping).
//float HeightScale = 0;  // Set a value = 0 to disable POM. Set a value > 0 to enable POM.
//// The height bias for the height map (used in the parallax occlusion mapping).
//float HeightBias = 0;
//// The mip level id for transitioning between the full POM computation and normal mapping.
//int PomLodThreshold = 3;
//// The minimum number of samples for sampling the height field profile
//int PomMinNumberOfSamples = 4;
//// The maximum number of samples for sampling the height field profile
//int PomMaxNumberOfSamples = 20;
//// Blurring factor for the soft shadows computation
//texture HeightTexture : Height;
//sampler2D HeightSampler = sampler_state
//{
//  Texture = <HeightTexture>;
//  MinFilter = LINEAR;
//  MagFilter = LINEAR;
//  MipFilter = LINEAR;
//  AddressU = Wrap;
//  AddressV = Wrap;
//};
//// The size of the height texture in pixels (width, height).
//uniform float2 HeightTextureSize = float2(256, 256);


//--------------------------------------------------------
// Fog
//--------------------------------------------------------


// Color of fog (RGBA). If alpha is 0, fog is disabled.
float4 FogColor : FOGCOLOR;

// Combined fog parameters.
float4 FogParameters : FOGPARAMETERS;  // (Start, End, Density, HeightFalloff)
#define FogStart FogParameters.x
#define FogEnd FogParameters.y
#define FogDensity FogParameters.z
#define FogHeightFalloff FogParameters.w



//-----------------------------------------------------------------------------
// Input, output
//-----------------------------------------------------------------------------

struct VSInput
{
  float4 Position : POSITION0;

  float4 Color : COLOR0;


  float2 TexCoord : TEXCOORD0;

  float3 Normal : NORMAL0;

  float3 Tangent : TANGENT0;

  float3 Binormal : BINORMAL0;



  float3 MorphPosition0 : POSITION1;
  float3 MorphNormal0 : NORMAL1;
  float3 MorphPosition1 : POSITION2;
  float3 MorphNormal1 : NORMAL2;
  float3 MorphPosition2: POSITION3;
  float3 MorphNormal2: NORMAL3;
  float3 MorphPosition3 : POSITION4;
  float3 MorphNormal3: NORMAL4;
  float3 MorphPosition4 : POSITION5;
  float3 MorphNormal4 : NORMAL5;


  uint4 BoneIndices : BLENDINDICES0;
  float4 BoneWeights : BLENDWEIGHT0;

};


struct VSOutput
{

  float4 Diffuse : COLOR;


  float2 TexCoord : TEXCOORD0;

  float4 PositionWorldAndFog : TEXCOORD1;  // W contains 1- Fog Intensity.
  float3 Normal : TEXCOORD2;

  float3 Tangent : TEXCOORD3;
  float3 Binormal : TEXCOORD4;


  float4 PositionProj : TEXCOORD5;
  float Depth : TEXCOORD6;

  //float2 ParallaxOffsetTangent : TEXCOORD7; // Parallax offset vector in tangent space.
  float4 Position : SV_Position;
};


struct PSInput
{

  float4 Diffuse : COLOR;


  float2 TexCoord : TEXCOORD0;

  float4 PositionWorldAndFog : TEXCOORD1;  // W contains 1- Fog Intensity.
  float3 Normal : TEXCOORD2;

  float3 Tangent : TEXCOORD3;
  float3 Binormal : TEXCOORD4;


  float4 PositionProj : TEXCOORD5;
  float Depth : TEXCOORD6;

  //float2 ParallaxOffsetTangent : TEXCOORD7; // Parallax offset vector in tangent space.
};


//-----------------------------------------------------------------------------
// Shaders
//-----------------------------------------------------------------------------

VSOutput VS(VSInput input, float4x4 world)
{
  float4 position = input.Position;
  float3 normal = input.Normal;

  float3 tangent = input.Tangent;

  float3 binormal = input.Binormal;


  

  // ----- Apply morph targets.
  position.xyz += MorphWeight0 * input.MorphPosition0;
  position.xyz += MorphWeight1 * input.MorphPosition1;
  position.xyz += MorphWeight2 * input.MorphPosition2;
  position.xyz += MorphWeight3 * input.MorphPosition3;
  position.xyz += MorphWeight4 * input.MorphPosition4;
  
  normal += MorphWeight0 * input.MorphNormal0;
  normal += MorphWeight1 * input.MorphNormal1;
  normal += MorphWeight2 * input.MorphNormal2;
  normal += MorphWeight3 * input.MorphNormal3;
  normal += MorphWeight4 * input.MorphNormal4;
  normal = normalize(normal);
  

  // Orthonormalize the neutral tangent against the new normal. (Subtract the
  // collinear elements of the new normal from the neutral tangent and normalize.)
  tangent = tangent - dot(tangent, normal) * normal;
  //tangent = normalize(tangent); Tangent is normalized in pixel shader.


  

  // ----- Apply skinning matrix.
  float4x3 skinningMatrix = (float4x3)0;
  skinningMatrix += Bones[input.BoneIndices.x] * input.BoneWeights.x;
  skinningMatrix += Bones[input.BoneIndices.y] * input.BoneWeights.y;
  skinningMatrix += Bones[input.BoneIndices.z] * input.BoneWeights.z;
  skinningMatrix += Bones[input.BoneIndices.w] * input.BoneWeights.w;
  position.xyz = mul(position, skinningMatrix);
  normal = mul(normal, (float3x3)skinningMatrix);

  tangent = mul(tangent, (float3x3)skinningMatrix);

  binormal = mul(binormal, (float3x3)skinningMatrix);



  
  // ----- Apply world, view, projection transformation.
  float4 positionWorld = mul(position, world);
  float4 positionView = mul(positionWorld, View);
  float4 positionProj = mul(positionView, Projection);

  float normalizedDepth = -positionView.z / CameraFar;

  float3 normalWorld = mul(normal, (float3x3)world);

  float3 tangentWorld = mul(tangent, (float3x3)world);

  float3 binormalWorld = mul(binormal, (float3x3)world);
#else
  // Derive binormal from normal and tangent.
  float3 binormalWorld = cross(normalWorld, tangentWorld);
  //binormalWorld = normalize(binormalWorld); Binormal is normalized in pixel shader.


  
  // Matrix that transforms column(!) vectors from world space to tangent space.
  //float3x3 worldToTangentMatrix = float3x3(output.Tangent,
  //                                         output.Binormal,
  //                                         output.Normal);
  
  //float3 cameraToPositionTangent = mul(worldToTangentMatrix, positionWorld - CameraPosition);
  //output.ParallaxOffsetTangent = ComputeMaxParallaxOffset(cameraToPositionTangent, HeightScale);
  
  float fog = 0;

  // ----- Fog
  if (FogColor.a > 0)
  {
    float3 cameraToPositionVector = positionWorld.xyz - CameraPosition;
    float cameraDistance = length(cameraToPositionVector);
    
    // Smoothstep distance fog
    float smoothRamp = ComputeSmoothFogIntensity(cameraDistance, FogStart, FogEnd);
    
    // Exponential height-based fog
    float opticalLength = GetOpticalLengthInHeightFog(cameraDistance, FogDensity, cameraToPositionVector, FogHeightFalloff);
    float exponentialFog = ComputeExponentialFogIntensity(opticalLength, 1);  // fogDensity is included in opticalLength!
    
    // We use this fog parameter to scale the alpha in the pixel shader. The correct
    // way would be to apply a fog color, but scaling the alpha looks ok and works
    // with non-uniform/view-dependent fog colors.
    fog = smoothRamp * exponentialFog * FogColor.a;
  }

  
  // ----- Output
  VSOutput output = (VSOutput)0;
  output.Position = positionProj;

  output.Diffuse.rgb = input.Color.rgb * DiffuseColor;

  output.Diffuse.a = input.Color.a * abs(Alpha);
#else
  // Can't do abs() in XNA because of FX compiler bug.
  output.Diffuse.a = input.Color.a * Alpha;



  output.TexCoord = input.TexCoord;

  output.PositionWorldAndFog = float4(positionWorld.xyz, (1 - fog));
  output.Normal =  normalWorld;

  output.Tangent = tangentWorld;
  output.Binormal = binormalWorld;


  output.PositionProj = positionProj;
  output.Depth = normalizedDepth;

  return output;
}


VSOutput VSNoInstancing(VSInput input)
{
  return VS(input, World);
}


VSOutput VSInstancing(VSInput input,
                      float4 worldColumn0 : BLENDWEIGHT0,
                      float4 worldColumn1 : BLENDWEIGHT1,
                      float4 worldColumn2 : BLENDWEIGHT2)
{
  float4x4 worldTransposed =
  {
    worldColumn0,
    worldColumn1,
    worldColumn2,
    float4(0, 0, 0, 1)
  };
  float4x4 world = transpose(worldTransposed);
  
  return VS(input, world);
}


float4 PS(PSInput input) : COLOR
{

  // Get the screen space texture coordinate for this position.
  float2 texCoordScreen = ProjectionToScreen(input.PositionProj, ViewportSize);
  float4 gBuffer0Sample = tex2Dlod(GBuffer0Sampler, float4(texCoordScreen, 0, 0));
  clip(GetGBufferDepth(gBuffer0Sample) - input.Depth);

  
  // Material properties.
  float3 diffuseColor = float3(1, 1, 1);
  float3 specularColor = SpecularColor;
  float3 emissiveColor = float3(0, 0, 0);
  float alpha = 1;
  

  diffuseColor = input.Diffuse.rgb;
  alpha = input.Diffuse.a;
#else
  diffuseColor = DiffuseColor;

  alpha = abs(Alpha);
#else
  // Can't do abs() in XNA because of FX compiler bug.
  alpha = Alpha;


  
  // Diffuse map: diffuse color + opacity (alpha)
  float4 diffuseMap = float4(1, 1, 1, 1);

  diffuseMap = tex2D(DiffuseSampler, input.TexCoord);

  // Diffuse color uses premultiplied alpha.
  // Undo premultiplication.
  diffuseMap.rgb /= diffuseMap.a;

  
  // Convert color from sRGB to linear space.
  diffuseMap.rgb = FromGamma(diffuseMap.rgb);


  diffuseColor *= diffuseMap.rgb;


  alpha *= diffuseMap.a;


  clip(alpha - 0.001f);

  // Specular map: non-premultiplied specular color + emissive
  float4 specularMap = float4(1, 1, 1, 0);

  specularMap = tex2D(SpecularSampler, input.TexCoord);
  // Convert color from sRGB to linear space.
  specularMap.rgb = FromGamma(specularMap.rgb);


  specularColor *= specularColor.rgb;



  emissiveColor = EmissiveColor;


  emissiveColor *= diffuseMap.rgb;
  emissiveColor *= specularMap.a;

  
  // Normalize tangent space vectors.
  float3 normal = normalize(input.Normal);

  float3 tangent = normalize(input.Tangent);
  float3 binormal = normalize(input.Binormal);
  
  // Normals maps are encoded using DXT5nm.
  float3 normalMapSample = GetNormalDxt5nm(NormalSampler, input.TexCoord); // optional: multiply with "Bumpiness" factor.
  normal = normal * normalMapSample.z + tangent * normalMapSample.x - binormal * normalMapSample.y;

  
  float3 position = input.PositionWorldAndFog.xyz;
  float3 cameraToPositionVector = position - CameraPosition;
  float cameraDistance = length(cameraToPositionVector);
  float3 viewDirection = cameraToPositionVector / cameraDistance;
  
  float3 diffuseLightAccumulated = 0;
  float3 specularLightAccumulated = 0;
  
  int i;
  // ----- Ambient light

  [unroll]
  for (i = 0; i < AMBIENT_LIGHT_COUNT; i++)
    diffuseLightAccumulated += ComputeAmbientLight(AmbientLight[i], AmbientLightAttenuation[i], AmbientLightUp[i], normal);

  
  // ----- Directional lights

  [unroll]
  for (i = 0; i < DIRECTIONAL_LIGHT_COUNT; i++)
  {
    float3 lightDiffuse, lightSpecular;
    ComputeDirectionalLight(DirectionalLightDiffuse[i],
                            DirectionalLightSpecular[i],
                            DirectionalLightDirection[i],
                            viewDirection,
                            normal,
                            SpecularPower,
                            lightDiffuse,
                            lightSpecular);
    
    // Optional texture for the first directional light.

    if (i == 0)
    {
      float4 lightTexCoord = mul(float4(position, 1), DirectionalLightTextureMatrix0);
      float3 textureColor = tex2Dproj(DirectionalLightTexture0Sampler, lightTexCoord);
      textureColor = FromGamma(textureColor);
      
      lightDiffuse *= textureColor;
      lightSpecular *= textureColor;
    }


    if (i == 0)
    {
      // Compute the shadow cascade index and the texture coords.
      int cascade;
      float3 shadowTexCoord;

      float planarCameraDistance = -dot(cameraToPositionVector, View._m02_m12_m22); // dot with "forward"
      ComputeCsmCascadeFast(float4(position, 1), planarCameraDistance, DirectionalLightShadowCascadeDistances, DirectionalLightShadowViewProjections, cascade, shadowTexCoord);
  #else
      ComputeCsmCascadeBest(float4(position, 1), DirectionalLightShadowViewProjections, cascade, shadowTexCoord);

      
      float shadow = DirectionalLightShadowFog;
      if (IsInRange(shadowTexCoord, 0, 1))
      {
        // Apply normal offset (using geometry normal, not perturbed normal).
        float3 offsetPosition = ApplyNormalOffset(position, normal, DirectionalLightDirection[0], DirectionalLightShadowNormalOffset[cascade]);
        shadowTexCoord.xy = GetShadowTexCoord(float4(offsetPosition, 1), DirectionalLightShadowViewProjections[cascade]).xy;
        
        // Transform the texture coords to valid texture atlas coords.
        float2 atlasTexCoord = ConvertToTextureAtlas(shadowTexCoord.xy, cascade, DirectionalLightShadowNumberOfCascades);
        
        // Shadow map bounds (left, top, right, bottom) inside texture atlas.
        float4 shadowMapBounds = GetShadowMapBounds(cascade, DirectionalLightShadowNumberOfCascades, DirectionalLightShadowMapSize);
        
        // Since this shadow uses an orthographic projection, the pixel depth
        // is the z value of the shadow projection space.
        // Apply bias against surface acne.
        float ourDepth = shadowTexCoord.z + DirectionalLightShadowDepthBias[cascade];
        
        // Compute the shadow factor (0 = no shadow, 1 = shadow).

        shadow = GetShadow(ourDepth, atlasTexCoord, DirectionalLightShadowMapSampler, DirectionalLightShadowMapSize, shadowMapBounds);
  #else
        shadow = GetShadowPcfJitteredWorld(
          position,
          ourDepth,
          atlasTexCoord,
          DirectionalLightShadowMapSampler,
          DirectionalLightShadowMapSize,
          JitterMapSampler,
          DirectionalLightShadowFilterRadius,
          DirectionalLightShadowJitterResolution,
          shadowMapBounds);

  
        // Fade out the shadow in the distance.
        shadow = ApplyShadowFog(
          CASCADE_SELECTION_BEST,
          cascade >= DirectionalLightShadowNumberOfCascades - 1,
          shadow,
          shadowTexCoord,
          cameraDistance,
          DirectionalLightShadowMaxDistance,
          DirectionalLightShadowFadeOutRange,
          DirectionalLightShadowFog);
      }
      
      // The shadow mask stores the inverse.
      float shadowTerm = 1 - shadow;
      lightDiffuse *= shadowTerm.xxx;
      lightSpecular *= shadowTerm.xxx;
    }

    
    diffuseLightAccumulated += lightDiffuse;
    specularLightAccumulated += lightSpecular;
  }

  
  // ----- Point lights

  [unroll]
  for (i = 0; i < POINT_LIGHT_COUNT; i++)
  {
    float3 lightDirection = position - PointLightPosition[i];
    float lightDistance = length(lightDirection);
    lightDirection = lightDirection / lightDistance;
    
    float3 lightDiffuse, lightSpecular;
    ComputePointLight(PointLightDiffuse[i],
                      PointLightSpecular[i],
                      PointLightRange[i],
                      PointLightAttenuation[i],
                      lightDirection,
                      lightDistance,
                      viewDirection,
                      normal,
                      SpecularPower,
                      lightDiffuse,
                      lightSpecular);
    
    // Optional texture for the first directional light.

    if (i == 0)
    {
      float3 textureColor = texCUBE(PointLightTexture0Sampler, mul(lightDirection, PointLightTextureMatrix0));
      textureColor = FromGamma(textureColor);
      
      lightDiffuse *= textureColor;
      lightSpecular *= textureColor;
    }

    
    diffuseLightAccumulated += lightDiffuse;
    specularLightAccumulated += lightSpecular;
  }

  
  // ----- Spotlights

  [unroll]
  for (i = 0; i < SPOTLIGHT_COUNT; i++)
  {
    float3 lightDirection = position - SpotlightPosition[i];
    float lightDistance = length(lightDirection);
    lightDirection = lightDirection / lightDistance;
    
    float3 lightDiffuse, lightSpecular;
    ComputeSpotlight(SpotlightDiffuse[i], SpotlightSpecular[i],
                     SpotlightRange[i], SpotlightAttenuation[i],
                     SpotlightFalloffAngle[i], SpotlightCutoffAngle[i],
                     SpotlightDirection[i],
                     lightDirection, lightDistance,
                     viewDirection, normal, SpecularPower,
                     lightDiffuse, lightSpecular);
    
    // Optional texture for the first directional light.

    if (i == 0)
    {
      float4 lightTexCoord = mul(float4(position, 1), SpotlightTextureMatrix0);
      float3 textureColor = tex2Dproj(SpotlightTexture0Sampler, lightTexCoord);
      textureColor = FromGamma(textureColor);
      
      lightDiffuse *= textureColor;
      lightSpecular *= textureColor;
    }

    
    diffuseLightAccumulated += lightDiffuse;
    specularLightAccumulated += lightSpecular;
  }

  
  // ----- Projector light

  {
    float3 lightDirection = position - ProjectorLightPosition;
    float lightDistance = length(lightDirection);
    lightDirection = lightDirection / lightDistance;
    
    float3 lightDiffuse, lightSpecular;
    ComputeProjectorLight(ProjectorLightDiffuse, ProjectorLightSpecular,
                          ProjectorLightTextureSampler, ProjectorLightTextureMatrix,
                          ProjectorLightRange, ProjectorLightAttenuation,
                          lightDirection, lightDistance,
                          viewDirection, position, normal,
                          SpecularPower,
                          lightDiffuse, lightSpecular);
    
    diffuseLightAccumulated += lightDiffuse;
    specularLightAccumulated += lightSpecular;
  }

  
  // ----- Light Probe (Environment Map)

  // Diffuse:
  // Currently we use a hardcoded mipmap level for the diffuse texture lookup.
  // In DirectX 11 we could compute the mipmap level and choose the 1x1 or 2x2 mip level.
  const float maxMipLevel = 8;
  
  float4 environmentMapDiffuse = texCUBElod(
    EnvironmentSampler,
    float4(mul(normal, (float3x3)EnvironmentMapMatrix), maxMipLevel));
  
  diffuseLightAccumulated += EnvironmentMapDiffuse * FromGamma(DecodeRgbm(environmentMapDiffuse, EnvironmentMapRgbmMax));
  
  // Specular:
  // Select mipmap level for specular reflections based on surface roughness (SpecularPower).
  // Note: The term log2(size * sqrt(3)) can be precomputed.
  float mipLevel = max(0, log2(EnvironmentMapSize * sqrt(3)) - 0.5 * log2(SpecularPower + 1));
  
  // DirectX ps_4_1: Clamp mipLevel.
  //mipLevel = max(mipLevel, EnvironmentMap.CalculateLevelOfDetail(EnvironmentSampler, viewDirectionReflected));
  
  float3 viewDirectionReflected = reflect(viewDirection, normal);
  
  float4 environmentMapSpecular = texCUBElod(
    EnvironmentSampler,
    float4(mul(viewDirectionReflected, (float3x3)EnvironmentMapMatrix), mipLevel));
  
  specularLightAccumulated += EnvironmentMapSpecular * FromGamma(DecodeRgbm(environmentMapSpecular, EnvironmentMapRgbmMax));

  
  // Combine material colors with lights.
  float3 result = diffuseColor * diffuseLightAccumulated;
  result += specularColor * specularLightAccumulated;

  result += emissiveColor;

  
  // ----- Parallax occlusion mapping
  //if (HeightScale > 0)
  //{
  //  float mipLevel = ComputeMipLevel(input.TexCoord, HeightTextureSize);
  
  //  input.TexCoord.xy = (input.TexCoord.xy, HeightSampler,
  //    normal, -viewDirection, input.ParallaxOffsetTangent, mipLevel,
  //    PomLodThreshold, PomMinNumberOfSamples, PomMaxNumberOfSamples);
  //}
  

  // Fog needs to be applied to the alpha value.
  float fog = input.PositionWorldAndFog.w;
  alpha *= fog;

  

  result = ToGamma(result);

  
  // Premultiply alpha.
  result *= alpha;
  
  return float4(result, alpha * BlendMode);
}


//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------


#define VSTARGET vs_3_0
#define PSTARGET ps_3_0
#else
#define VSTARGET vs_4_0
#define PSTARGET ps_4_0



#define SUPPORTS_INSTANCING 1



technique Default

< string InstancingTechnique = "DefaultInstancing"; >

{

  pass BackSides
  {
    CullMode = CW;
    VertexShader = compile VSTARGET VSNoInstancing();
    PixelShader = compile PSTARGET PS();
  }

  
  pass FrontSides
  {
    CullMode = CCW;
    VertexShader = compile VSTARGET VSNoInstancing();
    PixelShader = compile PSTARGET PS();
  }
}



technique DefaultInstancing
{

  pass BackSides
  {
    CullMode = CW;
    VertexShader = compile VSTARGET VSInstancing();
    PixelShader = compile PSTARGET PS();
  }

  
  pass FrontSides
  {
    CullMode = CCW;
    VertexShader = compile VSTARGET VSInstancing();
    PixelShader = compile PSTARGET PS();
  }
}

