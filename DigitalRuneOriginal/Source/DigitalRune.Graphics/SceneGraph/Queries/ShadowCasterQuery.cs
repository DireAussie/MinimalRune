﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using MinimalRune.Mathematics;
using MinimalRune.Mathematics.Algebra;


namespace MinimalRune.Graphics.SceneGraph
{
  /// <summary>
  /// Returns the shadow casting nodes that touch a specific reference scene node.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A <see cref="ShadowCasterQuery"/> can be executed against a scene by calling 
  /// <see cref="IScene.Query{T}"/>. The query can be used to get all shadow casters in a scene that
  /// touch a certain reference node (usually the light node).
  /// </para>
  /// <para>
  /// A scene node casts a shadow if <see cref="SceneNode.CastsShadows"/> is set. If the 
  /// reference node of the query or the <see cref="RenderContext.ReferenceNode"/> in the 
  /// <see cref="RenderContext"/> is a <see cref="LightNode"/> with a 
  /// <see cref="DirectionalLight"/>, then the query also checks the 
  /// <see cref="SceneNode.IsShadowCasterCulled"/> flag.
  /// </para>
  /// <para>
  /// <para>
  /// <strong>Terrain nodes:</strong><br/>
  /// <see cref="TerrainNode"/> are special. They only cast directional light shadows and are
  /// ignored for other light types.
  /// </para>
  /// </para>
  /// </remarks>
  public class ShadowCasterQuery : ISceneQuery
  {
    

    

    private bool _checkShadowCusterCulling;
    private Vector3 _cameraPosition;
    private float _lodBiasOverYScale;



    

    

    /// <inheritdoc/>
    public SceneNode ReferenceNode { get; private set; }


    /// <summary>
    /// Gets the scene nodes that cast shadows.
    /// </summary>
    /// <value>The scene nodes that cast shadows.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Performance")]
    public List<SceneNode> ShadowCasters { get; private set; }



    

    

    /// <summary>
    /// Initializes a new instance of the <see cref="ShadowCasterQuery"/> class.
    /// </summary>
    public ShadowCasterQuery()
    {
      ShadowCasters = new List<SceneNode>();
    }



    

    

    /// <inheritdoc/>
    public void Reset()
    {
      ReferenceNode = null;
      ShadowCasters.Clear();
    }


    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
    public void Set(SceneNode referenceNode, IList<SceneNode> nodes, RenderContext context)
    {
      ReferenceNode = referenceNode;
      ShadowCasters.Clear();

      // Check if the light is the directional light. If yes, we will check the 
      // IsShadowCasterCulled flag.
      var lightNode = referenceNode as LightNode;
      if (lightNode == null)
        lightNode = context.ReferenceNode as LightNode;
      _checkShadowCusterCulling = (lightNode != null) && (lightNode.Light is DirectionalLight);

      int numberOfNodes = nodes.Count;


      for (int i = 0; i < numberOfNodes; i++)
        Debug.Assert(nodes[i].ActualIsEnabled, "Scene query contains disabled nodes.");


      if (context.LodCameraNode == null)
      {
        // ----- No LOD
        for (int i = 0; i < numberOfNodes; i++)
        {
          var node = nodes[i];
          if (IsShadowCaster(node))
            ShadowCasters.Add(node);
        }
      }
      else
      {
        // ----- LOD
        // Get values for LOD computations.
        var cameraNode = context.LodCameraNode;
        _cameraPosition = cameraNode.PoseLocal.Position;
        _lodBiasOverYScale = 1 / Math.Abs(cameraNode.Camera.Projection.ToMatrix().M11)
                             * cameraNode.LodBias * context.LodBias;

        // Add nodes and evaluate LOD groups.
        for (int i = 0; i < numberOfNodes; i++)
          AddNodeWithLod(nodes[i], context);
      }
    }


    private void AddNodeWithLod(SceneNode node, RenderContext context)
    {
      if (!IsShadowCaster(node))
        return;

      bool hasMaxDistance = Numeric.IsPositiveFinite(node.MaxDistance);
      var lodGroupNode = node as LodGroupNode;
      bool isLodGroupNode = (lodGroupNode != null);

      float distance = 0;
      if (hasMaxDistance || isLodGroupNode)
      {
        Debug.Assert(
          node.ScaleWorld.X > 0 && node.ScaleWorld.Y > 0 && node.ScaleWorld.Z > 0,
          "Assuming that all scale factors are positive.");

        // Determine view-normalized distance between scene node and camera node.
        distance = (node.PoseWorld.Position - _cameraPosition).Length;
        distance *= _lodBiasOverYScale;
        distance /= node.ScaleWorld.LargestComponent;
      }

      // Distance Culling: Only handle nodes that are within MaxDistance.
      if (hasMaxDistance && distance >= node.MaxDistance)
        return;   // Ignore scene node.

      if (isLodGroupNode)
      {
        // Evaluate LOD group.
        var lodSelection = lodGroupNode.SelectLod(context, distance);
        AddSubtree(lodSelection.Current, context);
      }
      else
      {
        ShadowCasters.Add(node);
      }
    }


    private bool IsShadowCaster(SceneNode node)
    {
      if (_checkShadowCusterCulling)
      {
        // Flag CastsShadow must be set.
        // Flag IsShadowCasterCulled must NOT be set.
        return node.GetFlags(SceneNodeFlags.CastsShadows | SceneNodeFlags.IsShadowCasterCulled)
               == SceneNodeFlags.CastsShadows;
      }
      else
      {

        return node.GetFlag(SceneNodeFlags.CastsShadows)
               && !(node is TerrainNode);      // Terrain node is only rendered into directional light shadow.
#else
        return node.GetFlag(SceneNodeFlags.CastsShadows);

      }
    }


    private void AddSubtree(SceneNode node, RenderContext context)
    {
      if (node.IsEnabled)
      {
        AddNodeWithLod(node, context);
        if (node.Children != null)
          foreach (var childNode in node.Children)
            AddSubtree(childNode, context);
      }
    }

  }
}
