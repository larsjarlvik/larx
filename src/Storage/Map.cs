using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Larx.MapAssets;
using Larx.Terrain;
using OpenTK;

namespace Larx.Storage
{
    [Serializable]
    public class PlacedAsset
    {
        public string Model { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }

        public PlacedAsset(string model, Vector2 position, float rotation)
        {
            Model = model;
            Position = position;
            Rotation = rotation;
        }
    }

    [Serializable]
    public class MapDataContainer
    {
        public int MapSize { get; set; }
        public float[] TerrainElevations { get; set; }
        public float[][,] SplatMap { get; set; }
        public List<PlacedAsset> Assets { get; set; }
    }

    public static class Map
    {
        private const string MapFileName = "default.lrx";
        public static MapDataContainer MapData = new MapDataContainer();

        public static void New(int mapSize)
        {
            MapData.MapSize = mapSize;
            MapData.Assets = new List<PlacedAsset>();

            var paddedMapSize = MapData.MapSize + 1;
            MapData.TerrainElevations = Enumerable.Repeat(1.0f, (int)Math.Pow(paddedMapSize, 2)).ToArray();

            MapData.SplatMap = new float[TerrainRenderer.Textures.Length][,];
            for(var i = 0; i < MapData.SplatMap.Length; i++) MapData.SplatMap[i] = new float[State.SplatDetail, State.SplatDetail];
        }

        public static void Save(TerrainRenderer terrain, Assets assets)
        {
            MapData.TerrainElevations = terrain.GetTerrainElevations();

            using (var stream = File.Open(MapFileName, FileMode.Create))
            {
                using (var compressedStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(compressedStream, MapData);
                }
            }
        }

        public static void Load(TerrainRenderer terrain, Assets assets)
        {
            using (var stream = File.Open(MapFileName, FileMode.Open))
            {
                using (var decompressedStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var binaryFormatter = new BinaryFormatter();
                    MapData = (MapDataContainer)binaryFormatter.Deserialize(decompressedStream);
                }
            }

            terrain.Build();
        }
    }
}