using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


namespace AnimatedModelPipeline {
  // Aether.Extras - ClipReader
  public class ClipReader : ContentTypeReader<Clip> {
    protected override Clip Read (ContentReader input, Clip existingInstance) {
      Console.WriteLine ("Clip.Read");

      Clip animationClip = existingInstance;

      if (existingInstance == null) {
        TimeSpan duration = ReadDuration (input);
        Keyframe [] keyframes = ReadKeyframes (input, null);
        animationClip = new Clip (duration, keyframes);
      }
      else {
        animationClip.Duration = ReadDuration (input);
        ReadKeyframes (input, animationClip.Keyframes);
      }

      return animationClip;
    }

    private TimeSpan ReadDuration (ContentReader input) {
      return new TimeSpan (input.ReadInt64 ());
    }

    private Keyframe [] ReadKeyframes (ContentReader input, Keyframe [] existingInstance) {
      Keyframe [] keyframes = existingInstance;

      int count = input.ReadInt32 ();
      if (keyframes == null)
        keyframes = new Keyframe [count];

      for (int i = 0; i < count; i++) {
        keyframes [i]._bone = input.ReadInt32 ();
        keyframes [i]._time = new TimeSpan (input.ReadInt64 ());
        keyframes [i]._transform.M11 = input.ReadSingle ();
        keyframes [i]._transform.M12 = input.ReadSingle ();
        keyframes [i]._transform.M13 = input.ReadSingle ();
        keyframes [i]._transform.M14 = 0;
        keyframes [i]._transform.M21 = input.ReadSingle ();
        keyframes [i]._transform.M22 = input.ReadSingle ();
        keyframes [i]._transform.M23 = input.ReadSingle ();
        keyframes [i]._transform.M24 = 0;
        keyframes [i]._transform.M31 = input.ReadSingle ();
        keyframes [i]._transform.M32 = input.ReadSingle ();
        keyframes [i]._transform.M33 = input.ReadSingle ();
        keyframes [i]._transform.M34 = 0;
        keyframes [i]._transform.M41 = input.ReadSingle ();
        keyframes [i]._transform.M42 = input.ReadSingle ();
        keyframes [i]._transform.M43 = input.ReadSingle ();
        keyframes [i]._transform.M44 = 1;
      }

      return keyframes;
    }

  }

  // Aether.Extras - AnimationsReader
  public class AnimationsReader : ContentTypeReader<Animations> {
    protected override Animations Read (ContentReader input, Animations existingInstance) {
      Console.WriteLine ("AnimationsReader.Read");

      Animations animations = existingInstance;

      if (existingInstance == null) {
        Dictionary<string, Clip> clips = ReadAnimationClips (input, null);
        List<Matrix> bindPose = ReadBindPose (input, null);
        List<Matrix> invBindPose = ReadInvBindPose (input, null);
        List<int> skeletonHierarchy = ReadSkeletonHierarchy (input, null);
        Dictionary<string, int> boneMap = ReadBoneMap (input, null);
        animations = new Animations (bindPose, invBindPose, skeletonHierarchy, boneMap, clips);
      }
      else {
        ReadAnimationClips (input, animations.Clips);
        ReadBindPose (input, animations._bindPose);
        ReadInvBindPose (input, animations._invBindPose);
        ReadSkeletonHierarchy (input, animations._skeletonHierarchy);
        ReadBoneMap (input, animations._boneMap);
      }

      return animations;
    }

    private Dictionary<string, Clip> ReadAnimationClips (ContentReader input, Dictionary<string, Clip> existingInstance) {
      Dictionary<string, Clip> animationClips = existingInstance;

      int count = input.ReadInt32 ();
      if (animationClips == null)
        animationClips = new Dictionary<string, Clip> (count);

      for (int i = 0; i < count; i++) {
        string key = input.ReadString ();
        Clip val = input.ReadObject<Clip> ();
        if (existingInstance == null)
          animationClips.Add (key, val);
        else
          animationClips [key] = val;
      }

      return animationClips;
    }

