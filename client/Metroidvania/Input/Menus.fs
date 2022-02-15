#nowarn "62"

namespace game

open System

open Globals
open Globals.Utilities
open Globals.Input

module Menus =
  ()

  // TODO: convert to int-based

  //let navigate (dx: int, dy: int) (x: int, y: int) (opts: ('a * int * int) list) =
  //  let get_closest () = 
  //    let dist (x2, y2) = (x2 - x) * (x2 - x) + (y2 - y) * (y2 - y)
  //    opts
  //    |> List.sortWith (fun (_, x2, y2) (_, x3, y3) -> dist (x2, y2) - dist (x3, y3))
  //    |> List.head
  //  let finder filter comps =
  //    let opts' = 
  //      opts
  //      |> List.filter filter
  //      |> List.sortWith (fun x y -> 
  //        List.tryPick (fun f -> 
  //          let ans = f x y
  //          if ans <> 0
  //          then Some ans
  //          else None
  //        ) comps
  //        |> Option.defaultValue 0
  //      )
  //    if List.isEmpty opts'
  //    then get_closest ()
  //    else opts'.[0]
  //  match (dx, dy) with                                                                                                                                                                                          
  //  | (_, dy) when dy > 0 -> finder (fun (_, _, y2) -> y2 > y) [(fun (_, x2, _) (_, x3, _) -> Math.Abs (x2 - x) - Math.Abs (x3 - x)); (fun (_, _, y2) (_, _, y3) -> y2 - y3); (fun (_, x2, _) (_, x3, _) -> x2 - x3)]
  //  | (_, dy) when dy < 0 -> finder (fun (_, _, y2) -> y2 < y) [(fun (_, x2, _) (_, x3, _) -> Math.Abs (x2 - x) - Math.Abs (x3 - x)); (fun (_, _, y2) (_, _, y3) -> y3 - y2); (fun (_, x2, _) (_, x3, _) -> x2 - x3)]
  //  | (dx, _) when dx > 0 -> finder (fun (_, x2, _) -> x2 > x) [(fun (_, _, y2) (_, _, y3) -> Math.Abs (y2 - y) - Math.Abs (y3 - y)); (fun (_, x2, _) (_, x3, _) -> x2 - x3); (fun (_, _, y2) (_, _, y3) -> y2 - y3)]
  //  | (dx, _) when dx < 0 -> finder (fun (_, x2, _) -> x2 < x) [(fun (_, _, y2) (_, _, y3) -> Math.Abs (y2 - y) - Math.Abs (y3 - y)); (fun (_, x2, _) (_, x3, _) -> x3 - x2); (fun (_, _, y2) (_, _, y3) -> y2 - y3)]
  //  | _ -> get_closest ()

  //let navigate_f32 (dx: float32, dy: float32) (x: float32, y: float32) (opts: ('a * float32 * float32) list) = 
  //  let a, x, y = navigate (int dx, int dy) (int x, int y) (opts |> List.map (fun (a, x, y) -> a, int x, int y))
  //  a, float32 x, float32 y
    
  let limit_selection: int -> int -> int -> int = fun x y -> currify Math.Max x >> currify Math.Min y


  ()