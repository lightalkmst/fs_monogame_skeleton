#nowarn "62"
#nowarn "0058"

namespace game

open System
open System.Diagnostics
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open AnimatedModelPipeline

open Globals
open Globals.Input
open Globals.State
open Globals.Values
open Assets
open Config
open States
open States.Updating
open States.Drawing

module Engine =
  type Engine () as x =
    inherit Game ()
 
    let graphics = new GraphicsDeviceManager (x)
    let mutable sprite_batch = Unchecked.defaultof<SpriteBatch>
    let mutable ui_buffer = Unchecked.defaultof<RenderTarget2D>
    let mutable update_all_inputs = false
    let mutable initialized = false

    let mutable game_states: state list = [Opening 0]
    let mutable input: Input.input = {
      presses = None
      actions = [||]
    }

    // TODO: stateful rollover
    let get_total_time (game_time: GameTime) =
      let d = game_time.TotalGameTime.Days
      let h = game_time.TotalGameTime.Hours + d * 24
      let m = game_time.TotalGameTime.Minutes + h * 60
      let s = game_time.TotalGameTime.Seconds + m * 60
      game_time.TotalGameTime.Milliseconds + s * 1000

    override x.Initialize () =
      content <- new AtomizedContentManager (content.ServiceProvider)
      content.RootDirectory <- "Content"

      sprite_batch <- new SpriteBatch (x.GraphicsDevice)
      ui_buffer <- new RenderTarget2D (
        x.GraphicsDevice,
        Values.ui_buffer_width,
        Values.ui_buffer_height,
        false,
        x.GraphicsDevice.PresentationParameters.BackBufferFormat,
        DepthFormat.Depth24
      )
      base.Initialize ()
      
      graphics.PreferredBackBufferWidth <- config.screen_width
      graphics.PreferredBackBufferHeight <- config.screen_height
      graphics.ApplyChanges ()
      x.IsFixedTimeStep <- false

    override x.LoadContent () =
      //let dir = Config.Constants.app_data_path
      //let loaders: Assets.Loaders = {
      //  image = content.Load
      //  font = content.Load
      //  text = (( + ) dir) >> System.IO.File.ReadAllText
      //  sound = content.Load
      //}
      
      //Assets.bg_load_assets loaders Constants.menu_assets
      ()
 
    override x.Update game_time =
      if not initialized && get_total_time game_time > 0
      then 
        //Sound.Music.init ()
        //Sound.Music.play_opening_song content
        initialized <- true

      // IO
      let elapsed = game_time.ElapsedGameTime.Milliseconds
      input <- Input.get false Config.config

      // active game state path
      let data: update_data = {    
        past = List.tail game_states
        present = List.head game_states
        states = game_states
        inputs = input
        game_time = game_time
        total_time = get_total_time game_time
        config = config
      }
      let updater: game_updater =
        match List.head game_states with
        | Opening _ -> Opening.update
        | Dungeon _ -> Dungeon.update
        | Main _ -> Main.update
        | Settings _ -> Settings.update
      let updates = updater data
      game_states <- updates.states

      // IO
      if updates.config <> config
      then 
        Config.File.update_file updates.config
        config <- updates.config
        graphics.PreferredBackBufferWidth <- config.screen_width
        graphics.PreferredBackBufferHeight <- config.screen_height
        graphics.ApplyChanges ()

 
    override x.Draw game_time =
      //if Assets.are_loaded Constants.menu_assets
      //then
      //  x.GraphicsDevice.SetRenderTarget ui_buffer

      //  // active game state path
      //  sprite_batch.Begin ()
      //  let data: draw_data = {
      //    graphics = x.GraphicsDevice 
      //    sprite_batch = sprite_batch
      //    screen_buffer = ui_buffer
      //    present = List.head game_states
      //    input = input 
      //    game_time = game_time
      //    total_time = get_total_time game_time
      //  }
      //  let drawer: game_drawer =
      //    match List.head game_states with        
      //    | Opening _ -> Opening.draw
      //    | Dungeon _ -> Dungeon.draw
      //    | Main _ -> Main.draw
      //    | Settings _ -> Settings.draw
      //  drawer data
      //  sprite_batch.End ()

      //  x.GraphicsDevice.SetRenderTarget null

      //  sprite_batch.Begin (
      //    SpriteSortMode.Immediate, 
      //    BlendState.AlphaBlend, 
      //    SamplerState.LinearClamp, 
      //    DepthStencilState.Default, 
      //    RasterizerState.CullNone
      //  )
 
      //  sprite_batch.Draw (ui_buffer, new Rectangle(0, 0, config.screen_width, config.screen_height), Color.White)
      //  sprite_batch.End ()

    ()

