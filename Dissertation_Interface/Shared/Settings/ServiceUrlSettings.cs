﻿namespace Shared.Settings;

public class ServiceUrlSettings
{
    public const string SectionName = "ServiceUrls";

    public string UserApi { get; set; } = default!;
}