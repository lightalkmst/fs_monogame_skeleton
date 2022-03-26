using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
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

// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\AnimationsWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\ClipWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\CpuAnimatedVertexBufferWriter.cs

// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\GraphicsImporters\Serialization\DynamicIndexBufferWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\GraphicsImporters\Serialization\DynamicVertexBufferWriter.cs

namespace AnimatedModelPipeline {
  // Aether.Extras - DynamicIndexBufferWriter
  [ContentTypeWriter]
  class DynamicIndexBufferWriter : ContentTypeWriter<AnimatedIndexBufferContent> {
    protected override void Write (ContentWriter output, AnimatedIndexBufferContent buffer) {
      WriteIndexBuffer (output, buffer);

      output.Write (buffer.IsWriteOnly);

      return;
    }

    private static void WriteIndexBuffer (ContentWriter output, AnimatedIndexBufferContent buffer) {
      // check if the buffer contains values greater than UInt16.MaxValue
      var is16Bit = true;
      foreach (var index in buffer) {
        if (index > UInt16.MaxValue) {
          is16Bit = false;
          break;
        }
      }

      var stride = (is16Bit) ? 2 : 4;

      output.Write (is16Bit); // Is 16 bit
      output.Write ((UInt32) (buffer.Count * stride)); // Data size
      if (is16Bit) {
        foreach (var item in buffer)
          output.Write ((UInt16) item);
      }
      else {
        foreach (var item in buffer)
          output.Write (item);
      }
    }

