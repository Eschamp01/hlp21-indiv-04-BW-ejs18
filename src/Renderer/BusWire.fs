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

type Direction =  H | V

//type SegmentId = | SegmentId of string // -- make into a uuid

type Segment = 
    {
        //Id: string//SegmentId
        Start: XYPos
        End: XYPos
        Dir: Direction
        HostId: CommonTypes.ConnectionId 
    }

//let createSegment id startPos endPos dir = 
//    {Id =id; Start = startPos; End = endPos; Dir = dir}

//List.find (fun seg -> seg.Id = segId) segmentList


(*
type SegmentId = | SegmentId of string

type Segment = 
    {
        Id: string
        Dir: Direction
        Start: XYPos
        End: XYPos
        PrevSegId: string option
        NextSegId: string option
    }

let testSegment =
    {
        Id = "1"
        Dir = H
        Start = testStart
        End = testEndH
        PrevSegId = Some "0"
        NextSegId = Some "2"
    }
*)

(*
type Wire = {
    Id: CommonTypes.ConnectionId 
    SrcSymbol: CommonTypes.ComponentId
    TargetSymbol: CommonTypes.ComponentId
    }
*)

    //needs two port types defined to be able to test!
type Wire = {
    Id: CommonTypes.ConnectionId 
    //Remove SrcSymbol and TargetSymbol once port implementation is working
    SrcSymbol: CommonTypes.ComponentId
    TargetSymbol: CommonTypes.ComponentId
    //SrcPort: CommonTypes.InputPortId 
    //TargetPort: CommonTypes.OutputPortId
    Color: CommonTypes.HighLightColor
    Width: CommonTypes.Width
    Segments: Segment list
    }

type Model = {
    Symbol: Symbol.Model
    WX: Map<CommonTypes.ConnectionId,Wire>
    //Color: CommonTypes.HighLightColor
    }

//----------------------------Message Type-----------------------------------//

/// Messages to update buswire model
/// These are OK for the demo - but not the correct messages for
/// a production system. In the real system wires must connect
/// to ports, not symbols. In addition there will be other changes needed
/// for highlighting, width inference, etc
type Msg =
    | Symbol of Symbol.Msg
    | AddWire of (CommonTypes.InputPortId * CommonTypes.OutputPortId)
    //| SetColor of CommonTypes.HighLightColor
    | MouseMsg of MouseT
    | DeleteWires of CommonTypes.ConnectionId list
    | SelectWires of CommonTypes.ConnectionId list


/// look up wire in WireModel
let wire (wModel: Model) (wId: CommonTypes.ConnectionId): Wire =
    let result = wModel.WX.TryFind(wId) //returns a Wire option
    match result with
    | Some x -> x
    | _ -> failwithf "no wire with this ConnectionId found in the Model"


type WireRenderProps = {
    //key : CommonTypes.ConnectionId
    //WireP: Wire
    SrcP: XYPos 
    TgtP: XYPos
    // PortType: PortType -- for direction
    ColorP: string
    StrokeWidthP: string
    }

/// react virtual DOM SVG for one wire
/// In general one wire will be multiple (right-angled) segments.

let removeFirstAndLast lst = 
    match List.rev lst with
    |h::t -> 
        match List.rev t with
        | h::t -> t
        |_ -> failwithf "no list found"
    |_ -> failwithf "no list found"


