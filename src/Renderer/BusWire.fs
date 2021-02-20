module BusWire

open Fable.React
open Fable.React.Props
open Browser
open Elmish
open Elmish.React
open Helpers


//------------------------------------------------------------------------//
//------------------------------BusWire Types-----------------------------//
//------------------------------------------------------------------------//


/// type for buswires
/// for demo only. The real wires will
/// connect to Ports - not symbols, where each symbol has
/// a number of ports (see Issie Component and Port types) and have
/// extra information for highlighting, width, etc.
/// NB - how you define Ports for drawing - whether they correspond to
/// a separate datatype and Id, or whether port offsets from
/// component coordinates are held in some other way, is up to groups.

type Direction =  Left | Right

type Wire = {
    Id: CommonTypes.ConnectionId 
    SrcSymbol: CommonTypes.ComponentId
    TargetSymbol: CommonTypes.ComponentId
    }

(*
    needs two port types defined to be able to test!
type Wire = {
    Id: CommonTypes.ConnectionId 
    SrcPort: string //CommonTypes.Port.Id
    TargetPort: string //CommonTypes.Port.Id
    Color: CommonTypes.HighLightColor
    //Width = 
    }
*)

type Model = {
    Symbol: Symbol.Model
    WX: Map<CommonTypes.ConnectionId,Wire>
    Color: CommonTypes.HighLightColor
    }

//----------------------------Message Type-----------------------------------//

/// Messages to update buswire model
/// These are OK for the demo - but not the correct messages for
/// a production system. In the real system wires must connect
/// to ports, not symbols. In addition there will be other changes needed
/// for highlighting, width inference, etc
type Msg =
    | Symbol of Symbol.Msg
    | AddWire of (CommonTypes.ConnectionId * CommonTypes.ConnectionId)
    | SetColor of CommonTypes.HighLightColor
    | MouseMsg of MouseT


/// look up wire in WireModel
let wire (wModel: Model) (wId: CommonTypes.ConnectionId): Wire =
    let result = wModel.WX.TryFind(wId) //returns a Wire option
    match result with
    | Some x -> x
    | _ -> failwithf "no Wire with this connectionId found in the Model"
    (*
    match wModel.WXelement.ConnectionId with
    | wId -> return wire with Id, SrcSymbol and TargetSymbol of that element of WX
    {
        Id = wId
        SrcSymbol = //Source ComponentId from ConnectionId and Model
        TargetSymbol = //Target ComponentId from ConnectionId and Model
    }
    *)

type WireRenderProps = {
    key : CommonTypes.ConnectionId
    WireP: Wire
    SrcP: XYPos 
    TgtP: XYPos
    //SrcD: Direction // type Direction =  Left | Right -- uncomment once implemented
    //TgtD: Direction
    ColorP: string
    StrokeWidthP: string
    }

/// react virtual DOM SVG for one wire
/// In general one wire will be multiple (right-angled) segments.

let singleWireView = 
    FunctionComponent.Of(
        fun (props: WireRenderProps) ->
            line [
                X1 props.SrcP.X
                Y1 props.SrcP.Y
                X2 props.TgtP.X
                Y2 props.TgtP.Y
                // Qualify these props to avoid name collision with CSSProp
                SVGAttr.Stroke props.ColorP
                SVGAttr.StrokeWidth props.StrokeWidthP ] [])
(*
let wireVerticesList props = 
    let Xs, Ys, Xt, Yt = props.SrcP.X, props.SrcP.Y, props.TgtP.X, props.TgtP.Y
    [[(Xs, Ys);((Xs+Xt)/2.0, Ys)];[((Xs+Xt)/2.0, Ys);((Xs+Xt)/2.0, Yt)];[((Xs+Xt)/2.0, Yt);(Xt,Yt)]]

let lineSegment (twoCoordList : XYPos*XYPos) = 
    let Xa, Ya, Xb, Yb = fst(twoCoordList.[0]), snd(twoCoordList.[0]), fst(twoCoordList.[1]), snd(twoCoordList.[1])
    line [
                X1 Xa
                Y1 Ya
                X2 Xb
                Y2 Yb
                // Qualify these props to avoid name collision with CSSProp
                SVGAttr.Stroke "red"
                SVGAttr.StrokeWidth "2px" ] []



    //let wires =
    //    model.WX
    //    |> List.map ()

*)

