using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Apex.Serialization;
using Larx.Assets;
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
        public float Scale { get; set; }

        public PlacedAsset(string model, Vector2 position, float rotation, float scale)
        {
            Model = model;
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }

    [Serializable]
    public class MapDataContainer
    {
        public string Name { get; set; }
        public int MapSize { get; set; }
        public float[,] TerrainElevations { get; set; }
        public float[][,] SplatMap { get; set; }
        public Dictionary<string, List<PlacedAsset>> Assets { get; set; }
    }

    public static class Map
    {
        public static MapDataContainer MapData = new MapDataContainer();

        public static void New(int mapSize)
        {
            MapData.Name = "New Map";
            MapData.MapSize = mapSize;
            MapData.Assets = new Dictionary<string, List<PlacedAsset>>();
            MapData.TerrainElevations = new float[MapData.MapSize + 1, MapData.MapSize + 1];
            for (var z = 0; z <= Map.MapData.MapSize; z++)
                for (var x = 0; x <= Map.MapData.MapSize; x++)
                    MapData.TerrainElevations[x, z] = 1.0f;

            MapData.SplatMap = new float[TerrainConfig.Textures.Length][,];

            var size = (int)(MapData.MapSize * TerrainConfig.HeightMapDetail);
            for(var i = 0; i < MapData.SplatMap.Length; i++)
                MapData.SplatMap[i] = new float[size, size];
        }

        public static void Save(string name, TerrainRenderer terrain)
        {
            var path = getMapPath(name);

            MapData.Name = name;
            MapData.TerrainElevations = terrain.HeightMap.Heights;
            using (var stream = File.Open(path, FileMode.Create))
                using (var compressedStream = new GZipStream(stream, CompressionMode.Compress)) {
                    var binarySerializer = Binary.Create();
                    binarySerializer.Write(MapData, compressedStream);
                }
        }

        public static bool Load(string name, TerrainRenderer terrain, AssetRenderer assets)
        {
            var path = getMapPath(name);
            if (!File.Exists(path)) return false;

            using (var stream = File.Open(path, FileMode.Open))
                using (var decompressedStream = new GZipStream(stream, CompressionMode.Decompress)) {
                    var binarySerializer = Binary.Create();
                    MapData = binarySerializer.Read<MapDataContainer>(decompressedStream);
                }

            terrain.HeightMap.Heights = MapData.TerrainElevations;
            terrain.Update();
            terrain.HeightMap.Update();
            terrain.SplatMap.Refresh();
            assets.Refresh(terrain);
            return true;
        }

        public static string[] ListMaps()
        {
            return Directory.GetFiles("data/maps")
                .Select(x => new FileInfo(x).Name)
                .Where(x => x.EndsWith(".lrx"))
                .Select(x => x.Replace(".lrx", ""))
                .ToArray();
        }

        private static string getMapPath(string name)
        {
            return $"data/maps/{name}.lrx";
        }
    }
}