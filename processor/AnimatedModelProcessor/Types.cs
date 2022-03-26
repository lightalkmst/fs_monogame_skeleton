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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedModelPipeline {
  #region Reader
  // Aether.Extras - VertexIndicesWeightsPositionNormal
  [StructLayout (LayoutKind.Explicit)]
  public struct VertexIndicesWeightsPositionNormal : IVertexType {
    [FieldOffset (0)] public byte BlendIndex0;
    [FieldOffset (1)] public byte BlendIndex1;
    [FieldOffset (2)] public byte BlendIndex2;
    [FieldOffset (3)] public byte BlendIndex3;
    [FieldOffset (4)] public Vector4 BlendWeights;
    [FieldOffset (20)] public Vector3 Position;
    [FieldOffset (32)] public Vector3 Normal;


    #region IVertexType Members
    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration (
            new VertexElement []
            {
                    new VertexElement( 0, VertexElementFormat.Byte4, VertexElementUsage.BlendIndices, 0),
                    new VertexElement( 4, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
                    new VertexElement(20, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            });

    VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    #endregion


    public VertexIndicesWeightsPositionNormal (Vector3 position, Vector3 normal, Vector4 blendWeights, byte blendIndex0, byte blendIndex1, byte blendIndex2, byte blendIndex3) {
      this.BlendIndex0 = blendIndex0;
      this.BlendIndex1 = blendIndex1;
      this.BlendIndex2 = blendIndex2;
      this.BlendIndex3 = blendIndex3;
      this.BlendWeights = blendWeights;
      this.Position = position;
      this.Normal = normal;
    }

    public override string ToString () {
      return string.Format ("{{Position:{0} Normal:{1} BlendWeights:{2} BlendIndices:{3},{4},{5},{6} }}",
          new object [] { Position, Normal, BlendWeights, BlendIndex0, BlendIndex1, BlendIndex2, BlendIndex3 });
    }
  }

  // Aether.Extras - CpuAnimatedVertexBuffer
  public class CpuAnimatedVertexBuffer : DynamicVertexBuffer {
    private VertexIndicesWeightsPositionNormal [] cpuVertices;
    private VertexPositionNormalTexture [] gpuVertices;

    public CpuAnimatedVertexBuffer (GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount, BufferUsage bufferUsage) :
        base (graphicsDevice, vertexDeclaration, vertexCount, bufferUsage) {
    }

    internal void SetGpuVertices (VertexPositionNormalTexture [] vertices) {
      this.gpuVertices = vertices;
    }

    internal void SetCpuVertices (VertexIndicesWeightsPositionNormal [] vertices) {
      this.cpuVertices = vertices;
    }

    internal void UpdateVertices (Matrix [] boneTransforms, int startIndex, int elementCount) {
      Matrix transformSum = Matrix.Identity;

      //return;

      // skin all of the vertices
      for (int i = startIndex; i < (startIndex + elementCount); i++) {
        int b0 = cpuVertices [i].BlendIndex0;
        int b1 = cpuVertices [i].BlendIndex1;
        int b2 = cpuVertices [i].BlendIndex2;
        int b3 = cpuVertices [i].BlendIndex3;

        float w1 = cpuVertices [i].BlendWeights.X;
        float w2 = cpuVertices [i].BlendWeights.Y;
        float w3 = cpuVertices [i].BlendWeights.Z;
        float w4 = cpuVertices [i].BlendWeights.W;

        Matrix m1 = boneTransforms [b0];
        Matrix m2 = boneTransforms [b1];
        Matrix m3 = boneTransforms [b2];
        Matrix m4 = boneTransforms [b3];
        transformSum.M11 = (m1.M11 * w1) + (m2.M11 * w2) + (m3.M11 * w3) + (m4.M11 * w4);
        transformSum.M12 = (m1.M12 * w1) + (m2.M12 * w2) + (m3.M12 * w3) + (m4.M12 * w4);
        transformSum.M13 = (m1.M13 * w1) + (m2.M13 * w2) + (m3.M13 * w3) + (m4.M13 * w4);
        transformSum.M21 = (m1.M21 * w1) + (m2.M21 * w2) + (m3.M21 * w3) + (m4.M21 * w4);
        transformSum.M22 = (m1.M22 * w1) + (m2.M22 * w2) + (m3.M22 * w3) + (m4.M22 * w4);
        transformSum.M23 = (m1.M23 * w1) + (m2.M23 * w2) + (m3.M23 * w3) + (m4.M23 * w4);
        transformSum.M31 = (m1.M31 * w1) + (m2.M31 * w2) + (m3.M31 * w3) + (m4.M31 * w4);
        transformSum.M32 = (m1.M32 * w1) + (m2.M32 * w2) + (m3.M32 * w3) + (m4.M32 * w4);
        transformSum.M33 = (m1.M33 * w1) + (m2.M33 * w2) + (m3.M33 * w3) + (m4.M33 * w4);
        transformSum.M41 = (m1.M41 * w1) + (m2.M41 * w2) + (m3.M41 * w3) + (m4.M41 * w4);
        transformSum.M42 = (m1.M42 * w1) + (m2.M42 * w2) + (m3.M42 * w3) + (m4.M42 * w4);
        transformSum.M43 = (m1.M43 * w1) + (m2.M43 * w2) + (m3.M43 * w3) + (m4.M43 * w4);

        // Support the 4 Bone Influences - Position then Normal
        Vector3.Transform (ref cpuVertices [i].Position, ref transformSum, out gpuVertices [i].Position);
        Vector3.TransformNormal (ref cpuVertices [i].Normal, ref transformSum, out gpuVertices [i].Normal);
      }

      // put the vertices into our vertex buffer
      SetData (gpuVertices, 0, VertexCount, SetDataOptions.NoOverwrite);
    }
  }

  // Aether.Extras - Animations
  public class Animations {
    internal List<Matrix> _bindPose;
    internal List<Matrix> _invBindPose; // TODO: convert those from List<T> to simple T[] arrays.
    internal List<int> _skeletonHierarchy;
    internal Dictionary<string, int> _boneMap;

    private Matrix [] _boneTransforms;
    private Matrix [] _worldTransforms;
    private Matrix [] _animationTransforms;

    private int _currentKeyframe;


    public Dictionary<string, Clip> Clips { get; private set; }
    public Clip CurrentClip { get; private set; }
    public TimeSpan CurrentTime { get; private set; }

    /// <summary>
    /// The current bone transform matrices, relative to their parent bones.
    /// </summary>
    public Matrix [] BoneTransforms { get { return _boneTransforms; } }

    /// <summary>
    /// The current bone transform matrices, in absolute format.
    /// </summary>
    public Matrix [] WorldTransforms { get { return _worldTransforms; } }

    /// <summary>
    /// The current bone transform matrices, relative to the animation bind pose.
    /// </summary>
    public Matrix [] AnimationTransforms { get { return _animationTransforms; } }


    internal Animations (List<Matrix> bindPose, List<Matrix> invBindPose, List<int> skeletonHierarchy, Dictionary<string, int> boneMap, Dictionary<string, Clip> clips) {
      _bindPose = bindPose;
      _invBindPose = invBindPose;
      _skeletonHierarchy = skeletonHierarchy;
      _boneMap = boneMap;
      Clips = clips;

      // initialize
      _boneTransforms = new Matrix [_bindPose.Count];
      _worldTransforms = new Matrix [_bindPose.Count];
      _animationTransforms = new Matrix [_bindPose.Count];
    }

    public void SetClip (string clipName) {
      var clip = Clips ["Base Stack"];
      SetClip (clip);
    }

    public void SetClip (Clip clip) {
      if (clip == null)
        throw new ArgumentNullException ("clip");

      CurrentClip = clip;
      CurrentTime = TimeSpan.Zero;
      _currentKeyframe = 0;

      // Initialize bone transforms to the bind pose.
      _bindPose.CopyTo (_boneTransforms, 0);
    }

    public int GetBoneIndex (string boneName) {
      int boneIndex;
      if (!_boneMap.TryGetValue (boneName, out boneIndex))
        boneIndex = -1;
      return boneIndex;
    }

    public void Update (TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform) {
      UpdateBoneTransforms (time, relativeToCurrentTime);
      UpdateWorldTransforms (rootTransform);
      UpdateAnimationTransforms ();
    }

    public void UpdateBoneTransforms (TimeSpan time, bool relativeToCurrentTime) {
      // Update the animation position.
      if (relativeToCurrentTime) {
        time += CurrentTime;

        // If we reached the end, loop back to the start.
        while (time >= CurrentClip.Duration)
          time -= CurrentClip.Duration;
      }

      if (time < TimeSpan.Zero)
        throw new ArgumentOutOfRangeException ("time out of range");
      if (time > CurrentClip.Duration)
        //throw new ArgumentOutOfRangeException("time out of range");
        time = CurrentClip.Duration;

      // If the position moved backwards, reset the keyframe index.
      if (time < CurrentTime) {
        _currentKeyframe = 0;
        _bindPose.CopyTo (_boneTransforms, 0);
      }

      CurrentTime = time;

      // Read keyframe matrices.
      IList<Keyframe> keyframes = CurrentClip.Keyframes;

      while (_currentKeyframe < keyframes.Count) {
        Keyframe keyframe = keyframes [_currentKeyframe];

        // Stop when we've read up to the current time position.
        if (keyframe.Time > CurrentTime)
          break;

        // Use this keyframe.
        _boneTransforms [keyframe.Bone] = keyframe.Transform;

        _currentKeyframe++;
      }
    }

    public void UpdateWorldTransforms (Matrix rootTransform) {
      // Root bone.
      Matrix.Multiply (ref _boneTransforms [0], ref rootTransform, out _worldTransforms [0]);

      // Child bones.
      for (int bone = 1; bone < _worldTransforms.Length; bone++) {
        int parentBone = _skeletonHierarchy [bone];

        Matrix.Multiply (ref _boneTransforms [bone], ref _worldTransforms [parentBone], out _worldTransforms [bone]);
      }
    }

    public void UpdateAnimationTransforms () {
      for (int bone = 0; bone < _animationTransforms.Length; bone++) {
        Matrix _tmpInvBindPose = _invBindPose [bone]; //can not pass it as 'ref'
        Matrix.Multiply (ref _tmpInvBindPose, ref _worldTransforms [bone], out _animationTransforms [bone]);
      }
    }
  }

  // Aether.Extras - Clip
  public class Clip {
    public TimeSpan Duration { get; internal set; }
    public Keyframe [] Keyframes { get; private set; }

    internal Clip (TimeSpan duration, Keyframe [] keyframes) {
      Duration = duration;
      Keyframes = keyframes;
    }
  }

  // Aether.Extras - Keyframe
  public struct Keyframe {
    internal int _bone;
    internal TimeSpan _time;
    internal Matrix _transform;

    public int Bone {
      get { return _bone; }
      internal set { _bone = value; }
    }

    public TimeSpan Time {
      get { return _time; }
      internal set { _time = value; }
    }

    public Matrix Transform {
      get { return _transform; }
      internal set { _transform = value; }
    }

    public Keyframe (int bone, TimeSpan time, Matrix transform) {
      this._bone = bone;
      this._time = time;
      this._transform = transform;
    }

    public override string ToString () {
      return string.Format ("{{Time:{0} Bone:{1}}}",
          new object [] { Time, Bone });
    }
  }
  #endregion


  #region Content
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

    public ClipContent () {
      Duration = new TimeSpan ();
      Keyframes = new KeyframeContent [0];
    }

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

    public AnimationsContent () {
      BindPose = new List<Matrix> ();
      InvBindPose = new List<Matrix> ();
      SkeletonHierarchy = new List<int> ();
      BoneNames = new List<String> ();
      Clips = new Dictionary<string, ClipContent> ();
    }

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
  #endregion
}
