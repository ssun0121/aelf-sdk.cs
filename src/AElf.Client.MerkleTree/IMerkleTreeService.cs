using AElf.Client.Core;
using AElf.Contracts.MerkleTreeContract;

namespace AElf.Client.MerkleTree;

public interface IMerkleTreeService
{
    Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput);

    Task<long> GetLastLeafIndexAsync(GetLastLeafIndexInput input);

}