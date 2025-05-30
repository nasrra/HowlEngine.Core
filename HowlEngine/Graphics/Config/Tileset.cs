namespace HowlEngine.Graphics.Config{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public partial class Tileset
    {
        [JsonPropertyName("columns")]
        public long Columns { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("imageheight")]
        public long Imageheight { get; set; }

        [JsonPropertyName("imagewidth")]
        public long Imagewidth { get; set; }

        [JsonPropertyName("margin")]
        public long Margin { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("spacing")]
        public long Spacing { get; set; }

        [JsonPropertyName("tilecount")]
        public long Tilecount { get; set; }

        [JsonPropertyName("tiledversion")]
        public string Tiledversion { get; set; }

        [JsonPropertyName("tileheight")]
        public long Tileheight { get; set; }

        [JsonPropertyName("tiles")]
        public Tile[] Tiles { get; set; }

        [JsonPropertyName("tilewidth")]
        public long Tilewidth { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public partial class Tile
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("animation")]
        public Animation[] Animation { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("properties")]
        public Property[] Properties { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public partial class Animation
    {
        [JsonPropertyName("duration")]
        public long Duration { get; set; }

        [JsonPropertyName("tileid")]
        public long Tileid { get; set; }
    }

    public partial class Property
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
