#nowarn "62"
#nowarn "0058"

namespace game

open System
open System.Diagnostics
open System.Globalization
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open AnimatedModelPipeline

module Drawing =
  type view_data = {
    view: Matrix
    projection: Matrix
    time: GameTime
  }

  type entity_data = {
    model: string
    world: Matrix
    animation: string
    animation_start: GameTime
    time_scale: float
    texture: Texture2D
  }

  let draw_entity (v: view_data) (e: entity_data) =
    let model = Assets.content.Load<Model> (e.model)

    let animations = model.Tag :?> Animations
    if animations <> null
    then
      let time_change = v.time.TotalGameTime.Subtract e.animation_start.TotalGameTime * e.time_scale
      animations.SetClip (e.animation)
      animations.Update (time_change, true, Matrix.Identity) // TODO: change relativeToCurrentTime to false after starting to set animations
    
    let effect = Assets.content.Load<Effect> ("textured")
    effect.Parameters.["World"].SetValue e.world
    effect.Parameters.["View"].SetValue v.view
    effect.Parameters.["Projection"].SetValue v.projection
    // ambient lighting
    effect.Parameters.["AmbientColor"].SetValue (Color.Red.ToVector4 ())
    effect.Parameters.["AmbientIntensity"].SetValue 0.1f
    // diffuse lighting
    effect.Parameters.["DiffuseLightDirection"].SetValue (new Vector3 (1000.0f, 0.0f, 0.0f))
    effect.Parameters.["DiffuseIntensity"].SetValue 0.01f
    effect.Parameters.["DiffuseColor"].SetValue (Color.Green.ToVector4 ())
    // specular lighting
    effect.Parameters.["Shininess"].SetValue 400.0f
    effect.Parameters.["SpecularColor"].SetValue (Color.Blue.ToVector4 ())
    effect.Parameters.["SpecularIntensity"].SetValue 100.0f
    effect.Parameters.["ViewVector"].SetValue (new Vector3 (0.0f, 0.0f, 1000.0f))
    // texturing
    effect.Parameters.["ModelTexture"].SetValue e.texture

    for mesh in model.Meshes do                                 
      for part in mesh.MeshParts do
        part.Effect <- effect
        
        if animations <> null
        then part.UpdateVertices animations.AnimationTransforms

      // diffuse lighting
      let worldInverseTransposeMatrix = Matrix.Transpose (Matrix.Invert (mesh.ParentBone.Transform * e.world)) // TODO: determine actual
      effect.Parameters.["WorldInverseTranspose"].SetValue worldInverseTransposeMatrix

      mesh.Draw()
    ()
  ()
