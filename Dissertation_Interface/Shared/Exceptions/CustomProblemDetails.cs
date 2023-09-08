﻿using Microsoft.AspNetCore.Mvc;

namespace Shared.Middleware;

public class CustomProblemDetails : ProblemDetails
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

}