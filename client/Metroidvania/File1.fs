#nowarn "62"
#nowarn "0058"

namespace game2

open System
open System.Diagnostics
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content



open System
open System.Diagnostics
open System.Globalization
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
//open tainicom.Aether.Animation
//open tainicom.Aether.Shaders.Components
open AnimatedModelPipeline

module Engine =
  type DrawMode =
  | CPU = 1
  | GPU = 2

  type Engine () as x =
    inherit Game ()

    let graphics: GraphicsDeviceManager = new GraphicsDeviceManager(x)
    do x.Content.RootDirectory <- "Content"
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable font = Unchecked.defaultof<SpriteFont>
    
    let mutable _model_CPU = Unchecked.defaultof<Model>
    let mutable _model_GPU = Unchecked.defaultof<Model>
    let mutable _animations = Unchecked.defaultof<Animations>
    let mutable drawMode = DrawMode.CPU
    
    let mutable prevKeyboardState = Unchecked.defaultof<KeyboardState>     
    
    let mutable Position = Vector3.Zero
    let mutable Zoom = 2000f
    let mutable RotationY = 0.0f
    let mutable RotationX = 0.0f
    let mutable gameWorldRotation = Matrix.Identity          
    
    let mutable sw = new Stopwatch()
    let mutable msecMin = Double.MaxValue
    let mutable msecMax = 0.0
    let mutable avg = 0.0
    let mutable acc = 0.0
    let mutable c = 0

    override x.Initialize () =
      base.Initialize ()

    override x.LoadContent () =                          
      spriteBatch <- new SpriteBatch(x.GraphicsDevice)
      font <- x.Content.Load<SpriteFont>("font")
                   
      _model_CPU <- x.Content.Load<Model>("test4")
      _model_GPU <- x.Content.Load<Model>("test4")

      _animations <- _model_CPU.GetAnimations()

      Console.WriteLine ("Clip Names:")
      _animations.Clips
      |> Seq.map (|KeyValue|)  
      |> Map.ofSeq
      |> Map.iter (fun k v -> Console.WriteLine k)

      let clip = _animations.Clips.["Armature|Praise"]
      //let clip = _animations.Clips.["Armature|Action"]
      _animations.SetClip(clip)

    override x.Update gameTime =
      let keyboardState = Keyboard.GetState()
      let gamePadState = GamePad.GetState(PlayerIndex.One)

      // Allows the game to exit
      if keyboardState.IsKeyDown(Keys.Escape) || gamePadState.Buttons.Back = ButtonState.Pressed
      then x.Exit()

      if (keyboardState.IsKeyDown(Keys.Space) && prevKeyboardState.IsKeyUp(Keys.Space)) || gamePadState.Buttons.A = ButtonState.Pressed
      then
        let drawModesCount: int = Enum.GetValues(drawMode.GetType()).Length
        drawMode <- enum<DrawMode> <| ((int drawMode) + 1) % drawModesCount
        ()

      prevKeyboardState <- keyboardState

      _animations.Update(gameTime.ElapsedGameTime, true, Matrix.Identity)

      base.Update(gameTime)

    override x.Draw gameTime =
      x.GraphicsDevice.Clear(Color.Black);

      let aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio
      let projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.01f, 10000.0f);
      let view = Matrix.CreateLookAt(
          new Vector3(0.0f, 0.0f, -Zoom),
          new Vector3(0.0f, Zoom / 3.0f, 0.0f), 
          Vector3.Up);

      x.GraphicsDevice.BlendState <- BlendState.Opaque
      x.GraphicsDevice.RasterizerState <- RasterizerState.CullCounterClockwise
      x.GraphicsDevice.DepthStencilState <- DepthStencilState.Default
      x.GraphicsDevice.SamplerStates.[0] <- SamplerState.LinearWrap

      let mutable m = _model_CPU
      if drawMode = DrawMode.CPU
      then m <- _model_CPU
      else if drawMode = DrawMode.GPU
      then m <- _model_GPU

      let transforms = Array.zeroCreate<Matrix> m.Bones.Count
      m.CopyAbsoluteBoneTransformsTo(transforms)

      sw.Reset()
      sw.Start()

      for mesh in m.Meshes do
        for part in mesh.MeshParts do
          if drawMode = DrawMode.CPU
          then (part.Effect :?> BasicEffect).SpecularColor <- Vector3.Zero
          else if drawMode = DrawMode.GPU
          then (part.Effect :?> SkinnedEffect).SpecularColor <- Vector3.Zero
          x.ConfigureEffectMatrices(part.Effect :> obj :?> IEffectMatrices, Matrix.Identity, view, projection)
          x.ConfigureEffectLighting(part.Effect :> obj :?> IEffectLights)

          if drawMode = DrawMode.CPU
          then part.UpdateVertices(_animations.AnimationTransforms) // animate vertices on CPU
          //else if drawMode = DrawMode.GPU
          //then (part.Effect :?> SkinnedEffect).SetBoneTransforms(_animations.AnimationTransforms)// animate vertices on GPU
        mesh.Draw()
      sw.Stop()

      let mutable msec = sw.Elapsed.TotalMilliseconds
      msecMin <- Math.Min(msecMin, msec)
      if avg <> 0.0
      then msecMax <- Math.Max(msecMax, msec)
      acc <- acc + msec
      c <- c + 1
      if c > 60*2
      then
        avg <- acc / float c
        acc <- 0.0
        c <- 0

      spriteBatch.Begin();
      spriteBatch.DrawString(font, "Draw Mode: " + string drawMode, new Vector2(32.0f, 32.0f), Color.White);
      spriteBatch.DrawString(font, msec.ToString("#0.000",CultureInfo.InvariantCulture) + "ms", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 130), Color.White);
      spriteBatch.DrawString(font, avg.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (avg)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 100), Color.White);
      spriteBatch.DrawString(font, msecMin.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (min)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 70), Color.White);
      spriteBatch.DrawString(font, msecMax.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (max)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 40), Color.White);
      spriteBatch.End();
        
      base.Draw(gameTime);                   
      
    member x.ConfigureEffectMatrices(effect: IEffectMatrices, world: Matrix, view: Matrix, projection: Matrix) =
      effect.World <- world;
      effect.View <- view;
      effect.Projection <- projection;

    member x.ConfigureEffectLighting(effect) =
      effect.DirectionalLight0.Direction <- Vector3.Backward
      effect.DirectionalLight0.Enabled <- true;
      effect.DirectionalLight1.Enabled <- false;
      effect.DirectionalLight2.Enabled <- false;
