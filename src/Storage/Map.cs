using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Apex.Serialization;
using Larx.MapAssets;
using Larx.Terrain;
using OpenTK;

namespace Larx.Storage
{
    [Serializable]
    public class PlacedAsset
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public PlacedAsset(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    [Serializable]
    public class MapDataContainer
    {
        public int MapSize { get; set; }
        public float[,] TerrainElevations { get; set; }
        public float[][,] SplatMap { get; set; }
        public Dictionary<string, List<PlacedAsset>> Assets { get; set; }
    }

    public static class Map
    {
        private const string MapFileName = "default.lrx";
        public static MapDataContainer MapData = new MapDataContainer();

        public static void New(int mapSize)
        {
            MapData.MapSize = mapSize;
            MapData.Assets = new Dictionary<string, List<PlacedAsset>>();

            var paddedMapSize = MapData.MapSize + 1;
            MapData.TerrainElevations = new float[MapData.MapSize + 1, MapData.MapSize + 1];
            for (var z = 0; z <= Map.MapData.MapSize; z++)
                for (var x = 0; x <= Map.MapData.MapSize; x++)
                    MapData.TerrainElevations[x, z] = 1.0f;

            MapData.SplatMap = new float[TerrainConfig.Textures.Length][,];

            var size = (int)(MapData.MapSize * TerrainConfig.HeightMapDetail);
            for(var i = 0; i < MapData.SplatMap.Length; i++)
                MapData.SplatMap[i] = new float[size, size];
        }

        public static void Save(TerrainRenderer terrain, Assets assets)
        {
            MapData.TerrainElevations = terrain.HeightMap.Heights;

            using (var stream = File.Open(MapFileName, FileMode.Create))
                using (var compressedStream = new GZipStream(stream, CompressionMode.Compress)) {
                    var binarySerializer = Binary.Create();
                    binarySerializer.Write(MapData, compressedStream);
                }
        }

        public static void Load(TerrainRenderer terrain, Assets assets)
        {
            using (var stream = File.Open(MapFileName, FileMode.Open))
                using (var decompressedStream = new GZipStream(stream, CompressionMode.Decompress)) {
                    var binarySerializer = Binary.Create();
                    MapData = binarySerializer.Read<MapDataContainer>(decompressedStream);
                }

            terrain.HeightMap.Heights = MapData.TerrainElevations;
            terrain.Update();
            terrain.HeightMap.Update();
            terrain.SplatMap.Refresh();
            assets.Refresh(terrain);
        }
    }
}