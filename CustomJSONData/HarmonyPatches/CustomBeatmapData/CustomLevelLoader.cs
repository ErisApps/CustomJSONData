﻿namespace CustomJSONData.HarmonyPatches
{
    using System.Collections.Generic;
    using System.IO;
    using CustomJSONData.CustomBeatmap;
    using HarmonyLib;

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadCustomLevelInfoSaveData")]
    internal class CustomLevelLoaderLoadCustomLevelInfoSaveData
    {
        private static bool Prefix(ref StandardLevelInfoSaveData __result, string customLevelPath)
        {
            string path = Path.Combine(customLevelPath, "Info.dat");
            if (File.Exists(path))
            {
                __result = CustomLevelInfoSaveData.Deserialize(path);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CustomLevelLoader))]
    [HarmonyPatch("LoadBeatmapDataBeatmapData")]
    internal class CustomLevelLoaderLoadBeatmapDataBeatmapData
    {
        private static bool Prefix(ref BeatmapData __result, string customLevelPath, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData, BeatmapDataLoader ____beatmapDataLoader)
        {
            string path = Path.Combine(customLevelPath, difficultyFileName);
            if (File.Exists(path))
            {
                CustomBeatmapSaveData saveData = CustomBeatmapSaveData.Deserialize(path);

                List<BeatmapSaveData.NoteData> notes = saveData.notes;
                List<BeatmapSaveData.ObstacleData> obstacles = saveData.obstacles;
                List<BeatmapSaveData.EventData> events = saveData.events;
                List<BeatmapSaveData.WaypointData> waypoints = saveData.waypoints;
                BeatmapSaveData.SpecialEventKeywordFiltersData specialEventsKeywordFilters = saveData.specialEventsKeywordFilters;

                BeatmapDataLoaderGetBeatmapDataFromBeatmapSaveData.CustomBeatmapSaveData = saveData;
                __result = ____beatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(notes, waypoints, obstacles, events, specialEventsKeywordFilters, standardLevelInfoSaveData.beatsPerMinute, standardLevelInfoSaveData.shuffle, standardLevelInfoSaveData.shufflePeriod);
            }

            return false;
        }

        private static void Postfix(BeatmapData __result, string difficultyFileName, StandardLevelInfoSaveData standardLevelInfoSaveData)
        {
            if (__result != null && __result is CustomBeatmapData customBeatmapData && standardLevelInfoSaveData is CustomLevelInfoSaveData customLevelInfoSaveData)
            {
                customBeatmapData.SetLevelCustomData(customLevelInfoSaveData.beatmapCustomDatasByFilename[difficultyFileName], customLevelInfoSaveData.customData);
            }
        }
    }
}
