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


## AddWire functionality
given (CommonTypes.InputPortId * CommonTypes.OutputPortId), render a wire of the correct shape.

- [ ] Call interfacing function with symbol (portIds : (CommonTypes.InputPortId * CommonTypes.OutputPortId)) -> (portPos : (XYPos * XYPos))
- [ ] Given coordinates of ports, generate vertices (portPos : (XYPos * XYPos)) -> (vertices : (float * float) list)
- [ ] Given vertices, render the line segments for the line
