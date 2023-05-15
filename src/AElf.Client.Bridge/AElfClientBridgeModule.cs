using AElf.Client.Core;
using Volo.Abp.Modularity;

namespace AElf.Client.Bridge;

[DependsOn(
    typeof(AElfClientModule),
    typeof(CoreAElfModule)
)]
public class AElfClientBridgeModule : AbpModule
{
    
}