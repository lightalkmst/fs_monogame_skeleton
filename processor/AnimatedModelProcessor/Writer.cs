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

// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\AnimationsWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\ClipWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\AnimationImporters\Serialization\CpuAnimatedVertexBufferWriter.cs

// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\GraphicsImporters\Serialization\DynamicIndexBufferWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\GraphicsImporters\Serialization\DynamicModelWriter.cs
// C:\Users\omar\Desktop\gamedev\Aether.Extras\Content.Pipeline\GraphicsImporters\Serialization\DynamicVertexBufferWriter.cs

namespace AnimatedModelPipeline {
  [ContentTypeWriter]
  public class CPUAnimatedModelWriter : ContentTypeWriter<AnimatedModelContent> {
    protected override void Write (ContentWriter writer, AnimatedModelContent result) {
      //writer.Write (result.TextureAssets.Count);

      //foreach (var textureAsset in result.TextureAssets)
      //  writer.Write (textureAsset);

      //var fontFile = result.FontFile;
      //writer.Write (fontFile.Common.LineHeight);
      //writer.Write (fontFile.Chars.Count);

      //foreach (var c in fontFile.Chars) {
      //  writer.Write (c.Id);
      //  writer.Write (c.Page);
      //  writer.Write (c.X);
      //  writer.Write (c.Y);
      //  writer.Write (c.Width);
      //  writer.Write (c.Height);
      //  writer.Write (c.XOffset);
      //  writer.Write (c.YOffset);
      //  writer.Write (c.XAdvance);
      //}

      //writer.Write (fontFile.Kernings.Count);
      //foreach (var k in fontFile.Kernings) {
      //  writer.Write (k.First);
      //  writer.Write (k.Second);
      //  writer.Write (k.Amount);
      //}
    }

    public override string GetRuntimeType (TargetPlatform targetPlatform) {
      return "Microsoft.Xna.Framework.Graphics.Model";
    }

    public override string GetRuntimeReader (TargetPlatform targetPlatform) {
      return "Microsoft.Xna.Framework.Content.ModelReader, Microsoft.Xna.Framework.Graphics, Version=4.0.0.0, Culture=neutral, PublicKeyToken=842cf8be1de50553";
    }
  }
}