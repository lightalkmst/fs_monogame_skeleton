using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnimatedModelPipeline {
  public static class Extensions {
    public static Animations GetAnimations (this Model model) {
      var animations = model.Tag as Animations;
      return animations;
    }

    public static void UpdateVertices (this ModelMeshPart meshPart, Matrix [] boneTransforms) {
      var animatedVertexBuffer = meshPart.VertexBuffer as CpuAnimatedVertexBuffer;
      animatedVertexBuffer.UpdateVertices (boneTransforms, meshPart.VertexOffset, meshPart.NumVertices);
    }

  }
}
