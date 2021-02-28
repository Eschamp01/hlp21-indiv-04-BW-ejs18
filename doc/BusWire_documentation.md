#### BusWire Messages (Sent by Sheet)

1. `AddWire of (CommonTypes.InputPortId * CommonTypes.OutputPortId)`

Using the Ids of two ports, BusWire creates a new `wire` type, and adds it to its model.
*(First, an interfacing function with symbol, Symbol.getPortLocations, must be called. This will return a type XYPos\*XYPos to work with)*

2. `SelectWires of CommonTypes.ConnectionId list`

BusWire changes the colour of the `wire`s corresponding to `CommonTypes.ConnectionId list`. All other `wire`s in the model are set to their default colour.

3. `DeleteWires of CommonTypes.ConnectionId list`

BusWire removes the wires corresponding to `CommonTypes.ConnectionId list` from its model.

#### Sheet to BusWire Interfacing Functions

1. `getWireIfClicked (wModel:Model) (pos:XYPos) : CommonTypes.ConnectionId option`

Returns `Some CommonTypes.ConnectionId` where `CommonTypes.ConnectionId` is the Id of the closest `wire` in `wModel` to `pos`, providing `pos` is less than 3.0 pixels away from that `wire`.
*(Closeness is defined by euclidean distance, and is calculated by using each wire `Segment`)*

2. `getIntersectingWires (wModel:Model) (selectBox:BoundingBox) : CommonTypes.ConnectionId list`

Given a single `BoundingBox`, returns a `CommomTypes.ConnectionId list` corresponding to every `wire` in `wModel` that at least partially intersects the `BoundingBox`.

3. `getConnectedWires (wModel:model) (compIdList:CommonTypes.ComponentId list) : CommonTypes.ConnectionId list`

Returns a `CommonTypes.ConnectionId list` corresponding to every `wire` in `wModel` which is connected to any `component` in `compIdList`.

#### BusWire to Symbol interfacing functions

`let getPortLocations (portTuple : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) : (XYPos * XYPos)= 
    let portOneCoords = Symbol.getPortCoords fst(portTuple)
    let portTwoCoords = Symbol.getPortCoords snd(portTuple)
    (portOneCoords,portTwoCoords)`


Use Port.portType as direction inference. Output means wire goes right from port, Input means wire goes left from port.
*(This can be changed to allow inputs and outputs on both sides in the group stage)*


`getBoundingBoxes : Symbol.Model -> Map<CommonTypes.ComponentId, BoundingBox>`
Symbol returns a map of all components and their respective bounding boxes.

__Universal type addition for all group members (in CommonTypes):__
`type BoundingBox = { X: float; Y: float; W: float; H: float}`
