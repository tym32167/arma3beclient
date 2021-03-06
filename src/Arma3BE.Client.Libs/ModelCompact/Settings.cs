﻿using System.ComponentModel.DataAnnotations;

namespace Arma3BEClient.Libs.ModelCompact
{
    public class Settings
    {
        //[Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class CustomSettings
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }
    }
}