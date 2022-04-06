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

open game.Globals.Utilities
open game.Drawing
open game.Assets

module Engine =
  type Engine () as x =
    inherit Game ()

    let graphics: GraphicsDeviceManager = new GraphicsDeviceManager(x)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable font = Unchecked.defaultof<SpriteFont>
    
    let mutable _model_CPU = Unchecked.defaultof<Model>
    let mutable _model_GPU = Unchecked.defaultof<Model>
    let mutable _animations = Unchecked.defaultof<Animations>
    let mutable _original_animations = Unchecked.defaultof<Animations>
    
    let mutable prevKeyboardState = Unchecked.defaultof<KeyboardState>     
    
    let mutable Position = Vector3.Zero
    let mutable Zoom = 2000f
    let mutable RotationY = 0.0f
    let mutable RotationX = 0.0f
    let mutable gameWorldRotation = Matrix.Identity
    let mutable worlds = [|
      //Matrix.CreateWorld (new Vector3 (500.0f, 0.0f, 0.0f), new Vector3 (0.0f, 1.0f, 1.0f), new Vector3 (0.0f, 0.0f, -1.0f))
      
      Matrix.CreateWorld (Vector3.Left * 500.0f, Vector3.Left, Vector3.Up)
      Matrix.CreateWorld (Vector3.Right * 500.0f, Vector3.Right, Vector3.Up)
      Matrix.CreateWorld (Vector3.Left * 1000.0f, Vector3.Left, Vector3.Up)
      Matrix.CreateWorld (Vector3.Right * 1000.0f, Vector3.Right, Vector3.Up)
      Matrix.CreateWorld (Vector3.Left * 600.0f, Vector3.Left, Vector3.Up)
      Matrix.CreateWorld (Vector3.Right * 600.0f, Vector3.Right, Vector3.Up)
      Matrix.CreateWorld (Vector3.Left * 1100.0f, Vector3.Left, Vector3.Up)
      Matrix.CreateWorld (Vector3.Right * 1100.0f, Vector3.Right, Vector3.Up)
      //Matrix.Identity
    |]
    
    let mutable sw = new Stopwatch()
    let mutable msecMin = Double.MaxValue
    let mutable msecMax = 0.0
    let mutable avg = 0.0
    let mutable acc = 0.0
    let mutable c = 0

    let mutable effect = Unchecked.defaultof<Effect>
    let mutable texture = Unchecked.defaultof<Texture2D>

    let mutable entities = 
      worlds
      |> Array.mapi (fun i x ->
        {
          id = 0
          model = "test4"
          world = x
          animation = ["Armature|Praise"; "Armature|Pray"].[i % 2]
          texture = texture
          time_scale = 1.0
          animation_start = new GameTime ()
        }
      )

    override x.Initialize () =
      base.Initialize ()

    override x.LoadContent () = 
      content <- new AtomizedContentManager (x.Content.ServiceProvider)
      content.RootDirectory <- "Content"

      spriteBatch <- new SpriteBatch(x.GraphicsDevice)
      font <- content.Load<SpriteFont>("font")
                   
      _model_CPU <- content.Load<Model>("test4")
      _model_GPU <- content.Load<Model>("test4")

      _animations <- _model_CPU.GetAnimations()
      
      _original_animations <- _animations.Clone()

      Console.WriteLine ("Clip Names:")
      _animations.Clips
      |> Seq.map (|KeyValue|)  
      |> Map.ofSeq
      |> Map.iter (fun k v -> Console.WriteLine k)

      effect <- content.Load<Effect>("textured")

      texture <- content.Load<Texture2D>("test4_uv")

      ()

    override x.Update gameTime =
      let keyboardState = Keyboard.GetState()
      let gamePadState = GamePad.GetState(PlayerIndex.One)

      // Allows the game to exit
      if keyboardState.IsKeyDown(Keys.Escape) || gamePadState.Buttons.Back = ButtonState.Pressed
      then x.Exit()

      prevKeyboardState <- keyboardState

      base.Update(gameTime)

    override x.Draw gameTime =
      x.GraphicsDevice.Clear(Color.Black);

      let aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio
      //let world = Matrix.CreateWorld (new Vector3 (1000.0f, 0.0f, 0.0f), new Vector3 (0.0f, 0.0f, 0.0f), new Vector3 (0.0f, 1000.0f, 0.0f))
      let projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), aspectRatio, 0.01f, 10000.0f);
      let view = Matrix.CreateLookAt(
          Vector3.Forward * Zoom,
          Vector3.Up * (Zoom / 3.0f), 
          Vector3.Up);

      x.GraphicsDevice.BlendState <- BlendState.Opaque
      x.GraphicsDevice.RasterizerState <- RasterizerState.CullCounterClockwise
      x.GraphicsDevice.DepthStencilState <- DepthStencilState.Default
      x.GraphicsDevice.SamplerStates.[0] <- SamplerState.LinearWrap

      sw.Reset()
      sw.Start()

      let draw_model_with_effects () =
        entities
        |> Array.iter (fun x ->
          draw_entity {
            view = view
            projection = projection
            time = gameTime
          } x
        )

      draw_model_with_effects ()

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
      spriteBatch.DrawString(font, msec.ToString("#0.000",CultureInfo.InvariantCulture) + "ms", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 130), Color.White);
      spriteBatch.DrawString(font, avg.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (avg)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 100), Color.White);
      spriteBatch.DrawString(font, msecMin.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (min)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 70), Color.White);
      spriteBatch.DrawString(font, msecMax.ToString("#0.000",CultureInfo.InvariantCulture) + "ms (max)", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 40), Color.White);
      spriteBatch.DrawString(font, gameTime.TotalGameTime.Milliseconds.ToString("#0.000",CultureInfo.InvariantCulture) + "ms", new Vector2(32.0f, float32 <| x.GraphicsDevice.Viewport.Height - 10), Color.White);
      spriteBatch.End();
        
      base.Draw(gameTime);                   
      
    member x.ConfigureEffectMatrices(effect: IEffectMatrices, world: Matrix, view: Matrix, projection: Matrix) =
      effect.World <- world;
      effect.View <- view;
      effect.Projection <- projection;

    member x.ConfigureEffectLighting(effect: IEffectLights) =
      effect.DirectionalLight0.Direction <- Vector3.Backward
      effect.DirectionalLight0.Enabled <- true;
      effect.DirectionalLight1.Enabled <- false;
      effect.DirectionalLight2.Enabled <- false;
