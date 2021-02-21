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

let points1 = [(0,0);(2,0);(2,2);(2,4)]

let duplicatedList = List.collect (fun x -> [x;x]) points1
let lastElementRemoved = 
    match List.rev duplicatedList with
    |h::t -> Rev t
    |_ -> failwithf "no list found"


List.chunkBySize 2 points1

let getListFromPoints pointsList = 
    pointsList
    |> List.collect (fun x -> [x;x])

pointsList1 = [[(0, 0);(2, 0)];[(2, 0);(2, 2)];[(2, 2);(2,4)]]

points
|> getListFromPoints