(*
let makeWire (portCoords:XYPos*XYPos) = 
    let origin = {X=0.0;Y=0.0}
    let nullSegment = {Start=origin;End=origin;Dir=H;HostId=CommonTypes.ConnectionId(uuid())}
    let Xs, Ys, Xt, Yt = fst(portCoords).X, fst(portCoords).Y, snd(portCoords).X, snd(portCoords).Y
    let lineCoordsList = 
        let makeVertList =
            if (Xs-Xt)<(-40.0) then
                [(Xs,Ys);((Xs+Xt/2.0),Ys);((Xs+Xt/2.0),Yt);(Xt,Yt);(0.0,0.0);(0.0,0.0)]
                //{nullSegment with HostId = w.Id};{Start={X=Xs;Y=Ys};End={X=(Xs+Xt)/2.0;Y=Ys};};{};{};{nullSegment with HostId = w.Id}
            else if -80.0 > (Ys-Yt) then
                [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, (Ys+Yt)/2.0);(Xt-30.0, (Ys+Yt)/2.0);(Xt-30.0, Yt);(Xt,Yt)]
            else if (Ys-Yt) < 0.0 then
                if (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 then
                    [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, Ys-50.0);(Xt-30.0,Ys-50.0);(Xt-30.0,Yt);(Xt,Yt)]
                else
                    [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, Ys-50.0);(Xs-50.0,Ys-50.0);(Xs-50.0,Yt);(Xt,Yt)]
            else if 80.0 < (Ys-Yt) then
                [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, (Ys+Yt)/2.0);(Xt-30.0, (Ys+Yt)/2.0);(Xt-30.0, Yt);(Xt,Yt)]
            else if (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 then
                [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, Ys+50.0);(Xt-30.0,Ys+50.0);(Xt-30.0,Yt);(Xt,Yt)]
            else
                [(Xs, Ys);(Xs+30.0, Ys);(Xs+30.0, Ys+50.0);(Xs-50.0,Ys+50.0);(Xs-50.0,Yt);(Xt,Yt)]
        makeVertList
        |> List.collect (fun x -> [x;x])
        |> removeFirstAndLast
        |> List.chunkBySize 2
    let makeSegsFromLineList = 
        lineCoordsList
        |> List.map (fun (x,y))
    |> List.map (fun x -> {Start=;End=;Dir=})
*)

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


