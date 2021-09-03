namespace PerlinNoiseGenerator
{
    public static class Simplex
    {
        public static float[,] GenerateNoiseMap
            (int mapSizeX, int mapSizeY, float scale)
        {
            var cubeMap = new float[mapSizeX * 4, mapSizeY * 4];
            //Z STATIC
            for(int y = 0; y < mapSizeY; y++) {
                for(int x = 0; x < mapSizeX * 2; x++) {
                    //Generates FRONT
                    if(x < mapSizeX) {
                        cubeMap[mapSizeX+x, mapSizeY+y] = SimplexNoise.Noise.CalcPixel3D(x, y, 0, scale) / 255;                    
                    }
                    //Generates BACK
                    else {
                        cubeMap[mapSizeX*3+(x-mapSizeX), mapSizeY+y] = SimplexNoise.Noise.CalcPixel3D(mapSizeX-(x-mapSizeX), y, mapSizeY, scale) / 255;
                    }
                }
            }
            //X STATIC
            for(int y = 0; y < mapSizeY; y++) {
                for(int x = 0; x < mapSizeX * 2; x++) {
                    //Generates LEFT
                    if(x < mapSizeX) {
                        cubeMap[x, mapSizeY+y] = SimplexNoise.Noise.CalcPixel3D(0, y, mapSizeX-x, scale) / 255;                   
                    }
                    //Generates RIGHT
                    else {
                        cubeMap[mapSizeX*2+(x-mapSizeX), mapSizeY+y] = SimplexNoise.Noise.CalcPixel3D(mapSizeX, y, x-mapSizeX, scale) / 255;
                    }
                }
            }
            //Y STATIC
            for(int y = 0; y < mapSizeY * 2; y++) {
                for(int x = 0; x < mapSizeX; x++) {
                    //Generates TOP
                    if(y < mapSizeY) {
                        cubeMap[mapSizeX+x, y] = SimplexNoise.Noise.CalcPixel3D(x, 0, mapSizeY-y, scale) / 255;          
                    }
                    //Generates BOTTOM
                    else {
                        cubeMap[mapSizeX+x, mapSizeY*2+(y-mapSizeY)] = SimplexNoise.Noise.CalcPixel3D(x, mapSizeY, y-mapSizeY, scale) / 255;
                    }                
                }
            }

            return cubeMap;
        }
    }
}