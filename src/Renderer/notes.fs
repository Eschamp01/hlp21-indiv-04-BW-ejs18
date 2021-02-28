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

let view (model:Model) (dispatch: Dispatch<Msg>)=
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

let points1 = [(0.0,0.0);(2.0,0.0);(2.0,2.0);(2.0,4.0)]
//verticesList1 = [[(0.0, 0.0); (2.0, 0.0)]; [(2.0, 0.0); (2.0, 2.0)];[(2.0, 2.0); (2.0, 4.0)]]

let pointsToVerticesList (pointsList: (float*float) list) : (float*float) list list = 
    let removeFirstAndLast lst = 
        match List.rev lst with
        |h::t -> 
            match List.rev t with
            | h::t -> t
            |_ -> failwithf "no list found"
        |_ -> failwithf "no list found"
    pointsList
    |> List.collect (fun x -> [x;x])
    |> removeFirstAndLast
    |> List.chunkBySize 2

pointsToVerticesList points1

let getPoints (PortTuple : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) : (XYPos * XYPos)= 
    //Interfacing function with symbol to get port coordinates of type XYPos from Port Id provided by sheet
    let portOneCoords = Symbol.getPortCoords fst(PortTuple)
    let portTwoCoords = Symbol.getPortCoords snd(PortTuple)
    (portOneCoords,portTwoCoords)

//use Port.portType as direction inference -> Input means Right, Output means Left


let segmentIdList = ["0";"1";"2";"3";"4"]
let segId = "1"


match List.tryFind (fun x -> x = segId) segmentIdList with

// (segId:float * segDist:float) list
let testList = [(0.0,0.33);(3.0,1.0);(2.0,5.0);(4.0,7.0)]

let map1 = Map.ofList testList

map1.Count

let map2 = Map.filter (fun k v -> k < 0.0) map1

map2.Count

let minBySnd lst = 
    lst
    |> List.minBy snd 


minBySnd testList

let emptyMap = Map.empty

let wireList m = 
    m
    |> Map.toList
    |> List.map (fun (_,y) -> y)

wireList emptyMap

Map.count emptyMap



wireVertices list ---> Segment list ---> 


let list7 = [1;2;3;4]
let list8 = list7 @ [5]


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

