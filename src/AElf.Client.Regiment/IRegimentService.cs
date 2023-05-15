using AElf.Client.Core;
using AElf.Contracts.Regiment;
using AElf.Types;

namespace AElf.Client.Regiment;

public interface IRegimentService
{
    Task<SendTransactionResult> InitializeContractAsync(InitializeInput initializeInput);

    Task<SendTransactionResult> CreateRegimentAsync(CreateRegimentInput createRegimentInput);

    Task<SendTransactionResult> AddAdminAsync(AddAdminsInput addAdminsInput);

    Task<SendTransactionResult> DeleteAdminAsync(DeleteAdminsInput deleteAdminsInput);

    Task<RegimentInfo> GetRegimentInfoAsync(Address input);

    Task<RegimentMemberList> GetRegimentMemberListAsync(Address input);
    
    Task<Address> GetControllerAsync();
}