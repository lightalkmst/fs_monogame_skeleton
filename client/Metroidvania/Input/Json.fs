#nowarn "40"
#nowarn "58"
#nowarn "62"
namespace game

open System

open Globals
open Globals.Utilities
open Parser

module Json =
  type JValue =
  | JNull
  | JBool of bool
  | JInt of int
  | JFloat of float
  | JString of string
  | JObject of (string, JValue) Map
  | JArray of JValue list

  // JSON subset parser
  // supports everything except exponential scientific notation floats and whitespace outside of strings
  // sufficient to handle the range of expected output from backend
  let rec json_parser =
    let j_null = map_parser (!. JNull) (parse_string "null")

    let j_bool =
      map_parser (!. (JBool true)) (parse_string "true")
      .| map_parser (!. (JBool false)) (parse_string "false")

    let j_int = map_parser JInt parse_int

    let j_float = map_parser JFloat parse_float

    let parse_j_string =
      let parse_normal_char = parse_1 (not << (!~ List.contains ['\\'; '\"']))
      let parse_escaped =
        [
          ("\\\"", '\"')
          ("\\\\", '\\')
          ("\\/", '/')
          ("\\b", '\b')
          ("\\f", '\f')
          ("\\n", '\n')
          ("\\r", '\r')
          ("\\t", '\t')
        ]
        |> List.map (fun (s, c) -> map_parser (!. c) (parse_string s))
        |> choice
      let parse_unicode =
        let parse_prefix = parse_string "\\u"
        let parse_hex = parse_char_in (['0'..'9'] @ ['A'..'F'] @ ['a'..'f'])
        ignore_left_parser (parse_prefix .& (parse_hex .& parse_hex .& parse_hex .& parse_hex))
        |> map_parser (fun (((c, c2), c3), c4) ->
          let str = sprintf "%c%c%c%c" c c2 c3 c4
          Int32.Parse(str, Globalization.NumberStyles.HexNumber)
          |> char
        )
      (at_least_0_parser (parse_normal_char .| parse_escaped .| parse_unicode))
      |> between_parser (parse_char '"') (parse_char '"')

    let j_string = map_parser (List.toArray >> String >> JString) parse_j_string

    let j_array input = map_parser JArray (between_parser (parse_char '[') (parse_char ']') (repeat_separated_0_parser (parse_char ',') json_parser)) input

    let j_object input =
      let parse_key = ignore_right_parser (parse_j_string .& parse_char ':')
      let parse_entry = map_parser (List.toArray >> String) parse_key .& json_parser
      let parse_entries = repeat_separated_0_parser (parse_char ',') parse_entry
      input
      |> (
        parse_entries
        |> between_parser (parse_char '{') (parse_char '}')
        |> map_parser (Map.ofList >> JObject)
      )
    choice [
      j_null
      j_bool     
      j_float
      j_int
      j_string
      j_array
      j_object
    ]

  let rec string_of_json = function
  | JNull -> "null"
  | JBool x -> if x then "true" else "false"
  | JInt x -> string x
  | JFloat x -> string x
  | JString x -> "\"" + x + "\""
  | JObject x -> "{" + String.Join (",", List.map (fun (k, v) -> "\"" + k + "\":" + string_of_json v) (Map.toList x)) + "}"
  | JArray x -> "[" + String.Join (",", List.map string_of_json x) + "]"

  let get_bool =
    function
    | JBool x -> x
    | _ -> failwith "Json.get_bool: Not a JBool"

  let get_int =
    function
    | JInt x -> x
    | _ -> failwith "Json.get_int: Not a JInt"

  let get_float =
    function
    | JFloat x -> x
    | _ -> failwith "Json.get_float: Not a JFloat"

  let get_string =
    function
    | JString x -> x
    | _ -> failwith "Json.get_string: Not a JString"

  let get_object =
    function
    | JObject x -> x
    | _ -> failwith "Json.get_object: Not a JObject"

  let get_array =
    function
    | JArray x -> x
    | _ -> failwith "Json.get_array: Not a JArray"
    
  let has_value k = get_object >> Map.containsKey k
  let get_value k = get_object >> Map.find k
  let get_bool_value k = get_value k >> get_bool
  let get_int_value k = get_value k >> get_int
  let get_float_value k = get_value k >> get_float
  let get_string_value k = get_value k >> get_string
  let get_array_value k = get_value k >> get_array
  let get_object_value k = get_value k >> get_object
  let map_json_array f = List.map f >> List.toArray
  let map_get_json_array f = map_json_array (get_array >> f)

  let rec get_property f (xs: string list) (json: JValue) = 
    let get x json = 
      if Option.isSome json && has_value x json.Value
      then Some <| get_value x json.Value
      else None
    let rec get2 xs json =
      match xs with
      | [] -> json
      | x :: t -> get2 t <| get x json
    match get2 xs <| Some json with
    | Some y -> Some <| f y
    | None -> None
    