    private List<Matrix> ReadBindPose (ContentReader input, List<Matrix> existingInstance) {
      List<Matrix> bindPose = existingInstance;

      int count = input.ReadInt32 ();
      if (bindPose == null)
        bindPose = new List<Matrix> (count);

      for (int i = 0; i < count; i++) {
        Matrix val = input.ReadMatrix ();
        if (existingInstance == null)
          bindPose.Add (val);
        else
          bindPose [i] = val;
      }

      return bindPose;
    }

    private List<Matrix> ReadInvBindPose (ContentReader input, List<Matrix> existingInstance) {
      List<Matrix> invBindPose = existingInstance;

      int count = input.ReadInt32 ();
      if (invBindPose == null)
        invBindPose = new List<Matrix> (count);

      for (int i = 0; i < count; i++) {
        Matrix val = input.ReadMatrix ();
        if (existingInstance == null)
          invBindPose.Add (val);
        else
          invBindPose [i] = val;
      }

      return invBindPose;
    }

    private List<int> ReadSkeletonHierarchy (ContentReader input, List<int> existingInstance) {
      List<int> skeletonHierarchy = existingInstance;

      int count = input.ReadInt32 ();
      if (skeletonHierarchy == null)
        skeletonHierarchy = new List<int> (count);

      for (int i = 0; i < count; i++) {
        Int32 val = input.ReadInt32 ();
        if (existingInstance == null)
          skeletonHierarchy.Add (val);
        else
          skeletonHierarchy [i] = val;
      }

      return skeletonHierarchy;
    }

    private Dictionary<string, int> ReadBoneMap (ContentReader input, Dictionary<string, int> existingInstance) {
      Dictionary<string, int> boneMap = existingInstance;

      int count = input.ReadInt32 ();
      if (boneMap == null)
        boneMap = new Dictionary<string, int> (count);

      for (int boneIndex = 0; boneIndex < count; boneIndex++) {
        string key = input.ReadString ();
        if (existingInstance == null)
          boneMap.Add (key, boneIndex);
        else
          boneMap [key] = boneIndex;
      }

      return boneMap;
    }

  }

  // Aether.Extras - DynamicVertexBufferReader
  public class DynamicVertexBufferReader : ContentTypeReader<DynamicVertexBuffer> {
    protected override DynamicVertexBuffer Read (ContentReader input, DynamicVertexBuffer buffer) {
      Console.WriteLine ("DynamicVertexBuffer.Read");

      IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService) input.ContentManager.ServiceProvider.GetService (typeof (IGraphicsDeviceService));
      var device = graphicsDeviceService.GraphicsDevice;

      // read standard VertexBuffer
      var declaration = input.ReadRawObject<VertexDeclaration> ();
      var vertexCount = (int) input.ReadUInt32 ();
      int dataSize = vertexCount * declaration.VertexStride;
      byte [] data = new byte [dataSize];
      input.Read (data, 0, dataSize);

      // read extras
      bool IsWriteOnly = input.ReadBoolean ();

      if (buffer == null) {
        BufferUsage usage = (IsWriteOnly) ? BufferUsage.WriteOnly : BufferUsage.None;
        buffer = new DynamicVertexBuffer (device, declaration, vertexCount, usage);
      }
      buffer.SetData (data, 0, dataSize);

