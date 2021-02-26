#### Sheet to BusWire Interfacing Functions

1. `getWireIfClicked (wModel:Model) (pos:XYPos) : CommonTypes.ConnectionId option`

Returns `Some CommonTypes.ConnectionId` where `CommonTypes.ConnectionId` is the Id of the closest `wire` in `wModel` to `pos`, providing `pos` is less than 3.0 pixels away from that `wire`.
*(Closeness is defined by euclidean distance, and is calculated by using each wire `Segment`)*

2. `getIntersectingWires: (wModel:Model) (selectBox:BoundingBox) : CommonTypes.ConnectionId list`

Given a single `BoundingBox`, returns a `CommomTypes.ConnectionId list` corresponding to every `wire` in `wModel` that at least partially intersects the `BoundingBox`.

3. `getConnectedWires (wModel:model) (compIdList:CommonTypes.ComponentId list) -> CommonTypes.ConnectionId list`

Returns a `CommonTypes.ConnectionId list` corresponding to every `wire` in `wModel` which is connected to any `component` in `compIdList`.

#### BusWire Messages (Sent by Sheet)

1. `AddWire of (CommonTypes.InputPortId * CommonTypes.OutputPortId)`

Using the Ids of two ports, BusWire creates a new `wire` type, and adds it to its model.
*(First, an interfacing function with symbol, Symbol.getPortLocations, must be called. This will return a type XYPos\*XYPos to work with)*

2. `SelectWire of CommonTypes.ConnectionId`

BusWire changes the colour and/or width (depending on what looks best) of the wire corresponding to `CommonTypes.ConnectionId`.

3. `DeleteWire of CommonTypes.ConnectionId`

BusWire removes the wire corresponding to `CommonTypes.ConnectionId` from its model.

#### BusWire to Symbol interfacing functions

`let getPortLocations (PortTuple : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) : (XYPos * XYPos)= 
    let portOneCoords = Symbol.getPortCoords fst(PortTuple)
    let portTwoCoords = Symbol.getPortCoords snd(PortTuple)
    (portOneCoords,portTwoCoords)`


Use Port.portType as direction inference. Output means wire goes right from port, Input means wire goes left from port.
*(This can be changed to allow inputs and outputs on both sides in the group stage)*
