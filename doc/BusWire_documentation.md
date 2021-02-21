## Sheet to BusWire Interfacing Functions

`AddWire of (CommonTypes.InputPortId * CommonTypes.OutputPortId)`
Sheet send BusWire a message containing two Ids for two ports for BusWire to create a new wire in-between.

`DeleteWire of CommonTypes.ConnectionId`
Sheet sends BusWire an Id of a Wire to delete

`MoveConnection of {Something}`
TO-DO: What is the best way to move a wire?
This is left for later, note that the movement of a wire can be both due to a wire moving, and one of the symbols (ports) moving.

`SetColor of CommonTypes.ConnectionId * CommonTypes.HighLightColor`
BusWire changes the colour of the given wire.


#### AddWire functionality
given (CommonTypes.InputPortId * CommonTypes.OutputPortId), render a wire of the correct shape.

- [ ] Call interfacing function with symbol (portIds : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) -> (portPositions : (XYPos * XYPos))
- [ ] Given coordinates of ports, generate vertices (portPositions : (XYPos * XYPos)) -> (vertices : (float * float) list)
- [ ] Given vertices, render the line segments for the line

#### DeleteWire functionality

#### Defining Wire dataType, and conversion function from Issie Connection type -> Wire type


#### Obtaining port coordinates and directions for wire rendering

Use Port.portType as direction inference. Output means wire goes right from port, Input means wire goes left from port
This can be changed to allow inputs and outputs on both sides in the group stage, by working with Symbol a bit.

To get coordinates from ports, use interfacing function with Symbol similar to:

let getPoints (PortTuple : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) : (XYPos * XYPos)= 
    //Interfacing function with symbol to get port coordinates of type XYPos from Port Id provided by sheet
    let portOneCoords = Symbol.getPortCoords fst(PortTuple)
    let portTwoCoords = Symbol.getPortCoords snd(PortTuple)
    (portOneCoords,portTwoCoords)