      return buffer;
    }
  }

  // Aether.Extras - DynamicIndexBufferReader
  public class DynamicIndexBufferReader : ContentTypeReader<DynamicIndexBuffer> {
    protected override DynamicIndexBuffer Read (ContentReader input, DynamicIndexBuffer buffer) {
      Console.WriteLine ("DynamicIndexBuffer.Read");

      IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService) input.ContentManager.ServiceProvider.GetService (typeof (IGraphicsDeviceService));
      var device = graphicsDeviceService.GraphicsDevice;

      // read IndexBuffer
      var is16Bit = input.ReadBoolean ();
      var dataSize = (int) input.ReadUInt32 ();
      byte [] data = new byte [dataSize];
      input.Read (data, 0, dataSize);

      // read IsWriteOnly
      bool IsWriteOnly = input.ReadBoolean (); // TODO: i think i comment this out - omar


      if (buffer == null) {
        var elementSize = (is16Bit) ? IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits;
        var stride = (is16Bit) ? 2 : 4;
        var indexCount = dataSize / stride;
        BufferUsage usage = (IsWriteOnly) ? BufferUsage.WriteOnly : BufferUsage.None;
        buffer = new DynamicIndexBuffer (device, elementSize, indexCount, usage);
      }

      buffer.SetData (data, 0, dataSize);

      return buffer;
    }
  }

  // Aether.Extras - CpuAnimatedVertexBufferReader
  public class CpuAnimatedVertexBufferReader : ContentTypeReader<CpuAnimatedVertexBuffer> {
    protected override CpuAnimatedVertexBuffer Read (ContentReader input, CpuAnimatedVertexBuffer buffer) {
      Console.WriteLine ("CpuAnimatedVertexBuffer.Read");

      IGraphicsDeviceService graphicsDeviceService = (IGraphicsDeviceService) input.ContentManager.ServiceProvider.GetService (typeof (IGraphicsDeviceService));
      var device = graphicsDeviceService.GraphicsDevice;

      // read standard VertexBuffer
      var declaration = input.ReadRawObject<VertexDeclaration> ();
      var vertexCount = (int) input.ReadUInt32 ();
      // int dataSize = vertexCount * declaration.VertexStride;
      //byte[] data = new byte[dataSize];
      //input.Read(data, 0, dataSize);

      //read data                      
      var channels = declaration.GetVertexElements ();
      var cpuVertices = new VertexIndicesWeightsPositionNormal [vertexCount];
      var gpuVertices = new VertexPositionNormalTexture [vertexCount];

      for (int i = 0; i < vertexCount; i++) {
        foreach (var channel in channels) {
          switch (channel.VertexElementUsage) {
            case VertexElementUsage.Position:
              System.Diagnostics.Debug.Assert (channel.VertexElementFormat == VertexElementFormat.Vector3);
              var pos = input.ReadVector3 ();
              if (channel.UsageIndex == 0) {
                cpuVertices [i].Position = pos;
                gpuVertices [i].Position = pos;
              }
              break;

            case VertexElementUsage.Normal:
              System.Diagnostics.Debug.Assert (channel.VertexElementFormat == VertexElementFormat.Vector3);
              var nor = input.ReadVector3 ();
              if (channel.UsageIndex == 0) {
                cpuVertices [i].Normal = nor;
                gpuVertices [i].Normal = nor;
              }
              break;

            case VertexElementUsage.TextureCoordinate:
              System.Diagnostics.Debug.Assert (channel.VertexElementFormat == VertexElementFormat.Vector2);
              var tex = input.ReadVector2 ();
              if (channel.UsageIndex == 0) {
                gpuVertices [i].TextureCoordinate = tex;
              }
              break;

            case VertexElementUsage.BlendWeight:
              System.Diagnostics.Debug.Assert (channel.VertexElementFormat == VertexElementFormat.Vector4);
              var wei = input.ReadVector4 ();
              if (channel.UsageIndex == 0) {
                cpuVertices [i].BlendWeights = wei;
              }
              break;

            case VertexElementUsage.BlendIndices:
              System.Diagnostics.Debug.Assert (channel.VertexElementFormat == VertexElementFormat.Byte4);
              var i0 = input.ReadByte ();
              var i1 = input.ReadByte ();
              var i2 = input.ReadByte ();
              var i3 = input.ReadByte ();
              if (channel.UsageIndex == 0) {
                cpuVertices [i].BlendIndex0 = i0;
                cpuVertices [i].BlendIndex1 = i1;
                cpuVertices [i].BlendIndex2 = i2;
                cpuVertices [i].BlendIndex3 = i3;
              }
              break;

            default:
              throw new Exception ();
          }
        }
      }


      // read extras
      bool IsWriteOnly = input.ReadBoolean ();

      if (buffer == null) {
        BufferUsage usage = (IsWriteOnly) ? BufferUsage.WriteOnly : BufferUsage.None;
        buffer = new CpuAnimatedVertexBuffer (device, VertexPositionNormalTexture.VertexDeclaration, vertexCount, usage);
      }

      buffer.SetData (gpuVertices, 0, vertexCount);
      buffer.SetGpuVertices (gpuVertices);
      buffer.SetCpuVertices (cpuVertices);

      return buffer;
    }
  }
}
