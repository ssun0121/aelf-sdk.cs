using AElf.Client.Bridge;
using AElf.Client.Genesis;
using AElf.Client.MerkleTree;
using AElf.Client.Oracle;
using AElf.Client.Regiment;
using AElf.Client.Report;
using AElf.Client.TestBase;
using AElf.Client.Token;
using Volo.Abp.Modularity;

namespace AElf.Client.Test;

[DependsOn(
    typeof(AElfClientAbpTestBaseModule),
    typeof(AElfClientTokenModule),
    typeof(AElfClientGenesisModule),
    typeof(AElfClientMerkleTreeModule),
    typeof(AElfClientBridgeModule),
    typeof(AElfClientOracleModule),
    typeof(AElfClientReportModule),
    typeof(AElfClientRegimentModule)
)]
public class AElfClientAbpContractServiceTestModule : AbpModule
{
    
}