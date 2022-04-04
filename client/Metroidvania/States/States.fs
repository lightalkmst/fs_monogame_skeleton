#nowarn "58"
#nowarn "62"

namespace game

open System
                           
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

open Globals
open Globals.Values
open Globals.Utilities
open Globals.State
open Config
open Json
open Input
//open Parser

module States = 
  module Updating =
    type update_data = {
      past: state list
      present: state
      states: state list
      inputs: input
      game_time: Microsoft.Xna.Framework.GameTime
      total_time: int
      config: config
    }
    type game_update = {
      states: state list
      asset_requests: string list
      asset_frees: string list                                                 
      config: config
      //sounds: Sound_config list
    }
    type game_updater = update_data -> game_update
  
    // TODO: submodules for organization? state, drawing, json

    let dud_update = {
      states = []
      asset_requests = []
      asset_frees = []
      config = config
      //sounds = []
    }
    
    let pop_state l = (List.head l, List.tail l)
    
    let is_first_frame start (game_time: GameTime) = // TODO: fix
      game_time.TotalGameTime.Milliseconds - game_time.ElapsedGameTime.Milliseconds = start
      && game_time.ElapsedGameTime.Milliseconds <> 0
    
    let is_interval_frame t start (game_time: GameTime) = start % t > (game_time.TotalGameTime.Milliseconds - start) % t
    
    let maintain_state (data: update_data) =
      {
        dud_update with
          states = data.states
          config = data.config
      }
    
    let maintain_state_with (data: update_data) (state: state) =
      {
        maintain_state data with
          states = state :: data.past
      }

    //let maintain_state_with_sounds (data: update_data) (state: state) (sounds: Sound_config list) =
    //  {
    //    maintain_state_with data state with
    //      sounds = sounds
    //  }

  module Drawing =
    type draw_data = {
      graphics: GraphicsDevice
      sprite_batch: SpriteBatch
      screen_buffer: Texture2D
      present: state 
      input: input
      game_time: Microsoft.Xna.Framework.GameTime   
      total_time: int
    }
    type game_drawer = draw_data -> unit

    //let draw_background (data: draw_data) (img: string) = data.sprite_batch.Draw (Assets.get_image img, Rectangle(0, 0, ui_buffer_width, ui_buffer_height), Color.White)

    let screen_vector (pos: Vector2) = Vector2 (float32 ui_buffer_width * pos.X, float32 ui_buffer_height * pos.Y)

    let screen_rect (pos0: Vector2) (pos1: Vector2) = 
      let x0 = int <| float32 ui_buffer_width * pos0.X
      let y0 = int <| float32 ui_buffer_height * pos0.Y
      let x1 = (int <| float32 ui_buffer_width * pos1.X) - x0
      let y1 = (int <| float32 ui_buffer_height * pos1.Y) - y0
      Rectangle (x0, y0, x1, y1)

    //// calculate text lines
    //let get_text_lines (font: string) (size: float32) (width: float32) (s: string) =
    //  let sprite_font = Assets.get_font font
    //  let ss = s.Split ([|' '|])
    //  let mutable cx = 0.0f
    //  let dims = 
    //    ss 
    //    |> Array.map (!~ ( + ) " ")
    //    |> Array.map sprite_font.MeasureString 
    //    |> Array.map (( * ) size)
    //  let mutable i = 0
    //  let mutable ans = []
    //  let mutable row = []
    //  while i < ss.Length do
    //    if dims.[i].X + cx > width then
    //      cx <- 0.0f
    //      ans <- row :: ans
    //      row <- []
    //    cx <- cx + dims.[i].X
    //    row <- ss.[i] :: row
    //    i <- i + 1
    //  ans <- row :: ans
    //  ans
    //  |> List.map (List.rev >> (fun x -> String.Join (" ", x)))
    //  |> List.rev
    //  |> Array.ofList

    //// draw text lines

    //// TODO: optimize to cache string texture

    //let draw_string (font: string) (data: draw_data) (size: float32) (pos: Vector2) (s: string) =
    //  let sprite_font = Assets.get_font font
    //  data.sprite_batch.DrawString (sprite_font, s, pos, Color.Black, 0.0f, Vector2 (0.0f, 0.0f), size, SpriteEffects.None, 0.0f)

    //let draw_string_centered (font: string) (data: draw_data) (size: float32) (pos: Vector2) (s: string) =
    //  let sprite_font = Assets.get_font font
    //  let center_offset = sprite_font.MeasureString s
    //  draw_string font data size (pos - center_offset * 0.5f * size) s

    //let font_heights: (string, float32) Map = Map.ofList []

  //module Sound =
  //  let move_selection_sound: Sound_config = {
  //    sound = "tonk"
  //    hero = None
  //  }

  //  ()