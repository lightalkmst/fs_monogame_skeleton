#nowarn "0025"
#nowarn "0062"
namespace game

open System
open System.IO
open System.Text.RegularExpressions
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Content
open System.Threading

module Globals =
  module Utilities =
    let ( !. ) x _ = x
    let ( !~ ) f x y = f y x
    let ( !- ) f = f >> not
    let ( !-- ) f x = f x >> not
    let ( == ) = LanguagePrimitives.PhysicalEquality

    let identity x = x
    let currify f x y = f (x, y)
    let uncurrify f (x, y) = f x y
    let cons x y = x :: y

    let fst3 (x, _, _) = x
    let snd3 (_, x, _) = x
    let trd3 (_, _, x) = x

    let tup_cons<'a> : ('a * 'a list) -> 'a list = uncurrify cons

    let loop_selection x y z = if z < x then y else if z > y then x else z

    let key_of_string: string -> Keys = !~ Map.find (Map.ofList [
      "A", Keys.A
      "B", Keys.B
      "C", Keys.C
      "D", Keys.D
      "E", Keys.E
      "F", Keys.F
      "G", Keys.G
      "H", Keys.H
      "I", Keys.I
      "J", Keys.J
      "K", Keys.K
      "L", Keys.L
      "M", Keys.M
      "N", Keys.N
      "O", Keys.O
      "P", Keys.P
      "Q", Keys.Q
      "R", Keys.R
      "S", Keys.S
      "T", Keys.T
      "U", Keys.U
      "V", Keys.V
      "W", Keys.W
      "X", Keys.X
      "Y", Keys.Y
      "Z", Keys.Z
      "Backspace", Keys.Back
      "Delete", Keys.Delete
      "End", Keys.End
      "Enter", Keys.Enter
      "Escape", Keys.Escape
      "F1", Keys.F1
      "F10", Keys.F10
      "F11", Keys.F11
      "F12", Keys.F12
      "F2", Keys.F2
      "F3", Keys.F3
      "F4", Keys.F4
      "F5", Keys.F5
      "F6", Keys.F6
      "F7", Keys.F7
      "F8", Keys.F8
      "F9", Keys.F9
      "Home", Keys.Home
      "*", Keys.Multiply
      "0", Keys.D0
      "1", Keys.D1
      "2", Keys.D2
      "3", Keys.D3
      "4", Keys.D4
      "5", Keys.D5
      "6", Keys.D6
      "7", Keys.D7
      "8", Keys.D8
      "9", Keys.D9
      "NumPad 0", Keys.NumPad0
      "NumPad 1", Keys.NumPad1
      "NumPad 2", Keys.NumPad2
      "NumPad 3", Keys.NumPad3
      "NumPad 4", Keys.NumPad4
      "NumPad 5", Keys.NumPad5
      "NumPad 6", Keys.NumPad6
      "NumPad 7", Keys.NumPad7
      "NumPad 8", Keys.NumPad8
      "NumPad 9", Keys.NumPad9
      //Keys.OemAuto,
      "/", Keys.OemBackslash
      "]", Keys.OemCloseBrackets
      ",", Keys.OemComma
      "-", Keys.OemMinus
      "[", Keys.OemOpenBrackets
      ".", Keys.OemPeriod
      "|", Keys.OemPipe
      "+", Keys.OemPlus
      "?", Keys.OemQuestion
      "\"", Keys.OemQuotes
      ";", Keys.OemSemicolon
      "`", Keys.OemTilde
      "Page Down", Keys.PageDown
      "Page Up", Keys.PageUp
      "Pause", Keys.Pause
      "Space", Keys.Space
      "Tab", Keys.Tab
      "Left Arrow", Keys.Left
      "Right Arrow", Keys.Right
      "Up Arrow", Keys.Up
      "Down", Keys.Down
      "Enter", Keys.Enter
      "+", Keys.Add
      "-", Keys.Subtract
      "/", Keys.Divide
    ])

    let string_of_key: Keys -> string = !~ Map.find (Map.ofList [
      Keys.A, "A"
      Keys.B, "B"
      Keys.C, "C"
      Keys.D, "D"
      Keys.E, "E"
      Keys.F, "F"
      Keys.G, "G"
      Keys.H, "H"
      Keys.I, "I"
      Keys.J, "J"
      Keys.K, "K"
      Keys.L, "L"
      Keys.M, "M"
      Keys.N, "N"
      Keys.O, "O"
      Keys.P, "P"
      Keys.Q, "Q"
      Keys.R, "R"
      Keys.S, "S"
      Keys.T, "T"
      Keys.U, "U"
      Keys.V, "V"
      Keys.W, "W"
      Keys.X, "X"
      Keys.Y, "Y"
      Keys.Z, "Z"
      Keys.Back, "Backspace"
      Keys.Delete, "Delete"
      Keys.End, "End"
      Keys.Enter, "Enter"
      Keys.Escape, "Escape"
      Keys.F1, "F1"
      Keys.F10, "F10"
      Keys.F11, "F11"
      Keys.F12, "F12"
      Keys.F2, "F2"
      Keys.F3, "F3"
      Keys.F4, "F4"
      Keys.F5, "F5"
      Keys.F6, "F6"
      Keys.F7, "F7"
      Keys.F8, "F8"
      Keys.F9, "F9"
      Keys.Home, "Home"
      Keys.Multiply, "*"
      Keys.D0, "0"
      Keys.D1, "1"
      Keys.D2, "2"
      Keys.D3, "3"
      Keys.D4, "4"
      Keys.D5, "5"
      Keys.D6, "6"
      Keys.D7, "7"
      Keys.D8, "8"
      Keys.D9, "9"
      Keys.NumPad0, "NumPad 0"
      Keys.NumPad1, "NumPad 1"
      Keys.NumPad2, "NumPad 2"
      Keys.NumPad3, "NumPad 3"
      Keys.NumPad4, "NumPad 4"
      Keys.NumPad5, "NumPad 5"
      Keys.NumPad6, "NumPad 6"
      Keys.NumPad7, "NumPad 7"
      Keys.NumPad8, "NumPad 8"
      Keys.NumPad9, "NumPad 9"
      //Keys.OemAutoKeys.OemBackslash, "/"
      Keys.OemCloseBrackets, "]"
      Keys.OemComma, ","
      Keys.OemMinus, "-"
      Keys.OemOpenBrackets, "["
      Keys.OemPeriod, "."
      Keys.OemPipe, "|"
      Keys.OemPlus, "+"
      Keys.OemQuestion, "?"
      Keys.OemQuotes, "\""
      Keys.OemSemicolon, ";"
      Keys.OemTilde, "`"
      Keys.PageDown, "Page Down"
      Keys.PageUp, "Page Up"
      Keys.Pause, "Pause"
      Keys.Space, "Space"
      Keys.Tab, "Tab"
      Keys.Left, "Left Arrow"
      Keys.Right, "Right Arrow"
      Keys.Up, "Up Arrow"
      Keys.Down, "Down"
      Keys.Enter, "Enter"
    ])

  // common types
  type content =
  | Image of Texture2D
  | Model of Model
  | Font of SpriteFont
  | Text of string
  | Sound of SoundEffect
  | Loading

  module Input =
    type 'a Request_status =
    | Pending
    | Complete of 'a
    | Error

    //type press =
    //| Key of Keys
    //| Button of PlayerIndex * Buttons

    //type action = 
    //// common inputs
    //| Up
    //| Down
    //| Left
    //| Right
    //// action inputs
    //| Jump
    //| Attack
    //| Ability
    //| Interact
    //| Dash
    //| Clone
    //// menu inputs
    //| Scroll_left
    //| Scroll_right
    //| Confirm
    //| Cancel
    //| Detail
    //| More

    //type input = {
    //  presses: press array option
    //  actions: action array array
    //}

    //type config = {
    //  keybinds: (press, action) Map option array
    //  screen_height: int
    //  screen_width: int
    //  volume: int
    //}

    type press =
    | Key of Keys
    | Button of PlayerIndex * Buttons
    
    type action = 
    // common inputs
    | Up
    | Down
    | Left
    | Right
    // action inputs
    | Jump
    | Attack
    | Ability
    | Interact
    | Dash
    | Clone
    // menu inputs
    | Scroll_left
    | Scroll_right
    | Confirm
    | Cancel
    | Detail
    | More

    type binds = 
    | Keyboard of (Keys * action) array
    | Controller of PlayerIndex * (Buttons * action) array
    
    type input = {
      presses: press array option
      actions: action array option array
    }
    
    type config = {
      keybinds: binds option array
      screen_height: int
      screen_width: int
      volume: int
    }

  module State =
    module Settings =
      type state = {
        selection_y: int
        selection_x: int
        focused: bool
        temp_number: string
        config: Input.config
      }

    module Dungeon =
      type state = {
        x: int
        y: int
      }

    module Main =
      type state = {
        start: int
        main_selection_y: int
      }

    type state =
    | Opening of int
    | Main of Main.state
    | Dungeon of Dungeon.state
    | Settings of Settings.state

  module Values =
    module Constants =
      let dev_name = "Level 30 Wizard"
      let game_name = "metroidvania"
      let opening = "opening"
      let login = "login"
      let main = "main"
      let stages = "stages"
      let battle = "battle"
      let lobby = "lobby"
      let font = "font"
      let battle_font = "font"
      let small_card_front = "small_card_front"
      let glow = "glow"
      let commit = "commit"
      let tonk = "tonk"
      let menu_assets = [opening; login; main; stages; battle; lobby; font; glow; commit; battle_font; tonk]
      let file_separator = string Path.DirectorySeparatorChar

    let mutable ui_buffer_width = 1600
    let mutable ui_buffer_width_f32 = float32 ui_buffer_width
    let mutable ui_buffer_height = 900
    let mutable ui_buffer_height_f32 = float32 ui_buffer_height
    let mutable ui_buffer_x_center = ui_buffer_width / 2
    let mutable ui_buffer_x_center_f32 = float32 ui_buffer_x_center
    let mutable ui_buffer_y_center = ui_buffer_height / 2
    let mutable ui_buffer_y_center_f32 = float32 ui_buffer_y_center

  // use f# implicit lazy loading of modules to work around requiring the Game instance ContentManager for initialization
  let mutable content_manager: ContentManager = null