    //public override string GetRuntimeType (TargetPlatform targetPlatform) {
    //  return typeof (Animation).AssemblyQualifiedName;
    //}

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return typeof (DynamicIndexBufferReader).AssemblyQualifiedName;
    }
  }

  // Aether.Extras - ClipWriter
  [ContentTypeWriter]
  class ClipWriter : ContentTypeWriter<ClipContent> {
    protected override void Write (ContentWriter output, ClipContent value) {
      WriteDuration (output, value.Duration);
      WriteKeyframes (output, value.Keyframes);
    }

    private void WriteDuration (ContentWriter output, TimeSpan duration) {
      output.Write (duration.Ticks);
    }

    private void WriteKeyframes (ContentWriter output, IList<KeyframeContent> keyframes) {
      Int32 count = keyframes.Count;
      output.Write ((Int32) count);

      for (int i = 0; i < count; i++) {
        KeyframeContent keyframe = keyframes [i];
        output.Write (keyframe.Bone);
        output.Write (keyframe.Time.Ticks);
        output.Write (keyframe.Transform.M11);
        output.Write (keyframe.Transform.M12);
        output.Write (keyframe.Transform.M13);
        output.Write (keyframe.Transform.M21);
        output.Write (keyframe.Transform.M22);
        output.Write (keyframe.Transform.M23);
        output.Write (keyframe.Transform.M31);
        output.Write (keyframe.Transform.M32);
        output.Write (keyframe.Transform.M33);
        output.Write (keyframe.Transform.M41);
        output.Write (keyframe.Transform.M42);
        output.Write (keyframe.Transform.M43);
      }

      return;
    }

    public override string GetRuntimeType (TargetPlatform targetPlatform) {
      return typeof (Clip).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return typeof (ClipReader).AssemblyQualifiedName;
    }
  }

  // Aether.Extras - CpuAnimatedVertexBufferWriter
  [ContentTypeWriter]
  public class CpuAnimatedVertexBufferWriter : ContentTypeWriter<AnimatedVertexBufferContent> {
    protected override void Write (ContentWriter output, AnimatedVertexBufferContent buffer) {
      WriteVertexBuffer (output, buffer);

      output.Write (buffer.IsWriteOnly);
    }

    private void WriteVertexBuffer (ContentWriter output, AnimatedVertexBufferContent buffer) {
      var vertexCount = buffer.VertexData.Length / buffer.VertexDeclaration.VertexStride;
      output.WriteRawObject (buffer.VertexDeclaration);
      output.Write ((UInt32) vertexCount);
      output.Write (buffer.VertexData);
    }

    public override string GetRuntimeType (TargetPlatform targetPlatform) {
      return typeof (CpuAnimatedVertexBuffer).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return typeof (CpuAnimatedVertexBufferReader).AssemblyQualifiedName;
    }
  }

  // Aether.Extras - AnimationsDataWriter
  [ContentTypeWriter]
  class AnimationsDataWriter : ContentTypeWriter<AnimationsContent> {
    protected override void Write (ContentWriter output, AnimationsContent value) {
      WriteClips (output, value.Clips);
      WriteBindPose (output, value.BindPose);
      WriteInvBindPose (output, value.InvBindPose);
      WriteSkeletonHierarchy (output, value.SkeletonHierarchy);
      WriteBoneNames (output, value.BoneNames);
    }

    private void WriteClips (ContentWriter output, Dictionary<string, ClipContent> clips) {
      Int32 count = clips.Count;
      output.Write ((Int32) count);

      foreach (var clip in clips) {
        output.Write (clip.Key);
        output.WriteObject<ClipContent> (clip.Value);
      }

      return;
    }

    private void WriteBindPose (ContentWriter output, List<Microsoft.Xna.Framework.Matrix> bindPoses) {
      Int32 count = bindPoses.Count;
      output.Write ((Int32) count);

      for (int i = 0; i < count; i++)
        output.Write (bindPoses [i]);

      return;
    }

    private void WriteInvBindPose (ContentWriter output, List<Microsoft.Xna.Framework.Matrix> invBindPoses) {
      Int32 count = invBindPoses.Count;
      output.Write ((Int32) count);

      for (int i = 0; i < count; i++)
        output.Write (invBindPoses [i]);

      return;
    }

    private void WriteSkeletonHierarchy (ContentWriter output, List<int> skeletonHierarchy) {
      Int32 count = skeletonHierarchy.Count;
      output.Write ((Int32) count);

      for (int i = 0; i < count; i++)
        output.Write ((Int32) skeletonHierarchy [i]);

      return;
    }

    private void WriteBoneNames (ContentWriter output, List<string> boneNames) {
      Int32 count = boneNames.Count;
      output.Write ((Int32) count);

      for (int boneIndex = 0; boneIndex < count; boneIndex++) {
        var boneName = boneNames [boneIndex];
        output.Write (boneName);
      }

      return;
    }

    public override string GetRuntimeType (TargetPlatform targetPlatform) {
      return typeof (Animation).AssemblyQualifiedName;
    }

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return typeof (AnimationsReader).AssemblyQualifiedName;
    }
  }

  // Aether.Extras - DynamicModelWriter
  [ContentTypeWriter]
  public class CPUAnimatedModelWriter : ContentTypeWriter<AnimatedModelContent> {
    /// <summary>
    /// Write a Model xnb, compatible with the XNB Container Format.
    /// </summary>
    protected override void Write (ContentWriter output, AnimatedModelContent model) {
      WriteBones (output, model.Bones);
      WriteMeshes (output, model, model.Meshes);
      WriteBoneReference (output, model.Bones.Count, model.Source.Root);
      output.WriteObject (model.Source.Tag);
    }

    private void WriteBones (ContentWriter output, ModelBoneContentCollection bones) {
      var bonesCount = bones.Count;
      output.Write ((UInt32) bonesCount);

      foreach (var bone in bones) {
        output.WriteObject (bone.Name);
        output.Write (bone.Transform);
      }

      foreach (var bone in bones) {
        WriteBoneReference (output, bonesCount, bone.Parent);

        output.Write ((uint) bone.Children.Count);
        foreach (var child in bone.Children)
          WriteBoneReference (output, bonesCount, child);
      }

      return;
    }

    // The BoneReference type varies in size depending on the number of bones in the model. 
    // If bone count is less than 255 this value is serialized as a Byte, otherwise it is UInt32. 
    // If the reference value is zero the bone is null, otherwise (bone reference - 1) is an index into the model bone list.
    private void WriteBoneReference (ContentWriter output, int bonesCount, ModelBoneContent bone) {
      if (bone == null)
        output.Write ((byte) 0);
      else if (bonesCount < 255)
        output.Write ((byte) (bone.Index + 1));
      else
        output.Write ((UInt32) (bone.Index + 1));
    }

    private void WriteMeshes (ContentWriter output, AnimatedModelContent model, List<AnimatedMeshContent> meshes) {
      output.Write ((UInt32) meshes.Count);

      var bonesCount = model.Bones.Count;
      foreach (var mesh in meshes) {
        output.WriteObject (mesh.Name);
        WriteBoneReference (output, bonesCount, mesh.ParentBone);
        WriteBoundingSphere (output, mesh.BoundingSphere);
        output.WriteObject (mesh.Tag);

        WriteParts (output, model, mesh.MeshParts); // fails: https://github.com/MonoGame/MonoGame/blob/a9e5ae6befc40d7c86320ffdcfcd9d9b66f786a8/MonoGame.Framework/Content/ContentReaders/ModelReader.cs#L52
      }
    }

    private void WriteBoundingSphere (ContentWriter output, BoundingSphere value) {
      output.Write (value.Center);
      output.Write (value.Radius);
    }

    private void WriteParts (ContentWriter output, AnimatedModelContent model, List<AnimatedMeshPartContent> parts) {
      output.Write ((UInt32) parts.Count);

      foreach (var part in parts) {
        output.Write ((UInt32) part.VertexOffset);
        output.Write ((UInt32) part.NumVertices);
        output.Write ((UInt32) part.StartIndex);
        output.Write ((UInt32) part.PrimitiveCount);
        output.WriteObject (part.Tag);

        //var s = "part.VertexBuffer";
        //try {
        //  s = "part.VertexBuffer.Identity";
        //  if (part.VertexBuffer.Identity == null) {
        //    throw new Exception (s);
        //  } // empty
        //  s = "part.VertexBuffer.VertexDeclaration.Identity";
        //  if (part.VertexBuffer.VertexDeclaration.Identity == null) {
        //    throw new Exception (s);
        //  } // empty
        //}
        //catch (Exception) {
        //  throw new Exception (s);
        //}

        output.WriteSharedResource (part.VertexBuffer); // fails: todo: figure out how to get this to not be nullable object error
        output.WriteSharedResource (part.IndexBuffer);
        output.WriteSharedResource (part.Material);
      }
    }

    public override string GetRuntimeType (TargetPlatform targetPlatform) {
      return "Microsoft.Xna.Framework.Graphics.Model";
    }

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return "Microsoft.Xna.Framework.Content.ModelReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553";
    }
  }
}