let makeWireVerticesList props = 
    let Xs, Ys, Xt, Yt = props.SrcP.X, props.SrcP.Y, props.TgtP.X, props.TgtP.Y
    let list1 = [[(Xs, Ys);((Xs+Xt)/2.0, Ys)];[((Xs+Xt)/2.0, Ys);((Xs+Xt)/2.0, Yt)];[((Xs+Xt)/2.0, Yt);(Xt,Yt)]]
    let list2 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, (Ys+Yt)/2.0)];[(Xs+30.0, (Ys+Yt)/2.0);(Xt-30.0, (Ys+Yt)/2.0)];[(Xt-30.0, (Ys+Yt)/2.0);(Xt-30.0, Yt)];[(Xt-30.0, Yt);(Xt,Yt)]]
    let list3 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys-50.0)];[(Xs+30.0, Ys-50.0);(Xt-30.0,Ys-50.0)];[(Xt-30.0,Ys-50.0);(Xt-30.0,Yt)];[(Xt-30.0,Yt);(Xt,Yt)]]
    let list4 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys+50.0)];[(Xs+30.0, Ys+50.0);(Xt-30.0,Ys+50.0)];[(Xt-30.0,Ys+50.0);(Xt-30.0,Yt)];[(Xt-30.0,Yt);(Xt,Yt)]]
    let list5 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys-50.0)];[(Xs+30.0, Ys-50.0);(Xs-50.0,Ys-50.0)];[(Xs-50.0,Ys-50.0);(Xs-50.0,Yt)];[(Xs-50.0,Yt);(Xt,Yt)]]
    let list6 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys+50.0)];[(Xs+30.0, Ys+50.0);(Xs-50.0,Ys+50.0)];[(Xs-50.0,Ys+50.0);(Xs-50.0,Yt)];[(Xs-50.0,Yt);(Xt,Yt)]]
    match (Xs-Xt)<(-40.0) with
    | true -> list1
    | _ -> 
        match -80.0 > (Ys-Yt) with
        |true -> list2
        | _ -> 
            match (Ys-Yt) < 0.0 with
            | true -> 
                match (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 with
                | true -> list3
                | _ -> list5
            | _ -> 
                match 80.0 < (Ys-Yt) with
                | true -> list2
                | _ -> 
                    match (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 with
                    | true -> list4
                    | _ -> list6

let makeWireVerticesList2 (portCoords:XYPos*XYPos) = 
    let Xs, Ys, Xt, Yt = fst(portCoords).X, fst(portCoords).Y, snd(portCoords).X, snd(portCoords).Y
    let list1 = [[(Xs, Ys);((Xs+Xt)/2.0, Ys)];[((Xs+Xt)/2.0, Ys);((Xs+Xt)/2.0, Yt)];[((Xs+Xt)/2.0, Yt);(Xt,Yt)]]
    let list2 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, (Ys+Yt)/2.0)];[(Xs+30.0, (Ys+Yt)/2.0);(Xt-30.0, (Ys+Yt)/2.0)];[(Xt-30.0, (Ys+Yt)/2.0);(Xt-30.0, Yt)];[(Xt-30.0, Yt);(Xt,Yt)]]
    let list3 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys-50.0)];[(Xs+30.0, Ys-50.0);(Xt-30.0,Ys-50.0)];[(Xt-30.0,Ys-50.0);(Xt-30.0,Yt)];[(Xt-30.0,Yt);(Xt,Yt)]]
    let list4 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys+50.0)];[(Xs+30.0, Ys+50.0);(Xt-30.0,Ys+50.0)];[(Xt-30.0,Ys+50.0);(Xt-30.0,Yt)];[(Xt-30.0,Yt);(Xt,Yt)]]
    let list5 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys-50.0)];[(Xs+30.0, Ys-50.0);(Xs-50.0,Ys-50.0)];[(Xs-50.0,Ys-50.0);(Xs-50.0,Yt)];[(Xs-50.0,Yt);(Xt,Yt)]]
    let list6 = [[(Xs, Ys);(Xs+30.0, Ys)];[(Xs+30.0, Ys);(Xs+30.0, Ys+50.0)];[(Xs+30.0, Ys+50.0);(Xs-50.0,Ys+50.0)];[(Xs-50.0,Ys+50.0);(Xs-50.0,Yt)];[(Xs-50.0,Yt);(Xt,Yt)]]
    match (Xs-Xt)<(-40.0) with
    | true -> list1
    | _ -> 
        match -80.0 > (Ys-Yt) with
        |true -> list2
        | _ -> 
            match (Ys-Yt) < 0.0 with
            | true -> 
                match (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 with
                | true -> list3
                | _ -> list5
            | _ -> 
                match 80.0 < (Ys-Yt) with
                | true -> list2
                | _ -> 
                    match (Xs-Xt) > 20.0 || (Xs-Xt) < -100.0 with
                    | true -> list4
                    | _ -> list6

let makeSegmentsList (hostId:CommonTypes.ConnectionId) (portCoords: XYPos * XYPos) =
    let verticesList2 = makeWireVerticesList2 portCoords
    let makeSegmentFromVerticesList lst = 
        lst
        |> List.mapi (fun i (x: list<float * float>) -> 
            {
                Start = {X=fst(x.[0]);Y=snd(x.[0])}
                End = {X=fst(x.[1]);Y=snd(x.[1])}
                Dir= if i%2=0 then H else V
                HostId=hostId
            })

    makeSegmentFromVerticesList verticesList2


let lineSegment (colour: string) (width: string) (twoCoordList : (float*float) list) = 
    let Xa, Ya, Xb, Yb = fst(twoCoordList.[0]), snd(twoCoordList.[0]), fst(twoCoordList.[1]), snd(twoCoordList.[1])
    line [
                X1 Xa
                Y1 Ya
                X2 Xb
                Y2 Yb
                // Qualify these props to avoid name collision with CSSProp
                SVGAttr.Stroke colour
                SVGAttr.StrokeWidth width ] []


//Addwire of (InputPordId * OutputPortId) implementation:

//1. call symbol interfacing function Symbol.getInputPortLocation InputPortId and Symbol.getOutputPortLocation OutputPortId
//These functions give type XYPos as output. Two dummy port locations are defined below:

let testInputPortLocation = {X = 300.0; Y = 300.0} //Symbol.getInputPortLocation InputPortId
let testOutputPortLocation = {X = 700.0; Y = 700.0} //Symbol.getOutputPortLocation OutputPortId

//2. Create 

let testStart = {X=2.0;Y=2.0}
let testEndH = {X=9.0;Y=2.0}
let testEndV = {X=2.0;Y=6.0}

let testSegmentH = {Start = testStart ;End = testEndH; Dir = H; HostId = CommonTypes.ConnectionId (uuid())}
let testSegmentV = {Start = testStart ;End = testEndV; Dir = V; HostId = CommonTypes.ConnectionId (uuid())}


let getBoundingBox (seg:Segment) = 
    match seg.Dir with
    | H -> {X = seg.Start.X - 2.0; Y = seg.Start.Y - 2.0; W = seg.End.X - seg.Start.X + 4.0; H = 4.0}
    | V -> {X = seg.Start.X - 2.0; Y = seg.Start.Y - 2.0; W = 4.0; H = seg.End.Y - seg.Start.Y + 4.0}
    
//getBoundingBox testSegmentV


let boundingBoxesIntersect (bb1:BoundingBox) (bb2:BoundingBox) = 
    if (bb1.X >= bb2.X + bb2.W) || (bb2.X >= bb1.X + bb1.W) then
        false
    else if (bb1.Y <= bb2.Y + bb2.H) || (bb2.Y <= bb1.Y + bb1.H) then
        false
    else
        true

let segIntersectsSeg (seg1Coords:XYPos*XYPos) (seg2Coords:XYPos*XYPos) : bool =
    let x1, x2, y1, y2, x3, x4, y3, y4 = fst(seg1Coords).X, snd(seg1Coords).X, fst(seg1Coords).Y, snd(seg1Coords).Y, fst(seg2Coords).X, snd(seg2Coords).X, fst(seg2Coords).Y, snd(seg2Coords).Y
    let uA = ((x4-x3)*(y1-y3) - (y4-y3)*(x1-x3)) / ((y4-y3)*(x2-x1) - (x4-x3)*(y2-y1))
    let uB = ((x2-x1)*(y1-y3) - (y2-y1)*(x1-x3)) / ((y4-y3)*(x2-x1) - (x4-x3)*(y2-y1))
    
    if (uA >= 0.0 && uA <= 1.0 && uB >= 0.0 && uB <= 1.0) then

        let intersectionX = x1 + (uA * (x2-x1)) // if coordinates are wanted, maybe useful later
        let intersectionY = y1 + (uA * (y2-y1))

        true
    else
        false

// used like this: segIntersectsSeg (seg1.Start,seg1.End) (seg2.Start,seg2.End)

let segIntersectsBoundingBox (bb:BoundingBox) (seg:Segment) : bool =
    let x1, x2, y1, y2, x, y, w, h = seg.Start.X, seg.End.X, seg.Start.Y, seg.End.Y, bb.X, bb.Y, bb.W, bb.H
    let leftIntersection = segIntersectsSeg (seg.Start,seg.End) ({X=x;Y=y},{X=x;Y=y+h})
    let rightIntersection = segIntersectsSeg (seg.Start,seg.End) ({X=x+w;Y=y},{X=x+w;Y=y+h})
    let topIntersection = segIntersectsSeg (seg.Start,seg.End) ({X=x;Y=y},{X=x+w;Y=y})
    let bottomIntersection = segIntersectsSeg (seg.Start,seg.End) ({X=x;Y=y+h},{X=x+w;Y=y+h})

    // if segment intersects with any of the boudign box's sides, segment intersects bounding box
    (leftIntersection || rightIntersection || topIntersection || bottomIntersection)

let segPointDist (seg:Segment) (pos: XYPos) = 
    let x1, x2, x3, y1, y2, y3 = seg.Start.X, seg.End.X, pos.X, seg.Start.Y, seg.End.Y, pos.Y
    let px = x2 - x1
    let py = y2 - y1
    let norm = px**2.0 + py**2.0
    let u = 
        let uVal = ((x3 - x1) * px + (y3 - y1) * py) / norm
        if uVal > 1.0 then 1.0
        else if uVal < 0.0 then 0.0
        else uVal
    
    let x = x1 + u * px
    let y = y1 + u * py
    let dx = x - x3
    let dy = y - y3

    (dx*dx + dy*dy)**0.5

let pDist = segPointDist testSegmentH testInputPortLocation



let view (model:Model) (dispatch: Dispatch<Msg>)= 

    let listValsFromMap m = 
        m
        |> Map.toList
        |> List.map (fun (x,y) -> y)
    
    let wireSegment n = 
        model.WX
        |> listValsFromMap
        |> List.map (fun w ->
            let props = {
                //key = w.Id
                //WireP = w
                SrcP = Symbol.symbolPos model.Symbol w.SrcSymbol //change Symbol.symbolPos to Symbol.portPos or equivalent later
                TgtP = Symbol. symbolPos model.Symbol w.TargetSymbol 
                ColorP = w.Color.Text()
                StrokeWidthP = "2px"}
            let wireVerticesList = makeWireVerticesList props
            if wireVerticesList.Length > n then
                Some (lineSegment props.ColorP props.StrokeWidthP wireVerticesList.[n])
            else
                None)

    let wires = 
        [0..4]
        |> List.collect (fun n -> wireSegment n)
        |> List.choose id

    let symbols = Symbol.view model.Symbol (fun sMsg -> dispatch (Symbol sMsg))
    g [] [(g [] wires);symbols]

/// dummy init for testing: real init would probably start with no wires.
/// this initialisation is not realistic - ports are not used
/// this initialisation depends on details of Symbol.Model type.
let init (n:int) () =
    let symbols, cmd = Symbol.init()
    
    //let symIds = List.map (fun (sym:Symbol.Symbol) -> sym.Id) symbols //gets symbol Ids
    //let n = symIds.Length

    let makeWire (index:int)= 
        let s1,s2 = symbols.[2*index],symbols.[2*index+1]
        {
            Id=CommonTypes.ConnectionId (uuid())
            SrcSymbol = s1.Id
            TargetSymbol = s2.Id
            Segments = List.empty
            Color = CommonTypes.HighLightColor.Red
            Width = CommonTypes.Width.Two
        }
    
    let wxMap = 
        List.map (fun i -> makeWire i) [0..n]
        |> List.map (fun wire -> (wire.Id, wire))
        |> Map.ofList
    
    {WX=wxMap;Symbol=symbols},Cmd.none
    
    //{WX=Map.empty;Symbol=symbols; Color=CommonTypes.Red},Cmd.none //use this line and comment out the rest for no wires at start
    
let update (msg : Msg) (model : Model): Model*Cmd<Msg> =
    match msg with
    | Symbol sMsg -> 
        let sm,sCmd = Symbol.update sMsg model.Symbol
        {model with Symbol=sm}, Cmd.map Symbol sCmd
    
    | AddWire (portIds:CommonTypes.InputPortId * CommonTypes.OutputPortId) ->
        //Symbol.getPortLocations(portIds) --> returns (XYPos * XYPos)
        let dummyPortId1 = CommonTypes.ComponentId(uuid()) //actually component Ids, change this later (in group stage maybe)
        let dummyPortId2 = CommonTypes.ComponentId(uuid())
        //let wireWidthFromSymbol = Symbol.checkPortWidths portIds //returns Some wireWidth if same width (Blue wire, Width=wireWidth), else None (Red wire, Width=One)
        let wireWidthFromSymbol = Some 3

        let wireColour = 
            match wireWidthFromSymbol with
            | None -> CommonTypes.HighLightColor.Red
            | Some width -> 
                if width <= 0 then
                    CommonTypes.HighLightColor.Red //if wire width is less than 1, make wire red
                else
                    CommonTypes.HighLightColor.Blue
            
        let wireWidth = 
            match wireWidthFromSymbol with
            | Some width -> 
                if width <= 0 then
                    failwithf "wire width must be at least one!"
                else
                    match width with
                    | 1 -> CommonTypes.Width.One
                    | 2 -> CommonTypes.Width.Two
                    | 3 -> CommonTypes.Width.Three
                    | 4 -> CommonTypes.Width.Four
                    | 5 -> CommonTypes.Width.Five
                    | 6 -> CommonTypes.Width.Six
                    | 7 -> CommonTypes.Width.Seven
                    | _ -> CommonTypes.Width.Eight
            | None -> CommonTypes.Width.One
        //1. make wire type from two coordinates
        let hostId = CommonTypes.ConnectionId(uuid())
        let portCoords = ({X=100.0;Y=100.0},{X=800.0;Y=800.0})
        let segmentList = makeSegmentsList hostId portCoords
        let newWire = 
            {
                Id=hostId
                SrcSymbol=dummyPortId1 // fst(portIds)
                TargetSymbol=dummyPortId2 // snd(portIds)
                Color=wireColour
                Width=wireWidth
                Segments=segmentList
            }

        let wireAddedMap = Map.add newWire.Id newWire model.WX
        {model with WX = wireAddedMap}, Cmd.none

    | MouseMsg mMsg -> model, Cmd.ofMsg (Symbol (Symbol.MouseMsg mMsg))

    | SelectWires (connectionIds:CommonTypes.ConnectionId list) -> 
        let allDeselectedMap = Map.map (fun id wire -> {wire with Color = CommonTypes.HighLightColor.Blue}) model.WX
        let allDeselectedModel = {model with WX=allDeselectedMap}

        let rec selectWires (wModel:Model) (conIds:CommonTypes.ConnectionId list) =
            let selectWire (wModel:Model) (connectionId:CommonTypes.ConnectionId) = 
            // if wModel.WX key is conId then change colour to Green
                let currentlySelectedMap = 
                    Map.map (fun id wire -> 
                        if id=connectionId then
                            {wire with Color = CommonTypes.HighLightColor.Green} 
                        else
                            wire
                        )
                wModel.WX
                |> currentlySelectedMap

            match conIds with
            |h::t ->
                // change colour of all Ids in conIds to Green
                let wireSelectedMap = selectWire wModel h
                let wModel = {wModel with WX=wireSelectedMap}
                selectWires wModel t
            |[] -> wModel.WX

        let wiresSelectedMap = selectWires allDeselectedModel connectionIds
        {model with WX = wiresSelectedMap}, Cmd.none


    | DeleteWires (connectionIds:CommonTypes.ConnectionId list) -> 
        let rec removeWireList (wModel:Model) (conIds: CommonTypes.ConnectionId list) = 
            match conIds with
            |h::t -> 
                let newWireMap = Map.remove h wModel.WX
                let wModel = {wModel with WX=newWireMap}
                removeWireList wModel t
            |[] -> wModel.WX
        let wireDeletedMap = removeWireList model connectionIds
        {model with WX = wireDeletedMap}, Cmd.none


//---------------Other interface functions--------------------//

/// Given a point on the canvas, returns the wire ID of a wire within a few pixels
/// or None if no such. Where there are two close wires the nearest is taken. Used
/// to determine which wire (if any) to select on a mouse click
let getWireIfClicked (wModel: Model) (pos: XYPos) : CommonTypes.ConnectionId option = 
    let wireNum = Map.count wModel.WX
    if wireNum = 0 then
        None
    else
        let wireList = 
            wModel.WX
            |> Map.toList
            |> List.map (fun (_,y) -> y)

        let closestSegTuple = 
            wireList
            |> List.collect (fun x -> x.Segments)
            |> List.map (fun x -> (x, segPointDist x pos))
            |> List.minBy snd

        //returns wire connectionId if within 3 pixels -- change value to change
        if snd(closestSegTuple) < 3.0 then
            Some (fst(closestSegTuple)).HostId
        else
            None

let getIntersectingWires (wModel:Model) (selectBox:BoundingBox) : CommonTypes.ConnectionId list = 
    let wireIntersectsBoundingBox (w:Wire) (bb:BoundingBox) = 
        let boolList = List.map (fun seg -> segIntersectsBoundingBox bb seg) w.Segments
        List.contains true boolList

    wModel.WX
    //1. (id, wire) -> (id, bool) where True = wire intersects the bounding box
    |> Map.map (fun id wire -> wireIntersectsBoundingBox wire selectBox)
    //2. filter by true to get intersecting connectionIds
    |> Map.filter (fun id boolVal -> boolVal)
    //3. take keys
    |> Map.toList
    |> List.map (fun (x,y) -> x)

let getConnectedWires (wModel:Model) (compIdList:CommonTypes.ComponentId list) : CommonTypes.ConnectionId list =
    //1. Symbol.getPorts compIdList gives all portIds for the components given
    let portIdList = [CommonTypes.ComponentId(uuid());CommonTypes.ComponentId(uuid());CommonTypes.ComponentId(uuid())] //dummy
    let conIdList = []
    let rec getWiresFromAllPorts lst = 
        match lst with
        | h::t ->
            let tryGetWireFromPort = 
                let wireFromPort = Map.filter (fun id wire -> (wire.SrcSymbol = h || wire.TargetSymbol = h)) wModel.WX //change to SrcPort and TargetPort
                if wireFromPort.Count = 0 then
                    None
                else
                    let getWire = 
                        wireFromPort
                        |> Map.toList
                        |> List.map (fun (x,y) -> y)
                    Some getWire
            match tryGetWireFromPort with
            | Some wire -> 
                let conIdList = conIdList @ wire
                getWiresFromAllPorts t
            | None -> 
                getWiresFromAllPorts t
        | [] -> conIdList

    portIdList
    |> getWiresFromAllPorts
    |> List.distinct //connectionId list should have no repeat elements

//----------------------interface to Issie-----------------------//
let extractWire (wModel: Model) (sId:CommonTypes.ComponentId) : CommonTypes.Component= 
    failwithf "Not implemented"

let extractWires (wModel: Model) : CommonTypes.Component list = 
    failwithf "Not implemented"

/// Update the symbol with matching componentId to comp, or add a new symbol based on comp.
let updateSymbolModelWithComponent (symModel: Model) (comp:CommonTypes.Component) =
    failwithf "Not Implemented"
