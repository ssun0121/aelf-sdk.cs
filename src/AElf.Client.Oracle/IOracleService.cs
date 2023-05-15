using AElf.Client.Core;
using AElf.Contracts.Oracle;

namespace AElf.Client.Oracle;

public interface IOracleService
{
    Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput);

    Task<SendTransactionResult> CreateRegimentAsync(CreateRegimentInput input);

    Task<SendTransactionResult> AddAdminsAsync(AddAdminsInput input);
    
    Task<SendTransactionResult> DeleteAdminAsync(DeleteAdminsInput deleteAdminsInput);

    Task<SendTransactionResult> DeleteMemberListAsync(DeleteRegimentMemberInput input);
}