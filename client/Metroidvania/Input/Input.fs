#nowarn "62"

namespace game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

open Globals
open Globals.Input

module Input =
  module Keyboard =
    let keys = [|
      Keys.A; Keys.B; Keys.Back; Keys.C; Keys.D; Keys.Delete; Keys.E; Keys.End; Keys.Enter; Keys.Escape; Keys.F; Keys.F1; Keys.F10;
      Keys.F11; Keys.F12; Keys.F2; Keys.F3; Keys.F4; Keys.F5; Keys.F6; Keys.F7; Keys.F8; Keys.F9; Keys.G; Keys.H; Keys.Home; Keys.I; Keys.J;
      Keys.K; Keys.L; Keys.M; Keys.Multiply; Keys.N; Keys.NumPad0; Keys.NumPad1; Keys.NumPad2; Keys.NumPad3; Keys.NumPad4; Keys.NumPad5; Keys.NumPad6;
      Keys.NumPad7; Keys.NumPad8; Keys.NumPad9; Keys.O; Keys.OemAuto; Keys.OemBackslash; Keys.OemCloseBrackets; Keys.OemComma; Keys.OemMinus; Keys.OemOpenBrackets;
      Keys.OemPeriod; Keys.OemPipe; Keys.OemPlus; Keys.OemQuestion; Keys.OemQuotes; Keys.OemSemicolon; Keys.OemTilde; Keys.P; Keys.PageDown; Keys.PageUp; Keys.Pause;
      Keys.Q; Keys.R; Keys.S; Keys.Space; Keys.T; Keys.Tab; Keys.U; Keys.V; Keys.W; Keys.X; Keys.Y; Keys.Z;
  
      Keys.D1; Keys.D2; Keys.D3; Keys.D4; Keys.D5; Keys.D6; Keys.D7; Keys.D8; Keys.D9; Keys.D0;
  
      Keys.Left; Keys.Right; Keys.Up; Keys.Down; Keys.Enter
    |]
  
    let get_presses () =
      let state = Keyboard.GetState ()
      keys
      |> Array.filter state.IsKeyDown
      |> Array.map Key
  
    let get_actions binds =
      let state = Keyboard.GetState ()
      binds
      |> Array.filter (fst >> state.IsKeyDown)
      |> Array.map snd

  module Controller =
    let indexes = [|
      PlayerIndex.One
      PlayerIndex.Two
      PlayerIndex.Three
      PlayerIndex.Four
    |]

    let buttons = [|
      Buttons.A
      Buttons.B
      Buttons.Back
      Buttons.X
      Buttons.Y

      Buttons.A


      Buttons.DPadDown
    |]

    let get_presses () =
      indexes
      |> Array.filter (fun x -> (GamePad.GetCapabilities x).IsConnected)
      |> Array.map (fun x -> 
        let state = GamePad.GetState x
        buttons
        |> Array.filter (fun y -> state.IsButtonDown y)
        |> Array.map (fun y -> Button (x, y))
      )
      |> Array.concat

    let get_actions (i: PlayerIndex) binds =
      if (GamePad.GetCapabilities i).IsConnected
      then
        let state = GamePad.GetState i
        binds
        |> Array.filter (fst >> state.IsButtonDown)
        |> Array.map snd
      else [||]

  let get all (cfg: config) : input =
    {
      presses = 
        if all
        then Some <| Array.concat [| Keyboard.get_presses (); Controller.get_presses () |]
        else None
      actions = Array.map (function 
        | Some (Keyboard x) -> Some (Keyboard.get_actions x)
        | Some (Controller (i, x)) -> Some (Controller.get_actions i x)
        | None -> None
      ) cfg.keybinds
    }