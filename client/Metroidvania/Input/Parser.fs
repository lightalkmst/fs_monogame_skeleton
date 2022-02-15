namespace game

open System
open Globals
open Globals.Utilities

// https://fsharpforfunandprofit.com/posts/understanding-parser-combinators/
module Parser =
  type input = {
    string: string
    pos: int
  }

  type 'a result =
  | Success of 'a * input
  | Failure

  type 'a Parser = input -> 'a result

  let parse_epsilon x input = Success (x, input)

  // execute
  let run p input =
    match p {string = input; pos = 0} with
    | Failure -> raise <| ArgumentException "Input was not a valid JSON string"
    | Success (v, _) -> v

  // combinators
  let ( .| ) p p2 input =
    let res = p input
    match res with
    | Success _ -> res
    | Failure -> p2 input

  let ( .& ) p p2 input =
    match p input with
    | Success (v, input) ->
      match p2 input with
      | Success (v2, input) -> Success ((v, v2), input)
      | Failure -> Failure
    | Failure -> Failure

  let choice<'a> : 'a Parser list -> 'a Parser = List.reduce ( .| )

  let bind_parser f p input =
    match p input with
    | Success (v, input) -> (f v) input
    | Failure -> Failure

  let map_parser f = (f >> parse_epsilon) |> bind_parser

  let ( .<| ) p p2 = map_parser (fun (f, x) -> f x) (p .& p2)

  let lift2_parser f p p2 = parse_epsilon f .<| p .<| p2

  let sequence<'a> : 'a Parser list -> 'a list Parser =
    List.rev
    >> List.map (lift2_parser cons)
    >> List.fold ( |> ) (parse_epsilon [])

  let rec at_least_0_parser p input =
    match p input with
    | Success (v, input) ->
      match at_least_0_parser p input with
      | Success (v2, input) -> Success (v :: v2, input)
      | Failure -> failwith "Parser.parse_at_least_0"
    | Failure -> Success ([], input)

  let at_least_1_parser p = lift2_parser cons p (at_least_0_parser p)

  let optional_parser p = map_parser Some p .| parse_epsilon None

  let ignore_left_parser<'a, 'b> : ('a * 'b) Parser -> 'b Parser = map_parser snd

  let ignore_right_parser<'a, 'b> : ('a * 'b) Parser -> 'a Parser = map_parser fst

  let between_parser p p2 =
    ( .& ) p
    >> ignore_left_parser
    >> (fun x -> x .& p2)
    >> ignore_right_parser

  let repeat_separated_1_parser p p2 = map_parser tup_cons (p2 .& at_least_0_parser (ignore_left_parser (p .& p2)))

  let repeat_separated_0_parser p p2 = repeat_separated_1_parser p p2 .| parse_epsilon []

  // parsers
  let parse_1 f input =
    if input.pos < input.string.Length
    then
      let c = input.string.[input.pos]
      if f c
      then Success (c, { input with pos = input.pos + 1 })
      else Failure
    else Failure

  let parse_char c = parse_1 (( = ) c)

  let parse_char_in cs = parse_1 (fun c -> List.contains c cs)

  let parse_string =
    (fun (s: string) -> s.ToCharArray())
    >> Array.toList
    >> List.map parse_char
    >> sequence
    >> map_parser (List.toArray >> String)

  let parse_digits = at_least_1_parser <| parse_char_in ['0' .. '9']

  let parse_optional_sign =
    parse_char '-'
    |> optional_parser
    |> map_parser (function Some _ -> '-' | None -> '+')

  let parse_int = map_parser (tup_cons >> List.toArray >> String >> int) (parse_optional_sign .& parse_digits)

  let parse_float =
    (parse_optional_sign .& parse_digits .& parse_char '.' .& parse_digits)
    |> map_parser (fun (((sign, ds), dec), ds2) -> (List.toArray >> String >> float) (sign :: ds @ dec :: ds2))

  let parse_space = parse_1 Char.IsWhiteSpace

  let parse_spaces = at_least_1_parser parse_space

  let ignore_spaces_after_parser<'a> : 'a Parser -> 'a Parser =
    !~ ( .& ) parse_spaces
    >> ignore_right_parser