let getPortCoords (portIds : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) : (XYPos * XYPos) =
    failwithf "not implemented yet"

let getVertices (portPos : (XYPos * XYPos)) : (float * float) list =
    failwithf "not implemented yet"



let view (model:Model) (dispatch: Dispatch<Msg>)= 

    let listValsFromMap m = 
        m
        |> Map.toList
        |> List.map (fun (x,y) -> y)

    let wires = 
        model.WX
        |> listValsFromMap
        |> List.map (fun w ->
            let props = {
                key = w.Id
                WireP = w
                SrcP = Symbol.symbolPos model.Symbol w.SrcSymbol 
                TgtP = Symbol. symbolPos model.Symbol w.TargetSymbol 
                ColorP = model.Color.Text()
                StrokeWidthP = "2px"}
            singleWireView props)
    let symbols = Symbol.view model.Symbol (fun sMsg -> dispatch (Symbol sMsg))
    g [] [(g [] wires); symbols]

    (*
    let wires = 
        model.WX
        |> List.map (fun w ->
            let props = {
                key = w.Id
                WireP = w
                SrcP = Symbol.symbolPos model.Symbol w.SrcSymbol 
                TgtP = Symbol. symbolPos model.Symbol w.TargetSymbol 
                ColorP = model.Color.Text()
                StrokeWidthP = "2px" }
            singleWireView props)
    let symbols = Symbol.view model.Symbol (fun sMsg -> dispatch (Symbol sMsg))
    g [] [(g [] wires); symbols]
    *)

/// dummy init for testing: real init would probably start with no wires.
/// this initialisation is not realistic - ports are not used
/// this initialisation depends on details of Symbol.Model type.
let init (n:int) () =
    
    let symbols, cmd = Symbol.init()
    let symIds = List.map (fun (sym:Symbol.Symbol) -> sym.Id) symbols //gets symbol Ids
    let n = symIds.Length

    let makeWire = 
        let s1,s2 = symbols.[0],symbols.[1]
        {
            Id=CommonTypes.ConnectionId (uuid())
            SrcSymbol = s1.Id
            TargetSymbol = s2.Id
        }
    
    let wxMap = 
        List.map (fun i -> makeWire) [1..n]
        |> List.map (fun wire -> (wire.Id, wire))
        |> Map.ofList

    let wxMapEmpty = Map.empty //use WX=wxMapEmpty below to initialise with no wires!
    
    {WX=wxMap;Symbol=symbols; Color=CommonTypes.Red},Cmd.none
    
let update (msg : Msg) (model : Model): Model*Cmd<Msg> =
    match msg with
    | Symbol sMsg -> 
        let sm,sCmd = Symbol.update sMsg model.Symbol
        {model with Symbol=sm}, Cmd.map Symbol sCmd
    | AddWire _ -> failwithf "Not implemented"
    | SetColor c -> {model with Color = c}, Cmd.none
    | MouseMsg mMsg -> model, Cmd.ofMsg (Symbol (Symbol.MouseMsg mMsg))

//---------------Other interface functions--------------------//

/// Given a point on the canvas, returns the wire ID of a wire within a few pixels
/// or None if no such. Where there are two close wires the nearest is taken. Used
/// to determine which wire (if any) to select on a mouse click
let wireToSelectOpt (wModel: Model) (pos: XYPos) : CommonTypes.ConnectionId option = 
    failwith "Not implemented"

//----------------------interface to Issie-----------------------//
let extractWire (wModel: Model) (sId:CommonTypes.ComponentId) : CommonTypes.Component= 
    failwithf "Not implemented"

let extractWires (wModel: Model) : CommonTypes.Component list = 
    failwithf "Not implemented"

/// Update the symbol with matching componentId to comp, or add a new symbol based on comp.
let updateSymbolModelWithComponent (symModel: Model) (comp:CommonTypes.Component) =
    failwithf "Not Implemented"

