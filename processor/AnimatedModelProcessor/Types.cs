using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace AnimatedModelPipeline {
  // Aether.Extras - KeyframeContent
  public struct KeyframeContent {
    public int Bone;
    public TimeSpan Time;
    public Matrix Transform;

    public KeyframeContent (int bone, TimeSpan time, Matrix transform) {
      this.Bone = bone;
      this.Time = time;
      this.Transform = transform;
    }

    public override string ToString () {
      return string.Format ("{{Time:{0} Bone:{1}}}",
          new object [] { Time, Bone });
    }
  }

  // Aether.Extras - ClipContent
  public class ClipContent {
    public TimeSpan Duration { get; internal set; }
    public KeyframeContent [] Keyframes { get; private set; }

    internal ClipContent (TimeSpan duration, KeyframeContent [] keyframes) {
      Duration = duration;
      Keyframes = keyframes;
    }
  }

  // Aether.Extras - AnimationsContent
  public class AnimationsContent {
    public List<Matrix> BindPose { get; private set; }
    public List<Matrix> InvBindPose { get; private set; }
    public List<int> SkeletonHierarchy { get; private set; }
    public List<string> BoneNames { get; private set; }
    public Dictionary<string, ClipContent> Clips { get; private set; }


    internal AnimationsContent (List<Matrix> bindPose, List<Matrix> invBindPose, List<int> skeletonHierarchy, List<string> boneNames, Dictionary<string, ClipContent> clips) {
      BindPose = bindPose;
      InvBindPose = invBindPose;
      SkeletonHierarchy = skeletonHierarchy;
      BoneNames = boneNames;
      Clips = clips;
    }
  }

  // Aether.Extras - DynamicIndexBufferContent
  public class AnimatedIndexBufferContent : Collection<int> {
    protected internal Collection<int> Source { get; protected set; }

    public bool IsWriteOnly = false;

    public new int Count { get { return Source.Count; } }

    public AnimatedIndexBufferContent (Collection<int> source) {
      Source = source;
    }

    public new IEnumerator<int> GetEnumerator () {
      return Source.GetEnumerator ();
    }
  }

  // Aether.Extras - CpuAnimatedVertexBufferContent
  public class AnimatedVertexBufferContent : VertexBufferContent {
    protected internal VertexBufferContent Source { get; protected set; }

    public bool IsWriteOnly = false;

    public new VertexDeclarationContent VertexDeclaration { get { return Source.VertexDeclaration; } }

    public new byte [] VertexData { get { return Source.VertexData; } }

    public AnimatedVertexBufferContent (VertexBufferContent source) : base () {
      Source = source;
    }

    public AnimatedVertexBufferContent (VertexBufferContent source, int size) : base (size) {
      Source = source;
    }
  }

  // Aether.Extras - 
  public class AnimatedMeshPartContent {
    protected internal ModelMeshPartContent Source { get; protected set; }

    // Summary:
    //     Gets the offset, in bytes, from the first index of the of vertex buffer for this mesh part.
    public int VertexOffset { get; set; }

    // Summary:
    //     Gets the number of vertices used in this mesh part.
    public int NumVertices { get; set; }

    // Summary:
    //     Gets the location in the index buffer at which to start reading vertices.
    public int StartIndex { get; set; }

    // Summary:
    //     Gets the number of primitives to render for this mesh part.
    public int PrimitiveCount { get; set; }


    // Summary:
    //     Gets the vertex buffer associated with this mesh part.
    [ContentSerializerIgnore]
    public VertexBufferContent VertexBuffer { get; set; }

    // Summary:
    //     Gets the index buffer associated with this mesh part.
    [ContentSerializerIgnore]
    public Collection<int> IndexBuffer { get; set; }

    // Summary:
    //     Gets the material of this mesh part.
    [ContentSerializer (SharedResource = true)]
    public MaterialContent Material { get; set; }

    // Summary:
    //     Gets a user defined tag object.
    [ContentSerializer (SharedResource = true)]
    public object Tag { get; set; }


    public AnimatedMeshPartContent (ModelMeshPartContent source) {
      this.Source = source;
      this.VertexOffset = source.VertexOffset;
      this.NumVertices = source.NumVertices;
      this.StartIndex = source.StartIndex;
      this.PrimitiveCount = source.PrimitiveCount;
      this.VertexBuffer = source.VertexBuffer;
      this.IndexBuffer = source.IndexBuffer;
      this.Material = source.Material;
      this.Tag = Tag;
    }
  }

  // Aether.Extras - 
  public class AnimatedMeshContent {
    protected internal ModelMeshContent Source { get; protected set; }

    // Summary:
    //     Gets the mesh name.
    public string Name { get { return Source.Name; } }

    // Summary:
    //     Gets the parent bone.
    [ContentSerializerIgnore]
    public ModelBoneContent ParentBone { get { return Source.ParentBone; } }

    // Summary:
    //     Gets the bounding sphere for this mesh.
    public BoundingSphere BoundingSphere { get { return Source.BoundingSphere; } }

    // Summary:
    //     Gets the children mesh parts associated with this mesh.
    [ContentSerializerIgnore]
    public List<AnimatedMeshPartContent> MeshParts { get; private set; }

    // Summary:
    //     Gets a user defined tag object.
    [ContentSerializer (SharedResource = true)]
    public object Tag { get { return Source.Tag; } set { Source.Tag = value; } }


    public AnimatedMeshContent (ModelMeshContent source) {
      this.Source = source;

      //deep clone MeshParts
      MeshParts = new List<AnimatedMeshPartContent> (source.MeshParts.Count);
      foreach (var mesh in source.MeshParts)
        MeshParts.Add (new AnimatedMeshPartContent (mesh));
    }
  }

  // Aether.Extras - 
  public class AnimatedModelContent {
    [Flags]
    public enum BufferType : int {
      /// <summary>
      /// Use the default BufferReader
      /// </summary>
      Default = int.MinValue,

      /// <summary>
      /// Deserialize a Dynamic Buffer
      /// </summary> 
      Dynamic = 0,

      /// <summary>
      /// Deserialize a Dynamic Buffer with BufferUsage.WriteOnly
      /// </summary>
      DynamicWriteOnly = 0x01,
    }

    protected internal ModelContent Source { get; protected set; }
    public BufferType VertexBufferType = BufferType.Dynamic;
    public BufferType IndexBufferType = BufferType.Dynamic;

    // Summary:
    //     Gets the collection of bones that are referenced by this model.
    public ModelBoneContentCollection Bones { get { return Source.Bones; } }

    // Summary:
    //     Gets the collection of meshes that are associated with this model.
    [ContentSerializerIgnore]
    public List<AnimatedMeshContent> Meshes { get; private set; }

    // Summary:
    //     Gets the root bone of this model
    [ContentSerializerIgnore]
    public ModelBoneContent Root { get { return Source.Root; } }

    // Summary:
    //     Gets a user defined tag object.
    [ContentSerializer (SharedResource = true)]
    public object Tag { get { return Source.Tag; } set { Source.Tag = value; } }


    public AnimatedModelContent (Microsoft.Xna.Framework.Content.Pipeline.Processors.ModelContent content) {
      this.Source = content;

      //deep clone Meshes
      Meshes = new List<AnimatedMeshContent> (content.Meshes.Count);
      foreach (var mesh in content.Meshes)
        Meshes.Add (new AnimatedMeshContent (mesh));
    }
  }
}
