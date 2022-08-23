using AElf.Client.Abp.TestBase;
using AElf.Client.Abp.Token;
using AElf.Client.Abp.Whitelist;
using Volo.Abp.Modularity;

namespace AElf.Client.Abp.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule),
    typeof(AElfClientWhitelistModule)
    )]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}