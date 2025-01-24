# schellenberg-usb

Control Schellenberg devices using the RF stick. UI is only a REST api which easily can be integrated in any third part system (eg: Home Assistant).
Project is still under development.

## Docker for RaspberryPI

https://hub.docker.com/repository/docker/atilladocker/schellenberg-web2rf-api


## State Machine definition overview 

```mermaid
stateDiagram-v2
	Unknown --> Starting : Init
	Starting --> Idle : Started
	Idle --> Moving : MoveUpReceived / Function
	Idle --> Moving : MoveDownReceived / Function
	Idle --> Moving : MoveUpPressed / Function
	Idle --> Moving : MoveDownPressed / Function
	Idle --> Pairing : PairingStartedReceived
	Moving --> Idle : StopReceived / Function
	Moving --> Moving : MoveDownPressed / Function
	Moving --> Moving : MoveDownReceived / Function
	Moving --> Moving : MoveUpPressed / Function
	Moving --> Moving : MoveUpReceived / Function
	Pairing --> Idle : Paired / Function
    [*] --> Unknown
```

## Copy docker-compose.yaml file to server using ssh:

```batch 
scp -O .\docker-compose.yaml root@homeassistant.local:/home/schellenberg-service/
```