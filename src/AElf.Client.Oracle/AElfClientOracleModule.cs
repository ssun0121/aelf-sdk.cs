﻿using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.Oracle;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientOracleModule : AbpModule
{
}