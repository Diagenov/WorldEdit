﻿using Newtonsoft.Json;
using System.IO;

namespace WorldEdit
{
    public class Config
    {
        public int MaxUndoCount = 50;
        public bool StartSchematicNamesWithCreatorUserID = false;

        public static Config Read(string ConfigFile) =>
            !File.Exists(ConfigFile)
                ? new Config().Write(ConfigFile)
                : JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigFile));

        public Config Write(string ConfigFile)
        {
            File.WriteAllText(ConfigFile,
                JsonConvert.SerializeObject(this, Formatting.Indented));
            return this;
        }
    }
}