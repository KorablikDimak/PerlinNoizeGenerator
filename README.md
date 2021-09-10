# PerlinNoizeGenerator
## Description
procedural world generator based on perlin noise and simplex noise.
The project uses only one third-party library this is a **SimplexNoise**.
The generator creates convincing-looking flat and spherical maps that can be customized as you wish by changing humidity, temperature, water level, etc.
The generator was created using the unity 3d engine.
### Techniques used
- pattern strategy (used in class `NoiseMapGenerator`)
- delegate substitution (used in class `NoiseMapRenderer`)
- parallel computing, work with multithreading (used in many places)
- coroutines as a replacement for asynchronous unity methods (aslo used in many places)
### Capability
- maps are rendered reliably using generated normal and elevation maps
- you can save created textures
- rotate and change maps
### Project structure
- the `MapGen` directory contains classes and scripts responsible for generating numerical noise maps
- the `RenderMap` directory contains classes for creating and transforming textures
- the `UI` directory contains all the classes related to the user interface
- the `SimplexNoise` directory contains the only third-party library with simplex noise
### Usage
The code of this application can be used in your projects where procedural world generation is required. 
By changing and selecting parameters, you can get the cards you need to use.
You can also implement it into your game as a full-fledged module with the addition of your own functions.
## Possible options for development
- improvement of the basic procedural generation algorithm
  - it is he who spends most of the resources when creating the world
- adding procedural generation of trees and other static objects
- reworking the algorithm for creating rivers to make them more realistic
- creation of an ecosystem, fauna on the territory of the map
## Download
You can download the archive with the attachment at the link [PerlinNoiseGenerator](https://disk.yandex.ru/d/CavTnmzT1HiWDg)
